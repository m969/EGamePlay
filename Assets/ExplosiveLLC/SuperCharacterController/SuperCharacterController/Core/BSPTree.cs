using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Recursively paritions a mesh's vertices to allow to more quickly
/// narrow down the search for a nearest point on it's surface with respect to another
/// point
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class BSPTree : MonoBehaviour
{
    [SerializeField]
    bool drawMeshTreeOnStart;

    public class Node
    {
        public Vector3 partitionPoint;
        public Vector3 partitionNormal;

        public Node positiveChild;
        public Node negativeChild;

        public int[] triangles;
    };

    private int triangleCount;
    private int vertexCount;
    private Vector3[] vertices;
    private int[] tris;
    private Vector3[] triangleNormals;

    private Mesh mesh;

    private Node tree;

    void Awake()
    {
        mesh = GetComponent<MeshCollider>().sharedMesh;

        tris = mesh.triangles;
        vertices = mesh.vertices;

        vertexCount = mesh.vertices.Length;
        triangleCount = mesh.triangles.Length / 3;

        triangleNormals = new Vector3[triangleCount];

        for (int i = 0; i < tris.Length; i += 3)
        {
            Vector3 normal = Vector3.Cross((vertices[tris[i + 1]] - vertices[tris[i]]).normalized, (vertices[tris[i + 2]] - vertices[tris[i]]).normalized).normalized;

            triangleNormals[i / 3] = normal;
        }

        if (!drawMeshTreeOnStart)
            BuildTriangleTree();
    }

    void Start()
    {
        if (drawMeshTreeOnStart)
            BuildTriangleTree();
    }

    /// <summary>
    /// Returns the closest point on the mesh with respect to Vector3 point to
    /// </summary>
    public Vector3 ClosestPointOn(Vector3 to, float radius)
    {
        to = transform.InverseTransformPoint(to);

        List<int> triangles = new List<int>();

        FindClosestTriangles(tree, to, radius, triangles);

        Vector3 closest = ClosestPointOnTriangle(triangles.ToArray(), to);

        return transform.TransformPoint(closest);
    }

    void FindClosestTriangles(Node node, Vector3 to, float radius, List<int> triangles)
    {
        if (node.triangles == null)
        {
            if (PointDistanceFromPlane(node.partitionPoint, node.partitionNormal, to) <= radius)
            {
                FindClosestTriangles(node.positiveChild, to, radius, triangles);
                FindClosestTriangles(node.negativeChild, to, radius, triangles);
            }
            else if (PointAbovePlane(node.partitionPoint, node.partitionNormal, to))
            {
                FindClosestTriangles(node.positiveChild, to, radius, triangles);
            }
            else
            {
                FindClosestTriangles(node.negativeChild, to, radius, triangles);
            }
        }
        else
        {
            triangles.AddRange(node.triangles);
        }
    }

    Vector3 ClosestPointOnTriangle(int[] triangles, Vector3 to)
    {
        float shortestDistance = float.MaxValue;

        Vector3 shortestPoint = Vector3.zero;

        // Iterate through all triangles
        foreach (int triangle in triangles)
        {
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

    void BuildTriangleTree()
    {
        List<int> rootTriangles = new List<int>();

        for (int i = 0; i < tris.Length; i += 3)
        {
            rootTriangles.Add(i);
        }

        tree = new Node();

        RecursivePartition(rootTriangles, 0, tree);
    }

    void RecursivePartition(List<int> triangles, int depth, Node parent)
    {
        Vector3 partitionPoint = Vector3.zero;

        Vector3 maxExtents = new Vector3(-float.MaxValue, -float.MaxValue, -float.MaxValue);
        Vector3 minExtents = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

        foreach (int triangle in triangles)
        {
            partitionPoint += vertices[tris[triangle]] + vertices[tris[triangle + 1]] + vertices[tris[triangle + 2]];

            minExtents.x = Mathf.Min(minExtents.x, vertices[tris[triangle]].x, vertices[tris[triangle + 1]].x, vertices[tris[triangle + 2]].x);
            minExtents.y = Mathf.Min(minExtents.y, vertices[tris[triangle]].y, vertices[tris[triangle + 1]].y, vertices[tris[triangle + 2]].y);
            minExtents.z = Mathf.Min(minExtents.z, vertices[tris[triangle]].z, vertices[tris[triangle + 1]].z, vertices[tris[triangle + 2]].z);

            maxExtents.x = Mathf.Max(maxExtents.x, vertices[tris[triangle]].x, vertices[tris[triangle + 1]].x, vertices[tris[triangle + 2]].x);
            maxExtents.y = Mathf.Max(maxExtents.y, vertices[tris[triangle]].y, vertices[tris[triangle + 1]].y, vertices[tris[triangle + 2]].y);
            maxExtents.z = Mathf.Max(maxExtents.z, vertices[tris[triangle]].z, vertices[tris[triangle + 1]].z, vertices[tris[triangle + 2]].z);
        }

        // Centroid of all vertices
        partitionPoint /= vertexCount;

        // Better idea? Center of bounding box
        partitionPoint = minExtents + Math3d.SetVectorLength((maxExtents - minExtents), (maxExtents - minExtents).magnitude * 0.5f);

        Vector3 extentsMagnitude = new Vector3(Mathf.Abs(maxExtents.x - minExtents.x), Mathf.Abs(maxExtents.y - minExtents.y), Mathf.Abs(maxExtents.z - minExtents.z));

        Vector3 partitionNormal;

		if(extentsMagnitude.x >= extentsMagnitude.y && extentsMagnitude.x >= extentsMagnitude.z)
		{
			partitionNormal = Vector3.right;
		}
		else if(extentsMagnitude.y >= extentsMagnitude.x && extentsMagnitude.y >= extentsMagnitude.z)
		{
			partitionNormal = Vector3.up;
		}
		else
		{
			partitionNormal = Vector3.forward;
		}

        List<int> positiveTriangles;
        List<int> negativeTriangles;

        Split(triangles, partitionPoint, partitionNormal, out positiveTriangles, out negativeTriangles);

        parent.partitionNormal = partitionNormal;
        parent.partitionPoint = partitionPoint;

        Node posNode = new Node();
        parent.positiveChild = posNode;

        Node negNode = new Node();
        parent.negativeChild = negNode;

        if (positiveTriangles.Count < triangles.Count && positiveTriangles.Count > 3)
        {
            RecursivePartition(positiveTriangles, depth + 1, posNode);
        }
        else
        {
            posNode.triangles = positiveTriangles.ToArray();

            if (drawMeshTreeOnStart)
                DrawTriangleSet(posNode.triangles, DebugDraw.RandomColor());
        }

        if (negativeTriangles.Count < triangles.Count && negativeTriangles.Count > 3)
        {
            RecursivePartition(negativeTriangles, depth + 1, negNode);
        }
        else
        {
            negNode.triangles = negativeTriangles.ToArray();

            if (drawMeshTreeOnStart)
                DrawTriangleSet(negNode.triangles, DebugDraw.RandomColor());
        }

    }

    /// <summary>
    /// Splits a a set of input triangles by a partition plane into positive and negative sets, with triangles
    /// that are intersected by the partition plane being placed in both sets
    /// </summary>
    void Split(List<int> triangles, Vector3 partitionPoint, Vector3 partitionNormal, out List<int> positiveTriangles, out List<int> negativeTriangles)
    {
        positiveTriangles = new List<int>();
        negativeTriangles = new List<int>();

        foreach (int triangle in triangles)
        {
            bool firstPointAbove = PointAbovePlane(partitionPoint, partitionNormal, vertices[tris[triangle]]);
            bool secondPointAbove = PointAbovePlane(partitionPoint, partitionNormal, vertices[tris[triangle + 1]]);
            bool thirdPointAbove = PointAbovePlane(partitionPoint, partitionNormal, vertices[tris[triangle + 2]]);

            if (firstPointAbove && secondPointAbove && thirdPointAbove)
            {
                positiveTriangles.Add(triangle);
            }
            else if (!firstPointAbove && !secondPointAbove && !thirdPointAbove)
            {
                negativeTriangles.Add(triangle);
            }
            else
            {
                positiveTriangles.Add(triangle);
                negativeTriangles.Add(triangle);
            }
        }
    }

    bool PointAbovePlane(Vector3 planeOrigin, Vector3 planeNormal, Vector3 point)
    {
        return Vector3.Dot(point - planeOrigin, planeNormal) >= 0;
    }

    float PointDistanceFromPlane(Vector3 planeOrigin, Vector3 planeNormal, Vector3 point)
    {
        return Mathf.Abs(Vector3.Dot((point - planeOrigin), planeNormal));
    }

    /// <summary>
    /// Determines the closest point between a point and a triangle.
    /// Borrowed from RPGMesh class of the RPGController package for Unity, by fholm
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
    public static void ClosestPointOnTriangleToPoint(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, ref Vector3 point, out Vector3 result)
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

    void DrawTriangleSet(int[] triangles, Color color)
    {
        foreach (int triangle in triangles)
        {
            DebugDraw.DrawTriangle(vertices[tris[triangle]], vertices[tris[triangle + 1]], vertices[tris[triangle + 2]], color, transform);
        }
    }
}