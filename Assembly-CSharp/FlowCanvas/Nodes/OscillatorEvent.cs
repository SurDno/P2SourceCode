using NodeCanvas.Framework;
using ParadoxNotion.Design;

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
      curve = new AnimationCurve(new Keyframe[4]
      {
        new Keyframe(0.0f, 1f),
        new Keyframe(0.5f, 1f),
        new Keyframe(0.5f, -1f),
        new Keyframe(1f, -1f)
      });
      curve.postWrapMode = WrapMode.Loop;
    }

    protected override void RegisterPorts()
    {
      hi = AddFlowOutput("Hi");
      low = AddFlowOutput("Low");
      AddValueOutput("Value", () => value);
    }

    public override void OnGraphStarted() => time = 0.0f;

    public void Update()
    {
      value = curve.Evaluate(time);
      time += Time.deltaTime;
      Call(value >= 0.0 ? hi : low);
    }
  }
}
