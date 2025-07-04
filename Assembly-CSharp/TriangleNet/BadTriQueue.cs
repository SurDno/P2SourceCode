﻿using TriangleNet.Data;

namespace TriangleNet
{
  internal class BadTriQueue
  {
    private static readonly double SQRT2 = 1.4142135623730951;
    private int count = 0;
    private int firstnonemptyq = -1;
    private int[] nextnonemptyq = new int[4096];
    private BadTriangle[] queuefront = new BadTriangle[4096];
    private BadTriangle[] queuetail = new BadTriangle[4096];

    public int Count => count;

    public void Enqueue(BadTriangle badtri)
    {
      ++count;
      double num1;
      int num2;
      if (badtri.key >= 1.0)
      {
        num1 = badtri.key;
        num2 = 1;
      }
      else
      {
        num1 = 1.0 / badtri.key;
        num2 = 0;
      }
      int num3 = 0;
      double num4;
      for (; num1 > 2.0; num1 *= num4)
      {
        int num5 = 1;
        for (num4 = 0.5; num1 * num4 * num4 > 1.0; num4 *= num4)
          num5 *= 2;
        num3 += num5;
      }
      int num6 = 2 * num3 + (num1 > SQRT2 ? 1 : 0);
      int index1 = num2 <= 0 ? 2048 + num6 : 2047 - num6;
      if (queuefront[index1] == null)
      {
        if (index1 > firstnonemptyq)
        {
          nextnonemptyq[index1] = firstnonemptyq;
          firstnonemptyq = index1;
        }
        else
        {
          int index2 = index1 + 1;
          while (queuefront[index2] == null)
            ++index2;
          nextnonemptyq[index1] = nextnonemptyq[index2];
          nextnonemptyq[index2] = index1;
        }
        queuefront[index1] = badtri;
      }
      else
        queuetail[index1].nexttriang = badtri;
      queuetail[index1] = badtri;
      badtri.nexttriang = null;
    }

    public void Enqueue(
      ref Otri enqtri,
      double minedge,
      Vertex enqapex,
      Vertex enqorg,
      Vertex enqdest)
    {
      Enqueue(new BadTriangle {
        poortri = enqtri,
        key = minedge,
        triangapex = enqapex,
        triangorg = enqorg,
        triangdest = enqdest
      });
    }

    public BadTriangle Dequeue()
    {
      if (firstnonemptyq < 0)
        return null;
      --count;
      BadTriangle badTriangle = queuefront[firstnonemptyq];
      queuefront[firstnonemptyq] = badTriangle.nexttriang;
      if (badTriangle == queuetail[firstnonemptyq])
        firstnonemptyq = nextnonemptyq[firstnonemptyq];
      return badTriangle;
    }
  }
}
