using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TriangleNet.Geometry;
using TriangleNet.Log;

namespace TriangleNet.IO
{
  public static class FileReader
  {
    private static NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
    private static int startIndex;

    private static bool TryReadLine(StreamReader reader, out string[] token)
    {
      token = null;
      if (reader.EndOfStream)
        return false;
      string str;
      for (str = reader.ReadLine().Trim(); string.IsNullOrEmpty(str) || str.StartsWith("#"); str = reader.ReadLine().Trim())
      {
        if (reader.EndOfStream)
          return false;
      }
      token = str.Split(new char[2]{ ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
      return true;
    }

    private static void ReadVertex(
      InputGeometry data,
      int index,
      string[] line,
      int attributes,
      int marks)
    {
      double x = double.Parse(line[1], nfi);
      double y = double.Parse(line[2], nfi);
      int boundary = 0;
      double[] attribs = attributes == 0 ? null : new double[attributes];
      for (int index1 = 0; index1 < attributes; ++index1)
      {
        if (line.Length > 3 + index1)
          attribs[index1] = double.Parse(line[3 + index1]);
      }
      if (marks > 0 && line.Length > 3 + attributes)
        boundary = int.Parse(line[3 + attributes]);
      data.AddPoint(x, y, boundary, attribs);
    }

    public static void Read(string filename, out InputGeometry geometry)
    {
      geometry = null;
      string str = Path.ChangeExtension(filename, ".poly");
      if (File.Exists(str))
      {
        geometry = ReadPolyFile(str);
      }
      else
      {
        string nodefilename = Path.ChangeExtension(filename, ".node");
        geometry = ReadNodeFile(nodefilename);
      }
    }

    public static void Read(
      string filename,
      out InputGeometry geometry,
      out List<ITriangle> triangles)
    {
      triangles = null;
      Read(filename, out geometry);
      string str = Path.ChangeExtension(filename, ".ele");
      if (!File.Exists(str) || geometry == null)
        return;
      triangles = ReadEleFile(str);
    }

    public static InputGeometry Read(string filename)
    {
      InputGeometry geometry = null;
      Read(filename, out geometry);
      return geometry;
    }

    public static InputGeometry ReadNodeFile(string nodefilename)
    {
      return ReadNodeFile(nodefilename, false);
    }

    public static InputGeometry ReadNodeFile(string nodefilename, bool readElements)
    {
      startIndex = 0;
      int attributes = 0;
      int marks = 0;
      InputGeometry data;
      using (StreamReader reader = new StreamReader(nodefilename))
      {
        string[] token;
        if (!TryReadLine(reader, out token))
          throw new Exception("Can't read input file.");
        int capacity = int.Parse(token[0]);
        if (capacity < 3)
          throw new Exception("Input must have at least three input vertices.");
        if (token.Length > 1 && int.Parse(token[1]) != 2)
          throw new Exception("Triangle only works with two-dimensional meshes.");
        if (token.Length > 2)
          attributes = int.Parse(token[2]);
        if (token.Length > 3)
          marks = int.Parse(token[3]);
        data = new InputGeometry(capacity);
        if (capacity > 0)
        {
          for (int index = 0; index < capacity; ++index)
          {
            if (!TryReadLine(reader, out token))
              throw new Exception("Can't read input file (vertices).");
            if (token.Length < 3)
              throw new Exception("Invalid vertex.");
            if (index == 0)
              startIndex = int.Parse(token[0], nfi);
            ReadVertex(data, index, token, attributes, marks);
          }
        }
      }
      if (readElements)
      {
        string str = Path.ChangeExtension(nodefilename, ".ele");
        if (File.Exists(str))
          ReadEleFile(str, true);
      }
      return data;
    }

    public static InputGeometry ReadPolyFile(string polyfilename)
    {
      return ReadPolyFile(polyfilename, false, false);
    }

    public static InputGeometry ReadPolyFile(string polyfilename, bool readElements)
    {
      return ReadPolyFile(polyfilename, readElements, false);
    }

    public static InputGeometry ReadPolyFile(string polyfilename, bool readElements, bool readArea)
    {
      startIndex = 0;
      int attributes = 0;
      int marks = 0;
      InputGeometry data;
      using (StreamReader reader = new StreamReader(polyfilename))
      {
        string[] token;
        if (!TryReadLine(reader, out token))
          throw new Exception("Can't read input file.");
        int count = int.Parse(token[0]);
        if (token.Length > 1 && int.Parse(token[1]) != 2)
          throw new Exception("Triangle only works with two-dimensional meshes.");
        if (token.Length > 2)
          attributes = int.Parse(token[2]);
        if (token.Length > 3)
          marks = int.Parse(token[3]);
        if (count > 0)
        {
          data = new InputGeometry(count);
          for (int index = 0; index < count; ++index)
          {
            if (!TryReadLine(reader, out token))
              throw new Exception("Can't read input file (vertices).");
            if (token.Length < 3)
              throw new Exception("Invalid vertex.");
            if (index == 0)
              startIndex = int.Parse(token[0], nfi);
            ReadVertex(data, index, token, attributes, marks);
          }
        }
        else
        {
          data = ReadNodeFile(Path.ChangeExtension(polyfilename, ".node"));
          count = data.Count;
        }
        if (data.Points == null)
          throw new Exception("No nodes available.");
        if (!TryReadLine(reader, out token))
          throw new Exception("Can't read input file (segments).");
        int num1 = int.Parse(token[0]);
        int num2 = 0;
        if (token.Length > 1)
          num2 = int.Parse(token[1]);
        for (int index = 0; index < num1; ++index)
        {
          if (!TryReadLine(reader, out token))
            throw new Exception("Can't read input file (segments).");
          if (token.Length < 3)
            throw new Exception("Segment has no endpoints.");
          int p0 = int.Parse(token[1]) - startIndex;
          int p1 = int.Parse(token[2]) - startIndex;
          int boundary = 0;
          if (num2 > 0 && token.Length > 3)
            boundary = int.Parse(token[3]);
          if (p0 < 0 || p0 >= count)
          {
            if (Behavior.Verbose)
              SimpleLog.Instance.Warning("Invalid first endpoint of segment.", "MeshReader.ReadPolyfile()");
          }
          else if (p1 < 0 || p1 >= count)
          {
            if (Behavior.Verbose)
              SimpleLog.Instance.Warning("Invalid second endpoint of segment.", "MeshReader.ReadPolyfile()");
          }
          else
            data.AddSegment(p0, p1, boundary);
        }
        if (!TryReadLine(reader, out token))
          throw new Exception("Can't read input file (holes).");
        int num3 = int.Parse(token[0]);
        if (num3 > 0)
        {
          for (int index = 0; index < num3; ++index)
          {
            if (!TryReadLine(reader, out token))
              throw new Exception("Can't read input file (holes).");
            if (token.Length < 3)
              throw new Exception("Invalid hole.");
            data.AddHole(double.Parse(token[1], nfi), double.Parse(token[2], nfi));
          }
        }
        if (TryReadLine(reader, out token))
        {
          int num4 = int.Parse(token[0]);
          if (num4 > 0)
          {
            for (int index = 0; index < num4; ++index)
            {
              if (!TryReadLine(reader, out token))
                throw new Exception("Can't read input file (region).");
              if (token.Length < 4)
                throw new Exception("Invalid region attributes.");
              data.AddRegion(double.Parse(token[1], nfi), double.Parse(token[2], nfi), int.Parse(token[3]));
            }
          }
        }
      }
      if (readElements)
      {
        string str = Path.ChangeExtension(polyfilename, ".ele");
        if (File.Exists(str))
          ReadEleFile(str, readArea);
      }
      return data;
    }

    public static List<ITriangle> ReadEleFile(string elefilename)
    {
      return ReadEleFile(elefilename, false);
    }

    private static List<ITriangle> ReadEleFile(string elefilename, bool readArea)
    {
      int num1 = 0;
      List<ITriangle> triangleList;
      using (StreamReader reader = new StreamReader(elefilename))
      {
        bool flag = false;
        string[] token;
        if (!TryReadLine(reader, out token))
          throw new Exception("Can't read input file (elements).");
        num1 = int.Parse(token[0]);
        int num2 = 0;
        if (token.Length > 2)
        {
          num2 = int.Parse(token[2]);
          flag = true;
        }
        if (num2 > 1)
          SimpleLog.Instance.Warning("Triangle attributes not supported.", "FileReader.Read");
        triangleList = new List<ITriangle>(num1);
        for (int index = 0; index < num1; ++index)
        {
          if (!TryReadLine(reader, out token))
            throw new Exception("Can't read input file (elements).");
          InputTriangle inputTriangle = token.Length >= 4 ? new InputTriangle(int.Parse(token[1]) - startIndex, int.Parse(token[2]) - startIndex, int.Parse(token[3]) - startIndex) : throw new Exception("Triangle has no nodes.");
          if (num2 > 0 & flag)
          {
            int result = 0;
            flag = int.TryParse(token[4], out result);
            inputTriangle.region = result;
          }
          triangleList.Add(inputTriangle);
        }
      }
      if (readArea)
      {
        string str = Path.ChangeExtension(elefilename, ".area");
        if (File.Exists(str))
          ReadAreaFile(str, num1);
      }
      return triangleList;
    }

    private static double[] ReadAreaFile(string areafilename, int intriangles)
    {
      double[] numArray = null;
      using (StreamReader reader = new StreamReader(areafilename))
      {
        string[] token;
        if (!TryReadLine(reader, out token))
          throw new Exception("Can't read input file (area).");
        if (int.Parse(token[0]) != intriangles)
        {
          SimpleLog.Instance.Warning("Number of area constraints doesn't match number of triangles.", "ReadAreaFile()");
          return null;
        }
        numArray = new double[intriangles];
        for (int index = 0; index < intriangles; ++index)
        {
          if (!TryReadLine(reader, out token))
            throw new Exception("Can't read input file (area).");
          numArray[index] = token.Length == 2 ? double.Parse(token[1], nfi) : throw new Exception("Triangle has no nodes.");
        }
      }
      return numArray;
    }

    public static List<Edge> ReadEdgeFile(string edgeFile, int invertices)
    {
      List<Edge> edgeList = null;
      startIndex = 0;
      using (StreamReader reader = new StreamReader(edgeFile))
      {
        string[] token;
        if (!TryReadLine(reader, out token))
          throw new Exception("Can't read input file (segments).");
        int capacity = int.Parse(token[0]);
        int num = 0;
        if (token.Length > 1)
          num = int.Parse(token[1]);
        if (capacity > 0)
          edgeList = new List<Edge>(capacity);
        for (int index = 0; index < capacity; ++index)
        {
          if (!TryReadLine(reader, out token))
            throw new Exception("Can't read input file (segments).");
          if (token.Length < 3)
            throw new Exception("Segment has no endpoints.");
          int p0 = int.Parse(token[1]) - startIndex;
          int p1 = int.Parse(token[2]) - startIndex;
          int boundary = 0;
          if (num > 0 && token.Length > 3)
            boundary = int.Parse(token[3]);
          if (p0 < 0 || p0 >= invertices)
          {
            if (Behavior.Verbose)
              SimpleLog.Instance.Warning("Invalid first endpoint of segment.", "MeshReader.ReadPolyfile()");
          }
          else if (p1 < 0 || p1 >= invertices)
          {
            if (Behavior.Verbose)
              SimpleLog.Instance.Warning("Invalid second endpoint of segment.", "MeshReader.ReadPolyfile()");
          }
          else
            edgeList.Add(new Edge(p0, p1, boundary));
        }
      }
      return edgeList;
    }
  }
}
