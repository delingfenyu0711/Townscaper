using System;
using System.Collections.Generic;
using UnityEngine;


public class SubQuad
{
    public readonly Vertex_hex a;
    public readonly Vertex_mid b;
    public readonly Vertex_center c;
    public readonly Vertex_mid d;
    
    public List<SubQuad_Cube> subQuad_Cubes = new List<SubQuad_Cube>();
    public SubQuad(Vertex_hex a ,Vertex_mid b,Vertex_center c,Vertex_mid d,List<SubQuad> subQuads)
    {
        this.a = a;  
        this.b = b;
        this.c = c;
        this.d = d;
        
        subQuads.Add(this);
    }

    public void CalculateRelaxOffset()
    {
        Vector3 center = (a.currentPosition + b.currentPosition + c.currentPosition + d.currentPosition) / 4;
        Vector3 vector_a = (a.currentPosition
                            + Quaternion.AngleAxis(-90, Vector3.up) * (b.currentPosition - center) + center
                            + Quaternion.AngleAxis(-180, Vector3.up) * (c.currentPosition - center) + center
                            + Quaternion.AngleAxis(-270, Vector3.up) * (d.currentPosition - center) + center) / 4;
        
        Vector3 vector_b = Quaternion.AngleAxis(90,Vector3.up) * (vector_a - center) + center;
        Vector3 vector_c = Quaternion.AngleAxis(180,Vector3.up) * (vector_a - center) + center;
        Vector3 vector_d = Quaternion.AngleAxis(270,Vector3.up) * (vector_a - center) + center;
        
        a.offset += (vector_a - a.currentPosition) * 0.1f;
        b.offset += (vector_b - b.currentPosition) * 0.1f;
        c.offset += (vector_c - c.currentPosition) * 0.1f;
        d.offset += (vector_d - d.currentPosition) * 0.1f;
    }

    public Vector3 GetCenterPosition()
    {
        return (a.currentPosition + b.currentPosition + c.currentPosition + d.currentPosition) / 4;
    }
    
}

public class SubQuad_Cube
{
    public readonly SubQuad subQuad;
    public readonly int y;
    public string bit = "00000000";
    public readonly Vector3 centerPosition;
    public readonly Vertex_Y[] vertex_Ys = new Vertex_Y[8];
    public string pre_bit = "00000000";

    public SubQuad_Cube(SubQuad subQuad, int y)
    {
        this.subQuad = subQuad;
        this.y = y;

        // 计算中心位置，保留原有计算公式
        centerPosition = subQuad.GetCenterPosition() + 
                         Vector3.up * Grid.cellHeight * (y + 0.5f);

        // 上层顶点 (y+1) - 保持原有序列和赋值逻辑
        vertex_Ys[0] = subQuad.a.vertex_Ys[y + 1];
        vertex_Ys[1] = subQuad.b.vertex_Ys[y + 1];
        vertex_Ys[2] = subQuad.c.vertex_Ys[y + 1];
        vertex_Ys[3] = subQuad.d.vertex_Ys[y + 1];

        // 下层顶点 (y) - 分组注释提高可读性，不改变赋值逻辑
        vertex_Ys[4] = subQuad.a.vertex_Ys[y];
        vertex_Ys[5] = subQuad.b.vertex_Ys[y];
        vertex_Ys[6] = subQuad.c.vertex_Ys[y];
        vertex_Ys[7] = subQuad.d.vertex_Ys[y];
    }



    public void UpdateBit()
    {
        pre_bit = bit;
        // 使用StringBuilder提升字符串拼接性能
        System.Text.StringBuilder result = new System.Text.StringBuilder(8);
    
        // 循环处理8个顶点，避免重复代码
        for (int i = 0; i < 8; i++)
        {
            // 增加空引用检查，防止NullReferenceException
            if (vertex_Ys[i] == null)
            {
                result.Append('0');
                continue;
            }
        
            // 可在此处添加边界顶点过滤逻辑（如果需要）
            // bool isActive = !vertex_Ys[i].isBoundary && vertex_Ys[i].isActive;
            bool isActive = vertex_Ys[i].isActive;
        
            // 使用三元运算符精简判断逻辑
            result.Append(isActive ? '1' : '0');
        }
    
        bit = result.ToString();
    }
}