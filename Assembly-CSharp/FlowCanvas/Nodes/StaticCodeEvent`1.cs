using System;
using System.Reflection;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Description("Subscribes to a static C# System.Action<T> Event and is called when the event is raised")]
  [Category("Events/Script")]
  public class StaticCodeEvent<T> : EventNode
  {
    [SerializeField]
    private string eventName;
    [SerializeField]
    private Type targetType;
    private FlowOutput o;
    private Action<T> pointer;
    private T eventValue;

    public void SetEvent(EventInfo e)
    {
      targetType = e.RTReflectedType();
      eventName = e.Name;
      GatherPorts();
    }

    public override void OnGraphStarted()
    {
      if (string.IsNullOrEmpty(eventName))
      {
        Debug.LogError((object) "No Event Selected for 'Static Code Event'");
      }
      else
      {
        EventInfo eventInfo = targetType.RTGetEvent(eventName);
        if (eventInfo == null)
        {
          Debug.LogError((object) string.Format("Event {0} is not found", eventName));
        }
        else
        {
          base.OnGraphStarted();
          pointer = v =>
          {
            eventValue = v;
            o.Call();
          };
          eventInfo.AddEventHandler(null, pointer);
        }
      }
    }

    public override void OnGraphStoped()
    {
      if (string.IsNullOrEmpty(eventName))
        return;
      targetType.RTGetEvent(eventName).RemoveEventHandler(null, pointer);
    }

    protected override void RegisterPorts()
    {
      if (string.IsNullOrEmpty(eventName))
        return;
      o = AddFlowOutput(eventName);
      AddValueOutput("Value", () => eventValue);
    }
  }
}
