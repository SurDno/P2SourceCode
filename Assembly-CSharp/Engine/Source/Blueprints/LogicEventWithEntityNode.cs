// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.LogicEventWithEntityNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class LogicEventWithEntityNode : FlowControlNode
  {
    [Port("EventName")]
    private ValueInput<string> eventNameInput;
    [Port("Entity")]
    private ValueInput<IEntity> eventEntityInput;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      string name = this.eventNameInput.value;
      if (name != null)
        ServiceLocator.GetService<LogicEventService>().FireEntityEvent(name, this.eventEntityInput.value);
      this.output.Call();
    }
  }
}
