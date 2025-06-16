// Decompiled with JetBrains decompiler
// Type: Scripts.Utility.RaycastComparer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace Scripts.Utility
{
  public class RaycastComparer : IComparer<RaycastHit>
  {
    public static readonly RaycastComparer Instance = new RaycastComparer();

    public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
  }
}
