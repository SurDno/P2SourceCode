using Engine.Behaviours.Components;
using Engine.Behaviours.Unity.Mecanim;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;

public class NpcStateFightEscape : INpcState
{
  private NpcState npcState;
  private Pivot pivot;
  private NPCEnemy enemy;
  private NavMeshAgent agent;
  private Animator animator;
  private AnimatorState45 animatorState;
  protected FightAnimatorBehavior.AnimatorState fightAnimatorState;
  private NPCWeaponService weaponService;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private float escapeDistance;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status { get; protected set; }

  private bool TryInit()
  {
    if (inited)
      return true;
    enemy = pivot.GetNpcEnemy();
    agent = pivot.GetAgent();
    animator = pivot.GetAnimator();
    weaponService = pivot.GetNpcWeaponService();
    if ((UnityEngine.Object) animator == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("Null animator " + GameObject.name), (UnityEngine.Object) GameObject);
      Debug.LogError((object) ("Null animator " + GameObject.GetFullName()));
      failed = true;
      return false;
    }
    animatorState = AnimatorState45.GetAnimatorState(animator);
    fightAnimatorState = FightAnimatorBehavior.GetAnimatorState(animator);
    failed = false;
    inited = true;
    return true;
  }

  public NpcStateFightEscape(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(float escapeDistance)
  {
    if (!TryInit())
      return;
    prevAreaMask = agent.areaMask;
    agentWasEnabled = agent.enabled;
    this.escapeDistance = escapeDistance;
    weaponService.Weapon = npcState.Weapon;
    npcState.WeaponChangeEvent += State_WeaponChangeEvent;
    Status = NpcStateStatusEnum.Running;
    LocationItemComponent component = (LocationItemComponent) npcState.Owner.GetComponent<ILocationItemComponent>();
    if (component == null)
    {
      Debug.LogWarning((object) (GameObject.name + ": location component not found"));
      Status = NpcStateStatusEnum.Failed;
    }
    else
    {
      bool isIndoor = component.IsIndoor;
      NPCStateHelper.SetAgentAreaMask(agent, isIndoor);
      agent.enabled = true;
      if (!agent.isOnNavMesh)
      {
        Vector3 position = GameObject.transform.position;
        if (NavMeshUtility.SampleRaycastPosition(ref position, isIndoor ? 1f : 5f, isIndoor ? 2f : 10f, agent.areaMask))
        {
          agent.Warp(position);
        }
        else
        {
          Status = NpcStateStatusEnum.Failed;
          return;
        }
      }
      enemy.DesiredWalkSpeed = 0.0f;
      animatorState.SetTrigger("Fight.Triggers/CancelAttack");
      animatorState.ResetTrigger("Fight.Triggers/CancelEscape");
      if ((UnityEngine.Object) enemy == (UnityEngine.Object) null)
        Debug.LogError((object) "enemy == null");
      else if ((UnityEngine.Object) enemy.Enemy == (UnityEngine.Object) null)
        Debug.LogError((object) "enemy.Enemy == null");
      else if ((UnityEngine.Object) enemy.Enemy.transform == (UnityEngine.Object) null)
        Debug.LogError((object) "enemy.Enemy.transform == null");
      else if ((UnityEngine.Object) enemy.transform == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "enemy.transform == null");
      }
      else
      {
        if ((double) Vector3.Angle(enemy.Enemy.transform.position - enemy.transform.position, enemy.transform.forward) < 60.0)
          animatorState.SetTrigger("Fight.Triggers/Escape");
        else
          animatorState.SetTrigger("Fight.Triggers/EscapeImmediate");
        NavMeshHit hit;
        if (agent.isOnNavMesh || !NavMesh.SamplePosition(enemy.gameObject.transform.position, out hit, 1f, -1))
          return;
        agent.Warp(hit.position);
      }
    }
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon) => weaponService.Weapon = weapon;

  public void Shutdown()
  {
    if (failed)
      return;
    animatorState.ResetTrigger("Fight.Triggers/CancelAttack");
    animatorState.ResetTrigger("Fight.Triggers/Escape");
    animatorState.ResetTrigger("Fight.Triggers/EscapeImmediate");
    animatorState.SetTrigger("Fight.Triggers/CancelEscape");
    npcState.WeaponChangeEvent -= State_WeaponChangeEvent;
    agent.areaMask = prevAreaMask;
    agent.enabled = agentWasEnabled;
  }

  public void OnAnimatorMove()
  {
    if (failed)
      return;
    enemy?.OnExternalAnimatorMove();
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void Update()
  {
    if (failed || InstanceByRequest<EngineApplication>.Instance.IsPaused || Status != 0)
      return;
    if ((UnityEngine.Object) enemy == (UnityEngine.Object) null || (UnityEngine.Object) enemy.Enemy == (UnityEngine.Object) null)
    {
      Status = NpcStateStatusEnum.Failed;
    }
    else
    {
      enemy.RotationTarget = (Transform) null;
      enemy.RotateByPath = false;
      enemy.RetreatAngle = new float?();
      if (fightAnimatorState.IsReaction)
        return;
      Vector3 vector3 = enemy.Enemy.transform.position - enemy.transform.position;
      float magnitude = vector3.magnitude;
      if (escapeDistance != 0.0 && magnitude > (double) escapeDistance)
      {
        animatorState.SetTrigger("Fight.Triggers/CancelEscape");
        Status = NpcStateStatusEnum.Success;
      }
      else
      {
        vector3 /= magnitude;
        float? retreatDirection2 = PathfindingHelper.FindBestRetreatDirection2(enemy.transform, enemy.Enemy.transform);
        enemy.RotationTarget = enemy.Enemy.transform;
        enemy.RetreatAngle = retreatDirection2;
        agent.nextPosition = animator.rootPosition;
      }
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
