using System;
using TriangleNet.Data;
using TriangleNet.Geometry;

namespace TriangleNet.Tools
{
  public class Statistic
  {
    public static long InCircleCount = 0;
    public static long InCircleCountDecimal = 0;
    public static long CounterClockwiseCount = 0;
    public static long CounterClockwiseCountDecimal = 0;
    public static long Orient3dCount = 0;
    public static long HyperbolaCount = 0;
    public static long CircumcenterCount = 0;
    public static long CircleTopCount = 0;
    public static long RelocationCount = 0;
    private static readonly int[] plus1Mod3 = [
      1,
      2,
      0
    ];
    private static readonly int[] minus1Mod3 = [
      2,
      0,
      1
    ];
    private int[] angleTable;
    private int boundaryEdges;
    private int constrainedEdges;
    private int inHoles;
    private int inSegments;
    private int intBoundaryEdges;
    private int inTriangles;
    private int inVetrices;
    private double maxAngle;
    private int[] maxAngles;
    private double maxArea;
    private double maxAspect;
    private double maxEdge;
    private double minAngle;
    private int[] minAngles;
    private double minArea;
    private double minAspect;
    private double minEdge;
    private int outEdges;
    private int outTriangles;
    private int outVertices;

    public double ShortestEdge => minEdge;

    public double LongestEdge => maxEdge;

    public double ShortestAltitude => minAspect;

    public double LargestAspectRatio => maxAspect;

    public double SmallestArea => minArea;

    public double LargestArea => maxArea;

    public double SmallestAngle => minAngle;

    public double LargestAngle => maxAngle;

    public int InputVertices => inVetrices;

    public int InputTriangles => inTriangles;

    public int InputSegments => inSegments;

    public int InputHoles => inHoles;

    public int Vertices => outVertices;

    public int Triangles => outTriangles;

    public int Edges => outEdges;

    public int BoundaryEdges => boundaryEdges;

    public int InteriorBoundaryEdges => intBoundaryEdges;

    public int ConstrainedEdges => constrainedEdges;

    public int[] AngleHistogram => angleTable;

    public int[] MinAngleHistogram => minAngles;

    public int[] MaxAngleHistogram => maxAngles;

    private void GetAspectHistogram(Mesh mesh)
    {
      int[] numArray1 = new int[16];
      double[] numArray2 = [
        1.5,
        2.0,
        2.5,
        3.0,
        4.0,
        6.0,
        10.0,
        15.0,
        25.0,
        50.0,
        100.0,
        300.0,
        1000.0,
        10000.0,
        100000.0,
        0.0
      ];
      Otri otri = new Otri();
      Vertex[] vertexArray = new Vertex[3];
      double[] numArray3 = new double[3];
      double[] numArray4 = new double[3];
      double[] numArray5 = new double[3];
      otri.orient = 0;
      foreach (Triangle triangle in mesh.triangles.Values)
      {
        otri.triangle = triangle;
        vertexArray[0] = otri.Org();
        vertexArray[1] = otri.Dest();
        vertexArray[2] = otri.Apex();
        double num1 = 0.0;
        for (int index1 = 0; index1 < 3; ++index1)
        {
          int index2 = plus1Mod3[index1];
          int index3 = minus1Mod3[index1];
          numArray3[index1] = vertexArray[index2].x - vertexArray[index3].x;
          numArray4[index1] = vertexArray[index2].y - vertexArray[index3].y;
          numArray5[index1] = numArray3[index1] * numArray3[index1] + numArray4[index1] * numArray4[index1];
          if (numArray5[index1] > num1)
            num1 = numArray5[index1];
        }
        double num2 = Math.Abs((vertexArray[2].x - vertexArray[0].x) * (vertexArray[1].y - vertexArray[0].y) - (vertexArray[1].x - vertexArray[0].x) * (vertexArray[2].y - vertexArray[0].y)) / 2.0;
        double num3 = num2 * num2 / num1;
        double num4 = num1 / num3;
        int index = 0;
        while (num4 > numArray2[index] * numArray2[index] && index < 15)
          ++index;
        ++numArray1[index];
      }
    }

    public void Update(Mesh mesh, int sampleDegrees)
    {
      inVetrices = mesh.invertices;
      inTriangles = mesh.inelements;
      inSegments = mesh.insegments;
      inHoles = mesh.holes.Count;
      outVertices = mesh.vertices.Count - mesh.undeads;
      outTriangles = mesh.triangles.Count;
      outEdges = mesh.edges;
      boundaryEdges = mesh.hullsize;
      intBoundaryEdges = mesh.subsegs.Count - mesh.hullsize;
      constrainedEdges = mesh.subsegs.Count;
      Point[] pointArray = new Point[3];
      sampleDegrees = 60;
      double[] numArray1 = new double[sampleDegrees / 2 - 1];
      double[] numArray2 = new double[3];
      double[] numArray3 = new double[3];
      double[] numArray4 = new double[3];
      double num1 = Math.PI / sampleDegrees;
      double num2 = 180.0 / Math.PI;
      angleTable = new int[sampleDegrees];
      minAngles = new int[sampleDegrees];
      maxAngles = new int[sampleDegrees];
      for (int index = 0; index < sampleDegrees / 2 - 1; ++index)
      {
        numArray1[index] = Math.Cos(num1 * (index + 1));
        numArray1[index] = numArray1[index] * numArray1[index];
      }
      for (int index = 0; index < sampleDegrees; ++index)
        angleTable[index] = 0;
      minAspect = mesh.bounds.Width + mesh.bounds.Height;
      minAspect *= minAspect;
      maxAspect = 0.0;
      minEdge = minAspect;
      maxEdge = 0.0;
      minArea = minAspect;
      maxArea = 0.0;
      minAngle = 0.0;
      maxAngle = 2.0;
      bool flag1 = true;
      bool flag2 = true;
      foreach (Triangle triangle in mesh.triangles.Values)
      {
        double num3 = 0.0;
        double num4 = 1.0;
        pointArray[0] = triangle.vertices[0];
        pointArray[1] = triangle.vertices[1];
        pointArray[2] = triangle.vertices[2];
        double num5 = 0.0;
        for (int index1 = 0; index1 < 3; ++index1)
        {
          int index2 = plus1Mod3[index1];
          int index3 = minus1Mod3[index1];
          numArray2[index1] = pointArray[index2].X - pointArray[index3].X;
          numArray3[index1] = pointArray[index2].Y - pointArray[index3].Y;
          numArray4[index1] = numArray2[index1] * numArray2[index1] + numArray3[index1] * numArray3[index1];
          if (numArray4[index1] > num5)
            num5 = numArray4[index1];
          if (numArray4[index1] > maxEdge)
            maxEdge = numArray4[index1];
          if (numArray4[index1] < minEdge)
            minEdge = numArray4[index1];
        }
        double num6 = Math.Abs((pointArray[2].X - pointArray[0].X) * (pointArray[1].Y - pointArray[0].Y) - (pointArray[1].X - pointArray[0].X) * (pointArray[2].Y - pointArray[0].Y));
        if (num6 < minArea)
          minArea = num6;
        if (num6 > maxArea)
          maxArea = num6;
        double num7 = num6 * num6 / num5;
        if (num7 < minAspect)
          minAspect = num7;
        double num8 = num5 / num7;
        if (num8 > maxAspect)
          maxAspect = num8;
        for (int index4 = 0; index4 < 3; ++index4)
        {
          int index5 = plus1Mod3[index4];
          int index6 = minus1Mod3[index4];
          double num9 = numArray2[index5] * numArray2[index6] + numArray3[index5] * numArray3[index6];
          double num10 = num9 * num9 / (numArray4[index5] * numArray4[index6]);
          int index7 = sampleDegrees / 2 - 1;
          for (int index8 = index7 - 1; index8 >= 0; --index8)
          {
            if (num10 > numArray1[index8])
              index7 = index8;
          }
          if (num9 <= 0.0)
          {
            ++angleTable[index7];
            if (num10 > minAngle)
              minAngle = num10;
            if (flag1 && num10 < maxAngle)
              maxAngle = num10;
            if (num10 > num3)
              num3 = num10;
            if (flag2 && num10 < num4)
              num4 = num10;
          }
          else
          {
            ++angleTable[sampleDegrees - index7 - 1];
            if (flag1 || num10 > maxAngle)
            {
              maxAngle = num10;
              flag1 = false;
            }
            if (flag2 || num10 > num4)
            {
              num4 = num10;
              flag2 = false;
            }
          }
        }
        int index9 = sampleDegrees / 2 - 1;
        for (int index10 = index9 - 1; index10 >= 0; --index10)
        {
          if (num3 > numArray1[index10])
            index9 = index10;
        }
        ++minAngles[index9];
        int index11 = sampleDegrees / 2 - 1;
        for (int index12 = index11 - 1; index12 >= 0; --index12)
        {
          if (num4 > numArray1[index12])
            index11 = index12;
        }
        if (flag2)
          ++maxAngles[index11];
        else
          ++maxAngles[sampleDegrees - index11 - 1];
        flag2 = true;
      }
      minEdge = Math.Sqrt(minEdge);
      maxEdge = Math.Sqrt(maxEdge);
      minAspect = Math.Sqrt(minAspect);
      maxAspect = Math.Sqrt(maxAspect);
      minArea *= 0.5;
      maxArea *= 0.5;
      minAngle = minAngle < 1.0 ? num2 * Math.Acos(Math.Sqrt(minAngle)) : 0.0;
      if (maxAngle >= 1.0)
        maxAngle = 180.0;
      else
        maxAngle = !flag1 ? 180.0 - num2 * Math.Acos(Math.Sqrt(maxAngle)) : num2 * Math.Acos(Math.Sqrt(maxAngle));
    }
  }
}
