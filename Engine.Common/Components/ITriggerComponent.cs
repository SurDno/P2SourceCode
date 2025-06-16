namespace Engine.Common.Components
{
  public interface ITriggerComponent : IComponent
  {
    bool Contains(IEntity entity);

    event TriggerHandler EntityEnterEvent;

    event TriggerHandler EntityExitEvent;
  }
}
