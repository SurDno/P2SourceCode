using System.ComponentModel;
using FlowCanvas.Nodes;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CameraAnimatorNode : FlowControlNode
  {
    [Port("Value")]
    public Animator Value()
    {
      UnityEngine.Camera camera = GameCamera.Instance.Camera;
      return camera != null ? camera.GetComponent<Animator>() : null;
    }
  }
}
