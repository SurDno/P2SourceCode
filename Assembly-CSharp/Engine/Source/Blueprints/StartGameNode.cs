using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class StartGameNode : FlowControlNode
  {
    [Port("Project Name")]
    private ValueInput<string> projectNameInput;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      InstanceByRequest<GameDataService>.Instance.SetCurrentGameData(this.projectNameInput.value);
      ServiceLocator.GetService<GameLauncher>().RestartGame();
      this.output.Call();
    }
  }
}
