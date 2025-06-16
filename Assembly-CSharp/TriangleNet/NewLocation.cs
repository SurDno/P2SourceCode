using System;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.Tools;

namespace TriangleNet
{
  internal class NewLocation
  {
    private const double EPS = 1E-50;
    private Behavior behavior;
    private Mesh mesh;

    public NewLocation(Mesh mesh)
    {
      this.mesh = mesh;
      behavior = mesh.behavior;
    }

    public Point FindLocation(
      Vertex torg,
      Vertex tdest,
      Vertex tapex,
      ref double xi,
      ref double eta,
      bool offcenter,
      Otri badotri)
    {
      return behavior.MaxAngle == 0.0 ? FindNewLocationWithoutMaxAngle(torg, tdest, tapex, ref xi, ref eta, true, badotri) : FindNewLocation(torg, tdest, tapex, ref xi, ref eta, true, badotri);
    }

    private Point FindNewLocationWithoutMaxAngle(
      Vertex torg,
      Vertex tdest,
      Vertex tapex,
      ref double xi,
      ref double eta,
      bool offcenter,
      Otri badotri)
    {
      double offconstant = behavior.offconstant;
      int num1 = 0;
      Otri neighotri = new Otri();
      double[] thirdpoint = new double[2];
      double xi1 = 0.0;
      double eta1 = 0.0;
      double[] p1 = new double[5];
      double[] p2 = new double[4];
      double num2 = 0.06;
      double num3 = 1.0;
      double num4 = 1.0;
      int num5 = 0;
      double[] newloc = new double[2];
      double num6 = 0.0;
      double num7 = 0.0;
      ++Statistic.CircumcenterCount;
      double num8 = tdest.x - torg.x;
      double num9 = tdest.y - torg.y;
      double num10 = tapex.x - torg.x;
      double num11 = tapex.y - torg.y;
      double num12 = tapex.x - tdest.x;
      double num13 = tapex.y - tdest.y;
      double dodist = num8 * num8 + num9 * num9;
      double aodist = num10 * num10 + num11 * num11;
      double dadist = (tdest.x - tapex.x) * (tdest.x - tapex.x) + (tdest.y - tapex.y) * (tdest.y - tapex.y);
      double num14;
      if (Behavior.NoExact)
      {
        num14 = 0.5 / (num8 * num11 - num10 * num9);
      }
      else
      {
        num14 = 0.5 / Primitives.CounterClockwise(tdest, tapex, torg);
        --Statistic.CounterClockwiseCount;
      }
      double num15 = (num11 * dodist - num9 * aodist) * num14;
      double num16 = (num8 * aodist - num10 * dodist) * num14;
      Point point1 = new Point(torg.x + num15, torg.y + num16);
      Otri deltri = badotri;
      int num17 = LongestShortestEdge(aodist, dadist, dodist);
      double num18;
      double num19;
      double num20;
      double num21;
      double num22;
      double num23;
      double d1;
      double d2;
      double num24;
      Point point2;
      Point point3;
      Point point4;
      switch (num17)
      {
        case 123:
          num18 = num10;
          num19 = num11;
          num20 = num12;
          num21 = num13;
          num22 = num8;
          num23 = num9;
          d1 = aodist;
          d2 = dadist;
          num24 = dodist;
          point2 = tdest;
          point3 = torg;
          point4 = tapex;
          break;
        case 132:
          num18 = num10;
          num19 = num11;
          num20 = num8;
          num21 = num9;
          num22 = num12;
          num23 = num13;
          d1 = aodist;
          d2 = dodist;
          num24 = dadist;
          point2 = tdest;
          point3 = tapex;
          point4 = torg;
          break;
        case 213:
          num18 = num12;
          num19 = num13;
          num20 = num10;
          num21 = num11;
          num22 = num8;
          num23 = num9;
          d1 = dadist;
          d2 = aodist;
          num24 = dodist;
          point2 = torg;
          point3 = tdest;
          point4 = tapex;
          break;
        case 231:
          num18 = num12;
          num19 = num13;
          num20 = num8;
          num21 = num9;
          num22 = num10;
          num23 = num11;
          d1 = dadist;
          d2 = dodist;
          num24 = aodist;
          point2 = torg;
          point3 = tapex;
          point4 = tdest;
          break;
        case 312:
          num18 = num8;
          num19 = num9;
          num20 = num10;
          num21 = num11;
          num22 = num12;
          num23 = num13;
          d1 = dodist;
          d2 = aodist;
          num24 = dadist;
          point2 = tapex;
          point3 = tdest;
          point4 = torg;
          break;
        default:
          num18 = num8;
          num19 = num9;
          num20 = num12;
          num21 = num13;
          num22 = num10;
          num23 = num11;
          d1 = dodist;
          d2 = dadist;
          num24 = aodist;
          point2 = tapex;
          point3 = torg;
          point4 = tdest;
          break;
      }
      if (offcenter && offconstant > 0.0)
      {
        if (num17 == 213 || num17 == 231)
        {
          double num25 = 0.5 * num18 - offconstant * num19;
          double num26 = 0.5 * num19 + offconstant * num18;
          if (num25 * num25 + num26 * num26 < (num15 - num8) * (num15 - num8) + (num16 - num9) * (num16 - num9))
          {
            num15 = num8 + num25;
            num16 = num9 + num26;
          }
          else
            num1 = 1;
        }
        else if (num17 == 123 || num17 == 132)
        {
          double num27 = 0.5 * num18 + offconstant * num19;
          double num28 = 0.5 * num19 - offconstant * num18;
          if (num27 * num27 + num28 * num28 < num15 * num15 + num16 * num16)
          {
            num15 = num27;
            num16 = num28;
          }
          else
            num1 = 1;
        }
        else
        {
          double num29 = 0.5 * num18 - offconstant * num19;
          double num30 = 0.5 * num19 + offconstant * num18;
          if (num29 * num29 + num30 * num30 < num15 * num15 + num16 * num16)
          {
            num15 = num29;
            num16 = num30;
          }
          else
            num1 = 1;
        }
      }
      if (num1 == 1)
      {
        double num31 = (d2 + d1 - num24) / (2.0 * Math.Sqrt(d2) * Math.Sqrt(d1));
        bool isObtuse = num31 < 0.0 || Math.Abs(num31 - 0.0) <= 1E-50;
        num5 = DoSmoothing(deltri, torg, tdest, tapex, ref newloc);
        if (num5 > 0)
        {
          ++Statistic.RelocationCount;
          num15 = newloc[0] - torg.x;
          num16 = newloc[1] - torg.y;
          num6 = torg.x;
          num7 = torg.y;
          switch (num5)
          {
            case 1:
              mesh.DeleteVertex(ref deltri);
              break;
            case 2:
              deltri.LnextSelf();
              mesh.DeleteVertex(ref deltri);
              break;
            case 3:
              deltri.LprevSelf();
              mesh.DeleteVertex(ref deltri);
              break;
          }
        }
        else
        {
          double r = Math.Sqrt(d1) / (2.0 * Math.Sin(behavior.MinAngle * Math.PI / 180.0));
          double num32 = (point3.x + point4.x) / 2.0;
          double num33 = (point3.y + point4.y) / 2.0;
          double num34 = num32 + Math.Sqrt(r * r - d1 / 4.0) * (point3.y - point4.y) / Math.Sqrt(d1);
          double num35 = num33 + Math.Sqrt(r * r - d1 / 4.0) * (point4.x - point3.x) / Math.Sqrt(d1);
          double num36 = num32 - Math.Sqrt(r * r - d1 / 4.0) * (point3.y - point4.y) / Math.Sqrt(d1);
          double num37 = num33 - Math.Sqrt(r * r - d1 / 4.0) * (point4.x - point3.x) / Math.Sqrt(d1);
          double x3_1;
          double y3_1;
          if ((num34 - point2.x) * (num34 - point2.x) + (num35 - point2.y) * (num35 - point2.y) <= (num36 - point2.x) * (num36 - point2.x) + (num37 - point2.y) * (num37 - point2.y))
          {
            x3_1 = num34;
            y3_1 = num35;
          }
          else
          {
            x3_1 = num36;
            y3_1 = num37;
          }
          bool neighborsVertex1 = GetNeighborsVertex(badotri, point3.x, point3.y, point2.x, point2.y, ref thirdpoint, ref neighotri);
          double num38 = num15;
          double num39 = num16;
          if (!neighborsVertex1)
          {
            Point circumcenter = Primitives.FindCircumcenter(neighotri.Org(), neighotri.Dest(), neighotri.Apex(), ref xi1, ref eta1);
            double num40 = point3.y - point2.y;
            double num41 = point2.x - point3.x;
            double x2 = point1.x + num40;
            double y2 = point1.y + num41;
            CircleLineIntersection(point1.x, point1.y, x2, y2, x3_1, y3_1, r, ref p1);
            double num42;
            double num43;
            if (ChooseCorrectPoint((point3.x + point2.x) / 2.0, (point3.y + point2.y) / 2.0, p1[3], p1[4], point1.x, point1.y, isObtuse))
            {
              num42 = p1[3];
              num43 = p1[4];
            }
            else
            {
              num42 = p1[1];
              num43 = p1[2];
            }
            PointBetweenPoints(num42, num43, point1.x, point1.y, circumcenter.x, circumcenter.y, ref p2);
            if (p1[0] > 0.0)
            {
              if (Math.Abs(p2[0] - 1.0) <= 1E-50)
              {
                if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, circumcenter.x, circumcenter.y))
                {
                  num38 = num15;
                  num39 = num16;
                }
                else
                {
                  num38 = p2[2] - torg.x;
                  num39 = p2[3] - torg.y;
                }
              }
              else if (IsBadTriangleAngle(point4.x, point4.y, point3.x, point3.y, num42, num43))
              {
                double num44 = Math.Sqrt((num42 - point1.x) * (num42 - point1.x) + (num43 - point1.y) * (num43 - point1.y));
                double num45 = point1.x - num42;
                double num46 = point1.y - num43;
                double num47 = num45 / num44;
                double num48 = num46 / num44;
                double x3_2 = num42 + num47 * num2 * Math.Sqrt(d1);
                double y3_2 = num43 + num48 * num2 * Math.Sqrt(d1);
                if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, x3_2, y3_2))
                {
                  num38 = num15;
                  num39 = num16;
                }
                else
                {
                  num38 = x3_2 - torg.x;
                  num39 = y3_2 - torg.y;
                }
              }
              else
              {
                num38 = num42 - torg.x;
                num39 = num43 - torg.y;
              }
              if ((point2.x - point1.x) * (point2.x - point1.x) + (point2.y - point1.y) * (point2.y - point1.y) > num3 * ((point2.x - (num38 + torg.x)) * (point2.x - (num38 + torg.x)) + (point2.y - (num39 + torg.y)) * (point2.y - (num39 + torg.y))))
              {
                num38 = num15;
                num39 = num16;
              }
            }
          }
          bool neighborsVertex2 = GetNeighborsVertex(badotri, point4.x, point4.y, point2.x, point2.y, ref thirdpoint, ref neighotri);
          double num49 = num15;
          double num50 = num16;
          if (!neighborsVertex2)
          {
            Point circumcenter = Primitives.FindCircumcenter(neighotri.Org(), neighotri.Dest(), neighotri.Apex(), ref xi1, ref eta1);
            double num51 = point4.y - point2.y;
            double num52 = point2.x - point4.x;
            double x2 = point1.x + num51;
            double y2 = point1.y + num52;
            CircleLineIntersection(point1.x, point1.y, x2, y2, x3_1, y3_1, r, ref p1);
            double num53;
            double num54;
            if (ChooseCorrectPoint((point4.x + point2.x) / 2.0, (point4.y + point2.y) / 2.0, p1[3], p1[4], point1.x, point1.y, false))
            {
              num53 = p1[3];
              num54 = p1[4];
            }
            else
            {
              num53 = p1[1];
              num54 = p1[2];
            }
            PointBetweenPoints(num53, num54, point1.x, point1.y, circumcenter.x, circumcenter.y, ref p2);
            if (p1[0] > 0.0)
            {
              if (Math.Abs(p2[0] - 1.0) <= 1E-50)
              {
                if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, circumcenter.x, circumcenter.y))
                {
                  num49 = num15;
                  num50 = num16;
                }
                else
                {
                  num49 = p2[2] - torg.x;
                  num50 = p2[3] - torg.y;
                }
              }
              else if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num53, num54))
              {
                double num55 = Math.Sqrt((num53 - point1.x) * (num53 - point1.x) + (num54 - point1.y) * (num54 - point1.y));
                double num56 = point1.x - num53;
                double num57 = point1.y - num54;
                double num58 = num56 / num55;
                double num59 = num57 / num55;
                double x3_3 = num53 + num58 * num2 * Math.Sqrt(d1);
                double y3_3 = num54 + num59 * num2 * Math.Sqrt(d1);
                if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, x3_3, y3_3))
                {
                  num49 = num15;
                  num50 = num16;
                }
                else
                {
                  num49 = x3_3 - torg.x;
                  num50 = y3_3 - torg.y;
                }
              }
              else
              {
                num49 = num53 - torg.x;
                num50 = num54 - torg.y;
              }
              if ((point2.x - point1.x) * (point2.x - point1.x) + (point2.y - point1.y) * (point2.y - point1.y) > num3 * ((point2.x - (num49 + torg.x)) * (point2.x - (num49 + torg.x)) + (point2.y - (num50 + torg.y)) * (point2.y - (num50 + torg.y))))
              {
                num49 = num15;
                num50 = num16;
              }
            }
          }
          if (isObtuse)
          {
            num15 = num38;
            num16 = num39;
          }
          else if (num4 * ((point2.x - (num49 + torg.x)) * (point2.x - (num49 + torg.x)) + (point2.y - (num50 + torg.y)) * (point2.y - (num50 + torg.y))) > (point2.x - (num38 + torg.x)) * (point2.x - (num38 + torg.x)) + (point2.y - (num39 + torg.y)) * (point2.y - (num39 + torg.y)))
          {
            num15 = num49;
            num16 = num50;
          }
          else
          {
            num15 = num38;
            num16 = num39;
          }
        }
      }
      Point locationWithoutMaxAngle = new Point();
      if (num5 <= 0)
      {
        locationWithoutMaxAngle.x = torg.x + num15;
        locationWithoutMaxAngle.y = torg.y + num16;
      }
      else
      {
        locationWithoutMaxAngle.x = num6 + num15;
        locationWithoutMaxAngle.y = num7 + num16;
      }
      xi = (num11 * num15 - num10 * num16) * (2.0 * num14);
      eta = (num8 * num16 - num9 * num15) * (2.0 * num14);
      return locationWithoutMaxAngle;
    }

    private Point FindNewLocation(
      Vertex torg,
      Vertex tdest,
      Vertex tapex,
      ref double xi,
      ref double eta,
      bool offcenter,
      Otri badotri)
    {
      double offconstant = behavior.offconstant;
      int num1 = 0;
      Otri otri = new Otri();
      double[] thirdpoint = new double[2];
      double xi1 = 0.0;
      double eta1 = 0.0;
      double[] p1 = new double[5];
      double[] p2 = new double[4];
      double num2 = 0.06;
      double num3 = 1.0;
      double num4 = 1.0;
      int num5 = 0;
      double[] newloc = new double[2];
      double num6 = 0.0;
      double num7 = 0.0;
      double num8 = 0.0;
      double num9 = 0.0;
      double[] p3 = new double[3];
      double[] p4 = new double[4];
      ++Statistic.CircumcenterCount;
      double num10 = tdest.x - torg.x;
      double num11 = tdest.y - torg.y;
      double num12 = tapex.x - torg.x;
      double num13 = tapex.y - torg.y;
      double num14 = tapex.x - tdest.x;
      double num15 = tapex.y - tdest.y;
      double dodist = num10 * num10 + num11 * num11;
      double aodist = num12 * num12 + num13 * num13;
      double dadist = (tdest.x - tapex.x) * (tdest.x - tapex.x) + (tdest.y - tapex.y) * (tdest.y - tapex.y);
      double num16;
      if (Behavior.NoExact)
      {
        num16 = 0.5 / (num10 * num13 - num12 * num11);
      }
      else
      {
        num16 = 0.5 / Primitives.CounterClockwise(tdest, tapex, torg);
        --Statistic.CounterClockwiseCount;
      }
      double num17 = (num13 * dodist - num11 * aodist) * num16;
      double num18 = (num10 * aodist - num12 * dodist) * num16;
      Point point1 = new Point(torg.x + num17, torg.y + num18);
      Otri deltri = badotri;
      int num19 = LongestShortestEdge(aodist, dadist, dodist);
      double num20;
      double num21;
      double num22;
      double num23;
      double num24;
      double num25;
      double d1;
      double d2;
      double d3;
      Point point2;
      Point point3;
      Point point4;
      switch (num19)
      {
        case 123:
          num20 = num12;
          num21 = num13;
          num22 = num14;
          num23 = num15;
          num24 = num10;
          num25 = num11;
          d1 = aodist;
          d2 = dadist;
          d3 = dodist;
          point2 = tdest;
          point3 = torg;
          point4 = tapex;
          break;
        case 132:
          num20 = num12;
          num21 = num13;
          num22 = num10;
          num23 = num11;
          num24 = num14;
          num25 = num15;
          d1 = aodist;
          d2 = dodist;
          d3 = dadist;
          point2 = tdest;
          point3 = tapex;
          point4 = torg;
          break;
        case 213:
          num20 = num14;
          num21 = num15;
          num22 = num12;
          num23 = num13;
          num24 = num10;
          num25 = num11;
          d1 = dadist;
          d2 = aodist;
          d3 = dodist;
          point2 = torg;
          point3 = tdest;
          point4 = tapex;
          break;
        case 231:
          num20 = num14;
          num21 = num15;
          num22 = num10;
          num23 = num11;
          num24 = num12;
          num25 = num13;
          d1 = dadist;
          d2 = dodist;
          d3 = aodist;
          point2 = torg;
          point3 = tapex;
          point4 = tdest;
          break;
        case 312:
          num20 = num10;
          num21 = num11;
          num22 = num12;
          num23 = num13;
          num24 = num14;
          num25 = num15;
          d1 = dodist;
          d2 = aodist;
          d3 = dadist;
          point2 = tapex;
          point3 = tdest;
          point4 = torg;
          break;
        default:
          num20 = num10;
          num21 = num11;
          num22 = num14;
          num23 = num15;
          num24 = num12;
          num25 = num13;
          d1 = dodist;
          d2 = dadist;
          d3 = aodist;
          point2 = tapex;
          point3 = torg;
          point4 = tdest;
          break;
      }
      if (offcenter && offconstant > 0.0)
      {
        if (num19 == 213 || num19 == 231)
        {
          double num26 = 0.5 * num20 - offconstant * num21;
          double num27 = 0.5 * num21 + offconstant * num20;
          if (num26 * num26 + num27 * num27 < (num17 - num10) * (num17 - num10) + (num18 - num11) * (num18 - num11))
          {
            num17 = num10 + num26;
            num18 = num11 + num27;
          }
          else
            num1 = 1;
        }
        else if (num19 == 123 || num19 == 132)
        {
          double num28 = 0.5 * num20 + offconstant * num21;
          double num29 = 0.5 * num21 - offconstant * num20;
          if (num28 * num28 + num29 * num29 < num17 * num17 + num18 * num18)
          {
            num17 = num28;
            num18 = num29;
          }
          else
            num1 = 1;
        }
        else
        {
          double num30 = 0.5 * num20 - offconstant * num21;
          double num31 = 0.5 * num21 + offconstant * num20;
          if (num30 * num30 + num31 * num31 < num17 * num17 + num18 * num18)
          {
            num17 = num30;
            num18 = num31;
          }
          else
            num1 = 1;
        }
      }
      if (num1 == 1)
      {
        double num32 = (d2 + d1 - d3) / (2.0 * Math.Sqrt(d2) * Math.Sqrt(d1));
        bool isObtuse = num32 < 0.0 || Math.Abs(num32 - 0.0) <= 1E-50;
        num5 = DoSmoothing(deltri, torg, tdest, tapex, ref newloc);
        if (num5 > 0)
        {
          ++Statistic.RelocationCount;
          num17 = newloc[0] - torg.x;
          num18 = newloc[1] - torg.y;
          num6 = torg.x;
          num7 = torg.y;
          switch (num5)
          {
            case 1:
              mesh.DeleteVertex(ref deltri);
              break;
            case 2:
              deltri.LnextSelf();
              mesh.DeleteVertex(ref deltri);
              break;
            case 3:
              deltri.LprevSelf();
              mesh.DeleteVertex(ref deltri);
              break;
          }
        }
        else
        {
          double num33 = Math.Acos((d2 + d3 - d1) / (2.0 * Math.Sqrt(d2) * Math.Sqrt(d3))) * 180.0 / Math.PI;
          double num34 = behavior.MinAngle <= num33 ? num33 + 0.5 : behavior.MinAngle;
          double r = Math.Sqrt(d1) / (2.0 * Math.Sin(num34 * Math.PI / 180.0));
          double num35 = (point3.x + point4.x) / 2.0;
          double num36 = (point3.y + point4.y) / 2.0;
          double num37 = num35 + Math.Sqrt(r * r - d1 / 4.0) * (point3.y - point4.y) / Math.Sqrt(d1);
          double num38 = num36 + Math.Sqrt(r * r - d1 / 4.0) * (point4.x - point3.x) / Math.Sqrt(d1);
          double num39 = num35 - Math.Sqrt(r * r - d1 / 4.0) * (point3.y - point4.y) / Math.Sqrt(d1);
          double num40 = num36 - Math.Sqrt(r * r - d1 / 4.0) * (point4.x - point3.x) / Math.Sqrt(d1);
          double x3_1;
          double y3_1;
          if ((num37 - point2.x) * (num37 - point2.x) + (num38 - point2.y) * (num38 - point2.y) <= (num39 - point2.x) * (num39 - point2.x) + (num40 - point2.y) * (num40 - point2.y))
          {
            x3_1 = num37;
            y3_1 = num38;
          }
          else
          {
            x3_1 = num39;
            y3_1 = num40;
          }
          bool neighborsVertex1 = GetNeighborsVertex(badotri, point3.x, point3.y, point2.x, point2.y, ref thirdpoint, ref otri);
          double num41 = num17;
          double num42 = num18;
          double num43 = Math.Sqrt((x3_1 - num35) * (x3_1 - num35) + (y3_1 - num36) * (y3_1 - num36));
          double num44 = (x3_1 - num35) / num43;
          double num45 = (y3_1 - num36) / num43;
          double num46 = x3_1 + num44 * r;
          double num47 = y3_1 + num45 * r;
          double num48 = (2.0 * behavior.MaxAngle + num34 - 180.0) * Math.PI / 180.0;
          double x3_2 = num46 * Math.Cos(num48) + num47 * Math.Sin(num48) + x3_1 - x3_1 * Math.Cos(num48) - y3_1 * Math.Sin(num48);
          double y3_2 = -num46 * Math.Sin(num48) + num47 * Math.Cos(num48) + y3_1 + x3_1 * Math.Sin(num48) - y3_1 * Math.Cos(num48);
          double x1_1 = num46 * Math.Cos(num48) - num47 * Math.Sin(num48) + x3_1 - x3_1 * Math.Cos(num48) + y3_1 * Math.Sin(num48);
          double y1_1 = num46 * Math.Sin(num48) + num47 * Math.Cos(num48) + y3_1 - x3_1 * Math.Sin(num48) - y3_1 * Math.Cos(num48);
          double num49;
          double num50;
          double num51;
          double num52;
          if (ChooseCorrectPoint(x1_1, y1_1, point3.x, point3.y, x3_2, y3_2, true))
          {
            num49 = x3_2;
            num50 = y3_2;
            num51 = x1_1;
            num52 = y1_1;
          }
          else
          {
            num49 = x1_1;
            num50 = y1_1;
            num51 = x3_2;
            num52 = y3_2;
          }
          double x1_2 = (point3.x + point2.x) / 2.0;
          double y1_2 = (point3.y + point2.y) / 2.0;
          double num53;
          double num54;
          if (!neighborsVertex1)
          {
            Point circumcenter = Primitives.FindCircumcenter(otri.Org(), otri.Dest(), otri.Apex(), ref xi1, ref eta1);
            double num55 = point3.y - point2.y;
            double num56 = point2.x - point3.x;
            double x2 = point1.x + num55;
            double y2 = point1.y + num56;
            CircleLineIntersection(point1.x, point1.y, x2, y2, x3_1, y3_1, r, ref p1);
            double num57;
            double num58;
            if (ChooseCorrectPoint(x1_2, y1_2, p1[3], p1[4], point1.x, point1.y, isObtuse))
            {
              num57 = p1[3];
              num58 = p1[4];
            }
            else
            {
              num57 = p1[1];
              num58 = p1[2];
            }
            double x = point3.x;
            double y = point3.y;
            num53 = point4.x - point3.x;
            num54 = point4.y - point3.y;
            double x4 = num49;
            double y4 = num50;
            LineLineIntersection(point1.x, point1.y, x2, y2, x, y, x4, y4, ref p3);
            if (p3[0] > 0.0)
            {
              num8 = p3[1];
              num9 = p3[2];
            }
            PointBetweenPoints(num57, num58, point1.x, point1.y, circumcenter.x, circumcenter.y, ref p2);
            if (p1[0] > 0.0)
            {
              if (Math.Abs(p2[0] - 1.0) <= 1E-50)
              {
                PointBetweenPoints(p2[2], p2[3], point1.x, point1.y, num8, num9, ref p4);
                if (Math.Abs(p4[0] - 1.0) <= 1E-50 && p3[0] > 0.0)
                {
                  if ((point2.x - num49) * (point2.x - num49) + (point2.y - num50) * (point2.y - num50) > num3 * ((point2.x - num8) * (point2.x - num8) + (point2.y - num9) * (point2.y - num9)) && IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num49, num50) && MinDistanceToNeighbor(num49, num50, ref otri) > MinDistanceToNeighbor(num8, num9, ref otri))
                  {
                    num41 = num49 - torg.x;
                    num42 = num50 - torg.y;
                  }
                  else if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num8, num9))
                  {
                    double num59 = Math.Sqrt((num8 - point1.x) * (num8 - point1.x) + (num9 - point1.y) * (num9 - point1.y));
                    double num60 = point1.x - num8;
                    double num61 = point1.y - num9;
                    double num62 = num60 / num59;
                    double num63 = num61 / num59;
                    num8 += num62 * num2 * Math.Sqrt(d1);
                    num9 += num63 * num2 * Math.Sqrt(d1);
                    if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num8, num9))
                    {
                      num41 = num17;
                      num42 = num18;
                    }
                    else
                    {
                      num41 = num8 - torg.x;
                      num42 = num9 - torg.y;
                    }
                  }
                  else
                  {
                    num41 = p4[2] - torg.x;
                    num42 = p4[3] - torg.y;
                  }
                }
                else if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, circumcenter.x, circumcenter.y))
                {
                  num41 = num17;
                  num42 = num18;
                }
                else
                {
                  num41 = p2[2] - torg.x;
                  num42 = p2[3] - torg.y;
                }
              }
              else
              {
                PointBetweenPoints(num57, num58, point1.x, point1.y, num8, num9, ref p4);
                if (Math.Abs(p4[0] - 1.0) <= 1E-50 && p3[0] > 0.0)
                {
                  if ((point2.x - num49) * (point2.x - num49) + (point2.y - num50) * (point2.y - num50) > num3 * ((point2.x - num8) * (point2.x - num8) + (point2.y - num9) * (point2.y - num9)) && IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num49, num50) && MinDistanceToNeighbor(num49, num50, ref otri) > MinDistanceToNeighbor(num8, num9, ref otri))
                  {
                    num41 = num49 - torg.x;
                    num42 = num50 - torg.y;
                  }
                  else if (IsBadTriangleAngle(point4.x, point4.y, point3.x, point3.y, num8, num9))
                  {
                    double num64 = Math.Sqrt((num8 - point1.x) * (num8 - point1.x) + (num9 - point1.y) * (num9 - point1.y));
                    double num65 = point1.x - num8;
                    double num66 = point1.y - num9;
                    double num67 = num65 / num64;
                    double num68 = num66 / num64;
                    num8 += num67 * num2 * Math.Sqrt(d1);
                    num9 += num68 * num2 * Math.Sqrt(d1);
                    if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num8, num9))
                    {
                      num41 = num17;
                      num42 = num18;
                    }
                    else
                    {
                      num41 = num8 - torg.x;
                      num42 = num9 - torg.y;
                    }
                  }
                  else
                  {
                    num41 = p4[2] - torg.x;
                    num42 = p4[3] - torg.y;
                  }
                }
                else if (IsBadTriangleAngle(point4.x, point4.y, point3.x, point3.y, num57, num58))
                {
                  double num69 = Math.Sqrt((num57 - point1.x) * (num57 - point1.x) + (num58 - point1.y) * (num58 - point1.y));
                  double num70 = point1.x - num57;
                  double num71 = point1.y - num58;
                  double num72 = num70 / num69;
                  double num73 = num71 / num69;
                  double x3_3 = num57 + num72 * num2 * Math.Sqrt(d1);
                  double y3_3 = num58 + num73 * num2 * Math.Sqrt(d1);
                  if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, x3_3, y3_3))
                  {
                    num41 = num17;
                    num42 = num18;
                  }
                  else
                  {
                    num41 = x3_3 - torg.x;
                    num42 = y3_3 - torg.y;
                  }
                }
                else
                {
                  num41 = num57 - torg.x;
                  num42 = num58 - torg.y;
                }
              }
              if ((point2.x - point1.x) * (point2.x - point1.x) + (point2.y - point1.y) * (point2.y - point1.y) > num3 * ((point2.x - (num41 + torg.x)) * (point2.x - (num41 + torg.x)) + (point2.y - (num42 + torg.y)) * (point2.y - (num42 + torg.y))))
              {
                num41 = num17;
                num42 = num18;
              }
            }
          }
          bool neighborsVertex2 = GetNeighborsVertex(badotri, point4.x, point4.y, point2.x, point2.y, ref thirdpoint, ref otri);
          double num74 = num17;
          double num75 = num18;
          double x1_3 = (point4.x + point2.x) / 2.0;
          double y1_3 = (point4.y + point2.y) / 2.0;
          if (!neighborsVertex2)
          {
            Point circumcenter = Primitives.FindCircumcenter(otri.Org(), otri.Dest(), otri.Apex(), ref xi1, ref eta1);
            double num76 = point4.y - point2.y;
            double num77 = point2.x - point4.x;
            double x2 = point1.x + num76;
            double y2 = point1.y + num77;
            CircleLineIntersection(point1.x, point1.y, x2, y2, x3_1, y3_1, r, ref p1);
            double num78;
            double num79;
            if (ChooseCorrectPoint(x1_3, y1_3, p1[3], p1[4], point1.x, point1.y, false))
            {
              num78 = p1[3];
              num79 = p1[4];
            }
            else
            {
              num78 = p1[1];
              num79 = p1[2];
            }
            double x = point4.x;
            double y = point4.y;
            num53 = point3.x - point4.x;
            num54 = point3.y - point4.y;
            double x4 = num51;
            double y4 = num52;
            LineLineIntersection(point1.x, point1.y, x2, y2, x, y, x4, y4, ref p3);
            if (p3[0] > 0.0)
            {
              num8 = p3[1];
              num9 = p3[2];
            }
            PointBetweenPoints(num78, num79, point1.x, point1.y, circumcenter.x, circumcenter.y, ref p2);
            if (p1[0] > 0.0)
            {
              if (Math.Abs(p2[0] - 1.0) <= 1E-50)
              {
                PointBetweenPoints(p2[2], p2[3], point1.x, point1.y, num8, num9, ref p4);
                if (Math.Abs(p4[0] - 1.0) <= 1E-50 && p3[0] > 0.0)
                {
                  if ((point2.x - num51) * (point2.x - num51) + (point2.y - num52) * (point2.y - num52) > num3 * ((point2.x - num8) * (point2.x - num8) + (point2.y - num9) * (point2.y - num9)) && IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num51, num52) && MinDistanceToNeighbor(num51, num52, ref otri) > MinDistanceToNeighbor(num8, num9, ref otri))
                  {
                    num74 = num51 - torg.x;
                    num75 = num52 - torg.y;
                  }
                  else if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num8, num9))
                  {
                    double num80 = Math.Sqrt((num8 - point1.x) * (num8 - point1.x) + (num9 - point1.y) * (num9 - point1.y));
                    double num81 = point1.x - num8;
                    double num82 = point1.y - num9;
                    double num83 = num81 / num80;
                    double num84 = num82 / num80;
                    double x3_4 = num8 + num83 * num2 * Math.Sqrt(d1);
                    double y3_4 = num9 + num84 * num2 * Math.Sqrt(d1);
                    if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, x3_4, y3_4))
                    {
                      num74 = num17;
                      num75 = num18;
                    }
                    else
                    {
                      num74 = x3_4 - torg.x;
                      num75 = y3_4 - torg.y;
                    }
                  }
                  else
                  {
                    num74 = p4[2] - torg.x;
                    num75 = p4[3] - torg.y;
                  }
                }
                else if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, circumcenter.x, circumcenter.y))
                {
                  num74 = num17;
                  num75 = num18;
                }
                else
                {
                  num74 = p2[2] - torg.x;
                  num75 = p2[3] - torg.y;
                }
              }
              else
              {
                PointBetweenPoints(num78, num79, point1.x, point1.y, num8, num9, ref p4);
                if (Math.Abs(p4[0] - 1.0) <= 1E-50 && p3[0] > 0.0)
                {
                  if ((point2.x - num51) * (point2.x - num51) + (point2.y - num52) * (point2.y - num52) > num3 * ((point2.x - num8) * (point2.x - num8) + (point2.y - num9) * (point2.y - num9)) && IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num51, num52) && MinDistanceToNeighbor(num51, num52, ref otri) > MinDistanceToNeighbor(num8, num9, ref otri))
                  {
                    num74 = num51 - torg.x;
                    num75 = num52 - torg.y;
                  }
                  else if (IsBadTriangleAngle(point4.x, point4.y, point3.x, point3.y, num8, num9))
                  {
                    double num85 = Math.Sqrt((num8 - point1.x) * (num8 - point1.x) + (num9 - point1.y) * (num9 - point1.y));
                    double num86 = point1.x - num8;
                    double num87 = point1.y - num9;
                    double num88 = num86 / num85;
                    double num89 = num87 / num85;
                    double x3_5 = num8 + num88 * num2 * Math.Sqrt(d1);
                    double y3_5 = num9 + num89 * num2 * Math.Sqrt(d1);
                    if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, x3_5, y3_5))
                    {
                      num74 = num17;
                      num75 = num18;
                    }
                    else
                    {
                      num74 = x3_5 - torg.x;
                      num75 = y3_5 - torg.y;
                    }
                  }
                  else
                  {
                    num74 = p4[2] - torg.x;
                    num75 = p4[3] - torg.y;
                  }
                }
                else if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, num78, num79))
                {
                  double num90 = Math.Sqrt((num78 - point1.x) * (num78 - point1.x) + (num79 - point1.y) * (num79 - point1.y));
                  double num91 = point1.x - num78;
                  double num92 = point1.y - num79;
                  double num93 = num91 / num90;
                  double num94 = num92 / num90;
                  double x3_6 = num78 + num93 * num2 * Math.Sqrt(d1);
                  double y3_6 = num79 + num94 * num2 * Math.Sqrt(d1);
                  if (IsBadTriangleAngle(point3.x, point3.y, point4.x, point4.y, x3_6, y3_6))
                  {
                    num74 = num17;
                    num75 = num18;
                  }
                  else
                  {
                    num74 = x3_6 - torg.x;
                    num75 = y3_6 - torg.y;
                  }
                }
                else
                {
                  num74 = num78 - torg.x;
                  num75 = num79 - torg.y;
                }
              }
              if ((point2.x - point1.x) * (point2.x - point1.x) + (point2.y - point1.y) * (point2.y - point1.y) > num3 * ((point2.x - (num74 + torg.x)) * (point2.x - (num74 + torg.x)) + (point2.y - (num75 + torg.y)) * (point2.y - (num75 + torg.y))))
              {
                num74 = num17;
                num75 = num18;
              }
            }
          }
          if (isObtuse)
          {
            if (neighborsVertex1 & neighborsVertex2)
            {
              if (num4 * ((point2.x - x1_3) * (point2.x - x1_3) + (point2.y - y1_3) * (point2.y - y1_3)) > (point2.x - x1_2) * (point2.x - x1_2) + (point2.y - y1_2) * (point2.y - y1_2))
              {
                num17 = num74;
                num18 = num75;
              }
              else
              {
                num17 = num41;
                num18 = num42;
              }
            }
            else if (neighborsVertex1)
            {
              if (num4 * ((point2.x - (num74 + torg.x)) * (point2.x - (num74 + torg.x)) + (point2.y - (num75 + torg.y)) * (point2.y - (num75 + torg.y))) > (point2.x - x1_2) * (point2.x - x1_2) + (point2.y - y1_2) * (point2.y - y1_2))
              {
                num17 = num74;
                num18 = num75;
              }
              else
              {
                num17 = num41;
                num18 = num42;
              }
            }
            else if (neighborsVertex2)
            {
              if (num4 * ((point2.x - x1_3) * (point2.x - x1_3) + (point2.y - y1_3) * (point2.y - y1_3)) > (point2.x - (num41 + torg.x)) * (point2.x - (num41 + torg.x)) + (point2.y - (num42 + torg.y)) * (point2.y - (num42 + torg.y)))
              {
                num17 = num74;
                num18 = num75;
              }
              else
              {
                num17 = num41;
                num18 = num42;
              }
            }
            else if (num4 * ((point2.x - (num74 + torg.x)) * (point2.x - (num74 + torg.x)) + (point2.y - (num75 + torg.y)) * (point2.y - (num75 + torg.y))) > (point2.x - (num41 + torg.x)) * (point2.x - (num41 + torg.x)) + (point2.y - (num42 + torg.y)) * (point2.y - (num42 + torg.y)))
            {
              num17 = num74;
              num18 = num75;
            }
            else
            {
              num17 = num41;
              num18 = num42;
            }
          }
          else if (neighborsVertex1 & neighborsVertex2)
          {
            if (num4 * ((point2.x - x1_3) * (point2.x - x1_3) + (point2.y - y1_3) * (point2.y - y1_3)) > (point2.x - x1_2) * (point2.x - x1_2) + (point2.y - y1_2) * (point2.y - y1_2))
            {
              num17 = num74;
              num18 = num75;
            }
            else
            {
              num17 = num41;
              num18 = num42;
            }
          }
          else if (neighborsVertex1)
          {
            if (num4 * ((point2.x - (num74 + torg.x)) * (point2.x - (num74 + torg.x)) + (point2.y - (num75 + torg.y)) * (point2.y - (num75 + torg.y))) > (point2.x - x1_2) * (point2.x - x1_2) + (point2.y - y1_2) * (point2.y - y1_2))
            {
              num17 = num74;
              num18 = num75;
            }
            else
            {
              num17 = num41;
              num18 = num42;
            }
          }
          else if (neighborsVertex2)
          {
            if (num4 * ((point2.x - x1_3) * (point2.x - x1_3) + (point2.y - y1_3) * (point2.y - y1_3)) > (point2.x - (num41 + torg.x)) * (point2.x - (num41 + torg.x)) + (point2.y - (num42 + torg.y)) * (point2.y - (num42 + torg.y)))
            {
              num17 = num74;
              num18 = num75;
            }
            else
            {
              num17 = num41;
              num18 = num42;
            }
          }
          else if (num4 * ((point2.x - (num74 + torg.x)) * (point2.x - (num74 + torg.x)) + (point2.y - (num75 + torg.y)) * (point2.y - (num75 + torg.y))) > (point2.x - (num41 + torg.x)) * (point2.x - (num41 + torg.x)) + (point2.y - (num42 + torg.y)) * (point2.y - (num42 + torg.y)))
          {
            num17 = num74;
            num18 = num75;
          }
          else
          {
            num17 = num41;
            num18 = num42;
          }
        }
      }
      Point newLocation = new Point();
      if (num5 <= 0)
      {
        newLocation.x = torg.x + num17;
        newLocation.y = torg.y + num18;
      }
      else
      {
        newLocation.x = num6 + num17;
        newLocation.y = num7 + num18;
      }
      xi = (num13 * num17 - num12 * num18) * (2.0 * num16);
      eta = (num10 * num18 - num11 * num17) * (2.0 * num16);
      return newLocation;
    }

    private int LongestShortestEdge(double aodist, double dadist, double dodist)
    {
      int num1;
      int num2;
      int num3;
      if (dodist < aodist && dodist < dadist)
      {
        num1 = 3;
        if (aodist < dadist)
        {
          num2 = 2;
          num3 = 1;
        }
        else
        {
          num2 = 1;
          num3 = 2;
        }
      }
      else if (aodist < dadist)
      {
        num1 = 1;
        if (dodist < dadist)
        {
          num2 = 2;
          num3 = 3;
        }
        else
        {
          num2 = 3;
          num3 = 2;
        }
      }
      else
      {
        num1 = 2;
        if (aodist < dodist)
        {
          num2 = 3;
          num3 = 1;
        }
        else
        {
          num2 = 1;
          num3 = 3;
        }
      }
      return num1 * 100 + num3 * 10 + num2;
    }

    private int DoSmoothing(
      Otri badotri,
      Vertex torg,
      Vertex tdest,
      Vertex tapex,
      ref double[] newloc)
    {
      double[] numArray = new double[6];
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      double[] points1 = new double[500];
      double[] points2 = new double[500];
      double[] points3 = new double[500];
      int starPoints1 = GetStarPoints(badotri, torg, tdest, tapex, 1, ref points1);
      if (torg.type == VertexType.FreeVertex && starPoints1 != 0 && ValidPolygonAngles(starPoints1, points1) && (behavior.MaxAngle != 0.0 ? GetWedgeIntersection(starPoints1, points1, ref newloc) : GetWedgeIntersectionWithoutMaxAngle(starPoints1, points1, ref newloc)))
      {
        numArray[0] = newloc[0];
        numArray[1] = newloc[1];
        ++num1;
        num2 = 1;
      }
      int starPoints2 = GetStarPoints(badotri, torg, tdest, tapex, 2, ref points2);
      if (tdest.type == VertexType.FreeVertex && starPoints2 != 0 && ValidPolygonAngles(starPoints2, points2) && (behavior.MaxAngle != 0.0 ? GetWedgeIntersection(starPoints2, points2, ref newloc) : GetWedgeIntersectionWithoutMaxAngle(starPoints2, points2, ref newloc)))
      {
        numArray[2] = newloc[0];
        numArray[3] = newloc[1];
        ++num1;
        num3 = 2;
      }
      int starPoints3 = GetStarPoints(badotri, torg, tdest, tapex, 3, ref points3);
      if (tapex.type == VertexType.FreeVertex && starPoints3 != 0 && ValidPolygonAngles(starPoints3, points3) && (behavior.MaxAngle != 0.0 ? GetWedgeIntersection(starPoints3, points3, ref newloc) : GetWedgeIntersectionWithoutMaxAngle(starPoints3, points3, ref newloc)))
      {
        numArray[4] = newloc[0];
        numArray[5] = newloc[1];
        ++num1;
        num4 = 3;
      }
      if (num1 > 0)
      {
        if (num2 > 0)
        {
          newloc[0] = numArray[0];
          newloc[1] = numArray[1];
          return num2;
        }
        if (num3 > 0)
        {
          newloc[0] = numArray[2];
          newloc[1] = numArray[3];
          return num3;
        }
        if (num4 > 0)
        {
          newloc[0] = numArray[4];
          newloc[1] = numArray[5];
          return num4;
        }
      }
      return 0;
    }

    private int GetStarPoints(
      Otri badotri,
      Vertex p,
      Vertex q,
      Vertex r,
      int whichPoint,
      ref double[] points)
    {
      Otri neighotri = new Otri();
      double first_x = 0.0;
      double first_y = 0.0;
      double second_x = 0.0;
      double second_y = 0.0;
      double num1 = 0.0;
      double num2 = 0.0;
      double[] thirdpoint = new double[2];
      int index1 = 0;
      switch (whichPoint)
      {
        case 1:
          first_x = p.x;
          first_y = p.y;
          second_x = r.x;
          second_y = r.y;
          num1 = q.x;
          num2 = q.y;
          break;
        case 2:
          first_x = q.x;
          first_y = q.y;
          second_x = p.x;
          second_y = p.y;
          num1 = r.x;
          num2 = r.y;
          break;
        case 3:
          first_x = r.x;
          first_y = r.y;
          second_x = q.x;
          second_y = q.y;
          num1 = p.x;
          num2 = p.y;
          break;
      }
      Otri badotri1 = badotri;
      points[index1] = second_x;
      int index2 = index1 + 1;
      points[index2] = second_y;
      int index3 = index2 + 1;
      thirdpoint[0] = second_x;
      thirdpoint[1] = second_y;
      while (!GetNeighborsVertex(badotri1, first_x, first_y, second_x, second_y, ref thirdpoint, ref neighotri))
      {
        badotri1 = neighotri;
        second_x = thirdpoint[0];
        second_y = thirdpoint[1];
        points[index3] = thirdpoint[0];
        int index4 = index3 + 1;
        points[index4] = thirdpoint[1];
        index3 = index4 + 1;
        if (Math.Abs(thirdpoint[0] - num1) <= 1E-50 && Math.Abs(thirdpoint[1] - num2) <= 1E-50)
          goto label_8;
      }
      index3 = 0;
label_8:
      return index3 / 2;
    }

    private bool GetNeighborsVertex(
      Otri badotri,
      double first_x,
      double first_y,
      double second_x,
      double second_y,
      ref double[] thirdpoint,
      ref Otri neighotri)
    {
      Otri o2 = new Otri();
      bool neighborsVertex = false;
      Vertex vertex1 = null;
      Vertex vertex2 = null;
      Vertex vertex3 = null;
      int num1 = 0;
      int num2 = 0;
      for (badotri.orient = 0; badotri.orient < 3; ++badotri.orient)
      {
        badotri.Sym(ref o2);
        if (o2.triangle != Mesh.dummytri)
        {
          vertex1 = o2.Org();
          vertex2 = o2.Dest();
          vertex3 = o2.Apex();
          if ((vertex1.x != vertex2.x || vertex1.y != vertex2.y) && (vertex2.x != vertex3.x || vertex2.y != vertex3.y) && (vertex1.x != vertex3.x || vertex1.y != vertex3.y))
          {
            num1 = 0;
            if (Math.Abs(first_x - vertex1.x) < 1E-50 && Math.Abs(first_y - vertex1.y) < 1E-50)
              num1 = 11;
            else if (Math.Abs(first_x - vertex2.x) < 1E-50 && Math.Abs(first_y - vertex2.y) < 1E-50)
              num1 = 12;
            else if (Math.Abs(first_x - vertex3.x) < 1E-50 && Math.Abs(first_y - vertex3.y) < 1E-50)
              num1 = 13;
            num2 = 0;
            if (Math.Abs(second_x - vertex1.x) < 1E-50 && Math.Abs(second_y - vertex1.y) < 1E-50)
              num2 = 21;
            else if (Math.Abs(second_x - vertex2.x) < 1E-50 && Math.Abs(second_y - vertex2.y) < 1E-50)
              num2 = 22;
            else if (Math.Abs(second_x - vertex3.x) < 1E-50 && Math.Abs(second_y - vertex3.y) < 1E-50)
              num2 = 23;
          }
        }
        if (num1 == 11 && (num2 == 22 || num2 == 23) || num1 == 12 && (num2 == 21 || num2 == 23) || num1 == 13 && (num2 == 21 || num2 == 22))
          break;
      }
      switch (num1)
      {
        case 0:
          neighborsVertex = true;
          break;
        case 11:
          switch (num2)
          {
            case 22:
              thirdpoint[0] = vertex3.x;
              thirdpoint[1] = vertex3.y;
              break;
            case 23:
              thirdpoint[0] = vertex2.x;
              thirdpoint[1] = vertex2.y;
              break;
            default:
              neighborsVertex = true;
              break;
          }
          break;
        case 12:
          switch (num2)
          {
            case 21:
              thirdpoint[0] = vertex3.x;
              thirdpoint[1] = vertex3.y;
              break;
            case 23:
              thirdpoint[0] = vertex1.x;
              thirdpoint[1] = vertex1.y;
              break;
            default:
              neighborsVertex = true;
              break;
          }
          break;
        case 13:
          switch (num2)
          {
            case 21:
              thirdpoint[0] = vertex2.x;
              thirdpoint[1] = vertex2.y;
              break;
            case 22:
              thirdpoint[0] = vertex1.x;
              thirdpoint[1] = vertex1.y;
              break;
            default:
              neighborsVertex = true;
              break;
          }
          break;
        default:
          if (num2 == 0)
          {
            neighborsVertex = true;
          }
          break;
      }
      neighotri = o2;
      return neighborsVertex;
    }

    private bool GetWedgeIntersectionWithoutMaxAngle(
      int numpoints,
      double[] points,
      ref double[] newloc)
    {
      double[] numArray1 = new double[2 * numpoints];
      double[] numArray2 = new double[2 * numpoints];
      double[] numArray3 = new double[2 * numpoints];
      double[] numArray4 = new double[2000];
      double[] p = new double[3];
      double[] convexPoly = new double[500];
      int numpoints1 = 0;
      double x1 = points[2 * numpoints - 4];
      double y1 = points[2 * numpoints - 3];
      double x3 = points[2 * numpoints - 2];
      double y3 = points[2 * numpoints - 1];
      double num1 = behavior.MinAngle * Math.PI / 180.0;
      double num2;
      double num3;
      if (behavior.goodAngle == 1.0)
      {
        num2 = 0.0;
        num3 = 0.0;
      }
      else
      {
        num2 = 0.5 / Math.Tan(num1);
        num3 = 0.5 / Math.Sin(num1);
      }
      for (int index1 = 0; index1 < numpoints * 2; index1 += 2)
      {
        double point1 = points[index1];
        double point2 = points[index1 + 1];
        double num4 = x3 - x1;
        double num5 = y3 - y1;
        double num6 = Math.Sqrt(num4 * num4 + num5 * num5);
        numArray1[index1 / 2] = x1 + 0.5 * num4 - num2 * num5;
        numArray2[index1 / 2] = y1 + 0.5 * num5 + num2 * num4;
        numArray3[index1 / 2] = num3 * num6;
        numArray1[numpoints + index1 / 2] = numArray1[index1 / 2];
        numArray2[numpoints + index1 / 2] = numArray2[index1 / 2];
        numArray3[numpoints + index1 / 2] = numArray3[index1 / 2];
        double num7 = (x1 + x3) / 2.0;
        double num8 = (y1 + y3) / 2.0;
        double num9 = Math.Sqrt((numArray1[index1 / 2] - num7) * (numArray1[index1 / 2] - num7) + (numArray2[index1 / 2] - num8) * (numArray2[index1 / 2] - num8));
        double num10 = (numArray1[index1 / 2] - num7) / num9;
        double num11 = (numArray2[index1 / 2] - num8) / num9;
        double num12 = numArray1[index1 / 2] + num10 * numArray3[index1 / 2];
        double num13 = numArray2[index1 / 2] + num11 * numArray3[index1 / 2];
        double num14 = x3 - x1;
        double num15 = y3 - y1;
        double x2 = x3 * Math.Cos(num1) - y3 * Math.Sin(num1) + x1 - x1 * Math.Cos(num1) + y1 * Math.Sin(num1);
        double y2 = x3 * Math.Sin(num1) + y3 * Math.Cos(num1) + y1 - x1 * Math.Sin(num1) - y1 * Math.Cos(num1);
        numArray4[index1 * 16] = x1;
        numArray4[index1 * 16 + 1] = y1;
        numArray4[index1 * 16 + 2] = x2;
        numArray4[index1 * 16 + 3] = y2;
        num14 = x1 - x3;
        num15 = y1 - y3;
        double x4 = x1 * Math.Cos(num1) + y1 * Math.Sin(num1) + x3 - x3 * Math.Cos(num1) - y3 * Math.Sin(num1);
        double y4 = -x1 * Math.Sin(num1) + y1 * Math.Cos(num1) + y3 + x3 * Math.Sin(num1) - y3 * Math.Cos(num1);
        numArray4[index1 * 16 + 4] = x4;
        numArray4[index1 * 16 + 5] = y4;
        numArray4[index1 * 16 + 6] = x3;
        numArray4[index1 * 16 + 7] = y3;
        num14 = num12 - numArray1[index1 / 2];
        num15 = num13 - numArray2[index1 / 2];
        double num16 = num12;
        double num17 = num13;
        for (int index2 = 1; index2 < 4; ++index2)
        {
          double num18 = num12 * Math.Cos((Math.PI / 3.0 - num1) * index2) + num13 * Math.Sin((Math.PI / 3.0 - num1) * index2) + numArray1[index1 / 2] - numArray1[index1 / 2] * Math.Cos((Math.PI / 3.0 - num1) * index2) - numArray2[index1 / 2] * Math.Sin((Math.PI / 3.0 - num1) * index2);
          double num19 = -num12 * Math.Sin((Math.PI / 3.0 - num1) * index2) + num13 * Math.Cos((Math.PI / 3.0 - num1) * index2) + numArray2[index1 / 2] + numArray1[index1 / 2] * Math.Sin((Math.PI / 3.0 - num1) * index2) - numArray2[index1 / 2] * Math.Cos((Math.PI / 3.0 - num1) * index2);
          numArray4[index1 * 16 + 8 + 4 * (index2 - 1)] = num18;
          numArray4[index1 * 16 + 9 + 4 * (index2 - 1)] = num19;
          numArray4[index1 * 16 + 10 + 4 * (index2 - 1)] = num16;
          numArray4[index1 * 16 + 11 + 4 * (index2 - 1)] = num17;
          num16 = num18;
          num17 = num19;
        }
        double num20 = num12;
        double num21 = num13;
        for (int index3 = 1; index3 < 4; ++index3)
        {
          double num22 = num12 * Math.Cos((Math.PI / 3.0 - num1) * index3) - num13 * Math.Sin((Math.PI / 3.0 - num1) * index3) + numArray1[index1 / 2] - numArray1[index1 / 2] * Math.Cos((Math.PI / 3.0 - num1) * index3) + numArray2[index1 / 2] * Math.Sin((Math.PI / 3.0 - num1) * index3);
          double num23 = num12 * Math.Sin((Math.PI / 3.0 - num1) * index3) + num13 * Math.Cos((Math.PI / 3.0 - num1) * index3) + numArray2[index1 / 2] - numArray1[index1 / 2] * Math.Sin((Math.PI / 3.0 - num1) * index3) - numArray2[index1 / 2] * Math.Cos((Math.PI / 3.0 - num1) * index3);
          numArray4[index1 * 16 + 20 + 4 * (index3 - 1)] = num20;
          numArray4[index1 * 16 + 21 + 4 * (index3 - 1)] = num21;
          numArray4[index1 * 16 + 22 + 4 * (index3 - 1)] = num22;
          numArray4[index1 * 16 + 23 + 4 * (index3 - 1)] = num23;
          num20 = num22;
          num21 = num23;
        }
        if (index1 == 0)
        {
          LineLineIntersection(x1, y1, x2, y2, x3, y3, x4, y4, ref p);
          if (p[0] == 1.0)
          {
            convexPoly[0] = p[1];
            convexPoly[1] = p[2];
            convexPoly[2] = numArray4[index1 * 16 + 16];
            convexPoly[3] = numArray4[index1 * 16 + 17];
            convexPoly[4] = numArray4[index1 * 16 + 12];
            convexPoly[5] = numArray4[index1 * 16 + 13];
            convexPoly[6] = numArray4[index1 * 16 + 8];
            convexPoly[7] = numArray4[index1 * 16 + 9];
            convexPoly[8] = num12;
            convexPoly[9] = num13;
            convexPoly[10] = numArray4[index1 * 16 + 22];
            convexPoly[11] = numArray4[index1 * 16 + 23];
            convexPoly[12] = numArray4[index1 * 16 + 26];
            convexPoly[13] = numArray4[index1 * 16 + 27];
            convexPoly[14] = numArray4[index1 * 16 + 30];
            convexPoly[15] = numArray4[index1 * 16 + 31];
          }
        }
        x1 = x3;
        y1 = y3;
        x3 = point1;
        y3 = point2;
      }
      if (numpoints != 0)
      {
        int num24 = (numpoints - 1) / 2 + 1;
        int num25 = 0;
        int num26 = 0;
        int num27 = 1;
        int numvertices = 8;
        for (int index = 0; index < 32; index += 4)
        {
          numpoints1 = HalfPlaneIntersection(numvertices, ref convexPoly, numArray4[32 * num24 + index], numArray4[32 * num24 + 1 + index], numArray4[32 * num24 + 2 + index], numArray4[32 * num24 + 3 + index]);
          if (numpoints1 == 0)
            return false;
          numvertices = numpoints1;
        }
        for (int index4 = num26 + 1; index4 < numpoints - 1; ++index4)
        {
          for (int index5 = 0; index5 < 32; index5 += 4)
          {
            numpoints1 = HalfPlaneIntersection(numvertices, ref convexPoly, numArray4[32 * (num27 + num24 * num25) + index5], numArray4[32 * (num27 + num24 * num25) + 1 + index5], numArray4[32 * (num27 + num24 * num25) + 2 + index5], numArray4[32 * (num27 + num24 * num25) + 3 + index5]);
            if (numpoints1 == 0)
              return false;
            numvertices = numpoints1;
          }
          num27 += num25;
          num25 = (num25 + 1) % 2;
        }
        FindPolyCentroid(numpoints1, convexPoly, ref newloc);
        if (!behavior.fixedArea)
          return true;
      }
      return false;
    }

    private bool GetWedgeIntersection(int numpoints, double[] points, ref double[] newloc)
    {
      double[] numArray1 = new double[2 * numpoints];
      double[] numArray2 = new double[2 * numpoints];
      double[] numArray3 = new double[2 * numpoints];
      double[] numArray4 = new double[2000];
      double[] p1 = new double[3];
      double[] p2 = new double[3];
      double[] p3 = new double[3];
      double[] p4 = new double[3];
      double[] convexPoly = new double[500];
      int numpoints1 = 0;
      int num1 = 0;
      double x1 = points[2 * numpoints - 4];
      double y1 = points[2 * numpoints - 3];
      double x3 = points[2 * numpoints - 2];
      double y3 = points[2 * numpoints - 1];
      double num2 = behavior.MinAngle * Math.PI / 180.0;
      double num3 = Math.Sin(num2);
      double num4 = Math.Cos(num2);
      double num5 = behavior.MaxAngle * Math.PI / 180.0;
      double num6 = Math.Sin(num5);
      double num7 = Math.Cos(num5);
      double num8;
      double num9;
      if (behavior.goodAngle == 1.0)
      {
        num8 = 0.0;
        num9 = 0.0;
      }
      else
      {
        num8 = 0.5 / Math.Tan(num2);
        num9 = 0.5 / Math.Sin(num2);
      }
      for (int index1 = 0; index1 < numpoints * 2; index1 += 2)
      {
        double point1 = points[index1];
        double point2 = points[index1 + 1];
        double num10 = x3 - x1;
        double num11 = y3 - y1;
        double num12 = Math.Sqrt(num10 * num10 + num11 * num11);
        numArray1[index1 / 2] = x1 + 0.5 * num10 - num8 * num11;
        numArray2[index1 / 2] = y1 + 0.5 * num11 + num8 * num10;
        numArray3[index1 / 2] = num9 * num12;
        numArray1[numpoints + index1 / 2] = numArray1[index1 / 2];
        numArray2[numpoints + index1 / 2] = numArray2[index1 / 2];
        numArray3[numpoints + index1 / 2] = numArray3[index1 / 2];
        double num13 = (x1 + x3) / 2.0;
        double num14 = (y1 + y3) / 2.0;
        double num15 = Math.Sqrt((numArray1[index1 / 2] - num13) * (numArray1[index1 / 2] - num13) + (numArray2[index1 / 2] - num14) * (numArray2[index1 / 2] - num14));
        double num16 = (numArray1[index1 / 2] - num13) / num15;
        double num17 = (numArray2[index1 / 2] - num14) / num15;
        double num18 = numArray1[index1 / 2] + num16 * numArray3[index1 / 2];
        double num19 = numArray2[index1 / 2] + num17 * numArray3[index1 / 2];
        double num20 = x3 - x1;
        double num21 = y3 - y1;
        double x2_1 = x3 * num4 - y3 * num3 + x1 - x1 * num4 + y1 * num3;
        double y2_1 = x3 * num3 + y3 * num4 + y1 - x1 * num3 - y1 * num4;
        numArray4[index1 * 20] = x1;
        numArray4[index1 * 20 + 1] = y1;
        numArray4[index1 * 20 + 2] = x2_1;
        numArray4[index1 * 20 + 3] = y2_1;
        num20 = x1 - x3;
        num21 = y1 - y3;
        double x4_1 = x1 * num4 + y1 * num3 + x3 - x3 * num4 - y3 * num3;
        double y4_1 = -x1 * num3 + y1 * num4 + y3 + x3 * num3 - y3 * num4;
        numArray4[index1 * 20 + 4] = x4_1;
        numArray4[index1 * 20 + 5] = y4_1;
        numArray4[index1 * 20 + 6] = x3;
        numArray4[index1 * 20 + 7] = y3;
        num20 = num18 - numArray1[index1 / 2];
        num21 = num19 - numArray2[index1 / 2];
        double num22 = num18;
        double num23 = num19;
        double num24 = 2.0 * behavior.MaxAngle + behavior.MinAngle - 180.0;
        double num25;
        double num26;
        if (num24 <= 0.0)
        {
          num1 = 4;
          num25 = 1.0;
          num26 = 1.0;
        }
        else if (num24 <= 5.0)
        {
          num1 = 6;
          num25 = 2.0;
          num26 = 2.0;
        }
        else if (num24 <= 10.0)
        {
          num1 = 8;
          num25 = 3.0;
          num26 = 3.0;
        }
        else
        {
          num1 = 10;
          num25 = 4.0;
          num26 = 4.0;
        }
        double num27 = num24 * Math.PI / 180.0;
        for (int index2 = 1; index2 < num25; ++index2)
        {
          if (num25 != 1.0)
          {
            double num28 = num18 * Math.Cos(num27 / (num25 - 1.0) * index2) + num19 * Math.Sin(num27 / (num25 - 1.0) * index2) + numArray1[index1 / 2] - numArray1[index1 / 2] * Math.Cos(num27 / (num25 - 1.0) * index2) - numArray2[index1 / 2] * Math.Sin(num27 / (num25 - 1.0) * index2);
            double num29 = -num18 * Math.Sin(num27 / (num25 - 1.0) * index2) + num19 * Math.Cos(num27 / (num25 - 1.0) * index2) + numArray2[index1 / 2] + numArray1[index1 / 2] * Math.Sin(num27 / (num25 - 1.0) * index2) - numArray2[index1 / 2] * Math.Cos(num27 / (num25 - 1.0) * index2);
            numArray4[index1 * 20 + 8 + 4 * (index2 - 1)] = num28;
            numArray4[index1 * 20 + 9 + 4 * (index2 - 1)] = num29;
            numArray4[index1 * 20 + 10 + 4 * (index2 - 1)] = num22;
            numArray4[index1 * 20 + 11 + 4 * (index2 - 1)] = num23;
            num22 = num28;
            num23 = num29;
          }
        }
        num20 = x1 - x3;
        num21 = y1 - y3;
        double x4_2 = x1 * num7 + y1 * num6 + x3 - x3 * num7 - y3 * num6;
        double y4_2 = -x1 * num6 + y1 * num7 + y3 + x3 * num6 - y3 * num7;
        numArray4[index1 * 20 + 20] = x3;
        numArray4[index1 * 20 + 21] = y3;
        numArray4[index1 * 20 + 22] = x4_2;
        numArray4[index1 * 20 + 23] = y4_2;
        double num30 = num18;
        double num31 = num19;
        for (int index3 = 1; index3 < num26; ++index3)
        {
          if (num26 != 1.0)
          {
            double num32 = num18 * Math.Cos(num27 / (num26 - 1.0) * index3) - num19 * Math.Sin(num27 / (num26 - 1.0) * index3) + numArray1[index1 / 2] - numArray1[index1 / 2] * Math.Cos(num27 / (num26 - 1.0) * index3) + numArray2[index1 / 2] * Math.Sin(num27 / (num26 - 1.0) * index3);
            double num33 = num18 * Math.Sin(num27 / (num26 - 1.0) * index3) + num19 * Math.Cos(num27 / (num26 - 1.0) * index3) + numArray2[index1 / 2] - numArray1[index1 / 2] * Math.Sin(num27 / (num26 - 1.0) * index3) - numArray2[index1 / 2] * Math.Cos(num27 / (num26 - 1.0) * index3);
            numArray4[index1 * 20 + 24 + 4 * (index3 - 1)] = num30;
            numArray4[index1 * 20 + 25 + 4 * (index3 - 1)] = num31;
            numArray4[index1 * 20 + 26 + 4 * (index3 - 1)] = num32;
            numArray4[index1 * 20 + 27 + 4 * (index3 - 1)] = num33;
            num30 = num32;
            num31 = num33;
          }
        }
        num20 = x3 - x1;
        num21 = y3 - y1;
        double x2_2 = x3 * num7 - y3 * num6 + x1 - x1 * num7 + y1 * num6;
        double y2_2 = x3 * num6 + y3 * num7 + y1 - x1 * num6 - y1 * num7;
        numArray4[index1 * 20 + 36] = x2_2;
        numArray4[index1 * 20 + 37] = y2_2;
        numArray4[index1 * 20 + 38] = x1;
        numArray4[index1 * 20 + 39] = y1;
        if (index1 == 0)
        {
          switch (num1)
          {
            case 4:
              LineLineIntersection(x1, y1, x2_1, y2_1, x3, y3, x4_1, y4_1, ref p1);
              LineLineIntersection(x1, y1, x2_1, y2_1, x3, y3, x4_2, y4_2, ref p2);
              LineLineIntersection(x1, y1, x2_2, y2_2, x3, y3, x4_2, y4_2, ref p3);
              LineLineIntersection(x1, y1, x2_2, y2_2, x3, y3, x4_1, y4_1, ref p4);
              if (p1[0] == 1.0 && p2[0] == 1.0 && p3[0] == 1.0 && p4[0] == 1.0)
              {
                convexPoly[0] = p1[1];
                convexPoly[1] = p1[2];
                convexPoly[2] = p2[1];
                convexPoly[3] = p2[2];
                convexPoly[4] = p3[1];
                convexPoly[5] = p3[2];
                convexPoly[6] = p4[1];
                convexPoly[7] = p4[2];
              }
              break;
            case 6:
              LineLineIntersection(x1, y1, x2_1, y2_1, x3, y3, x4_1, y4_1, ref p1);
              LineLineIntersection(x1, y1, x2_1, y2_1, x3, y3, x4_2, y4_2, ref p2);
              LineLineIntersection(x1, y1, x2_2, y2_2, x3, y3, x4_1, y4_1, ref p3);
              if (p1[0] == 1.0 && p2[0] == 1.0 && p3[0] == 1.0)
              {
                convexPoly[0] = p1[1];
                convexPoly[1] = p1[2];
                convexPoly[2] = p2[1];
                convexPoly[3] = p2[2];
                convexPoly[4] = numArray4[index1 * 20 + 8];
                convexPoly[5] = numArray4[index1 * 20 + 9];
                convexPoly[6] = num18;
                convexPoly[7] = num19;
                convexPoly[8] = numArray4[index1 * 20 + 26];
                convexPoly[9] = numArray4[index1 * 20 + 27];
                convexPoly[10] = p3[1];
                convexPoly[11] = p3[2];
              }
              break;
            case 8:
              LineLineIntersection(x1, y1, x2_1, y2_1, x3, y3, x4_1, y4_1, ref p1);
              LineLineIntersection(x1, y1, x2_1, y2_1, x3, y3, x4_2, y4_2, ref p2);
              LineLineIntersection(x1, y1, x2_2, y2_2, x3, y3, x4_1, y4_1, ref p3);
              if (p1[0] == 1.0 && p2[0] == 1.0 && p3[0] == 1.0)
              {
                convexPoly[0] = p1[1];
                convexPoly[1] = p1[2];
                convexPoly[2] = p2[1];
                convexPoly[3] = p2[2];
                convexPoly[4] = numArray4[index1 * 20 + 12];
                convexPoly[5] = numArray4[index1 * 20 + 13];
                convexPoly[6] = numArray4[index1 * 20 + 8];
                convexPoly[7] = numArray4[index1 * 20 + 9];
                convexPoly[8] = num18;
                convexPoly[9] = num19;
                convexPoly[10] = numArray4[index1 * 20 + 26];
                convexPoly[11] = numArray4[index1 * 20 + 27];
                convexPoly[12] = numArray4[index1 * 20 + 30];
                convexPoly[13] = numArray4[index1 * 20 + 31];
                convexPoly[14] = p3[1];
                convexPoly[15] = p3[2];
              }
              break;
            case 10:
              LineLineIntersection(x1, y1, x2_1, y2_1, x3, y3, x4_1, y4_1, ref p1);
              LineLineIntersection(x1, y1, x2_1, y2_1, x3, y3, x4_2, y4_2, ref p2);
              LineLineIntersection(x1, y1, x2_2, y2_2, x3, y3, x4_1, y4_1, ref p3);
              if (p1[0] == 1.0 && p2[0] == 1.0 && p3[0] == 1.0)
              {
                convexPoly[0] = p1[1];
                convexPoly[1] = p1[2];
                convexPoly[2] = p2[1];
                convexPoly[3] = p2[2];
                convexPoly[4] = numArray4[index1 * 20 + 16];
                convexPoly[5] = numArray4[index1 * 20 + 17];
                convexPoly[6] = numArray4[index1 * 20 + 12];
                convexPoly[7] = numArray4[index1 * 20 + 13];
                convexPoly[8] = numArray4[index1 * 20 + 8];
                convexPoly[9] = numArray4[index1 * 20 + 9];
                convexPoly[10] = num18;
                convexPoly[11] = num19;
                convexPoly[12] = numArray4[index1 * 20 + 28];
                convexPoly[13] = numArray4[index1 * 20 + 29];
                convexPoly[14] = numArray4[index1 * 20 + 32];
                convexPoly[15] = numArray4[index1 * 20 + 33];
                convexPoly[16] = numArray4[index1 * 20 + 34];
                convexPoly[17] = numArray4[index1 * 20 + 35];
                convexPoly[18] = p3[1];
                convexPoly[19] = p3[2];
              }
              break;
          }
        }
        x1 = x3;
        y1 = y3;
        x3 = point1;
        y3 = point2;
      }
      if (numpoints != 0)
      {
        int num34 = (numpoints - 1) / 2 + 1;
        int num35 = 0;
        int num36 = 0;
        int num37 = 1;
        int numvertices = num1;
        for (int index = 0; index < 40; index += 4)
        {
          if ((num1 != 4 || index != 8 && index != 12 && index != 16 && index != 24 && index != 28 && index != 32) && (num1 != 6 || index != 12 && index != 16 && index != 28 && index != 32) && (num1 != 8 || index != 16 && index != 32))
          {
            numpoints1 = HalfPlaneIntersection(numvertices, ref convexPoly, numArray4[40 * num34 + index], numArray4[40 * num34 + 1 + index], numArray4[40 * num34 + 2 + index], numArray4[40 * num34 + 3 + index]);
            if (numpoints1 == 0)
              return false;
            numvertices = numpoints1;
          }
        }
        for (int index4 = num36 + 1; index4 < numpoints - 1; ++index4)
        {
          for (int index5 = 0; index5 < 40; index5 += 4)
          {
            if ((num1 != 4 || index5 != 8 && index5 != 12 && index5 != 16 && index5 != 24 && index5 != 28 && index5 != 32) && (num1 != 6 || index5 != 12 && index5 != 16 && index5 != 28 && index5 != 32) && (num1 != 8 || index5 != 16 && index5 != 32))
            {
              numpoints1 = HalfPlaneIntersection(numvertices, ref convexPoly, numArray4[40 * (num37 + num34 * num35) + index5], numArray4[40 * (num37 + num34 * num35) + 1 + index5], numArray4[40 * (num37 + num34 * num35) + 2 + index5], numArray4[40 * (num37 + num34 * num35) + 3 + index5]);
              if (numpoints1 == 0)
                return false;
              numvertices = numpoints1;
            }
          }
          num37 += num35;
          num35 = (num35 + 1) % 2;
        }
        FindPolyCentroid(numpoints1, convexPoly, ref newloc);
        if (behavior.MaxAngle == 0.0)
          return true;
        int num38 = 0;
        for (int index = 0; index < numpoints * 2 - 2; index += 2)
        {
          if (IsBadTriangleAngle(newloc[0], newloc[1], points[index], points[index + 1], points[index + 2], points[index + 3]))
            ++num38;
        }
        if (IsBadTriangleAngle(newloc[0], newloc[1], points[0], points[1], points[numpoints * 2 - 2], points[numpoints * 2 - 1]))
          ++num38;
        if (num38 == 0)
          return true;
        int num39 = numpoints <= 2 ? 20 : 30;
        for (int index6 = 0; index6 < 2 * numpoints; index6 += 2)
        {
          for (int index7 = 1; index7 < num39; ++index7)
          {
            newloc[0] = 0.0;
            newloc[1] = 0.0;
            for (int index8 = 0; index8 < 2 * numpoints; index8 += 2)
            {
              double num40 = 1.0 / numpoints;
              if (index8 == index6)
              {
                newloc[0] = newloc[0] + 0.1 * index7 * num40 * points[index8];
                newloc[1] = newloc[1] + 0.1 * index7 * num40 * points[index8 + 1];
              }
              else
              {
                double num41 = (1.0 - 0.1 * index7 * num40) / (numpoints - 1.0);
                newloc[0] = newloc[0] + num41 * points[index8];
                newloc[1] = newloc[1] + num41 * points[index8 + 1];
              }
            }
            int num42 = 0;
            for (int index9 = 0; index9 < numpoints * 2 - 2; index9 += 2)
            {
              if (IsBadTriangleAngle(newloc[0], newloc[1], points[index9], points[index9 + 1], points[index9 + 2], points[index9 + 3]))
                ++num42;
            }
            if (IsBadTriangleAngle(newloc[0], newloc[1], points[0], points[1], points[numpoints * 2 - 2], points[numpoints * 2 - 1]))
              ++num42;
            if (num42 == 0)
              return true;
          }
        }
      }
      return false;
    }

    private bool ValidPolygonAngles(int numpoints, double[] points)
    {
      for (int index = 0; index < numpoints; ++index)
      {
        if (index == numpoints - 1)
        {
          if (IsBadPolygonAngle(points[index * 2], points[index * 2 + 1], points[0], points[1], points[2], points[3]))
            return false;
        }
        else if (index == numpoints - 2)
        {
          if (IsBadPolygonAngle(points[index * 2], points[index * 2 + 1], points[(index + 1) * 2], points[(index + 1) * 2 + 1], points[0], points[1]))
            return false;
        }
        else if (IsBadPolygonAngle(points[index * 2], points[index * 2 + 1], points[(index + 1) * 2], points[(index + 1) * 2 + 1], points[(index + 2) * 2], points[(index + 2) * 2 + 1]))
          return false;
      }
      return true;
    }

    private bool IsBadPolygonAngle(
      double x1,
      double y1,
      double x2,
      double y2,
      double x3,
      double y3)
    {
      double num1 = x1 - x2;
      double num2 = y1 - y2;
      double num3 = x2 - x3;
      double num4 = y2 - y3;
      double num5 = x3 - x1;
      double num6 = y3 - y1;
      double d1 = num1 * num1 + num2 * num2;
      double d2 = num3 * num3 + num4 * num4;
      double num7 = num5 * num5 + num6 * num6;
      return Math.Acos((d1 + d2 - num7) / (2.0 * Math.Sqrt(d1) * Math.Sqrt(d2))) < 2.0 * Math.Acos(Math.Sqrt(behavior.goodAngle));
    }

    private void LineLineIntersection(
      double x1,
      double y1,
      double x2,
      double y2,
      double x3,
      double y3,
      double x4,
      double y4,
      ref double[] p)
    {
      double num1 = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
      double num2 = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3);
      double num3 = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3);
      if (Math.Abs(num1 - 0.0) < 1E-50 && Math.Abs(num3 - 0.0) < 1E-50 && Math.Abs(num2 - 0.0) < 1E-50)
        p[0] = 0.0;
      else if (Math.Abs(num1 - 0.0) < 1E-50)
      {
        p[0] = 0.0;
      }
      else
      {
        p[0] = 1.0;
        double num4 = num2 / num1;
        double num5 = num3 / num1;
        p[1] = x1 + num4 * (x2 - x1);
        p[2] = y1 + num4 * (y2 - y1);
      }
    }

    private int HalfPlaneIntersection(
      int numvertices,
      ref double[] convexPoly,
      double x1,
      double y1,
      double x2,
      double y2)
    {
      double[][] polys = new double[3][]
      {
        new double[2],
        new double[2],
        new double[2]
      };
      double[] numArray = null;
      int num1 = 0;
      int num2 = 0;
      double num3 = x2 - x1;
      double num4 = y2 - y1;
      int num5 = SplitConvexPolygon(numvertices, convexPoly, x1, y1, x2, y2, ref polys);
      if (num5 == 3)
      {
        num1 = numvertices;
      }
      else
      {
        for (int index1 = 0; index1 < num5; ++index1)
        {
          double num6 = 1E+17;
          double num7 = -1E+17;
          for (int index2 = 1; index2 <= 2.0 * polys[index1][0] - 1.0; index2 += 2)
          {
            double num8 = num3 * (polys[index1][index2 + 1] - y1) - num4 * (polys[index1][index2] - x1);
            num6 = num8 < num6 ? num8 : num6;
            num7 = num8 > num7 ? num8 : num7;
          }
          if ((Math.Abs(num6) > Math.Abs(num7) ? num6 : num7) > 0.0)
          {
            numArray = polys[index1];
            num2 = 1;
            break;
          }
        }
        if (num2 == 1)
        {
          for (; num1 < numArray[0]; ++num1)
          {
            convexPoly[2 * num1] = numArray[2 * num1 + 1];
            convexPoly[2 * num1 + 1] = numArray[2 * num1 + 2];
          }
        }
      }
      return num1;
    }

    private int SplitConvexPolygon(
      int numvertices,
      double[] convexPoly,
      double x1,
      double y1,
      double x2,
      double y2,
      ref double[][] polys)
    {
      int num1 = 0;
      double[] p = new double[3];
      double[] numArray1 = new double[100];
      int num2 = 0;
      double[] numArray2 = new double[100];
      int num3 = 0;
      double num4 = 1E-12;
      int num5 = 0;
      int num6 = 0;
      int num7 = 0;
      int num8 = 0;
      int num9 = 0;
      int num10 = 0;
      int num11 = 0;
      int num12 = 0;
      for (int index1 = 0; index1 < 2 * numvertices; index1 += 2)
      {
        int index2 = index1 + 2 >= 2 * numvertices ? 0 : index1 + 2;
        LineLineSegmentIntersection(x1, y1, x2, y2, convexPoly[index1], convexPoly[index1 + 1], convexPoly[index2], convexPoly[index2 + 1], ref p);
        if (Math.Abs(p[0] - 0.0) <= num4)
        {
          if (num1 == 1)
          {
            ++num3;
            numArray2[2 * num3 - 1] = convexPoly[index2];
            numArray2[2 * num3] = convexPoly[index2 + 1];
          }
          else
          {
            ++num2;
            numArray1[2 * num2 - 1] = convexPoly[index2];
            numArray1[2 * num2] = convexPoly[index2 + 1];
          }
          ++num5;
        }
        else if (Math.Abs(p[0] - 2.0) <= num4)
        {
          ++num2;
          numArray1[2 * num2 - 1] = convexPoly[index2];
          numArray1[2 * num2] = convexPoly[index2 + 1];
          ++num6;
        }
        else
        {
          ++num7;
          if (Math.Abs(p[1] - convexPoly[index2]) <= num4 && Math.Abs(p[2] - convexPoly[index2 + 1]) <= num4)
          {
            ++num8;
            if (num1 == 1)
            {
              ++num3;
              numArray2[2 * num3 - 1] = convexPoly[index2];
              numArray2[2 * num3] = convexPoly[index2 + 1];
              ++num2;
              numArray1[2 * num2 - 1] = convexPoly[index2];
              numArray1[2 * num2] = convexPoly[index2 + 1];
              ++num1;
            }
            else if (num1 == 0)
            {
              ++num11;
              ++num2;
              numArray1[2 * num2 - 1] = convexPoly[index2];
              numArray1[2 * num2] = convexPoly[index2 + 1];
              if (index1 + 4 < 2 * numvertices)
              {
                int num13 = LinePointLocation(x1, y1, x2, y2, convexPoly[index1], convexPoly[index1 + 1]);
                int num14 = LinePointLocation(x1, y1, x2, y2, convexPoly[index1 + 4], convexPoly[index1 + 5]);
                if (num13 != num14 && num13 != 0 && num14 != 0)
                {
                  ++num12;
                  ++num3;
                  numArray2[2 * num3 - 1] = convexPoly[index2];
                  numArray2[2 * num3] = convexPoly[index2 + 1];
                  ++num1;
                }
              }
            }
          }
          else if (Math.Abs(p[1] - convexPoly[index1]) > num4 || Math.Abs(p[2] - convexPoly[index1 + 1]) > num4)
          {
            ++num9;
            ++num2;
            numArray1[2 * num2 - 1] = p[1];
            numArray1[2 * num2] = p[2];
            ++num3;
            numArray2[2 * num3 - 1] = p[1];
            numArray2[2 * num3] = p[2];
            switch (num1)
            {
              case 0:
                ++num3;
                numArray2[2 * num3 - 1] = convexPoly[index2];
                numArray2[2 * num3] = convexPoly[index2 + 1];
                break;
              case 1:
                ++num2;
                numArray1[2 * num2 - 1] = convexPoly[index2];
                numArray1[2 * num2] = convexPoly[index2 + 1];
                break;
            }
            ++num1;
          }
          else
          {
            ++num10;
            if (num1 == 1)
            {
              ++num3;
              numArray2[2 * num3 - 1] = convexPoly[index2];
              numArray2[2 * num3] = convexPoly[index2 + 1];
            }
            else
            {
              ++num2;
              numArray1[2 * num2 - 1] = convexPoly[index2];
              numArray1[2 * num2] = convexPoly[index2 + 1];
            }
          }
        }
      }
      int num15;
      if (num1 != 0 && num1 != 2)
      {
        num15 = 3;
      }
      else
      {
        num15 = num1 == 0 ? 1 : 2;
        numArray1[0] = num2;
        numArray2[0] = num3;
        polys[0] = numArray1;
        if (num1 == 2)
          polys[1] = numArray2;
      }
      return num15;
    }

    private int LinePointLocation(
      double x1,
      double y1,
      double x2,
      double y2,
      double x,
      double y)
    {
      if (Math.Atan((y2 - y1) / (x2 - x1)) * 180.0 / Math.PI == 90.0)
      {
        if (Math.Abs(x1 - x) <= 1E-11)
          return 0;
      }
      else if (Math.Abs(y1 + (y2 - y1) * (x - x1) / (x2 - x1) - y) <= 1E-50)
        return 0;
      double num = (x2 - x1) * (y - y1) - (y2 - y1) * (x - x1);
      if (Math.Abs(num - 0.0) <= 1E-11)
        return 0;
      return num > 0.0 ? 1 : 2;
    }

    private void LineLineSegmentIntersection(
      double x1,
      double y1,
      double x2,
      double y2,
      double x3,
      double y3,
      double x4,
      double y4,
      ref double[] p)
    {
      double num1 = 1E-13;
      double num2 = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
      double num3 = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3);
      double num4 = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3);
      if (Math.Abs(num2 - 0.0) < num1)
      {
        if (Math.Abs(num4 - 0.0) < num1 && Math.Abs(num3 - 0.0) < num1)
          p[0] = 2.0;
        else
          p[0] = 0.0;
      }
      else
      {
        double num5 = num4 / num2;
        double num6 = num3 / num2;
        if (num5 < -num1 || num5 > 1.0 + num1)
        {
          p[0] = 0.0;
        }
        else
        {
          p[0] = 1.0;
          p[1] = x1 + num6 * (x2 - x1);
          p[2] = y1 + num6 * (y2 - y1);
        }
      }
    }

    private void FindPolyCentroid(int numpoints, double[] points, ref double[] centroid)
    {
      centroid[0] = 0.0;
      centroid[1] = 0.0;
      for (int index = 0; index < 2 * numpoints; index += 2)
      {
        centroid[0] = centroid[0] + points[index];
        centroid[1] = centroid[1] + points[index + 1];
      }
      centroid[0] = centroid[0] / numpoints;
      centroid[1] = centroid[1] / numpoints;
    }

    private void CircleLineIntersection(
      double x1,
      double y1,
      double x2,
      double y2,
      double x3,
      double y3,
      double r,
      ref double[] p)
    {
      double num1 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
      double num2 = 2.0 * ((x2 - x1) * (x1 - x3) + (y2 - y1) * (y1 - y3));
      double num3 = x3 * x3 + y3 * y3 + x1 * x1 + y1 * y1 - 2.0 * (x3 * x1 + y3 * y1) - r * r;
      double d = num2 * num2 - 4.0 * num1 * num3;
      if (d < 0.0)
        p[0] = 0.0;
      else if (Math.Abs(d - 0.0) < 1E-50)
      {
        p[0] = 1.0;
        double num4 = -num2 / (2.0 * num1);
        p[1] = x1 + num4 * (x2 - x1);
        p[2] = y1 + num4 * (y2 - y1);
      }
      else if (d > 0.0 && Math.Abs(num1 - 0.0) >= 1E-50)
      {
        p[0] = 2.0;
        double num5 = (-num2 + Math.Sqrt(d)) / (2.0 * num1);
        p[1] = x1 + num5 * (x2 - x1);
        p[2] = y1 + num5 * (y2 - y1);
        double num6 = (-num2 - Math.Sqrt(d)) / (2.0 * num1);
        p[3] = x1 + num6 * (x2 - x1);
        p[4] = y1 + num6 * (y2 - y1);
      }
      else
        p[0] = 0.0;
    }

    private bool ChooseCorrectPoint(
      double x1,
      double y1,
      double x2,
      double y2,
      double x3,
      double y3,
      bool isObtuse)
    {
      double num1 = (x2 - x3) * (x2 - x3) + (y2 - y3) * (y2 - y3);
      double num2 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
      return !isObtuse ? num2 < num1 : num2 >= num1;
    }

    private void PointBetweenPoints(
      double x1,
      double y1,
      double x2,
      double y2,
      double x,
      double y,
      ref double[] p)
    {
      if ((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y) < (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1))
      {
        p[0] = 1.0;
        p[1] = (x - x2) * (x - x2) + (y - y2) * (y - y2);
        p[2] = x;
        p[3] = y;
      }
      else
      {
        p[0] = 0.0;
        p[1] = 0.0;
        p[2] = 0.0;
        p[3] = 0.0;
      }
    }

    private bool IsBadTriangleAngle(
      double x1,
      double y1,
      double x2,
      double y2,
      double x3,
      double y3)
    {
      double num1 = x1 - x2;
      double num2 = y1 - y2;
      double num3 = x2 - x3;
      double num4 = y2 - y3;
      double num5 = x3 - x1;
      double num6 = y3 - y1;
      double num7 = num1 * num1;
      double num8 = num2 * num2;
      double num9 = num3 * num3;
      double num10 = num4 * num4;
      double num11 = num5 * num5;
      double num12 = num6 * num6;
      double num13 = num7 + num8;
      double num14 = num9 + num10;
      double num15 = num11 + num12;
      double num16;
      double num17;
      if (num13 < num14 && num13 < num15)
      {
        num16 = num13;
        double num18 = num3 * num5 + num4 * num6;
        num17 = num18 * num18 / (num14 * num15);
      }
      else if (num14 < num15)
      {
        num16 = num14;
        double num19 = num1 * num5 + num2 * num6;
        num17 = num19 * num19 / (num13 * num15);
      }
      else
      {
        num16 = num15;
        double num20 = num1 * num3 + num2 * num4;
        num17 = num20 * num20 / (num13 * num14);
      }
      double num21;
      double num22;
      if (num13 > num14 && num13 > num15)
      {
        num21 = num13;
        num22 = (num14 + num15 - num13) / (2.0 * Math.Sqrt(num14 * num15));
      }
      else if (num14 > num15)
      {
        num21 = num14;
        num22 = (num13 + num15 - num14) / (2.0 * Math.Sqrt(num13 * num15));
      }
      else
      {
        num21 = num15;
        num22 = (num13 + num14 - num15) / (2.0 * Math.Sqrt(num13 * num14));
      }
      return num17 > behavior.goodAngle || behavior.MaxAngle != 0.0 && num22 < behavior.maxGoodAngle;
    }

    private double MinDistanceToNeighbor(double newlocX, double newlocY, ref Otri searchtri)
    {
      Otri otri = new Otri();
      LocateResult locateResult = LocateResult.Outside;
      Point point = new Point(newlocX, newlocY);
      Vertex pa = searchtri.Org();
      Vertex pb = searchtri.Dest();
      if (pa.x == point.x && pa.y == point.y)
      {
        locateResult = LocateResult.OnVertex;
        searchtri.Copy(ref otri);
      }
      else if (pb.x == point.x && pb.y == point.y)
      {
        searchtri.LnextSelf();
        locateResult = LocateResult.OnVertex;
        searchtri.Copy(ref otri);
      }
      else
      {
        double num = Primitives.CounterClockwise(pa, pb, point);
        if (num < 0.0)
        {
          searchtri.SymSelf();
          searchtri.Copy(ref otri);
          locateResult = mesh.locator.PreciseLocate(point, ref otri, false);
        }
        else if (num == 0.0)
        {
          if (pa.x < point.x == point.x < pb.x && pa.y < point.y == point.y < pb.y)
          {
            locateResult = LocateResult.OnEdge;
            searchtri.Copy(ref otri);
          }
        }
        else
        {
          searchtri.Copy(ref otri);
          locateResult = mesh.locator.PreciseLocate(point, ref otri, false);
        }
      }
      if (locateResult == LocateResult.OnVertex || locateResult == LocateResult.Outside)
        return 0.0;
      Vertex vertex1 = otri.Org();
      Vertex vertex2 = otri.Dest();
      Vertex vertex3 = otri.Apex();
      double neighbor = (vertex1.x - point.x) * (vertex1.x - point.x) + (vertex1.y - point.y) * (vertex1.y - point.y);
      double num1 = (vertex2.x - point.x) * (vertex2.x - point.x) + (vertex2.y - point.y) * (vertex2.y - point.y);
      double num2 = (vertex3.x - point.x) * (vertex3.x - point.x) + (vertex3.y - point.y) * (vertex3.y - point.y);
      if (neighbor <= num1 && neighbor <= num2)
        return neighbor;
      return num1 <= num2 ? num1 : num2;
    }
  }
}
