// Decompiled with JetBrains decompiler
// Type: GameWindowSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;
using UnityEngine;

#nullable disable
public class GameWindowSelector : MonoBehaviour
{
  [SerializeField]
  public HideableView mapAvailableView;
  [SerializeField]
  public HideableView mindMapAvailableView;
  [SerializeField]
  public HideableView inventoryAvailableView;
  [SerializeField]
  public HideableView boundCharactersAvailableView;

  private void OnEnable()
  {
    InterfaceBlockingService service = ServiceLocator.GetService<InterfaceBlockingService>();
    if (service == null)
      return;
    if ((Object) this.mapAvailableView != (Object) null)
      this.mapAvailableView.Visible = !service.BlockMapInterface;
    if ((Object) this.mindMapAvailableView != (Object) null)
      this.mindMapAvailableView.Visible = !service.BlockMindMapInterface;
    if ((Object) this.inventoryAvailableView != (Object) null)
      this.inventoryAvailableView.Visible = !service.BlockInventoryInterface;
    if ((Object) this.boundCharactersAvailableView != (Object) null)
      this.boundCharactersAvailableView.Visible = !service.BlockBoundsInterface;
  }
}
