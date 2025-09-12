using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Triangle
{
    public readonly Vertex_hex a;
    public readonly Vertex_hex b;
    public readonly Vertex_hex c;

    public readonly Vertex_hex[] vertices;

    public readonly Edge ab;
    public readonly Edge bc;
    public readonly Edge ac;
    public readonly Edge[] edges;

    public readonly Vertex_triangleCenter center;

    public Triangle(Vertex_hex a, Vertex_hex b, Vertex_hex c, List<Vertex_mid> mids, List<Vertex_center> centers,
        List<Edge> edges,
        List<Triangle> triangles)
    {
        this.a = a;
        this.b = b;
        this.c = c;

        vertices = new Vertex_hex[]
        {
            a, b, c
        };

        //创建边线
        ab = Edge.FindEdge(a, b, edges);
        bc = Edge.FindEdge(b, c, edges);
        ac = Edge.FindEdge(a, c, edges);
        if (ab == null)
        {
            ab = new Edge(a, b, mids, edges);
        }

        if (bc == null)
        {
            bc = new Edge(b, c, mids, edges);
        }

        if (ac == null)
        {
            ac = new Edge(a, c, mids, edges);
        }

        this.edges = new Edge[]
        {
            ab, bc, ac
        };

        center = new Vertex_triangleCenter(this);
        triangles.Add(this);
        centers.Add(center);

    }

    public Triangle(Vertex_hex a, Vertex_hex b, Vertex_hex c, List<Triangle> triangles)
    {
        this.a = a;
        this.b = b;
        this.c = c;


        triangles.Add(this);
    }


    public static void Triangles_Ring(int radius, List<Vertex_hex> hexes, List<Vertex_mid> mids,
        List<Vertex_center> centers, List<Edge> edges,
        List<Triangle> triangles)
    {
        List<Vertex_hex> inner = Vertex_hex.GrabRing(radius - 1, hexes);
        List<Vertex_hex> outer = Vertex_hex.GrabRing(radius, hexes);
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                //创建两个顶点在外圈，一个顶点在内圈的三角形
                Vertex_hex a = outer[i * radius + j];
                Vertex_hex b = outer[(i * radius + j + 1) % outer.Count];
                Vertex_hex c = inner[(i * (radius - 1) + j) % inner.Count];
                new Triangle(a, b, c, mids, centers, edges, triangles);
                //创建一个顶点在外圈，两个顶点在外圈的三角形
                if (j > 0)
                {
                    //因为j可能出现大于count，所以需要减一
                    Vertex_hex d = inner[i * (radius - 1) + j - 1];
                    new Triangle(a, c, d, mids, centers, edges, triangles);
                }
            }
        }
    }

    public static void Triangle_Hex(List<Vertex_hex> vertices, List<Vertex_mid> mids, List<Vertex_center> centers,
        List<Edge> edges,
        List<Triangle> triangles)
    {
        //因为当半径为0时不会生成顶点所以循环得从1开始
        for (int i = 1; i <= Grid.radius; i++)
        {
            Triangles_Ring(i, vertices, mids, centers, edges, triangles);
        }
    }

    //相邻三角形
    public bool IsNeighbor(Triangle target)
    {
        HashSet<Edge> intersection = new HashSet<Edge>(edges);
        intersection.IntersectWith(target.edges);
        return intersection.Count == 1;
    }

    public List<Triangle> FindAllNeighborTriangles(List<Triangle> triangles)
    {
        List<Triangle> result = new List<Triangle>();
        foreach (var triangle in triangles)
        {
            if (IsNeighbor(triangle))
            {
                result.Add(triangle);
            }
        }

        return result;
    }

    public Edge NeighborEdge(Triangle neighbor)
    {
        HashSet<Edge> intersection = new HashSet<Edge>(edges);
        intersection.IntersectWith(neighbor.edges);
        return intersection.Single();
    }

    public Vertex_hex IsolatedVertex_Self(Triangle neighbor)
    {
        HashSet<Vertex_hex> exceptions = new HashSet<Vertex_hex>(vertices);
        exceptions.ExceptWith(NeighborEdge(neighbor).hexes);
        return exceptions.Single();
    }

    public Vertex_hex IsolatedVertex_Neighbor(Triangle neighbor)
    {
        HashSet<Vertex_hex> exceptions = new HashSet<Vertex_hex>(neighbor.vertices);
        exceptions.ExceptWith(NeighborEdge(neighbor).hexes);
        return exceptions.Single();
    }

    public void MergeNeighborTriangles(Triangle neighbor, List<Vertex_mid> mids, List<Vertex_center> centers,
        List<Edge> edges,
        List<Triangle> triangles, List<Quad> quads)
    {
        Vertex_hex a = IsolatedVertex_Self(neighbor);
        Vertex_hex b = vertices[(Array.IndexOf(vertices, a) + 1) % 3];
        Vertex_hex c = IsolatedVertex_Neighbor(neighbor);
        Vertex_hex d = neighbor.vertices[(Array.IndexOf(neighbor.vertices, c) + 1) % 3];
        Quad quad = new Quad(a, b, c, d, centers, edges, quads);
        edges.Remove(NeighborEdge(neighbor));
        mids.Remove(NeighborEdge(neighbor).mid);
        centers.Remove(this.center);
        centers.Remove(neighbor.center);
        triangles.Remove(this);
        triangles.Remove(neighbor);
    }

    public static bool HashNeighborTriangles(List<Triangle> triangles)
    {
        foreach (var triangle in triangles)
        {
            foreach (var a in triangles)
            {
                foreach (var b in triangles)
                {
                    if (a.IsNeighbor(b))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static void RandomlyMergeTriangles(List<Vertex_mid> mids, List<Vertex_center> centers, List<Edge> edges,
        List<Triangle> triangles,
        List<Quad> quads)
    {
        int randomIndex = UnityEngine.Random.Range(0, triangles.Count);
        List<Triangle> neighbors = triangles[randomIndex].FindAllNeighborTriangles(triangles);
        if (neighbors.Count != 0)
        {
            int randomNeighborIndex = UnityEngine.Random.Range(0, neighbors.Count);
            triangles[randomIndex]
                .MergeNeighborTriangles(neighbors[randomNeighborIndex], mids, centers, edges, triangles, quads);
        }
    }

    public void Subdivide(List<SubQuad> subQuads)
    {
        SubQuad quad_a = new SubQuad(a, ab.mid, center, ac.mid, subQuads);
        SubQuad quad_b = new SubQuad(b, bc.mid, center, ab.mid, subQuads);
        SubQuad quad_c = new SubQuad(c, ac.mid, center, bc.mid, subQuads);
    }
}