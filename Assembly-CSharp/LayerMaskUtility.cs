using UnityEngine;

public static class LayerMaskUtility
{
  public static bool Contains(this LayerMask mask, int layer) => (mask.value & 1 << layer) != 0;

  public static int GetMask(this LayerMask mask) => 1 << mask;

  public static int GetIndex(this LayerMask mask)
  {
    int index = 0;
    int num = mask.value;
    while (num > 1)
    {
      num >>= 1;
      ++index;
    }
    return index;
  }
}
