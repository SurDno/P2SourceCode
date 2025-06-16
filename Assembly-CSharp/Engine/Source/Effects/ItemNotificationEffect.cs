using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Services;
using Inspectors;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ItemNotificationEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;

    public string Name => this.GetType().Name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public bool Prepare(float currentRealTime, float currentGameTime) => true;

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      IEntity entity = this.AbilityItem.Item;
      ServiceLocator.GetService<NotificationService>().AddNotify(NotificationEnum.ItemBroken, new object[1]
      {
        (object) entity.Template
      });
      return false;
    }

    public void Cleanup()
    {
    }
  }
}
