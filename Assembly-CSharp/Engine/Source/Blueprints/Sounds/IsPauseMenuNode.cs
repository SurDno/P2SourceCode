// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.IsPauseMenuNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu;
using Engine.Source.UI;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsPauseMenuNode : FlowControlNode
  {
    [Port("Value")]
    private bool Value()
    {
      UIService service = ServiceLocator.GetService<UIService>();
      if (service == null || !service.IsInitialize)
        return false;
      UIWindow active = service.Active;
      return !((Object) active == (Object) null) && active is IPauseMenu;
    }
  }
}
