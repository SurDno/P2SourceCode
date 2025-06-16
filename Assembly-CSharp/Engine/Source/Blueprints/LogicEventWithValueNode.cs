// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.LogicEventWithValueNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class LogicEventWithValueNode : FlowControlNode
  {
    [Port("EventName")]
    private ValueInput<string> eventNameInput;
    [Port("Value")]
    private ValueInput<string> eventValueInput;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      string name = this.eventNameInput.value;
      if (name != null)
        ServiceLocator.GetService<LogicEventService>().FireValueEvent(name, this.eventValueInput.value);
      this.output.Call();
    }
  }
}
