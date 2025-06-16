using FlowCanvas.Nodes;
using System.ComponentModel;
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
      return (Object) camera != (Object) null ? camera.GetComponent<Animator>() : (Animator) null;
    }
  }
}
