// Decompiled with JetBrains decompiler
// Type: TriangleNet.IO.DebugWriter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using TriangleNet.Data;
using TriangleNet.Geometry;

#nullable disable
namespace TriangleNet.IO
{
  internal class DebugWriter
  {
    private static NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
    private static readonly DebugWriter instance = new DebugWriter();
    private int iteration;
    private string session;
    private StreamWriter stream;
    private string tmpFile;
    private int triangles;
    private int[] vertices;

    public static DebugWriter Session => DebugWriter.instance;

    private DebugWriter()
    {
    }

    public void Start(string session)
    {
      this.iteration = 0;
      this.session = session;
      if (this.stream != null)
        throw new Exception("A session is active. Finish before starting a new.");
      this.tmpFile = Path.GetTempFileName();
      this.stream = new StreamWriter(this.tmpFile);
    }

    public void Write(Mesh mesh, bool skip = false)
    {
      this.WriteMesh(mesh, skip);
      this.triangles = mesh.Triangles.Count;
    }

    public void Finish() => this.Finish(this.session + ".mshx");

    private void Finish(string path)
    {
      if (this.stream == null)
        return;
      this.stream.Flush();
      this.stream.Dispose();
      this.stream = (StreamWriter) null;
      using (FileStream fileStream = new FileStream(path, FileMode.Create))
      {
        using (GZipStream gzipStream = new GZipStream((Stream) fileStream, CompressionMode.Compress, false))
        {
          byte[] bytes = Encoding.UTF8.GetBytes("#!N" + (object) this.iteration + Environment.NewLine);
          gzipStream.Write(bytes, 0, bytes.Length);
          byte[] buffer = File.ReadAllBytes(this.tmpFile);
          gzipStream.Write(buffer, 0, buffer.Length);
        }
      }
      File.Delete(this.tmpFile);
    }

    private void WriteGeometry(InputGeometry geometry)
    {
      this.stream.WriteLine("#!G{0}", (object) this.iteration++);
    }

    private void WriteMesh(Mesh mesh, bool skip)
    {
      if (this.triangles == mesh.triangles.Count & skip)
        return;
      this.stream.WriteLine("#!M{0}", (object) this.iteration++);
      if (this.VerticesChanged(mesh))
      {
        this.HashVertices(mesh);
        this.stream.WriteLine("{0}", (object) mesh.vertices.Count);
        foreach (Vertex vertex in mesh.vertices.Values)
          this.stream.WriteLine("{0} {1} {2} {3}", (object) vertex.hash, (object) vertex.x.ToString((IFormatProvider) DebugWriter.nfi), (object) vertex.y.ToString((IFormatProvider) DebugWriter.nfi), (object) vertex.mark);
      }
      else
        this.stream.WriteLine("0");
      this.stream.WriteLine("{0}", (object) mesh.subsegs.Count);
      Osub osub = new Osub();
      osub.orient = 0;
      foreach (Segment segment in mesh.subsegs.Values)
      {
        if (segment.hash > 0)
        {
          osub.seg = segment;
          Vertex vertex1 = osub.Org();
          Vertex vertex2 = osub.Dest();
          this.stream.WriteLine("{0} {1} {2} {3}", (object) osub.seg.hash, (object) vertex1.hash, (object) vertex2.hash, (object) osub.seg.boundary);
        }
      }
      Otri otri = new Otri();
      Otri o2 = new Otri();
      otri.orient = 0;
      this.stream.WriteLine("{0}", (object) mesh.triangles.Count);
      foreach (Triangle triangle in mesh.triangles.Values)
      {
        otri.triangle = triangle;
        Vertex vertex3 = otri.Org();
        Vertex vertex4 = otri.Dest();
        Vertex vertex5 = otri.Apex();
        int num1 = (Point) vertex3 == (Point) null ? -1 : vertex3.hash;
        int num2 = (Point) vertex4 == (Point) null ? -1 : vertex4.hash;
        int num3 = (Point) vertex5 == (Point) null ? -1 : vertex5.hash;
        this.stream.Write("{0} {1} {2} {3}", (object) otri.triangle.hash, (object) num1, (object) num2, (object) num3);
        otri.orient = 1;
        otri.Sym(ref o2);
        int hash1 = o2.triangle.hash;
        otri.orient = 2;
        otri.Sym(ref o2);
        int hash2 = o2.triangle.hash;
        otri.orient = 0;
        otri.Sym(ref o2);
        int hash3 = o2.triangle.hash;
        this.stream.WriteLine(" {0} {1} {2}", (object) hash1, (object) hash2, (object) hash3);
      }
    }

    private bool VerticesChanged(Mesh mesh)
    {
      if (this.vertices == null || mesh.Vertices.Count != this.vertices.Length)
        return true;
      int num = 0;
      foreach (Point vertex in (IEnumerable<Vertex>) mesh.Vertices)
      {
        if (vertex.id != this.vertices[num++])
          return true;
      }
      return false;
    }

    private void HashVertices(Mesh mesh)
    {
      if (this.vertices == null || mesh.Vertices.Count != this.vertices.Length)
        this.vertices = new int[mesh.Vertices.Count];
      int num = 0;
      foreach (Vertex vertex in (IEnumerable<Vertex>) mesh.Vertices)
        this.vertices[num++] = vertex.id;
    }
  }
}
