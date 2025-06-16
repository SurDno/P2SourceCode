// Decompiled with JetBrains decompiler
// Type: AOField
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (DeferredProjector))]
public class AOField : MonoBehaviour
{
  private static bool isPlayerOutdoor;
  private static bool allowed;
  private static List<AOField> all;

  public static bool IsPlayerOutdoor
  {
    get => AOField.isPlayerOutdoor;
    set
    {
      if (AOField.isPlayerOutdoor == value)
        return;
      AOField.isPlayerOutdoor = value;
      AOField.UpdateAll();
    }
  }

  public static bool Allowed
  {
    get => AOField.allowed;
    set
    {
      if (AOField.allowed == value)
        return;
      AOField.allowed = value;
      AOField.UpdateAll();
    }
  }

  private static void Register(AOField instance)
  {
    if (AOField.all == null)
      AOField.all = new List<AOField>();
    AOField.all.Add(instance);
  }

  private static void Unregister(AOField instance)
  {
    if (AOField.all == null)
      return;
    int index1 = AOField.all.IndexOf(instance);
    if (index1 == -1)
      return;
    int index2 = AOField.all.Count - 1;
    if (index1 != index2)
      AOField.all[index1] = AOField.all[index2];
    AOField.all.RemoveAt(index2);
  }

  private static void UpdateAll()
  {
    if (AOField.all == null)
      return;
    for (int index = 0; index < AOField.all.Count; ++index)
      AOField.all[index].UpdateDecal();
  }

  private void OnDisable() => AOField.Unregister(this);

  private void OnEnable()
  {
    this.UpdateDecal();
    AOField.Register(this);
  }

  private void UpdateDecal()
  {
    this.GetComponent<DeferredProjector>().enabled = AOField.Allowed && AOField.IsPlayerOutdoor;
  }
}
