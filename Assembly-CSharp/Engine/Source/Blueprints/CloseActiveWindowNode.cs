// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.CloseActiveWindowNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CloseActiveWindowNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() => CoroutineService.Instance.Route(this.Route(output))));
    }

    private IEnumerator Route(FlowOutput output)
    {
      UIService uiService = ServiceLocator.GetService<UIService>();
      while ((Object) uiService.Active != (Object) null && !(uiService.Active is IHudWindow) && !(uiService.Active is IDialogWindow))
      {
        uiService.Pop();
        while (uiService.IsTransition)
          yield return (object) null;
      }
      output.Call();
    }
  }
}
