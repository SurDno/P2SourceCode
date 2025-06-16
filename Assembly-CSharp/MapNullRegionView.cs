// Decompiled with JetBrains decompiler
// Type: MapNullRegionView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;

#nullable disable
public class MapNullRegionView : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
  [SerializeField]
  private MapRegionInfoWindow regionInfoWindow;

  public void OnPointerEnter(PointerEventData eventData) => this.regionInfoWindow.Hide();
}
