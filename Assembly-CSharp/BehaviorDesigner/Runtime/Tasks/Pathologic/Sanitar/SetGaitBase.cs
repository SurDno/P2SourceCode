using Engine.Behaviours.Components;
using Engine.Source.Components;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic.Sanitar
{
  public abstract class SetGaitBase : Action
  {
    protected EngineBehavior behavior;

    public abstract EngineBehavior.GaitType GetGait();

    public override TaskStatus OnUpdate()
    {
      if ((Object) behavior == (Object) null)
      {
        behavior = gameObject.GetComponent<EngineBehavior>();
        if ((Object) behavior == (Object) null)
        {
          Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (BehaviorComponent).Name + " engine component"), (Object) gameObject);
          return TaskStatus.Failure;
        }
      }
      behavior.Gait = GetGait();
      return TaskStatus.Success;
    }
  }
}
