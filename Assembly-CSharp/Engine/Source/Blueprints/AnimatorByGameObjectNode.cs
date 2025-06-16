using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class AnimatorByGameObjectNode : FlowControlNode
  {
    [Port("GameObject")]
    private ValueInput<GameObject> goInput;

    [Port("Animator")]
    private Animator Animator() => goInput.value.GetComponent<Animator>();
  }
}
