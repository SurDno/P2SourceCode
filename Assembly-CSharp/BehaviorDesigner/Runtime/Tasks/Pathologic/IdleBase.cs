using Engine.Common.Generator;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  public abstract class IdleBase : Action
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat primaryIdleProbability = (SharedFloat) 0.7f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The amount of time to wait. Use 0 for infinite idle.")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat idleTime = (SharedFloat) 1f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("Should the wait be randomized?")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool randomIdle = (SharedBool) false;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The minimum wait time if random wait is enabled")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat randomIdleMin = (SharedFloat) 1f;
    [BehaviorDesigner.Runtime.Tasks.Tooltip("The maximum wait time if random wait is enabled")]
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedFloat randomIdleMax = (SharedFloat) 1f;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    public SharedBool MakeObstacle = (SharedBool) false;
    private float waitDuration;
    private float startTime;
    private float pauseTime;
    private bool infinite = false;
    private NpcState npcState;

    protected abstract void DoIdle(NpcState state, float primaryIdleProbability);

    public override void OnAwake()
    {
      this.npcState = this.gameObject.GetComponent<NpcState>();
      if (!((Object) this.npcState == (Object) null))
        return;
      Debug.LogWarning((object) (this.gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"));
    }

    public override void OnStart()
    {
      if ((Object) this.npcState == (Object) null)
        return;
      this.infinite = (double) this.idleTime.Value == 0.0 && !this.randomIdle.Value;
      this.DoIdle(this.npcState, this.primaryIdleProbability.Value);
      this.startTime = Time.time;
      if (this.randomIdle.Value)
        this.waitDuration = Random.Range(this.randomIdleMin.Value, this.randomIdleMax.Value);
      else
        this.waitDuration = this.idleTime.Value;
    }

    public override TaskStatus OnUpdate()
    {
      if ((Object) this.npcState == (Object) null)
        return TaskStatus.Failure;
      return this.infinite || (double) this.startTime + (double) this.waitDuration >= (double) Time.time ? TaskStatus.Running : TaskStatus.Success;
    }

    public override void OnPause(bool paused)
    {
      if (paused)
        this.pauseTime = Time.time;
      else
        this.startTime += Time.time - this.pauseTime;
    }

    public override void OnReset()
    {
      this.idleTime = (SharedFloat) 1f;
      this.randomIdle = (SharedBool) false;
      this.randomIdleMin = (SharedFloat) 1f;
      this.randomIdleMax = (SharedFloat) 1f;
    }
  }
}
