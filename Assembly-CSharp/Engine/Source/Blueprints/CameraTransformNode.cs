using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CameraTransformNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.AddValueOutput<Transform>("Transform", (ValueHandler<Transform>) (() => GameCamera.Instance.CameraTransform));
    }
  }
}
