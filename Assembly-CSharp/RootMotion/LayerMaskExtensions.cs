using System.Collections.Generic;
using UnityEngine;

namespace RootMotion
{
  public static class LayerMaskExtensions
  {
    public static bool Contains(LayerMask mask, int layer)
    {
      return mask == (mask | 1 << layer);
    }

    public static LayerMask Create(params string[] layerNames)
    {
      return NamesToMask(layerNames);
    }

    public static LayerMask Create(params int[] layerNumbers)
    {
      return LayerNumbersToMask(layerNumbers);
    }

    public static LayerMask NamesToMask(params string[] layerNames)
    {
      LayerMask mask = 0;
      foreach (string layerName in layerNames)
        mask = mask | 1 << LayerMask.NameToLayer(layerName);
      return mask;
    }

    public static LayerMask LayerNumbersToMask(params int[] layerNumbers)
    {
      LayerMask mask = 0;
      foreach (int layerNumber in layerNumbers)
        mask = mask | 1 << layerNumber;
      return mask;
    }

    public static LayerMask Inverse(this LayerMask original) => ~original;

    public static LayerMask AddToMask(this LayerMask original, params string[] layerNames)
    {
      return original | NamesToMask(layerNames);
    }

    public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames)
    {
      return ~((LayerMask) ~original | NamesToMask(layerNames));
    }

    public static string[] MaskToNames(this LayerMask original)
    {
      List<string> stringList = new List<string>();
      for (int layer = 0; layer < 32; ++layer)
      {
        int num = 1 << layer;
        if ((original & num) == num)
        {
          string name = LayerMask.LayerToName(layer);
          if (!string.IsNullOrEmpty(name))
            stringList.Add(name);
        }
      }
      return stringList.ToArray();
    }

    public static int[] MaskToNumbers(this LayerMask original)
    {
      List<int> intList = new List<int>();
      for (int index = 0; index < 32; ++index)
      {
        int num = 1 << index;
        if ((original & num) == num)
          intList.Add(index);
      }
      return intList.ToArray();
    }

    public static string MaskToString(this LayerMask original) => original.MaskToString(", ");

    public static string MaskToString(this LayerMask original, string delimiter)
    {
      return string.Join(delimiter, original.MaskToNames());
    }
  }
}
