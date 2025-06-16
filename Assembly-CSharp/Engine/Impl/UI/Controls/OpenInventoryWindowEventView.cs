// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.OpenInventoryWindowEventView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services;
using Engine.Source.UI;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class OpenInventoryWindowEventView : OpenWindowEventView<IInventoryWindow>
  {
    protected override bool PrepareWindow()
    {
      IStorageComponent component = ServiceLocator.GetService<ISimulation>().Player?.GetComponent<IStorageComponent>();
      if (component == null)
        return false;
      InterfaceBlockingService service = ServiceLocator.GetService<InterfaceBlockingService>();
      if (service != null && service.BlockInventoryInterface)
        return false;
      ServiceLocator.GetService<UIService>().Get<IInventoryWindow>().Actor = component;
      return true;
    }
  }
}
