// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.NpcPushEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.AttackerPlayer;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Commons.Abilities;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Effects
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class NpcPushEffect : IEffect
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected string name = "";
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterEffectQueueEnum queue = ParameterEffectQueueEnum.None;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected bool self = false;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float velocity = 1f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true, Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected float time = 1f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
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

    public string Name => this.name;

    [Inspected]
    public AbilityItem AbilityItem { get; set; }

    public IEntity Target { get; set; }

    public ParameterEffectQueueEnum Queue => this.queue;

    public void Cleanup()
    {
      if (!((Object) this.pushed != (Object) null))
        return;
      this.pushed.IsPushed = false;
      if (this.pushed is PlayerEnemy && this.playerController != null)
        this.playerController.PushVelocity = Vector3.zero;
    }

    public bool Prepare(float currentRealTime, float currentGameTime)
    {
      this.startTime = currentRealTime;
      this.lastTime = currentRealTime;
      if (this.self)
      {
        this.pushed = ((IEntityView) this.AbilityItem.Self).GameObject.GetComponent<EnemyBase>();
        this.pusher = ((IEntityView) this.Target).GameObject.GetComponent<EnemyBase>();
      }
      else
      {
        this.pusher = ((IEntityView) this.AbilityItem.Self).GameObject.GetComponent<EnemyBase>();
        this.pushed = ((IEntityView) this.Target).GameObject.GetComponent<EnemyBase>();
      }
      if ((Object) this.pushed == (Object) null || (Object) this.pusher == (Object) null)
        return true;
      this.pushDirection = (this.pushed.transform.position - this.pusher.transform.position).normalized;
      this.pushed.IsPushed = true;
      if (this.pushed is PlayerEnemy)
      {
        this.playerController = (this.self ? this.AbilityItem.Self : this.Target).GetComponent<ControllerComponent>();
        this.playerController.PushVelocity = this.pushDirection * this.velocity;
      }
      else
      {
        this.pushed.Push(this.pushDirection, this.pusher);
        this.pushed.PushMove(this.pushDirection * this.velocity * this.npcVelocityMultiplier * this.npcPushScale);
      }
      return true;
    }

    public bool Compute(float currentRealTime, float currentGameTime)
    {
      if ((Object) this.pushed == (Object) null || (Object) this.pusher == (Object) null)
        return false;
      if ((double) currentRealTime - (double) this.startTime < (double) this.time)
      {
        this.pushed.IsPushed = true;
        if (this.pushed is PlayerEnemy)
        {
          float num = currentRealTime - this.lastTime;
          this.playerController.PushVelocity = (this.pushDirection * this.velocity) with
          {
            y = -1f
          };
          this.lastTime = currentRealTime;
        }
        return true;
      }
      this.pushed.IsPushed = false;
      if (this.pushed is PlayerEnemy)
        this.playerController.PushVelocity = Vector3.zero;
      return false;
    }
  }
}
