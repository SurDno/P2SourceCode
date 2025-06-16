// Decompiled with JetBrains decompiler
// Type: TriangleNet.Primitives
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using TriangleNet.Geometry;
using TriangleNet.Tools;

#nullable disable
namespace TriangleNet
{
  public static class Primitives
  {
    private static double splitter;
    private static double epsilon;
    private static double ccwerrboundA;
    private static double iccerrboundA;

    public static void ExactInit()
    {
      bool flag = true;
      double num1 = 0.5;
      Primitives.epsilon = 1.0;
      Primitives.splitter = 1.0;
      double num2 = 1.0;
      double num3;
      do
      {
        num3 = num2;
        Primitives.epsilon *= num1;
        if (flag)
          Primitives.splitter *= 2.0;
        flag = !flag;
        num2 = 1.0 + Primitives.epsilon;
      }
      while (num2 != 1.0 && num2 != num3);
      ++Primitives.splitter;
      Primitives.ccwerrboundA = (3.0 + 16.0 * Primitives.epsilon) * Primitives.epsilon;
      Primitives.iccerrboundA = (10.0 + 96.0 * Primitives.epsilon) * Primitives.epsilon;
    }

    public static double CounterClockwise(Point pa, Point pb, Point pc)
    {
      ++Statistic.CounterClockwiseCount;
      double num1 = (pa.x - pc.x) * (pb.y - pc.y);
      double num2 = (pa.y - pc.y) * (pb.x - pc.x);
      double num3 = num1 - num2;
      if (Behavior.NoExact)
        return num3;
      double num4;
      if (num1 > 0.0)
      {
        if (num2 <= 0.0)
          return num3;
        num4 = num1 + num2;
      }
      else
      {
        if (num1 >= 0.0 || num2 >= 0.0)
          return num3;
        num4 = -num1 - num2;
      }
      double num5 = Primitives.ccwerrboundA * num4;
      return num3 >= num5 || -num3 >= num5 ? num3 : (double) Primitives.CounterClockwiseDecimal(pa, pb, pc);
    }

    private static Decimal CounterClockwiseDecimal(Point pa, Point pb, Point pc)
    {
      ++Statistic.CounterClockwiseCountDecimal;
      Decimal num1 = ((Decimal) pa.x - (Decimal) pc.x) * ((Decimal) pb.y - (Decimal) pc.y);
      Decimal num2 = ((Decimal) pa.y - (Decimal) pc.y) * ((Decimal) pb.x - (Decimal) pc.x);
      Decimal num3 = num1 - num2;
      Decimal num4;
      if (num1 > 0.0M)
      {
        if (num2 <= 0.0M)
          return num3;
        num4 = num1 + num2;
      }
      else if (num1 < 0.0M && !(num2 >= 0.0M))
        num4 = -num1 - num2;
      return num3;
    }

    public static double InCircle(Point pa, Point pb, Point pc, Point pd)
    {
      ++Statistic.InCircleCount;
      double num1 = pa.x - pd.x;
      double num2 = pb.x - pd.x;
      double num3 = pc.x - pd.x;
      double num4 = pa.y - pd.y;
      double num5 = pb.y - pd.y;
      double num6 = pc.y - pd.y;
      double num7 = num2 * num6;
      double num8 = num3 * num5;
      double num9 = num1 * num1 + num4 * num4;
      double num10 = num3 * num4;
      double num11 = num1 * num6;
      double num12 = num2 * num2 + num5 * num5;
      double num13 = num1 * num5;
      double num14 = num2 * num4;
      double num15 = num3 * num3 + num6 * num6;
      double num16 = num9 * (num7 - num8) + num12 * (num10 - num11) + num15 * (num13 - num14);
      if (Behavior.NoExact)
        return num16;
      double num17 = (Math.Abs(num7) + Math.Abs(num8)) * num9 + (Math.Abs(num10) + Math.Abs(num11)) * num12 + (Math.Abs(num13) + Math.Abs(num14)) * num15;
      double num18 = Primitives.iccerrboundA * num17;
      return num16 > num18 || -num16 > num18 ? num16 : (double) Primitives.InCircleDecimal(pa, pb, pc, pd);
    }

    private static Decimal InCircleDecimal(Point pa, Point pb, Point pc, Point pd)
    {
      ++Statistic.InCircleCountDecimal;
      Decimal num1 = (Decimal) pa.x - (Decimal) pd.x;
      Decimal num2 = (Decimal) pb.x - (Decimal) pd.x;
      Decimal num3 = (Decimal) pc.x - (Decimal) pd.x;
      Decimal num4 = (Decimal) pa.y - (Decimal) pd.y;
      Decimal num5 = (Decimal) pb.y - (Decimal) pd.y;
      Decimal num6 = (Decimal) pc.y - (Decimal) pd.y;
      Decimal num7 = num2 * num6;
      Decimal num8 = num3 * num5;
      Decimal num9 = num1 * num1 + num4 * num4;
      Decimal num10 = num3 * num4;
      Decimal num11 = num1 * num6;
      Decimal num12 = num2 * num2 + num5 * num5;
      Decimal num13 = num1 * num5;
      Decimal num14 = num2 * num4;
      Decimal num15 = num3 * num3 + num6 * num6;
      return num9 * (num7 - num8) + num12 * (num10 - num11) + num15 * (num13 - num14);
    }

    public static double NonRegular(Point pa, Point pb, Point pc, Point pd)
    {
      return Primitives.InCircle(pa, pb, pc, pd);
    }

    public static Point FindCircumcenter(
      Point torg,
      Point tdest,
      Point tapex,
      ref double xi,
      ref double eta,
      double offconstant)
    {
      ++Statistic.CircumcenterCount;
      double num1 = tdest.x - torg.x;
      double num2 = tdest.y - torg.y;
      double num3 = tapex.x - torg.x;
      double num4 = tapex.y - torg.y;
      double num5 = num1 * num1 + num2 * num2;
      double num6 = num3 * num3 + num4 * num4;
      double num7 = (tdest.x - tapex.x) * (tdest.x - tapex.x) + (tdest.y - tapex.y) * (tdest.y - tapex.y);
      double num8;
      if (Behavior.NoExact)
      {
        num8 = 0.5 / (num1 * num4 - num3 * num2);
      }
      else
      {
        num8 = 0.5 / Primitives.CounterClockwise(tdest, tapex, torg);
        --Statistic.CounterClockwiseCount;
      }
      double num9 = (num4 * num5 - num2 * num6) * num8;
      double num10 = (num1 * num6 - num3 * num5) * num8;
      if (num5 < num6 && num5 < num7)
      {
        if (offconstant > 0.0)
        {
          double num11 = 0.5 * num1 - offconstant * num2;
          double num12 = 0.5 * num2 + offconstant * num1;
          if (num11 * num11 + num12 * num12 < num9 * num9 + num10 * num10)
          {
            num9 = num11;
            num10 = num12;
          }
        }
      }
      else if (num6 < num7)
      {
        if (offconstant > 0.0)
        {
          double num13 = 0.5 * num3 + offconstant * num4;
          double num14 = 0.5 * num4 - offconstant * num3;
          if (num13 * num13 + num14 * num14 < num9 * num9 + num10 * num10)
          {
            num9 = num13;
            num10 = num14;
          }
        }
      }
      else if (offconstant > 0.0)
      {
        double num15 = 0.5 * (tapex.x - tdest.x) - offconstant * (tapex.y - tdest.y);
        double num16 = 0.5 * (tapex.y - tdest.y) + offconstant * (tapex.x - tdest.x);
        if (num15 * num15 + num16 * num16 < (num9 - num1) * (num9 - num1) + (num10 - num2) * (num10 - num2))
        {
          num9 = num1 + num15;
          num10 = num2 + num16;
        }
      }
      xi = (num4 * num9 - num3 * num10) * (2.0 * num8);
      eta = (num1 * num10 - num2 * num9) * (2.0 * num8);
      return new Point(torg.x + num9, torg.y + num10);
    }

    public static Point FindCircumcenter(
      Point torg,
      Point tdest,
      Point tapex,
      ref double xi,
      ref double eta)
    {
      ++Statistic.CircumcenterCount;
      double num1 = tdest.x - torg.x;
      double num2 = tdest.y - torg.y;
      double num3 = tapex.x - torg.x;
      double num4 = tapex.y - torg.y;
      double num5 = num1 * num1 + num2 * num2;
      double num6 = num3 * num3 + num4 * num4;
      double num7;
      if (Behavior.NoExact)
      {
        num7 = 0.5 / (num1 * num4 - num3 * num2);
      }
      else
      {
        num7 = 0.5 / Primitives.CounterClockwise(tdest, tapex, torg);
        --Statistic.CounterClockwiseCount;
      }
      double num8 = (num4 * num5 - num2 * num6) * num7;
      double num9 = (num1 * num6 - num3 * num5) * num7;
      xi = (num4 * num8 - num3 * num9) * (2.0 * num7);
      eta = (num1 * num9 - num2 * num8) * (2.0 * num7);
      return new Point(torg.x + num8, torg.y + num9);
    }
  }
}
