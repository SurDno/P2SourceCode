using NodeCanvas.Framework;
using UnityEngine;

namespace FlowCanvas.Nodes
{
  public abstract class EventNode<T> : EventNode where T : Component
  {
    public BBParameter<T> target;

    public override string name
    {
      get
      {
        return string.Format("{0} ({1})", base.name.ToUpper(), !target.isNull || target.useBlackboard ? target.ToString() : (object) "Self");
      }
    }

    protected virtual string[] GetTargetMessageEvents() => null;

    public override void OnGraphStarted()
    {
      if (target.isNull && !target.useBlackboard)
        target.value = graphAgent.GetComponent<T>();
      if (target.isNull)
      {
        Fail(string.Format("Target is missing component of type '{0}'", typeof (T).Name));
      }
      else
      {
        string[] targetMessageEvents = GetTargetMessageEvents();
        if (targetMessageEvents == null || targetMessageEvents.Length == 0)
          return;
        RegisterEvents(target.value, targetMessageEvents);
      }
    }

    public override void OnGraphStoped()
    {
      UnRegisterEvents(target.value, GetTargetMessageEvents());
    }
  }
}
