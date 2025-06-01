using UnityEngine;

public static class DebugDraw
{
    public static void DrawMarker(Vector3 position, float size, Color color, float duration, bool depthTest = true)
    {
        Vector3 line1PosA = position + Vector3.up * size * 0.5f;
        Vector3 line1PosB = position - Vector3.up * size * 0.5f;

        Vector3 line2PosA = position + Vector3.right * size * 0.5f;
        Vector3 line2PosB = position - Vector3.right * size * 0.5f;

        Vector3 line3PosA = position + Vector3.forward * size * 0.5f;
        Vector3 line3PosB = position - Vector3.forward * size * 0.5f;

        Debug.DrawLine(line1PosA, line1PosB, color, duration, depthTest);
        Debug.DrawLine(line2PosA, line2PosB, color, duration, depthTest);
        Debug.DrawLine(line3PosA, line3PosB, color, duration, depthTest);
    }

    // Courtesy of robertbu
    public static void DrawPlane(Vector3 position, Vector3 normal, float size, Color color, float duration, bool depthTest = true) 
    {
        Vector3 v3;

		if(normal.normalized != Vector3.forward)
		{
			v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
		}
		else
		{
			v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;
		}
 
        Vector3 corner0 = position + v3 * size;
        Vector3 corner2 = position - v3 * size;
 
        Quaternion q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        Vector3 corner1 = position + v3 * size;
        Vector3 corner3 = position - v3 * size;

        Debug.DrawLine(corner0, corner2, color, duration, depthTest);
        Debug.DrawLine(corner1, corner3, color, duration, depthTest);
        Debug.DrawLine(corner0, corner1, color, duration, depthTest);
        Debug.DrawLine(corner1, corner2, color, duration, depthTest);
        Debug.DrawLine(corner2, corner3, color, duration, depthTest);
        Debug.DrawLine(corner3, corner0, color, duration, depthTest);
        Debug.DrawRay(position, normal * size, color, duration, depthTest);
    }

    public static void DrawVector(Vector3 position, Vector3 direction, float raySize, float markerSize, Color color, float duration, bool depthTest = true)
    {
        Debug.DrawRay(position, direction * raySize, color, 0, false);
        DebugDraw.DrawMarker(position + direction * raySize, markerSize, color, 0, false);
    }

    public static void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
    {
        Debug.DrawLine(a, b, color);
        Debug.DrawLine(b, c, color);
        Debug.DrawLine(c, a, color);
    }

    public static void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, Color color, Transform t)
    {
        a = t.TransformPoint(a);
        b = t.TransformPoint(b);
        c = t.TransformPoint(c);

        Debug.DrawLine(a, b, color);
        Debug.DrawLine(b, c, color);
        Debug.DrawLine(c, a, color);
    }

    public static void DrawMesh(Mesh mesh, Color color, Transform t)
    {
        for(int i = 0; i < mesh.triangles.Length; i += 3)
        {
            DrawTriangle(mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i + 1]], mesh.vertices[mesh.triangles[i + 2]], color, t);
        }
    }

    public static Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}