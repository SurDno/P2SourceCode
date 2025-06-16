// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.StaticCodeEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion;
using ParadoxNotion.Design;
using System;
using System.Reflection;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Description("Subscribes to a static C# System.Action Event and is called when the event is raised")]
  [Category("Events/Script")]
  public class StaticCodeEvent : EventNode
  {
    [SerializeField]
    private string eventName;
    [SerializeField]
    private System.Type targetType;
    private FlowOutput o;
    private Action pointer;

    public void SetEvent(EventInfo e)
    {
      this.targetType = e.RTReflectedType();
      this.eventName = e.Name;
      this.GatherPorts();
    }

    public override void OnGraphStarted()
    {
      if (string.IsNullOrEmpty(this.eventName))
      {
        Debug.LogError((object) "No Event Selected for 'Static Code Event'");
      }
      else
      {
        EventInfo eventInfo = this.targetType.RTGetEvent(this.eventName);
        if (eventInfo == (EventInfo) null)
        {
          Debug.LogError((object) string.Format("Event {0} is not found", (object) this.eventName));
        }
        else
        {
          base.OnGraphStarted();
          this.pointer = (Action) (() => this.o.Call());
          eventInfo.AddEventHandler((object) null, (Delegate) this.pointer);
        }
      }
    }

    public override void OnGraphStoped()
    {
      if (string.IsNullOrEmpty(this.eventName))
        return;
      this.targetType.RTGetEvent(this.eventName).RemoveEventHandler((object) null, (Delegate) this.pointer);
    }

    protected override void RegisterPorts()
    {
      if (string.IsNullOrEmpty(this.eventName))
        return;
      this.o = this.AddFlowOutput(this.eventName);
    }
  }
}
