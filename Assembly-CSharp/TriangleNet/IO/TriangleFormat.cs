// Decompiled with JetBrains decompiler
// Type: TriangleNet.IO.TriangleFormat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using TriangleNet.Geometry;

#nullable disable
namespace TriangleNet.IO
{
  public class TriangleFormat : IGeometryFormat, IMeshFormat
  {
    public InputGeometry Read(string filename)
    {
      string extension = Path.GetExtension(filename);
      switch (extension)
      {
        case ".node":
          return FileReader.ReadNodeFile(filename);
        case ".poly":
          return FileReader.ReadPolyFile(filename);
        default:
          throw new NotSupportedException("File format '" + extension + "' not supported.");
      }
    }

    public Mesh Import(string filename)
    {
      string extension = Path.GetExtension(filename);
      if (extension == ".node" || extension == ".poly" || extension == ".ele")
      {
        InputGeometry geometry;
        List<ITriangle> triangles;
        FileReader.Read(filename, out geometry, out triangles);
        if (geometry != null && triangles != null)
        {
          Mesh mesh = new Mesh();
          mesh.Load(geometry, triangles);
          return mesh;
        }
      }
      throw new NotSupportedException("Could not load '" + filename + "' file.");
    }

    public void Write(Mesh mesh, string filename)
    {
      FileWriter.WritePoly(mesh, Path.ChangeExtension(filename, ".poly"));
      FileWriter.WriteElements(mesh, Path.ChangeExtension(filename, ".ele"));
    }
  }
}
