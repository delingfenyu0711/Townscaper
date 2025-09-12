using System.Collections.Generic;

public class Grid
{
    public static int radius;
    public static int height;
    public static int cellSize;
    public static int cellHeight;

    public readonly List<Vertex_hex> hexes = new List<Vertex_hex>();
    public readonly List<Vertex_mid> mids = new List<Vertex_mid>();
    public readonly List<Vertex_center> centers = new List<Vertex_center>();
    public readonly List<Vertex> vertices = new List<Vertex>();
    public readonly List<Triangle> triangles = new List<Triangle>();
    public readonly List<Edge> edges = new List<Edge>();
    public readonly List<Quad> quads = new List<Quad>();
    public readonly List<SubQuad> subQuads = new List<SubQuad>();

    public Grid(int radius, int height, int cellSize, int cellHeight, int relaxTimes)
    {
        Grid.radius = radius;
        Grid.cellSize = cellSize;
        Grid.height = height;
        Grid.cellHeight = cellHeight;

        Vertex_hex.Hex(hexes);
        Triangle.Triangle_Hex(hexes, mids, centers, edges, triangles);
        while (Triangle.HashNeighborTriangles(triangles))
        {
            Triangle.RandomlyMergeTriangles(mids, centers, edges, triangles, quads);
        }

        vertices.AddRange(hexes);
        vertices.AddRange(mids);
        vertices.AddRange(centers);

        foreach (var triangle in triangles)
        {
            triangle.Subdivide(subQuads);
        }

        foreach (var quad in quads)
        {
            quad.Subdivide(subQuads);
        }

        for (int i = 0; i < relaxTimes; i++)
        {
            foreach (var subQuad in subQuads)
            {
                subQuad.CalculateRelaxOffset();
            }

            foreach (var vertex in vertices)
            {
                vertex.Relax();
            }
        }

        foreach (var vertex in vertices)
        {
            vertex.BoundaryCheck();
            for (int i = 0; i < Grid.height + 1; i++)
            {
                vertex.vertex_Ys.Add(new Vertex_Y(vertex,i));
            }
        }

        foreach (var subQuad in subQuads)
        {
            for (int i = 0; i < Grid.height; i++)
            {
                subQuad.subQuad_Cubes.Add(new SubQuad_Cube(subQuad,i));
            }
        }
    }
}