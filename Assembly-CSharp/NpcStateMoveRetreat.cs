using System;
using System.Collections;
using Engine.Behaviours.Components;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.Gizmos;
using Inspectors;

public class NpcStateMoveRetreat : INpcState
{
  private const float sectorAngleInDegrees = 180f;
  private const int rayCount = 15;
  private const float searchDistance = 15f;
  private const float testFrequency = 2f;
  private NpcState npcState;
  private Pivot pivot;
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private NPCWeaponService weaponService;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private Transform target;
  private float retreatDistance;
  private float timeLeftToNextBestRetreatDirectionCheck;
  private float retreatAngle;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status { get; private set; }

  private bool TryInit()
  {
    if (inited)
      return true;
    behavior = pivot.GetBehavior();
    agent = pivot.GetAgent();
    weaponService = pivot.GetNpcWeaponService();
    failed = false;
    inited = true;
    return true;
  }

  public NpcStateMoveRetreat(NpcState npcState, Pivot pivot)
  {
    this.npcState = npcState;
    this.pivot = pivot;
    GameObject = npcState.gameObject;
  }

  public void Activate(Transform target, float retreatDistance)
  {
    if (!TryInit())
      return;
    this.target = target;
    this.retreatDistance = retreatDistance;
    prevAreaMask = agent.areaMask;
    agentWasEnabled = agent.enabled;
    LocationItemComponent component = (LocationItemComponent) npcState.Owner.GetComponent<ILocationItemComponent>();
    if (component == null)
    {
      Debug.LogWarning((object) (GameObject.name + ": location component not found"));
    }
    else
    {
      bool isIndoor = component.IsIndoor;
      NPCStateHelper.SetAgentAreaMask(agent, isIndoor);
      agent.enabled = true;
      if (agent.hasPath)
        agent.ResetPath();
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
      float? retreatDirection = FindBestRetreatDirection(GameObject.transform.position, target.transform.position);
      if (!retreatDirection.HasValue)
      {
        Status = NpcStateStatusEnum.Failed;
      }
      else
      {
        Status = NpcStateStatusEnum.Running;
        retreatAngle = retreatDirection.Value;
        behavior.StartMovement(GetRetreatDirection(retreatAngle));
        timeLeftToNextBestRetreatDirectionCheck = 0.5f;
        if (!((UnityEngine.Object) weaponService != (UnityEngine.Object) null))
          return;
        weaponService.Weapon = WeaponEnum.Unknown;
      }
    }
  }

  public void Shutdown()
  {
    if (failed)
      return;
    agent.areaMask = prevAreaMask;
    agent.enabled = agentWasEnabled;
    if (!((UnityEngine.Object) weaponService != (UnityEngine.Object) null))
      return;
    weaponService.Weapon = npcState.Weapon;
  }

  public void OnAnimatorMove()
  {
    if (failed)
      return;
    behavior.OnExternalAnimatorMove();
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
    if (Status != 0)
    {
      if (!((UnityEngine.Object) target != (UnityEngine.Object) null))
        return;
      behavior.Move(GetRetreatDirection(retreatAngle), 0.0f);
    }
    else if ((UnityEngine.Object) target == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "Null target");
      Status = NpcStateStatusEnum.Failed;
    }
    else
    {
      Vector3 position1 = target.position;
      Vector3 position2 = GameObject.transform.position;
      if ((double) (position2 - position1).magnitude > retreatDistance)
      {
        Status = NpcStateStatusEnum.Success;
      }
      else
      {
        timeLeftToNextBestRetreatDirectionCheck -= Time.deltaTime;
        if (timeLeftToNextBestRetreatDirectionCheck <= 0.0)
        {
          float? retreatDirection = FindBestRetreatDirection(position2, position1);
          if (!retreatDirection.HasValue)
          {
            Status = NpcStateStatusEnum.Failed;
            return;
          }
          retreatAngle = retreatDirection.Value;
          timeLeftToNextBestRetreatDirectionCheck += 0.5f;
        }
        behavior.Move(GetRetreatDirection(retreatAngle), 15f);
      }
    }
  }

  private Vector3 GetRetreatDirection(float retreatAngle)
  {
    Vector3 normalized = (target.position - GameObject.transform.position).normalized;
    return -(Quaternion.AngleAxis(retreatAngle, Vector3.up) * normalized);
  }

  private float? FindBestRetreatDirection(Vector3 myPosition, Vector3 enemyPosition)
  {
    Vector3 normalized = (enemyPosition - myPosition).normalized;
    float num1 = 3f;
    float num2 = num1;
    float num3 = num1;
    float angle1 = 0.0f;
    for (int index = 0; index < 15; ++index)
    {
      float angle2 = (float) (index * 180.0 / 14.0 - 90.0);
      Vector3 vector3 = -(Quaternion.AngleAxis(angle2, Vector3.up) * normalized);
      NavMeshHit hit;
      float num4 = !NavMesh.Raycast(myPosition, myPosition + vector3 * 15f, out hit, -1) ? 15f : hit.distance;
      float num5 = hit.distance * Mathf.Cos((float) (0.5 * angle2 * (Math.PI / 180.0)));
      if (InstanceByRequest<EngineApplication>.Instance.IsDebug)
      {
        Vector3 from = GameObject.transform.position;
        Vector3 to = myPosition + num4 * vector3;
        Color color = Color.green;
        CoroutineService.Instance.Route(ExecuteSecond(0.5f, (Action) (() => ServiceLocator.GetService<GizmoService>().DrawLine(from, to, color))));
      }
      if (num5 > (double) num2)
      {
        num2 = num5;
        num3 = num4;
        angle1 = angle2;
      }
    }
    if (num2 == (double) num1)
      return new float?();
    if (InstanceByRequest<EngineApplication>.Instance.IsDebug)
    {
      Vector3 vector3 = -(Quaternion.AngleAxis(angle1, Vector3.up) * normalized);
      Vector3 from = GameObject.transform.position;
      Vector3 to = myPosition + num3 * vector3;
      Color color = Color.red;
      CoroutineService.Instance.Route(ExecuteSecond(0.5f, (Action) (() => ServiceLocator.GetService<GizmoService>().DrawLine(from, to, color))));
    }
    return angle1;
  }

  public IEnumerator ExecuteSecond(float delay, Action action)
  {
    float time = Time.unscaledTime;
    while (time + (double) delay > (double) Time.unscaledTime)
    {
      Action action1 = action;
      if (action1 != null)
        action1();
      yield return null;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
