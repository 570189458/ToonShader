using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlugTangentTools
{
    [MenuItem("Tools/模型平均法线写入切线数据")]
    public static void WriteAverageNormalToTangentTool()
    {
        MeshFilter[] meshFilters = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            Mesh mesh = meshFilter.sharedMesh;
            WriteAverageNormalToTangent(mesh);
        }

        SkinnedMeshRenderer[] skinnedMeshRenderers =
            Selection.activeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            Mesh mesh = skinnedMeshRenderer.sharedMesh;
            WriteAverageNormalToTangent(mesh);
        }
    }

    public static void WriteAverageNormalToTangent(Mesh mesh)
    {
        var averageNormalHash = new Dictionary<Vector3, Vector3>();
        // 遍历所有顶点，将顶点和法线加入字典中，如果该顶点有多个法线则计算所有法线的平均
        for (var i = 0; i < mesh.vertexCount; i++)
        {
            if (!averageNormalHash.ContainsKey(mesh.vertices[i]))
            {
                averageNormalHash.Add(mesh.vertices[i], mesh.normals[i]);
            }
            else
            {
                averageNormalHash[mesh.vertices[i]] =
                    (averageNormalHash[mesh.vertices[i]] + mesh.normals[i]).normalized;
            }
        }
        
        // 将法线数据作为切线数据
        var averageNormals = new Vector3[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            averageNormals[i] = averageNormalHash[mesh.vertices[i]];
        }

        var tangents = new Vector4[mesh.vertexCount];
        for (int i = 0; i < mesh.vertexCount; i++)
        {
            tangents[i] = new Vector4(averageNormals[i].x, averageNormals[i].y, averageNormals[i].z, 0);
        }

        mesh.tangents = tangents;
    }
}
