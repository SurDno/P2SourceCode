using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NpcPushEffect : IEffect
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected string name = "";
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected bool self = false;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float velocity = 1f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float time = 1f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float npcPushScale = 0.33f;
    private EnemyBase pusher;
    private EnemyBase pushed;
    private float startTime;
    private float lastTime;
    private float npcVelocityMultiplier = 500f;
    private Vector3 pushDirection;
    private ControllerComponent playerController;
    public ShotType punchType;

    public string Name => name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => queue;

    public void Cleanup()
    {
      if (!((Object) pushed != (Object) null))
        return;
      pushed.IsPushed = false;
      if (pushed is PlayerEnemy && playerController != null)
        playerController.PushVelocity = Vector3.zero;
    }

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      startTime = currentRealTime;
      lastTime = currentRealTime;
      if (self)
      {
        pushed = ((IEntityView) AbilityItem.Self).GameObject.GetComponent<EnemyBase>();
        pusher = ((IEntityView) Target).GameObject.GetComponent<EnemyBase>();
      }
      else
      {
        pusher = ((IEntityView) AbilityItem.Self).GameObject.GetComponent<EnemyBase>();
        pushed = ((IEntityView) Target).GameObject.GetComponent<EnemyBase>();
      }
      if ((Object) pushed == (Object) null || (Object) pusher == (Object) null)
        return true;
      pushDirection = (pushed.transform.position - pusher.transform.position).normalized;
      pushed.IsPushed = true;
      if (pushed is PlayerEnemy)
      {
        playerController = (self ? AbilityItem.Self : Target).GetComponent<ControllerComponent>();
        playerController.PushVelocity = pushDirection * velocity;
      }
      else
      {
        pushed.Push(pushDirection, pusher);
        pushed.PushMove(pushDirection * velocity * npcVelocityMultiplier * npcPushScale);
      }
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      if ((Object) pushed == (Object) null || (Object) pusher == (Object) null)
        return false;
      if (currentRealTime - (double) startTime < time)
      {
        pushed.IsPushed = true;
        if (pushed is PlayerEnemy)
        {
          float num = currentRealTime - lastTime;
          playerController.PushVelocity = (pushDirection * velocity) with
          {
            y = -1f
          };
          lastTime = currentRealTime;
        }
        return true;
      }
      pushed.IsPushed = false;
      if (pushed is PlayerEnemy)
        playerController.PushVelocity = Vector3.zero;
      return false;
    }
  }
}
