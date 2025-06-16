using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GetEulerAnglesNode : FlowControlNode
  {
    private ValueInput<Transform> transformInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      transformInput = AddValueInput<Transform>("Transform");
      AddValueOutput("EulerAngles", () =>
      {
        Transform transform = transformInput.value;
        return transform != null ? transform.eulerAngles : Vector3.zero;
      });
    }
  }
}
