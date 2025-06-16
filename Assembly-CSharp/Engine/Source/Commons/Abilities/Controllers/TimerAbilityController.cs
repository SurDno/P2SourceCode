using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class TimerAbilityController : IAbilityController, IUpdatable
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool realTime = false;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected double interval = 0.0;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected double timeout = 0.0;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected double intervalTime;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected double timeoutTime;
    private IEntity owner;
    private AbilityItem abilityItem;
    private bool timeOutWorked;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
      if (timeout == 0.0)
        return;
      timeOutWorked = false;
      TimeService service = ServiceLocator.GetService<TimeService>();
      timeoutTime = realTime ? service.RealTime.TotalSeconds : service.AbsoluteGameTime.TotalSeconds;
    }

    public void Shutdown()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
    }

    public void ComputeUpdate()
    {
      TimeService service = ServiceLocator.GetService<TimeService>();
      double num = realTime ? service.RealTime.TotalSeconds : service.AbsoluteGameTime.TotalSeconds;
      if (interval > 0.0 && intervalTime + interval <= num)
      {
        intervalTime = num;
        abilityItem.Active = true;
        abilityItem.Active = false;
      }
      if (timeout <= 0.0 || timeoutTime + timeout > num || timeOutWorked)
        return;
      timeOutWorked = true;
      abilityItem.Active = true;
      abilityItem.Active = false;
    }
  }
}
