using UnityEngine;

public static class SuperCollider
{
    public static bool ClosestPointOnSurface(Collider collider, Vector3 to, float radius, out Vector3 closestPointOnSurface)
    {
        if(collider is BoxCollider)
        {
            closestPointOnSurface = SuperCollider.ClosestPointOnSurface((BoxCollider)collider, to);
            return true;
        }
        else if(collider is SphereCollider)
        {
            closestPointOnSurface = SuperCollider.ClosestPointOnSurface((SphereCollider)collider, to);
            return true;
        }
        else if(collider is CapsuleCollider)
        {
            closestPointOnSurface = SuperCollider.ClosestPointOnSurface((CapsuleCollider)collider, to);
            return true;
        }
        else if(collider is MeshCollider)
        {
            BSPTree bspTree = collider.GetComponent<BSPTree>();

            if(bspTree != null)
            {
                closestPointOnSurface = bspTree.ClosestPointOn(to, radius);
                return true;
            }

            BSPTree bsp = collider.GetComponent<BSPTree>();

            if(bsp != null)
            {
                closestPointOnSurface = bsp.ClosestPointOn(to, radius);
                return true;
            }

            BruteForceMesh bfm = collider.GetComponent<BruteForceMesh>();

            if(bfm != null)
            {
                closestPointOnSurface = bfm.ClosestPointOn(to);
                return true;
            }
        }
        else if(collider is TerrainCollider)
        {
            closestPointOnSurface = SuperCollider.ClosestPointOnSurface((TerrainCollider)collider, to, radius, false);
            return true;
        }

        Debug.LogError(string.Format("{0} does not have an implementation for ClosestPointOnSurface; GameObject.Name='{1}'", collider.GetType(), collider.gameObject.name));
        closestPointOnSurface = Vector3.zero;
        return false;
    }
    
    public static Vector3 ClosestPointOnSurface(SphereCollider collider, Vector3 to)
    {
        Vector3 p;

        p = to - (collider.transform.position + collider.center);
        p.Normalize();

        p *= collider.radius * collider.transform.localScale.x;
        p += collider.transform.position + collider.center;

        return p;
    }

    public static Vector3 ClosestPointOnSurface(BoxCollider collider, Vector3 to)
    {
        // Cache the collider transform
        var ct = collider.transform;

        // Firstly, transform the point into the space of the collider
        var local = ct.InverseTransformPoint(to);

        // Now, shift it to be in the center of the box
        local -= collider.center;

        //Pre multiply to save operations.
        var halfSize = collider.size * 0.5f;

        // Clamp the points to the collider's extents
        var localNorm = new Vector3(
                Mathf.Clamp(local.x, -halfSize.x, halfSize.x),
                Mathf.Clamp(local.y, -halfSize.y, halfSize.y),
                Mathf.Clamp(local.z, -halfSize.z, halfSize.z)
            );

        //Calculate distances from each edge
        var dx = Mathf.Min(Mathf.Abs(halfSize.x - localNorm.x), Mathf.Abs(-halfSize.x - localNorm.x));
        var dy = Mathf.Min(Mathf.Abs(halfSize.y - localNorm.y), Mathf.Abs(-halfSize.y - localNorm.y));
        var dz = Mathf.Min(Mathf.Abs(halfSize.z - localNorm.z), Mathf.Abs(-halfSize.z - localNorm.z));

        // Select a face to project on
        if(dx < dy && dx < dz)
        {
            localNorm.x = Mathf.Sign(localNorm.x) * halfSize.x;
        }
        else if(dy < dx && dy < dz)
        {
            localNorm.y = Mathf.Sign(localNorm.y) * halfSize.y;
        }
        else if(dz < dx && dz < dy)
        {
            localNorm.z = Mathf.Sign(localNorm.z) * halfSize.z;
        }

        // Now we undo our transformations
        localNorm += collider.center;

        // Return resulting point
        return ct.TransformPoint(localNorm);
    }

    // Courtesy of Moodie
    public static Vector3 ClosestPointOnSurface(CapsuleCollider collider, Vector3 to)
    {
        Transform ct = collider.transform; // Transform of the collider

        float lineLength = collider.height - collider.radius * 2; // The length of the line connecting the center of both sphere
        Vector3 dir = Vector3.up;

        Vector3 upperSphere = dir * lineLength * 0.5f + collider.center; // The position of the radius of the upper sphere in local coordinates
        Vector3 lowerSphere = -dir * lineLength * 0.5f + collider.center; // The position of the radius of the lower sphere in local coordinates

        Vector3 local = ct.InverseTransformPoint(to); // The position of the controller in local coordinates

        Vector3 p = Vector3.zero; // Contact point
        Vector3 pt = Vector3.zero; // The point we need to use to get a direction vector with the controller to calculate contact point

		// Controller is contacting with cylinder, not spheres
		if(local.y < lineLength * 0.5f && local.y > -lineLength * 0.5f)
		{
			pt = dir * local.y + collider.center;
		}
		// Controller is contacting with the upper sphere 
		else if(local.y > lineLength * 0.5f)
		{
			pt = upperSphere;
		}
		// Controller is contacting with lower sphere
		else if(local.y < -lineLength * 0.5f)
		{
			pt = lowerSphere;
		}

        //Calculate contact point in local coordinates and return it in world coordinates
        p = local - pt;
        p.Normalize();
        p = p * collider.radius + pt;
        return ct.TransformPoint(p);

    }

    public static Vector3 ClosestPointOnSurface(TerrainCollider collider, Vector3 to, float radius, bool debug=false)
    {
        var terrainData = collider.terrainData;

        var local = collider.transform.InverseTransformPoint(to);

        // Calculate the size of each tile on the terrain horizontally and vertically
        float pixelSizeX = terrainData.size.x / (terrainData.heightmapResolution - 1);
        float pixelSizeZ = terrainData.size.z / (terrainData.heightmapResolution - 1);

        var percentZ = Mathf.Clamp01(local.z / terrainData.size.z);
        var percentX = Mathf.Clamp01(local.x / terrainData.size.x);

        float positionX = percentX * (terrainData.heightmapResolution - 1);
        float positionZ = percentZ * (terrainData.heightmapResolution - 1);

        // Calculate our position, in tiles, on the terrain
        int pixelX = Mathf.FloorToInt(positionX);
        int pixelZ = Mathf.FloorToInt(positionZ);

        // Calculate the distance from our point to the edge of the tile we are in
        float distanceX = (positionX - pixelX) * pixelSizeX;
        float distanceZ = (positionZ - pixelZ) * pixelSizeZ;

        // Find out how many tiles we are overlapping on the X plane
        float radiusExtentsLeftX = radius - distanceX;
        float radiusExtentsRightX = radius - (pixelSizeX - distanceX);

        int overlappedTilesXLeft = radiusExtentsLeftX > 0 ? Mathf.FloorToInt(radiusExtentsLeftX / pixelSizeX) + 1 : 0;
        int overlappedTilesXRight = radiusExtentsRightX > 0 ? Mathf.FloorToInt(radiusExtentsRightX / pixelSizeX) + 1 : 0;

        // Find out how many tiles we are overlapping on the Z plane
        float radiusExtentsLeftZ = radius - distanceZ;
        float radiusExtentsRightZ = radius - (pixelSizeZ - distanceZ);

        int overlappedTilesZLeft = radiusExtentsLeftZ > 0 ? Mathf.FloorToInt(radiusExtentsLeftZ / pixelSizeZ) + 1 : 0;
        int overlappedTilesZRight = radiusExtentsRightZ > 0 ? Mathf.FloorToInt(radiusExtentsRightZ / pixelSizeZ) + 1 : 0;

        // Retrieve the heights of the pixels we are testing against
        int startPositionX = pixelX - overlappedTilesXLeft;
        int startPositionZ = pixelZ - overlappedTilesZLeft;

        int numberOfXPixels = overlappedTilesXRight + overlappedTilesXLeft + 1;
        int numberOfZPixels = overlappedTilesZRight + overlappedTilesZLeft + 1;

        // Account for if we are off the terrain
        if(startPositionX < 0)
        {
            numberOfXPixels -= Mathf.Abs(startPositionX);
            startPositionX = 0;
        }

        if(startPositionZ < 0)
        {
            numberOfZPixels -= Mathf.Abs(startPositionZ);
            startPositionZ = 0;
        }

        if(startPositionX + numberOfXPixels + 1 > terrainData.heightmapResolution)
        {
            numberOfXPixels = terrainData.heightmapResolution - startPositionX - 1;
        }

        if(startPositionZ + numberOfZPixels + 1 > terrainData.heightmapResolution)
        {
            numberOfZPixels = terrainData.heightmapResolution - startPositionZ - 1;
        }

        // Retrieve the heights of the tile we are in and all overlapped tiles
        var heights = terrainData.GetHeights(startPositionX, startPositionZ, numberOfXPixels + 1, numberOfZPixels + 1);

        // Pre-scale the heights data to be world-scale instead of 0...1
        for(int i = 0; i < numberOfXPixels + 1; i++)
        {
            for(int j = 0; j < numberOfZPixels + 1; j++)
            {
                heights[j, i] *= terrainData.size.y;
            }
        }

        // Find the shortest distance to any triangle in the set gathered
        float shortestDistance = float.MaxValue;

        Vector3 shortestPoint = Vector3.zero;

        for(int x = 0; x < numberOfXPixels; x++)
        {
            for(int z = 0; z < numberOfZPixels; z++)
            {
                // Build the set of points that creates the two triangles that form this tile
                Vector3 a = new Vector3((startPositionX + x) * pixelSizeX, heights[z, x], (startPositionZ + z) * pixelSizeZ);
                Vector3 b = new Vector3((startPositionX + x + 1) * pixelSizeX, heights[z, x + 1], (startPositionZ + z) * pixelSizeZ);
                Vector3 c = new Vector3((startPositionX + x) * pixelSizeX, heights[z + 1, x], (startPositionZ + z + 1) * pixelSizeZ);
                Vector3 d = new Vector3((startPositionX + x + 1) * pixelSizeX, heights[z + 1, x + 1], (startPositionZ + z + 1) * pixelSizeZ);

                Vector3 nearest;

                BSPTree.ClosestPointOnTriangleToPoint(ref a, ref d, ref c, ref local, out nearest);

                float distance = (local - nearest).sqrMagnitude;

                if(distance <= shortestDistance)
                {
                    shortestDistance = distance;
                    shortestPoint = nearest;
                }

                BSPTree.ClosestPointOnTriangleToPoint(ref a, ref b, ref d, ref local, out nearest);

                distance = (local - nearest).sqrMagnitude;

                if(distance <= shortestDistance)
                {
                    shortestDistance = distance;
                    shortestPoint = nearest;
                }

                if(debug)
                {
                    DebugDraw.DrawTriangle(a, d, c, Color.cyan);
                    DebugDraw.DrawTriangle(a, b, d, Color.red);
                }
            }
        }

        return collider.transform.TransformPoint(shortestPoint);
    }
}