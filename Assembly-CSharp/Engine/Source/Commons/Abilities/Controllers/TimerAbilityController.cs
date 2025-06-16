// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Abilities.Controllers.TimerAbilityController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Inspectors;

#nullable disable
namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class TimerAbilityController : IAbilityController, IUpdatable
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected bool realTime = false;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected double interval = 0.0;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected double timeout = 0.0;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected double intervalTime = 0.0;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected double timeoutTime = 0.0;
    private IEntity owner;
    private AbilityItem abilityItem;
    private bool timeOutWorked;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
      if (this.timeout == 0.0)
        return;
      this.timeOutWorked = false;
      TimeService service = ServiceLocator.GetService<TimeService>();
      this.timeoutTime = this.realTime ? service.RealTime.TotalSeconds : service.AbsoluteGameTime.TotalSeconds;
    }

    public void Shutdown()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void ComputeUpdate()
    {
      TimeService service = ServiceLocator.GetService<TimeService>();
      double num = this.realTime ? service.RealTime.TotalSeconds : service.AbsoluteGameTime.TotalSeconds;
      if (this.interval > 0.0 && this.intervalTime + this.interval <= num)
      {
        this.intervalTime = num;
        this.abilityItem.Active = true;
        this.abilityItem.Active = false;
      }
      if (this.timeout <= 0.0 || this.timeoutTime + this.timeout > num || this.timeOutWorked)
        return;
      this.timeOutWorked = true;
      this.abilityItem.Active = true;
      this.abilityItem.Active = false;
    }
  }
}
