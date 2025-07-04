﻿using System;
using System.Collections.Generic;
using System.IO;
using TriangleNet.Geometry;

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
        FileReader.Read(filename, out InputGeometry geometry, out List<ITriangle> triangles);
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
