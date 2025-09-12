using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ModuleLibrary")]
public class ModuleLibrary : ScriptableObject
{
    [SerializeField] private GameObject importedModules;
    private Dictionary<string, List<Module>> moduleLibrary = new Dictionary<string, List<Module>>();

    private void Awake()
    {
        importedModules = Resources.Load<GameObject>("Fbx/Townscaper");
        ImportedModule();
    }


    public void ImportedModule()
    {
        moduleLibrary.Clear();
        for (int i = 1; i < 256; i++)
        {
            moduleLibrary.Add(Convert.ToString(i, 2).PadLeft(8, '0'), new List<Module>());
        }

        foreach (Transform child in importedModules.transform)
        {
            Mesh mesh = child.GetComponent<MeshFilter>().sharedMesh;
            string name = child.name;
            moduleLibrary[name].Add(new Module(name, mesh, 0, false));

            if (!RotationEqualCheck(name))
            {
                moduleLibrary[RotateName(name, 1)].Add(new Module(RotateName(name, 1), mesh, 0, false));
                if (!RotationTwiceEqualCheck(name))
                {
                    moduleLibrary[RotateName(name, 2)].Add(new Module(RotateName(name, 2), mesh, 0, false));
                    moduleLibrary[RotateName(name, 3)].Add(new Module(RotateName(name, 3), mesh, 0, false));
                    if (!FlipRotationEqualCheck(name))
                    {
                        moduleLibrary[FlipName(name)].Add(new Module(FlipName(name), mesh, 0, true));
                        moduleLibrary[RotateName(FlipName(name), 1)].Add(new Module(FlipName(name), mesh, 1, true));
                        moduleLibrary[RotateName(FlipName(name), 2)].Add(new Module(FlipName(name), mesh, 2, true));
                        moduleLibrary[RotateName(FlipName(name), 3)].Add(new Module(FlipName(name), mesh, 3, true));
                    }
                }
            }
        }
    }

    private string RotateName(string name, int time)
    {
        string result = name;
        for (int i = 0; i < time; i++)
        {
            result = result[3] + result.Substring(0, 3) + result[7] + result.Substring(4, 3);
        }

        return result;
    }

    private string FlipName(string name)
    {
        return name[3].ToString() + name[2] + name[1] + name[0] + name[7] + name[6] + name[5] + name[4];
    }

    private bool RotationEqualCheck(string name)
    {
        // return name[0] == name[1] && name[1] == name[2] && name[2] == name[3] && name[3] == name[4] &&
        //        name[4] == name[5] && name[5] == name[6] && name[6] == name[7];
        return name.Length == 8 && name.All(c => c == name[0]);
    }

    private bool RotationTwiceEqualCheck(string name)
    {
        //return name[0] == name[2] && name[1] == name[3] && name[4] == name[6] && name[5] == name[7];
        // 定义需要检查的索引对
        var indexPairs = new (int, int)[] { (0, 2), (1, 3), (4, 6), (5, 7) };
        // 检查所有索引对是否都满足相等条件
        return indexPairs.All(pair => name[pair.Item1] == name[pair.Item2]);
    }

    private bool FlipRotationEqualCheck(string name)
    {
        /*
        string symmetry_vertical = name[3].ToString() + name[2] + name[1] + name[0] + name[7] +  name[6] + name[5] +  name[4];
        string symmetry_horizontal = name[1].ToString() + name[0] + name[3] + name[2] + name[5] +  name[4] + name[7] +  name[6];
        string symmetry_02 = name[0].ToString() + name[3] + name[2] + name[1] + name[4] +  name[7] + name[6] +  name[5];
        string symmetry_13 = name[2].ToString() + name[1] + name[0] + name[3] + name[6] +  name[5] + name[4] +  name[7];
        return name == symmetry_horizontal || name == symmetry_vertical || name == symmetry_02 || name == symmetry_13;
        */
        // 先检查名称长度是否合法（避免索引越界）
        if (name.Length != 8)
        {
            Debug.LogError($"模块名称长度必须为8，当前名称: {name}");
            return false;
        }

        // 直接通过字符索引比较，避免创建中间字符串
        // 垂直对称 (上下翻转)
        bool isVerticalSymmetry = 
            name[0] == name[3] && 
            name[1] == name[2] && 
            name[4] == name[7] && 
            name[5] == name[6];

        // 水平对称 (左右翻转)
        bool isHorizontalSymmetry = 
            name[0] == name[1] && 
            name[2] == name[3] && 
            name[4] == name[5] && 
            name[6] == name[7];

        // 对称02
        bool isSymmetry02 = 
            name[1] == name[3] && 
            name[2] == name[2] && // 自身相等，可省略
            name[5] == name[7] && 
            name[6] == name[6];   // 自身相等，可省略

        // 对称13
        bool isSymmetry13 = 
            name[0] == name[2] && 
            name[1] == name[1] && // 自身相等，可省略
            name[4] == name[6] && 
            name[5] == name[5];   // 自身相等，可省略

        // 简化后的有效判断（去掉自身相等的冗余条件）
        isSymmetry02 = name[1] == name[3] && name[5] == name[7];
        isSymmetry13 = name[0] == name[2] && name[4] == name[6];

        return isHorizontalSymmetry || isVerticalSymmetry || isSymmetry02 || isSymmetry13;
    }

    public List<Module> GetModules(string name)
    {
        List<Module> result;
        if (moduleLibrary.TryGetValue(name, out result))
        {
            return result;
        }
        return null;
    }
}
   