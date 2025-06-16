using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class RestartGameNode : FlowControlNode
  {
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      InstanceByRequest<GameDataService>.Instance.SetCurrentGameData("");
      ServiceLocator.GetService<GameLauncher>().RestartGame();
      output.Call();
    }
  }
}
