using System;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using Engine.Source.Services.Utilities;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class OpenTradeWindowNode : FlowControlNode
  {
    private ValueInput<IMarketComponent> targetInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        IMarketComponent target = targetInput.value;
        if (target == null)
          return;
        ((MarketComponent) target).FillPrices();
        UIServiceUtility.PushWindow(output, (Action<ITradeWindow>) (window =>
        {
          window.Actor = ServiceLocator.GetService<ISimulation>().Player.GetComponent<IStorageComponent>();
          window.Market = target;
        }));
      });
      targetInput = AddValueInput<IMarketComponent>("Market");
    }
  }
}
