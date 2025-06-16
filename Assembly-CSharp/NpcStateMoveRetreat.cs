using Engine.Behaviours.Components;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Components.Utilities;
using Engine.Source.Services.Gizmos;
using Inspectors;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
  private float retreatAngle = 0.0f;
  private bool inited;
  private bool failed;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status { get; private set; }

  private bool TryInit()
  {
    if (this.inited)
      return true;
    this.behavior = this.pivot.GetBehavior();
    this.agent = this.pivot.GetAgent();
    this.weaponService = this.pivot.GetNpcWeaponService();
    this.failed = false;
    this.inited = true;
    return true;
  }

  public NpcStateMoveRetreat(NpcState npcState, Pivot pivot)
  {
    this.npcState = npcState;
    this.pivot = pivot;
    this.GameObject = npcState.gameObject;
  }

  public void Activate(Transform target, float retreatDistance)
  {
    if (!this.TryInit())
      return;
    this.target = target;
    this.retreatDistance = retreatDistance;
    this.prevAreaMask = this.agent.areaMask;
    this.agentWasEnabled = this.agent.enabled;
    LocationItemComponent component = (LocationItemComponent) this.npcState.Owner.GetComponent<ILocationItemComponent>();
    if (component == null)
    {
      Debug.LogWarning((object) (this.GameObject.name + ": location component not found"));
    }
    else
    {
      bool isIndoor = component.IsIndoor;
      NPCStateHelper.SetAgentAreaMask(this.agent, isIndoor);
      this.agent.enabled = true;
      if (this.agent.hasPath)
        this.agent.ResetPath();
      if (!this.agent.isOnNavMesh)
      {
        Vector3 position = this.GameObject.transform.position;
        if (NavMeshUtility.SampleRaycastPosition(ref position, isIndoor ? 1f : 5f, isIndoor ? 2f : 10f, this.agent.areaMask))
        {
          this.agent.Warp(position);
        }
        else
        {
          Debug.Log((object) "Can't sample navmesh", (UnityEngine.Object) this.GameObject);
          this.Status = NpcStateStatusEnum.Failed;
          return;
        }
      }
      float? retreatDirection = this.FindBestRetreatDirection(this.GameObject.transform.position, target.transform.position);
      if (!retreatDirection.HasValue)
      {
        this.Status = NpcStateStatusEnum.Failed;
      }
      else
      {
        this.Status = NpcStateStatusEnum.Running;
        this.retreatAngle = retreatDirection.Value;
        this.behavior.StartMovement(this.GetRetreatDirection(this.retreatAngle));
        this.timeLeftToNextBestRetreatDirectionCheck = 0.5f;
        if (!((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null))
          return;
        this.weaponService.Weapon = WeaponEnum.Unknown;
      }
    }
  }

  public void Shutdown()
  {
    if (this.failed)
      return;
    this.agent.areaMask = this.prevAreaMask;
    this.agent.enabled = this.agentWasEnabled;
    if (!((UnityEngine.Object) this.weaponService != (UnityEngine.Object) null))
      return;
    this.weaponService.Weapon = this.npcState.Weapon;
  }

  public void OnAnimatorMove()
  {
    if (this.failed)
      return;
    this.behavior.OnExternalAnimatorMove();
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!this.failed)
      ;
  }

  public void Update()
  {
    if (this.failed || InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    if (this.Status != 0)
    {
      if (!((UnityEngine.Object) this.target != (UnityEngine.Object) null))
        return;
      this.behavior.Move(this.GetRetreatDirection(this.retreatAngle), 0.0f);
    }
    else if ((UnityEngine.Object) this.target == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "Null target");
      this.Status = NpcStateStatusEnum.Failed;
    }
    else
    {
      Vector3 position1 = this.target.position;
      Vector3 position2 = this.GameObject.transform.position;
      if ((double) (position2 - position1).magnitude > (double) this.retreatDistance)
      {
        this.Status = NpcStateStatusEnum.Success;
      }
      else
      {
        this.timeLeftToNextBestRetreatDirectionCheck -= Time.deltaTime;
        if ((double) this.timeLeftToNextBestRetreatDirectionCheck <= 0.0)
        {
          float? retreatDirection = this.FindBestRetreatDirection(position2, position1);
          if (!retreatDirection.HasValue)
          {
            this.Status = NpcStateStatusEnum.Failed;
            return;
          }
          this.retreatAngle = retreatDirection.Value;
          this.timeLeftToNextBestRetreatDirectionCheck += 0.5f;
        }
        this.behavior.Move(this.GetRetreatDirection(this.retreatAngle), 15f);
      }
    }
  }

  private Vector3 GetRetreatDirection(float retreatAngle)
  {
    Vector3 normalized = (this.target.position - this.GameObject.transform.position).normalized;
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
      float angle2 = (float) ((double) index * 180.0 / 14.0 - 90.0);
      Vector3 vector3 = -(Quaternion.AngleAxis(angle2, Vector3.up) * normalized);
      NavMeshHit hit;
      float num4 = !NavMesh.Raycast(myPosition, myPosition + vector3 * 15f, out hit, -1) ? 15f : hit.distance;
      float num5 = hit.distance * Mathf.Cos((float) (0.5 * (double) angle2 * (Math.PI / 180.0)));
      if (InstanceByRequest<EngineApplication>.Instance.IsDebug)
      {
        Vector3 from = this.GameObject.transform.position;
        Vector3 to = myPosition + num4 * vector3;
        Color color = Color.green;
        CoroutineService.Instance.Route(this.ExecuteSecond(0.5f, (Action) (() => ServiceLocator.GetService<GizmoService>().DrawLine(from, to, color))));
      }
      if ((double) num5 > (double) num2)
      {
        num2 = num5;
        num3 = num4;
        angle1 = angle2;
      }
    }
    if ((double) num2 == (double) num1)
      return new float?();
    if (InstanceByRequest<EngineApplication>.Instance.IsDebug)
    {
      Vector3 vector3 = -(Quaternion.AngleAxis(angle1, Vector3.up) * normalized);
      Vector3 from = this.GameObject.transform.position;
      Vector3 to = myPosition + num3 * vector3;
      Color color = Color.red;
      CoroutineService.Instance.Route(this.ExecuteSecond(0.5f, (Action) (() => ServiceLocator.GetService<GizmoService>().DrawLine(from, to, color))));
    }
    return new float?(angle1);
  }

  public IEnumerator ExecuteSecond(float delay, Action action)
  {
    float time = Time.unscaledTime;
    while ((double) time + (double) delay > (double) Time.unscaledTime)
    {
      Action action1 = action;
      if (action1 != null)
        action1();
      yield return (object) null;
    }
  }

  public void OnLodStateChanged(bool enabled)
  {
  }
}
