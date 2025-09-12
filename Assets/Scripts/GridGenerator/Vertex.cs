using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vertex
{
    public Vector3 initialPosition;
    public Vector3 currentPosition;
    public Vector3 offset = Vector3.zero;
    
    public List<Vertex_Y> vertex_Ys = new List<Vertex_Y>();
    
    public bool isBoundary = false;
    public void Relax()
    {
        currentPosition = initialPosition + offset;
    }
    
    public void BoundaryCheck()
    {
        //判断是否是边界Hex
        bool isBoundaryHex = this is Vertex_hex && ((Vertex_hex)this).coord.radius == Grid.radius;
        //判断是否为边界Mid
        bool isBoundaryMid =  this is Vertex_mid && ((Vertex_mid)this).edge.hexes.ToArray()[0].coord.radius == Grid.radius &&
                              ((Vertex_mid)this).edge.hexes.ToArray()[1].coord.radius == Grid.radius;
        isBoundary = isBoundaryHex || isBoundaryMid;
    }
}

/// <summary>
/// 基于六边形的三维坐标系
/// </summary>
public class Coord
{
    public readonly int q;
    public readonly int r;
    public readonly int s;
    /// <summary>
    /// 半径
    /// </summary>
    public readonly int radius;
    /// <summary>
    /// 世界坐标
    /// </summary>
    public readonly Vector3 worldPostion;

    

    /// <summary>
    /// 构造函数进行赋值
    /// </summary>
    /// <param name="q">三维坐标系的x</param>
    /// <param name="r">三维坐标系的y</param>
    /// <param name="s">三维坐标系的z</param>
    public Coord(int q, int r, int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
        this.radius = Mathf.Max(Mathf.Abs(q), Mathf.Abs(r), Mathf.Abs(s));
        worldPostion = WorldPosition();
    }

    /// <summary>
    /// 世界坐标系转换
    /// </summary>
    /// <returns>返回unity坐标</returns>
    public Vector3 WorldPosition()
    {
        return new Vector3(q * Mathf.Sqrt(3) / 2, 0, -(float)r - ((float)q / 2)) * 2 * Grid.cellSize;
    }

    /// <summary>
    /// 六边形坐标系的六个方向
    /// </summary>
    public static Coord[] directions = new Coord[]
    {
        new Coord(0, 1, -1),
        new Coord(-1, 1, 0),
        new Coord(-1, 0, 1),
        new Coord(0, -1, 1),
        new Coord(1, -1, 0),
        new Coord(1, 0, -1)
    };
    
    

    
    public static Coord Direction(int direction)
    {
        return Coord.directions[direction];
    }
    
    public Coord Add(Coord coord)
    {
        return new Coord(q + coord.q, r + coord.r, s + coord.s);
    }

    public Coord Neighbor(int direction)
    {
        return Add(Direction(direction));
    }

    public Coord Scale(int k)
    {
        return new Coord(q * k, r * k, s * k);
    }

    /// <summary>
    /// 坐标系环
    /// </summary>
    /// <param name="radius">生成六边形的半径</param>
    /// <returns>坐标系集合</returns>
    public static List<Coord> Coord_Ring(int radius)
    {
        List<Coord> result = new List<Coord>();
        if (radius == 0)
        {
            result.Add(new Coord(0, 0,0));
        }
        else
        {
            Coord coord = Coord.Direction(4).Scale(radius);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    result.Add(coord);
                    coord = coord.Neighbor(i);
                }
            }
        }
        return result;
    }

    
    public static List<Coord> Coord_Hex()
    {
        List<Coord> result = new List<Coord>();
        for (int i = 0; i <= Grid.radius; i++)
        {
            result.AddRange(Coord_Ring(i));
        }
        return result;
    }
}

public class Vertex_hex : Vertex
{
    public readonly Coord coord;

    public Vertex_hex(Coord coord)
    {
        this.coord = coord;
        initialPosition = coord.worldPostion;
        currentPosition = initialPosition;
    }

    public static void Hex(List<Vertex_hex> vertices)
    {
        foreach (var coord in Coord.Coord_Hex())
        {
            vertices.Add(new Vertex_hex(coord));
        }
    }

    public static List<Vertex_hex> GrabRing(int radius, List<Vertex_hex> vertices)
    {
        if (radius == 0)
            return vertices.GetRange(0, 1);
        return vertices.GetRange(radius * (radius - 1) * 3 + 1, radius * 6);
    }
}

public class Vertex_mid : Vertex
{
    public readonly Edge edge;
    public Vertex_mid(Edge edge, List<Vertex_mid> mids)
    {
        this.edge = edge;
        Vertex_hex a = edge.hexes.ToArray()[0];
        Vertex_hex b = edge.hexes.ToArray()[1];
        mids.Add(this);
        initialPosition = (a.initialPosition + b.initialPosition) / 2;
        currentPosition = initialPosition;
    }
}

public class Vertex_center : Vertex
{
    
}

public class Vertex_triangleCenter : Vertex_center
{
    public Vertex_triangleCenter(Triangle triangle)
    {
        initialPosition = (triangle.a.initialPosition + triangle.b.initialPosition +  triangle.c.initialPosition) / 3;
        currentPosition = initialPosition;
    }
}

public class Vertex_quadCenter : Vertex_center
{
    public Vertex_quadCenter(Quad quad)
    {
        initialPosition = (quad.a.initialPosition + quad.b.initialPosition +  quad.c.initialPosition + quad.d.initialPosition) / 4;
        currentPosition = initialPosition;
    }
}

public class Vertex_Y
{
    public readonly Vertex vertex;
    public readonly int y;
    public readonly bool isBoundary;
    public bool isActive;
    public readonly Vector3 worldPostion;

    public Vertex_Y(Vertex vertex, int y)
    {
        this.vertex = vertex;
        this.y = y;
        isBoundary = vertex.isBoundary || y == Grid.height || y == 0;
        worldPostion = vertex.currentPosition + Vector3.up * (y * Grid.cellHeight);
    }
}