using Engine.Behaviours.Components;
using Engine.Behaviours.Engines.Controllers;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Inspectors;

public class NpcStateFightIdle : INpcState
{
  private Pivot pivot;
  private Animator animator;
  private NpcState npcState;
  private NPCEnemy enemy;
  private NavMeshAgent agent;
  private NPCWeaponService weaponService;
  private IKController ikController;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; protected set; }

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
    ikController = GameObject.GetComponent<IKController>();
    failed = false;
    inited = true;
    return true;
  }

  public NpcStateFightIdle(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(bool aim)
  {
    if (!TryInit())
      return;
    prevAreaMask = agent.areaMask;
    agentWasEnabled = agent.enabled;
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
          Debug.Log((object) "Can't sample navmesh", (UnityEngine.Object) GameObject);
          Status = NpcStateStatusEnum.Failed;
          return;
        }
      }
      if (!((UnityEngine.Object) ikController != (UnityEngine.Object) null & aim) || !((UnityEngine.Object) enemy != (UnityEngine.Object) null) || !((UnityEngine.Object) enemy.Enemy != (UnityEngine.Object) null))
        return;
      ikController.WeaponTarget = enemy.Enemy.transform;
    }
  }

  private void State_WeaponChangeEvent(WeaponEnum weapon) => weaponService.Weapon = weapon;

  public void Shutdown()
  {
    if (failed)
      return;
    npcState.WeaponChangeEvent -= State_WeaponChangeEvent;
    agent.areaMask = prevAreaMask;
    agent.enabled = agentWasEnabled;
    if (!((UnityEngine.Object) ikController != (UnityEngine.Object) null))
      return;
    ikController.WeaponTarget = (Transform) null;
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
    if (failed || InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    enemy.DesiredWalkSpeed = 0.0f;
    agent.nextPosition = animator.rootPosition;
    Status = NpcStateStatusEnum.Running;
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
