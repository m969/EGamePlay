using UnityEngine;

/// <summary>
/// Primarily used for debugging purposes, this runs the same nearest point on mesh algorith
/// as the BSPTree, however there is no partitioning of the mesh; every triangle is evaluated
/// to find the nearest
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class BruteForceMesh : MonoBehaviour
{
    private int triangleCount;
    private Vector3[] vertices;
    private int[] tris;
    private Vector3[] triangleNormals;

    private Mesh mesh;

	void Awake()
	{
        mesh = GetComponent<MeshCollider>().sharedMesh;

        tris = mesh.triangles;
        vertices = mesh.vertices;

        triangleCount = mesh.triangles.Length / 3;

        triangleNormals = new Vector3[triangleCount];

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 normal = Vector3.Cross((vertices[tris[i + 1]] - vertices[tris[i]]).normalized, (vertices[tris[i + 2]] - vertices[tris[i]]).normalized).normalized;

            triangleNormals[i / 3] = normal;
        }
	}

    public Vector3 ClosestPointOn(Vector3 to)
    {
        to = transform.InverseTransformPoint(to);

        Vector3 closest = ClosestPointOnTriangle(tris, to);

        return transform.TransformPoint(closest);
    }

    Vector3 ClosestPointOnTriangle(int[] triangles, Vector3 to)
    {
        float shortestDistance = float.MaxValue;

        Vector3 shortestPoint = Vector3.zero;

        // Iterate through all triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int triangle = i;

            Vector3 p1 = vertices[tris[triangle]];
            Vector3 p2 = vertices[tris[triangle + 1]];
            Vector3 p3 = vertices[tris[triangle + 2]];

            Vector3 nearest;
                
            ClosestPointOnTriangleToPoint(ref p1, ref p2, ref p3, ref to, out nearest);

            float distance = (to - nearest).sqrMagnitude;

            if (distance <= shortestDistance)
            {
                shortestDistance = distance;
                shortestPoint = nearest;
            }
        }

        return shortestPoint;
    }

    /// <summary>
    /// Determines the closest point between a point and a triangle.
    /// 
    /// The code in this method is copyrighted by the SlimDX Group under the MIT license:
    /// 
    /// Copyright (c) 2007-2010 SlimDX Group
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// 
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <param name="vertex1">The first vertex to test.</param>
    /// <param name="vertex2">The second vertex to test.</param>
    /// <param name="vertex3">The third vertex to test.</param>
    /// <param name="result">When the method completes, contains the closest point between the two objects.</param>
    void ClosestPointOnTriangleToPoint(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, ref Vector3 point, out Vector3 result)
    {
        //Source: Real-Time Collision Detection by Christer Ericson
        //Reference: Page 136

        //Check if P in vertex region outside A
        Vector3 ab = vertex2 - vertex1;
        Vector3 ac = vertex3 - vertex1;
        Vector3 ap = point - vertex1;

        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0.0f && d2 <= 0.0f)
        {
            result = vertex1; //Barycentric coordinates (1,0,0)
            return;
        }

        //Check if P in vertex region outside B
        Vector3 bp = point - vertex2;
        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0.0f && d4 <= d3)
        {
            result = vertex2; // barycentric coordinates (0,1,0)
            return;
        }

        //Check if P in edge region of AB, if so return projection of P onto AB
        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
        {
            float v = d1 / (d1 - d3);
            result = vertex1 + v * ab; //Barycentric coordinates (1-v,v,0)
            return;
        }

        //Check if P in vertex region outside C
        Vector3 cp = point - vertex3;
        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0.0f && d5 <= d6)
        {
            result = vertex3; //Barycentric coordinates (0,0,1)
            return;
        }

        //Check if P in edge region of AC, if so return projection of P onto AC
        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
        {
            float w = d2 / (d2 - d6);
            result = vertex1 + w * ac; //Barycentric coordinates (1-w,0,w)
            return;
        }

        //Check if P in edge region of BC, if so return projection of P onto BC
        float va = d3 * d6 - d5 * d4;
        if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
        {
            float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            result = vertex2 + w * (vertex3 - vertex2); //Barycentric coordinates (0,1-w,w)
            return;
        }

        //P inside face region. Compute Q through its barycentric coordinates (u,v,w)
        float denom = 1.0f / (va + vb + vc);
        float v2 = vb * denom;
        float w2 = vc * denom;
        result = vertex1 + ab * v2 + ac * w2; //= u*vertex1 + v*vertex2 + w*vertex3, u = va * denom = 1.0f - v - w
    }
}