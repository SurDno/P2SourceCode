// Decompiled with JetBrains decompiler
// Type: MapRouteViewSplit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Regions;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class MapRouteViewSplit : MapRouteView
{
  [SerializeField]
  private MapRouteView[] views;

  public override void SetRoute(IList<FastTravelPointEnum> route)
  {
    for (int index = 0; index < this.views.Length; ++index)
      this.views[index]?.SetRoute(route);
  }
}
