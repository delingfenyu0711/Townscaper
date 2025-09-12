
using System.Collections.Generic;

public class Quad
{
    public readonly Vertex_hex a;
    public readonly Vertex_hex b;
    public readonly Vertex_hex c;
    public readonly Vertex_hex d;
    
    public readonly Edge ab;
    public readonly Edge bc;
    public readonly Edge cd;
    public readonly Edge ad;
    
    public readonly Vertex_quadCenter center;

    public Quad(Vertex_hex a, Vertex_hex b, Vertex_hex c, Vertex_hex d, List<Vertex_center> centers,List<Edge> edges,List<Quad> quads)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        
        ab = Edge.FindEdge(a,b,edges);
        bc = Edge.FindEdge(b,c,edges);
        cd = Edge.FindEdge(c,d,edges);
        ad = Edge.FindEdge(a,d,edges);

        center = new Vertex_quadCenter(this);
        quads.Add(this);
        centers.Add(center);
    }
    
    public void Subdivide(List<SubQuad> subQuads)
    {
        SubQuad quad_a = new SubQuad(a, ab.mid, center,ad.mid,subQuads);
        SubQuad quad_b = new SubQuad(b, bc.mid, center,ab.mid,subQuads);
        SubQuad quad_c = new SubQuad(c, cd.mid, center,bc.mid,subQuads);
        SubQuad quad_d = new SubQuad(d, ad.mid, center,cd.mid,subQuads);
    }
    
}