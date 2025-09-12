﻿using System.Collections.Generic;

public class Edge
{
    public readonly HashSet<Vertex_hex> hexes;
    public readonly Vertex_mid mid;
    

    public Edge(Vertex_hex a, Vertex_hex b, List<Vertex_mid> mids, List<Edge> edges)
    {
        hexes = new HashSet<Vertex_hex> { a, b };
        edges.Add(this);
        mid = new Vertex_mid(this, mids);
    }

    public static Edge FindEdge(Vertex_hex a, Vertex_hex b, List<Edge> edges)
    {
        foreach (var edge in edges)
        {
            if (edge.hexes.Contains(a) && edge.hexes.Contains(b))
                return edge;
        }
        return null;
    }
}