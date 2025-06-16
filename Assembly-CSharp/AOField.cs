using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (DeferredProjector))]
public class AOField : MonoBehaviour
{
  private static bool isPlayerOutdoor;
  private static bool allowed;
  private static List<AOField> all;

  public static bool IsPlayerOutdoor
  {
    get => isPlayerOutdoor;
    set
    {
      if (isPlayerOutdoor == value)
        return;
      isPlayerOutdoor = value;
      UpdateAll();
    }
  }

  public static bool Allowed
  {
    get => allowed;
    set
    {
      if (allowed == value)
        return;
      allowed = value;
      UpdateAll();
    }
  }

  private static void Register(AOField instance)
  {
    if (all == null)
      all = new List<AOField>();
    all.Add(instance);
  }

  private static void Unregister(AOField instance)
  {
    if (all == null)
      return;
    int index1 = all.IndexOf(instance);
    if (index1 == -1)
      return;
    int index2 = all.Count - 1;
    if (index1 != index2)
      all[index1] = all[index2];
    all.RemoveAt(index2);
  }

  private static void UpdateAll()
  {
    if (all == null)
      return;
    for (int index = 0; index < all.Count; ++index)
      all[index].UpdateDecal();
  }

  private void OnDisable() => Unregister(this);

  private void OnEnable()
  {
    UpdateDecal();
    Register(this);
  }

  private void UpdateDecal()
  {
    GetComponent<DeferredProjector>().enabled = Allowed && IsPlayerOutdoor;
  }
}
