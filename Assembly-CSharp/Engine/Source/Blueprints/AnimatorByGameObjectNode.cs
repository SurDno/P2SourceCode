using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class AnimatorByGameObjectNode : FlowControlNode
  {
    [Port("GameObject")]
    private ValueInput<GameObject> goInput;

    [Port("Animator")]
    private UnityEngine.Animator Animator() => goInput.value.GetComponent<UnityEngine.Animator>();
  }
}
