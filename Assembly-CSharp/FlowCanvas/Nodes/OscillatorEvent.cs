// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.OscillatorEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("OSC Pulse")]
  [Category("Events/Other")]
  [Description("Calls Hi when curve value is greater than 0, else calls Low.\nThe curve is evaluated over time and it's evaluated value is exposed")]
  public class OscillatorEvent : EventNode, IUpdatable
  {
    public AnimationCurve curve;
    private float time;
    private float value;
    private FlowOutput hi;
    private FlowOutput low;

    public OscillatorEvent()
    {
      this.curve = new AnimationCurve(new Keyframe[4]
      {
        new Keyframe(0.0f, 1f),
        new Keyframe(0.5f, 1f),
        new Keyframe(0.5f, -1f),
        new Keyframe(1f, -1f)
      });
      this.curve.postWrapMode = WrapMode.Loop;
    }

    protected override void RegisterPorts()
    {
      this.hi = this.AddFlowOutput("Hi");
      this.low = this.AddFlowOutput("Low");
      this.AddValueOutput<float>("Value", (ValueHandler<float>) (() => this.value));
    }

    public override void OnGraphStarted() => this.time = 0.0f;

    public void Update()
    {
      this.value = this.curve.Evaluate(this.time);
      this.time += Time.deltaTime;
      this.Call((double) this.value >= 0.0 ? this.hi : this.low);
    }
  }
}
