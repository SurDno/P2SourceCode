using ParadoxNotion;
using ParadoxNotion.Design;
using System;
using System.Reflection;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  [Description("Subscribes to a C# System.Action Event and is called when the event is raised")]
  [Category("Events/Script")]
  public class CodeEvent : EventNode<Transform>
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
        Debug.LogError((object) "No Event Selected for CodeEvent, or target is NULL");
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
          Component component = this.target.value.GetComponent(this.targetType);
          if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          {
            Debug.LogError((object) "Target is null");
          }
          else
          {
            this.pointer = (Action) (() => this.o.Call());
            eventInfo.AddEventHandler((object) component, (Delegate) this.pointer);
          }
        }
      }
    }

    public override void OnGraphStoped()
    {
      if (string.IsNullOrEmpty(this.eventName) || (UnityEngine.Object) this.target.value == (UnityEngine.Object) null)
        return;
      this.targetType.RTGetEvent(this.eventName).RemoveEventHandler((object) this.target.value.GetComponent(this.targetType), (Delegate) this.pointer);
    }

    protected override void RegisterPorts()
    {
      if (string.IsNullOrEmpty(this.eventName))
        return;
      this.o = this.AddFlowOutput(this.eventName);
    }
  }
}
