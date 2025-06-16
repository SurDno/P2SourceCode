// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.OpenTradeWindowNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Services.Utilities;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class OpenTradeWindowNode : FlowControlNode
  {
    private ValueInput<IMarketComponent> targetInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IMarketComponent target = this.targetInput.value;
        if (target == null)
          return;
        ((MarketComponent) target).FillPrices();
        UIServiceUtility.PushWindow<ITradeWindow>(output, (Action<ITradeWindow>) (window =>
        {
          window.Actor = ServiceLocator.GetService<ISimulation>().Player.GetComponent<IStorageComponent>();
          window.Market = target;
        }));
      }));
      this.targetInput = this.AddValueInput<IMarketComponent>("Market");
    }
  }
}
