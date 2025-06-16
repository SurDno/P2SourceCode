using Engine.Behaviours.Components;
using Engine.Source.Components;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic.Sanitar
{
  public abstract class SetGaitBase : Action
  {
    protected EngineBehavior behavior;

    public abstract EngineBehavior.GaitType GetGait();

    public override TaskStatus OnUpdate()
    {
      if ((Object) this.behavior == (Object) null)
      {
        this.behavior = this.gameObject.GetComponent<EngineBehavior>();
        if ((Object) this.behavior == (Object) null)
        {
          Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (BehaviorComponent).Name + " engine component"), (Object) this.gameObject);
          return TaskStatus.Failure;
        }
      }
      this.behavior.Gait = this.GetGait();
      return TaskStatus.Success;
    }
  }
}
