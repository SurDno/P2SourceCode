using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TriangleNet.Data;
using TriangleNet.Geometry;

namespace TriangleNet.IO
{
  public static class FileWriter
  {
    private static NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;

    public static void Write(Mesh mesh, string filename)
    {
      FileWriter.WritePoly(mesh, Path.ChangeExtension(filename, ".poly"));
      FileWriter.WriteElements(mesh, Path.ChangeExtension(filename, ".ele"));
    }

    public static void WriteNodes(Mesh mesh, string filename)
    {
      using (StreamWriter writer = new StreamWriter(filename))
        FileWriter.WriteNodes(writer, mesh);
    }

    private static void WriteNodes(StreamWriter writer, Mesh mesh)
    {
      int num = mesh.vertices.Count;
      Behavior behavior = mesh.behavior;
      if (behavior.Jettison)
        num = mesh.vertices.Count - mesh.undeads;
      if (writer == null)
        return;
      writer.WriteLine("{0} {1} {2} {3}", (object) num, (object) mesh.mesh_dim, (object) mesh.nextras, behavior.UseBoundaryMarkers ? (object) "1" : (object) "0");
      if (mesh.numbering == NodeNumbering.None)
        mesh.Renumber();
      if (mesh.numbering == NodeNumbering.Linear)
      {
        FileWriter.WriteNodes(writer, (IEnumerable<Vertex>) mesh.vertices.Values, behavior.UseBoundaryMarkers, mesh.nextras, behavior.Jettison);
      }
      else
      {
        Vertex[] nodes = new Vertex[mesh.vertices.Count];
        foreach (Vertex vertex in mesh.vertices.Values)
          nodes[vertex.id] = vertex;
        FileWriter.WriteNodes(writer, (IEnumerable<Vertex>) nodes, behavior.UseBoundaryMarkers, mesh.nextras, behavior.Jettison);
      }
    }

    private static void WriteNodes(
      StreamWriter writer,
      IEnumerable<Vertex> nodes,
      bool markers,
      int attribs,
      bool jettison)
    {
      int num = 0;
      foreach (Vertex node in nodes)
      {
        if (!jettison || node.type != VertexType.UndeadVertex)
        {
          writer.Write("{0} {1} {2}", (object) num, (object) node.x.ToString((IFormatProvider) FileWriter.nfi), (object) node.y.ToString((IFormatProvider) FileWriter.nfi));
          for (int index = 0; index < attribs; ++index)
            writer.Write(" {0}", (object) node.attributes[index].ToString((IFormatProvider) FileWriter.nfi));
          if (markers)
            writer.Write(" {0}", (object) node.mark);
          writer.WriteLine();
          ++num;
        }
      }
    }

    public static void WriteElements(Mesh mesh, string filename)
    {
      Otri otri = new Otri();
      bool useRegions = mesh.behavior.useRegions;
      int num = 0;
      otri.orient = 0;
      using (StreamWriter streamWriter = new StreamWriter(filename))
      {
        streamWriter.WriteLine("{0} 3 {1}", (object) mesh.triangles.Count, (object) (useRegions ? 1 : 0));
        foreach (Triangle triangle in mesh.triangles.Values)
        {
          otri.triangle = triangle;
          Vertex vertex1 = otri.Org();
          Vertex vertex2 = otri.Dest();
          Vertex vertex3 = otri.Apex();
          streamWriter.Write("{0} {1} {2} {3}", (object) num, (object) vertex1.id, (object) vertex2.id, (object) vertex3.id);
          if (useRegions)
            streamWriter.Write(" {0}", (object) otri.triangle.region);
          streamWriter.WriteLine();
          triangle.id = num++;
        }
      }
    }

    public static void WritePoly(Mesh mesh, string filename)
    {
      FileWriter.WritePoly(mesh, filename, true);
    }

    public static void WritePoly(Mesh mesh, string filename, bool writeNodes)
    {
      Osub osub = new Osub();
      bool useBoundaryMarkers = mesh.behavior.UseBoundaryMarkers;
      using (StreamWriter writer = new StreamWriter(filename))
      {
        if (writeNodes)
          FileWriter.WriteNodes(writer, mesh);
        else
          writer.WriteLine("0 {0} {1} {2}", (object) mesh.mesh_dim, (object) mesh.nextras, useBoundaryMarkers ? (object) "1" : (object) "0");
        writer.WriteLine("{0} {1}", (object) mesh.subsegs.Count, useBoundaryMarkers ? (object) "1" : (object) "0");
        osub.orient = 0;
        int num1 = 0;
        foreach (Segment segment in mesh.subsegs.Values)
        {
          osub.seg = segment;
          Vertex vertex1 = osub.Org();
          Vertex vertex2 = osub.Dest();
          if (useBoundaryMarkers)
            writer.WriteLine("{0} {1} {2} {3}", (object) num1, (object) vertex1.id, (object) vertex2.id, (object) osub.seg.boundary);
          else
            writer.WriteLine("{0} {1} {2}", (object) num1, (object) vertex1.id, (object) vertex2.id);
          ++num1;
        }
        int num2 = 0;
        writer.WriteLine("{0}", (object) mesh.holes.Count);
        foreach (Point hole in mesh.holes)
        {
          StreamWriter streamWriter = writer;
          // ISSUE: variable of a boxed type
          __Boxed<int> local = (ValueType) num2++;
          double num3 = hole.X;
          string str1 = num3.ToString((IFormatProvider) FileWriter.nfi);
          num3 = hole.Y;
          string str2 = num3.ToString((IFormatProvider) FileWriter.nfi);
          streamWriter.WriteLine("{0} {1} {2}", (object) local, (object) str1, (object) str2);
        }
        if (mesh.regions.Count <= 0)
          return;
        int num4 = 0;
        writer.WriteLine("{0}", (object) mesh.regions.Count);
        foreach (RegionPointer region in mesh.regions)
        {
          StreamWriter streamWriter = writer;
          object[] objArray = new object[4]
          {
            (object) num4,
            null,
            null,
            null
          };
          double num5 = region.point.X;
          objArray[1] = (object) num5.ToString((IFormatProvider) FileWriter.nfi);
          num5 = region.point.Y;
          objArray[2] = (object) num5.ToString((IFormatProvider) FileWriter.nfi);
          objArray[3] = (object) region.id;
          streamWriter.WriteLine("{0} {1} {2} {3}", objArray);
          ++num4;
        }
      }
    }

    public static void WriteEdges(Mesh mesh, string filename)
    {
      Otri otri = new Otri();
      Otri o2 = new Otri();
      Osub os = new Osub();
      Behavior behavior = mesh.behavior;
      using (StreamWriter streamWriter = new StreamWriter(filename))
      {
        streamWriter.WriteLine("{0} {1}", (object) mesh.edges, behavior.UseBoundaryMarkers ? (object) "1" : (object) "0");
        long num = 0;
        foreach (Triangle triangle in mesh.triangles.Values)
        {
          otri.triangle = triangle;
          for (otri.orient = 0; otri.orient < 3; ++otri.orient)
          {
            otri.Sym(ref o2);
            if (otri.triangle.id < o2.triangle.id || o2.triangle == Mesh.dummytri)
            {
              Vertex vertex1 = otri.Org();
              Vertex vertex2 = otri.Dest();
              if (behavior.UseBoundaryMarkers)
              {
                if (behavior.useSegments)
                {
                  otri.SegPivot(ref os);
                  if (os.seg == Mesh.dummysub)
                    streamWriter.WriteLine("{0} {1} {2} {3}", (object) num, (object) vertex1.id, (object) vertex2.id, (object) 0);
                  else
                    streamWriter.WriteLine("{0} {1} {2} {3}", (object) num, (object) vertex1.id, (object) vertex2.id, (object) os.seg.boundary);
                }
                else
                  streamWriter.WriteLine("{0} {1} {2} {3}", (object) num, (object) vertex1.id, (object) vertex2.id, o2.triangle == Mesh.dummytri ? (object) "1" : (object) "0");
              }
              else
                streamWriter.WriteLine("{0} {1} {2}", (object) num, (object) vertex1.id, (object) vertex2.id);
              ++num;
            }
          }
        }
      }
    }

    public static void WriteNeighbors(Mesh mesh, string filename)
    {
      Otri otri = new Otri();
      Otri o2 = new Otri();
      int num = 0;
      using (StreamWriter streamWriter = new StreamWriter(filename))
      {
        streamWriter.WriteLine("{0} 3", (object) mesh.triangles.Count);
        Mesh.dummytri.id = -1;
        foreach (Triangle triangle in mesh.triangles.Values)
        {
          otri.triangle = triangle;
          otri.orient = 1;
          otri.Sym(ref o2);
          int id1 = o2.triangle.id;
          otri.orient = 2;
          otri.Sym(ref o2);
          int id2 = o2.triangle.id;
          otri.orient = 0;
          otri.Sym(ref o2);
          int id3 = o2.triangle.id;
          streamWriter.WriteLine("{0} {1} {2} {3}", (object) num++, (object) id1, (object) id2, (object) id3);
        }
      }
    }

    public static void WriteVoronoi(Mesh mesh, string filename)
    {
      Otri otri = new Otri();
      Otri o2 = new Otri();
      double xi = 0.0;
      double eta = 0.0;
      int num1 = 0;
      otri.orient = 0;
      using (StreamWriter streamWriter1 = new StreamWriter(filename))
      {
        streamWriter1.WriteLine("{0} 2 {1} 0", (object) mesh.triangles.Count, (object) mesh.nextras);
        foreach (Triangle triangle in mesh.triangles.Values)
        {
          otri.triangle = triangle;
          Point circumcenter = Primitives.FindCircumcenter((Point) otri.Org(), (Point) otri.Dest(), (Point) otri.Apex(), ref xi, ref eta);
          streamWriter1.Write("{0} {1} {2}", (object) num1, (object) circumcenter.X.ToString((IFormatProvider) FileWriter.nfi), (object) circumcenter.Y.ToString((IFormatProvider) FileWriter.nfi));
          for (int index = 0; index < mesh.nextras; ++index)
            streamWriter1.Write(" 0");
          streamWriter1.WriteLine();
          otri.triangle.id = num1++;
        }
        streamWriter1.WriteLine("{0} 0", (object) mesh.edges);
        int num2 = 0;
        foreach (Triangle triangle in mesh.triangles.Values)
        {
          otri.triangle = triangle;
          for (otri.orient = 0; otri.orient < 3; ++otri.orient)
          {
            otri.Sym(ref o2);
            if (otri.triangle.id < o2.triangle.id || o2.triangle == Mesh.dummytri)
            {
              int id1 = otri.triangle.id;
              if (o2.triangle == Mesh.dummytri)
              {
                Vertex vertex1 = otri.Org();
                Vertex vertex2 = otri.Dest();
                StreamWriter streamWriter2 = streamWriter1;
                object[] objArray = new object[4]
                {
                  (object) num2,
                  (object) id1,
                  null,
                  null
                };
                double num3 = vertex2[1] - vertex1[1];
                objArray[2] = (object) num3.ToString((IFormatProvider) FileWriter.nfi);
                num3 = vertex1[0] - vertex2[0];
                objArray[3] = (object) num3.ToString((IFormatProvider) FileWriter.nfi);
                streamWriter2.WriteLine("{0} {1} -1 {2} {3}", objArray);
              }
              else
              {
                int id2 = o2.triangle.id;
                streamWriter1.WriteLine("{0} {1} {2}", (object) num2, (object) id1, (object) id2);
              }
              ++num2;
            }
          }
        }
      }
    }

    public static void WriteOffFile(Mesh mesh, string filename)
    {
      long num1 = (long) mesh.vertices.Count;
      if (mesh.behavior.Jettison)
        num1 = (long) (mesh.vertices.Count - mesh.undeads);
      int num2 = 0;
      using (StreamWriter streamWriter = new StreamWriter(filename))
      {
        streamWriter.WriteLine("OFF");
        streamWriter.WriteLine("{0}  {1}  {2}", (object) num1, (object) mesh.triangles.Count, (object) mesh.edges);
        foreach (Vertex vertex in mesh.vertices.Values)
        {
          if (!mesh.behavior.Jettison || vertex.type != VertexType.UndeadVertex)
          {
            streamWriter.WriteLine(" {0}  {1}  0.0", (object) vertex[0].ToString((IFormatProvider) FileWriter.nfi), (object) vertex[1].ToString((IFormatProvider) FileWriter.nfi));
            vertex.id = num2++;
          }
        }
        Otri otri;
        otri.orient = 0;
        foreach (Triangle triangle in mesh.triangles.Values)
        {
          otri.triangle = triangle;
          Vertex vertex1 = otri.Org();
          Vertex vertex2 = otri.Dest();
          Vertex vertex3 = otri.Apex();
          streamWriter.WriteLine(" 3   {0}  {1}  {2}", (object) vertex1.id, (object) vertex2.id, (object) vertex3.id);
        }
      }
    }
  }
}
