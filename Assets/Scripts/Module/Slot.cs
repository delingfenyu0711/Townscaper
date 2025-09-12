using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] public List<Module> possibleModules;
    public SubQuad_Cube subQuad_cube;
    
    public GameObject module;

    public Material material;
    
    private void Awake()
    {
        module = new GameObject("Module",typeof(MeshFilter),typeof(MeshRenderer));
        module.transform.SetParent(transform);
        module.transform.localPosition = Vector3.zero;
    }

    public void Initialize(ModuleLibrary moduleLibrary,SubQuad_Cube subQuad_cube,Material material)
    {
        this.subQuad_cube = subQuad_cube;
        ResetPossibleModules(moduleLibrary);
        this.material = material;
    }
    public void ResetPossibleModules(ModuleLibrary moduleLibrary)
    {
        possibleModules = moduleLibrary.GetModules(subQuad_cube.bit);
    }

    private void RotateModule(Mesh mesh,int rotation)
    {
        if (rotation != 0)
        {
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Quaternion.AngleAxis(90 * rotation,Vector3.up) * vertices[i];
            }
            mesh.vertices = vertices;
        }
    }

    private void FlipModule(Mesh mesh,bool flip)
    {
        if (flip)
        {
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(-vertices[i].x, vertices[i].y, vertices[i].z);
            }
            mesh.vertices = vertices;
            mesh.triangles = mesh.triangles.Reverse().ToArray();
        }
    }

    private void DeformModule(Mesh mesh,SubQuad_Cube subQuad_cube)
    {
        Vector3[] vertices = mesh.vertices;
        SubQuad subQuad = subQuad_cube.subQuad;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 ad_x = Vector3.Lerp(subQuad.a.currentPosition, subQuad.d.currentPosition, (vertices[i].x + 0.5f));
            Vector3 ac_x = Vector3.Lerp(subQuad.b.currentPosition, subQuad.c.currentPosition, (vertices[i].x + 0.5f));
            vertices[i] = Vector3.Lerp(ad_x, ac_x, ((vertices[i].z + 0.5f))) + Vector3.up * vertices[i].y * Grid.cellHeight - subQuad.GetCenterPosition();
        }
        mesh.vertices = vertices;
    }

    public void UpdateModule(Module module)
    {
        this.module.GetComponent<MeshFilter>().mesh = module.mesh;
        FlipModule(this.module.GetComponent<MeshFilter>().mesh,module.flip);
        RotateModule(this.module.GetComponent<MeshFilter>().mesh,module.rotation);
        DeformModule(this.module.GetComponent<MeshFilter>().mesh,subQuad_cube);
        this.module.GetComponent<MeshRenderer>().material = material;
        this.module.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        this.module.GetComponent<MeshFilter>().mesh.RecalculateBounds();
        
    }
}