﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ExporterRuntimeS : ScriptableObject
{
  private static int vertexOffset;
  private static int normalOffset;
  private static int uvOffset;

  private static string MeshToString(MeshFilter mf, Dictionary<string, OMATRuntime> materialList)
  {
    Mesh sharedMesh = mf.sharedMesh;
    Material[] sharedMaterials = mf.GetComponent<Renderer>().sharedMaterials;
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("g ").Append(mf.name).Append("\n");
    foreach (Vector3 vertex in sharedMesh.vertices)
    {
      Vector3 vector3 = mf.transform.TransformPoint(vertex);
      stringBuilder.Append(string.Format("v {0} {1} {2}\n", (float) -(double) vector3.x, vector3.y, vector3.z));
    }
    stringBuilder.Append("\n");
    foreach (Vector3 normal in sharedMesh.normals)
    {
      Vector3 vector3 = mf.transform.TransformDirection(normal);
      stringBuilder.Append(string.Format("vn {0} {1} {2}\n", (float) -(double) vector3.x, vector3.y, vector3.z));
    }
    stringBuilder.Append("\n");
    foreach (Vector2 vector2 in sharedMesh.uv)
    {
      Vector3 vector3 = vector2;
      stringBuilder.Append(string.Format("vt {0} {1}\n", vector3.x, vector3.y));
    }
    for (int submesh = 0; submesh < sharedMesh.subMeshCount; ++submesh)
    {
      stringBuilder.Append("\n");
      stringBuilder.Append("usemtl ").Append(sharedMaterials[submesh].name).Append("\n");
      stringBuilder.Append("usemap ").Append(sharedMaterials[submesh].name).Append("\n");
      try
      {
        OMATRuntime omatRuntime = new OMATRuntime();
        omatRuntime.name = sharedMaterials[submesh].name;
        omatRuntime.textureName = null;
        materialList.Add(omatRuntime.name, omatRuntime);
      }
      catch (ArgumentException ex)
      {
      }
      int[] triangles = sharedMesh.GetTriangles(submesh);
      for (int index = 0; index < triangles.Length; index += 3)
        stringBuilder.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", triangles[index] + 1 + vertexOffset, triangles[index + 1] + 1 + normalOffset, triangles[index + 2] + 1 + uvOffset));
    }
    vertexOffset += sharedMesh.vertices.Length;
    normalOffset += sharedMesh.normals.Length;
    uvOffset += sharedMesh.uv.Length;
    return stringBuilder.ToString();
  }

  private static void Clear()
  {
    vertexOffset = 0;
    normalOffset = 0;
    uvOffset = 0;
  }

  private static Dictionary<string, OMATRuntime> PrepareFileWrite()
  {
    Clear();
    return new Dictionary<string, OMATRuntime>();
  }

  private static void MaterialsToFile(
    Dictionary<string, OMATRuntime> materialList,
    string folder,
    string filename)
  {
    using (StreamWriter streamWriter = new StreamWriter(folder + "/" + filename + ".mtl"))
    {
      foreach (KeyValuePair<string, OMATRuntime> material in materialList)
      {
        streamWriter.Write("\n");
        streamWriter.Write("newmtl {0}\n", material.Key);
        streamWriter.Write("Ka  1.0 1.0 1.0\n");
        streamWriter.Write("Kd  1.0 1.0 1.0\n");
        streamWriter.Write("Ks  1.0 1.0 1.0\n");
        streamWriter.Write("d  1.0\n");
        streamWriter.Write("Ns  0.0\n");
        streamWriter.Write("illum 2\n");
        if (material.Value.textureName != null)
        {
          string str1 = material.Value.textureName;
          int num = str1.LastIndexOf('/');
          if (num >= 0)
            str1 = str1.Substring(num + 1).Trim();
          string str2 = str1;
          string destFileName = folder + "/" + str1;
          File.Copy(material.Value.textureName, destFileName);
          streamWriter.Write("map_Kd {0}", str2);
        }
        streamWriter.Write("\n\n\n");
      }
    }
  }

  private static void MeshesToFile(MeshFilter[] mf, string folder, string filename)
  {
    Dictionary<string, OMATRuntime> materialList = PrepareFileWrite();
    using (StreamWriter streamWriter = new StreamWriter(folder + "/" + filename + ".obj"))
    {
      streamWriter.Write("mtllib ./" + filename + ".mtl\n");
      for (int index = 0; index < mf.Length; ++index)
        streamWriter.Write(MeshToString(mf[index], materialList));
    }
    MaterialsToFile(materialList, folder, filename);
  }

  public static void ExportGOToOBJ(Transform[] selection, string mainPath, string filename)
  {
    mainPath = mainPath + "/" + filename + "/";
    Directory.CreateDirectory(mainPath);
    int num1 = 0;
    ArrayList arrayList = new ArrayList();
    for (int index = 0; index < selection.Length; ++index)
    {
      foreach (object componentsInChild in selection[index].GetComponentsInChildren(typeof (MeshFilter)))
      {
        ++num1;
        arrayList.Add(componentsInChild);
      }
    }
    if (num1 > 0)
    {
      MeshFilter[] mf = new MeshFilter[arrayList.Count];
      for (int index = 0; index < arrayList.Count; ++index)
        mf[index] = (MeshFilter) arrayList[index];
      if (filename.Equals(""))
        filename = selection[0].gameObject.name;
      int num2 = filename.LastIndexOf('/');
      if (num2 >= 0)
        filename = filename.Substring(num2 + 1).Trim();
      MeshesToFile(mf, mainPath, filename);
      Debug.Log("Exported: " + mainPath + "/" + filename);
    }
    else
      Debug.Log("Error exporting. Make sure you selected the object.");
  }
}
