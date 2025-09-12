using System;
using UnityEditor;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private int radius;
    [SerializeField] private int height;
    private Grid grid;

    [SerializeField] private int cellHeight;
    [SerializeField] private int cellSize;

    //[Range(1,120)] public int relaxTimes;
    [SerializeField] private int relaxTimes;

    [SerializeField] private ModuleLibrary moduleLibrary;
    [SerializeField] private Material moduleMaterial;

    public bool isView;
    public bool isLine;
    public bool isBit;
    
    public Transform addSphere;
    public Transform deleteSphere;

    private void Awake()
    {
        grid = new Grid(radius, height, cellSize, cellHeight, relaxTimes);
        moduleLibrary = Resources.Load<ModuleLibrary>("Data/ModuleLibrary");
        moduleLibrary = Instantiate(moduleLibrary);
        moduleMaterial = Resources.Load<Material>("Materials/ModuleMaterial");

        addSphere = GameObject.Find("Debug_AddSphere").transform;
        deleteSphere = GameObject.Find("Debug_DeleteSphere").transform;
    }

    private void Update()
    {
        /*if (relaxTimes > 0)
        {
            foreach (var subQuad in grid.subQuads)
            {
                subQuad.CalculateRelaxOffset();
            }

            foreach (var vertex in grid.vertices)
            {
                vertex.Relax();
            }

            relaxTimes -= 1;
        }*/
        foreach (var vertex in grid.vertices)
        {
            foreach (var vertex_Y in vertex.vertex_Ys)
            {
                if (!vertex_Y.isBoundary)
                {
                    //Debug.Log($"{!vertex_Y.isBoundary}");
                    if (!vertex_Y.isActive && Vector3.Distance(vertex_Y.worldPostion, addSphere.position) < 2f && !vertex_Y.isBoundary)
                    {
                        vertex_Y.isActive = true;
                    }
                    else if (vertex_Y.isActive && Vector3.Distance(vertex_Y.worldPostion, deleteSphere.position) < 2f)
                    {
                        vertex_Y.isActive = false;
                    }
                }
               
            }
        }

        foreach (var subQuad in grid.subQuads)
        {
            foreach (var subQuad_Cube in subQuad.subQuad_Cubes)
            {
                subQuad_Cube.UpdateBit();
                if (subQuad_Cube.pre_bit != subQuad_Cube.bit)
                {
                    UpdateSlot(subQuad_Cube);
                }
            }
        }

    }

    private void UpdateSlot(SubQuad_Cube subQuad_Cube)
    {
        string name = $"Slot_{grid.subQuads.IndexOf(subQuad_Cube.subQuad)}_{subQuad_Cube.y}";
        GameObject slot_GameObject;
        if (transform.Find(name))
        {
            slot_GameObject = transform.Find(name).gameObject;
        }
        else
        {
            slot_GameObject = null;
        }

        if (slot_GameObject == null)
        {
            if (!subQuad_Cube.bit.Equals("00000000") && !subQuad_Cube.bit.Equals("11111111"))
            {
                //Debug.Log($"2.{subQuad_Cube.bit}");
                slot_GameObject = new GameObject(name, typeof(Slot));
                slot_GameObject.transform.SetParent(transform);
                slot_GameObject.transform.localPosition = subQuad_Cube.centerPosition;
                Slot slot = slot_GameObject.GetComponent<Slot>();
                slot.Initialize(moduleLibrary, subQuad_Cube, moduleMaterial);
                slot.UpdateModule(slot.possibleModules[0]);
            }
        }
        else
        {
            Slot slot = slot_GameObject.GetComponent<Slot>();
            //Debug.Log($"1.{subQuad_Cube.bit}");
            if (subQuad_Cube.bit.Equals("00000000") || subQuad_Cube.bit.Equals("11111111"))
            {
                Destroy(slot_GameObject);
                Resources.UnloadUnusedAssets();
            }
            else
            {
                slot.ResetPossibleModules(moduleLibrary);
                slot.UpdateModule(slot.possibleModules[0]);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (grid != null)
        {
            #region 网格的平滑（Testcode）

            /*
                        Gizmos.color = Color.yellow;
                        foreach (var vertex in grid.hexes)
                        {
                            Gizmos.DrawSphere(vertex.currentPosition, 0.25f);
                        }

                        Gizmos.color = Color.green;
                        foreach (var triangle in grid.triangles)
                        {
                            Gizmos.DrawLine(triangle.a.currentPosition, triangle.b.currentPosition);
                            Gizmos.DrawLine(triangle.b.currentPosition, triangle.c.currentPosition);
                            Gizmos.DrawLine(triangle.c.currentPosition, triangle.a.currentPosition);
                            Gizmos.DrawSphere((triangle.a.currentPosition + triangle.b.currentPosition + triangle.c.currentPosition)/3,
                                0.05f);
                        }
                        Gizmos.color = Color.green;
                        foreach (var quad in grid.quads)
                        {
                            Gizmos.DrawLine(quad.a.currentPosition, quad.b.currentPosition);
                            Gizmos.DrawLine(quad.b.currentPosition, quad.c.currentPosition);
                            Gizmos.DrawLine(quad.c.currentPosition, quad.d.currentPosition);
                            Gizmos.DrawLine(quad.d.currentPosition, quad.a.currentPosition);
                        }
                        Gizmos.color = Color.red;
                        foreach (var mid in grid.mids)
                        {
                            Gizmos.DrawSphere(mid.initialPosition,0.2f);
                        }
                        Gizmos.color = Color.cyan;
                        foreach (var center in grid.centers)
                        {
                            Gizmos.DrawSphere(center.initialPosition,0.2f);
                        }
                        Gizmos.color = Color.green;
                        foreach (var subQuad in grid.subQuads)
                        {
                            Gizmos.DrawLine(subQuad.a.currentPosition,
                                subQuad.b.currentPosition);
                            Gizmos.DrawLine(subQuad.b.currentPosition,
                                subQuad.c.currentPosition);
                            Gizmos.DrawLine(subQuad.c.currentPosition,
                                subQuad.d.currentPosition);
                            Gizmos.DrawLine(subQuad.d.currentPosition,
                                subQuad.a.currentPosition);
                        }*/

            #endregion

            
            
            if (isView)
            {
                foreach (var vertex in grid.vertices)
                {
                    foreach (var vertex_Y in vertex.vertex_Ys)
                    {
                        if (vertex_Y.isActive)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(vertex_Y.worldPostion, 0.3f);
                        }
                        else
                        {
                            Gizmos.color = Color.gray;
                            Gizmos.DrawSphere(vertex_Y.worldPostion, 0.1f);
                        }
                    }
                } 
            }
           

            foreach (var subQuad in grid.subQuads)
            {
                foreach (var subQuad_Cube in subQuad.subQuad_Cubes)
                {
                    if (isLine)
                    {
                        Gizmos.color = Color.gray;
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[0].worldPostion, subQuad_Cube.vertex_Ys[1].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[1].worldPostion, subQuad_Cube.vertex_Ys[2].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[2].worldPostion, subQuad_Cube.vertex_Ys[3].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[3].worldPostion, subQuad_Cube.vertex_Ys[0].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[4].worldPostion, subQuad_Cube.vertex_Ys[5].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[5].worldPostion, subQuad_Cube.vertex_Ys[6].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[6].worldPostion, subQuad_Cube.vertex_Ys[7].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[7].worldPostion, subQuad_Cube.vertex_Ys[4].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[0].worldPostion, subQuad_Cube.vertex_Ys[4].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[1].worldPostion, subQuad_Cube.vertex_Ys[5].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[2].worldPostion, subQuad_Cube.vertex_Ys[6].worldPostion);
                        Gizmos.DrawLine(subQuad_Cube.vertex_Ys[3].worldPostion, subQuad_Cube.vertex_Ys[7].worldPostion);

                    }
                    
                    
                    if (isBit)
                    {
                        /*GUI.color = Color.blue;
                        Handles.Label(subQuad_Cube.centerPosition, subQuad_Cube.bit);*/
                        // 绘制中心位置（黄色球体）
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawSphere(subQuad_Cube.centerPosition, 0.1f);

                        // 绘制8个顶点（不同颜色区分上层/下层）
                        for (int i = 0; i < 8; i++)
                        {
                            if (subQuad_Cube.vertex_Ys[i] == null) continue;

                            // 上层顶点（0-3）用红色，下层顶点（4-7）用蓝色
                            Gizmos.color = i < 4 ? Color.red : Color.blue;
                            Gizmos.DrawSphere(subQuad_Cube.vertex_Ys[i].worldPostion, 0.05f);

                            // 显示顶点索引
                            UnityEditor.Handles.Label(
                                subQuad_Cube.vertex_Ys[i].worldPostion + Vector3.up * 0.1f, 
                                $"v{i}({(i < 4 ? "上" : "下")})"
                            );
                        }
                    }
                   
                }
            }

        }
    }
}