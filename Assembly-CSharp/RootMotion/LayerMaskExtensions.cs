using System.Collections.Generic;
using UnityEngine;

namespace RootMotion
{
  public static class LayerMaskExtensions
  {
    public static bool Contains(LayerMask mask, int layer)
    {
      return (int) mask == ((int) mask | 1 << layer);
    }

    public static LayerMask Create(params string[] layerNames)
    {
      return LayerMaskExtensions.NamesToMask(layerNames);
    }

    public static LayerMask Create(params int[] layerNumbers)
    {
      return LayerMaskExtensions.LayerNumbersToMask(layerNumbers);
    }

    public static LayerMask NamesToMask(params string[] layerNames)
    {
      LayerMask mask = (LayerMask) 0;
      foreach (string layerName in layerNames)
        mask = (LayerMask) ((int) mask | 1 << LayerMask.NameToLayer(layerName));
      return mask;
    }

    public static LayerMask LayerNumbersToMask(params int[] layerNumbers)
    {
      LayerMask mask = (LayerMask) 0;
      foreach (int layerNumber in layerNumbers)
        mask = (LayerMask) ((int) mask | 1 << layerNumber);
      return mask;
    }

    public static LayerMask Inverse(this LayerMask original) => (LayerMask) ~(int) original;

    public static LayerMask AddToMask(this LayerMask original, params string[] layerNames)
    {
      return (LayerMask) ((int) original | (int) LayerMaskExtensions.NamesToMask(layerNames));
    }

    public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames)
    {
      return (LayerMask) ~((int) (LayerMask) ~(int) original | (int) LayerMaskExtensions.NamesToMask(layerNames));
    }

    public static string[] MaskToNames(this LayerMask original)
    {
      List<string> stringList = new List<string>();
      for (int layer = 0; layer < 32; ++layer)
      {
        int num = 1 << layer;
        if (((int) original & num) == num)
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
        if (((int) original & num) == num)
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
