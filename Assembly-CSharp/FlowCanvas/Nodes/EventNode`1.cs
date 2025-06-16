// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.EventNode`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using NodeCanvas.Framework;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  public abstract class EventNode<T> : EventNode where T : Component
  {
    public BBParameter<T> target;

    public override string name
    {
      get
      {
        return string.Format("{0} ({1})", (object) base.name.ToUpper(), !this.target.isNull || this.target.useBlackboard ? (object) this.target.ToString() : (object) "Self");
      }
    }

    protected virtual string[] GetTargetMessageEvents() => (string[]) null;

    public override void OnGraphStarted()
    {
      if (this.target.isNull && !this.target.useBlackboard)
        this.target.value = this.graphAgent.GetComponent<T>();
      if (this.target.isNull)
      {
        this.Fail(string.Format("Target is missing component of type '{0}'", (object) typeof (T).Name));
      }
      else
      {
        string[] targetMessageEvents = this.GetTargetMessageEvents();
        if (targetMessageEvents == null || targetMessageEvents.Length == 0)
          return;
        this.RegisterEvents((Component) this.target.value, targetMessageEvents);
      }
    }

    public override void OnGraphStoped()
    {
      this.UnRegisterEvents((Component) this.target.value, this.GetTargetMessageEvents());
    }
  }
}
