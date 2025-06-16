using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class TriggerNode : FlowControlNode
  {
    private bool value;

    [Port("Set")]
    public void Set() => this.value = true;

    [Port("Reset")]
    public void Reset() => this.value = false;

    [Port("Value")]
    private bool Value() => this.value;
  }
}
