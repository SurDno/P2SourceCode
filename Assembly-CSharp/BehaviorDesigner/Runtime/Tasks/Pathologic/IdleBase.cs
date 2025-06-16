using Engine.Common.Generator;

namespace BehaviorDesigner.Runtime.Tasks.Pathologic
{
  public abstract class IdleBase : Action
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat primaryIdleProbability = 0.7f;
    [Tooltip("The amount of time to wait. Use 0 for infinite idle.")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat idleTime = 1f;
    [Tooltip("Should the wait be randomized?")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedBool randomIdle = false;
    [Tooltip("The minimum wait time if random wait is enabled")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat randomIdleMin = 1f;
    [Tooltip("The maximum wait time if random wait is enabled")]
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [SerializeField]
    public SharedFloat randomIdleMax = 1f;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [SerializeField]
    public SharedBool MakeObstacle = false;
    private float waitDuration;
    private float startTime;
    private float pauseTime;
    private bool infinite;
    private NpcState npcState;

    protected abstract void DoIdle(NpcState state, float primaryIdleProbability);

    public override void OnAwake()
    {
      npcState = gameObject.GetComponent<NpcState>();
      if (!((Object) npcState == (Object) null))
        return;
      Debug.LogWarning((object) (gameObject.name + ": doesn't contain " + typeof (NpcState).Name + " engine component"));
    }

    public override void OnStart()
    {
      if ((Object) npcState == (Object) null)
        return;
      infinite = idleTime.Value == 0.0 && !randomIdle.Value;
      DoIdle(npcState, primaryIdleProbability.Value);
      startTime = Time.time;
      if (randomIdle.Value)
        waitDuration = Random.Range(randomIdleMin.Value, randomIdleMax.Value);
      else
        waitDuration = idleTime.Value;
    }

    public override TaskStatus OnUpdate()
    {
      if ((Object) npcState == (Object) null)
        return TaskStatus.Failure;
      return infinite || startTime + (double) waitDuration >= (double) Time.time ? TaskStatus.Running : TaskStatus.Success;
    }

    public override void OnPause(bool paused)
    {
      if (paused)
        pauseTime = Time.time;
      else
        startTime += Time.time - pauseTime;
    }

    public override void OnReset()
    {
      idleTime = 1f;
      randomIdle = false;
      randomIdleMin = 1f;
      randomIdleMax = 1f;
    }
  }
}
