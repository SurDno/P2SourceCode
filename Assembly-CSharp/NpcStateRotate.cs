using System;
using Engine.Behaviours.Components;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;

public class NpcStateRotate : INpcState
{
  private ModeEnum mode;
  private NpcState npcState;
  private Pivot pivot;
  private EngineBehavior behavior;
  private NavMeshAgent agent;
  private NPCWeaponService weaponService;
  private Rigidbody rigidbody;
  private bool rigidbodyWasGravity;
  private bool agentWasEnabled;
  private Transform target;
  private Quaternion rotation;
  private bool done;
  [Inspected]
  private bool inited;
  [Inspected]
  private bool failed;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status
  {
    get => done ? NpcStateStatusEnum.Success : NpcStateStatusEnum.Running;
  }

  private bool TryInit()
  {
    if (inited)
      return true;
    behavior = pivot.GetBehavior();
    weaponService = pivot.GetNpcWeaponService();
    rigidbody = pivot.GetRigidbody();
    agent = pivot.GetAgent();
    if ((UnityEngine.Object) agent == (UnityEngine.Object) null)
    {
      Debug.LogError((object) ("No navmesh agent " + GameObject.name), (UnityEngine.Object) GameObject);
      failed = true;
      return false;
    }
    failed = false;
    inited = true;
    return true;
  }

  public NpcStateRotate(NpcState npcState, Pivot pivot)
  {
    GameObject = npcState.gameObject;
    this.pivot = pivot;
    this.npcState = npcState;
  }

  public void Activate(Transform target)
  {
    if (!TryInit())
      return;
    this.target = target;
    done = false;
    agentWasEnabled = agent.enabled;
    bool indoor = true;
    if (npcState.Owner != null)
    {
      LocationItemComponent component = (LocationItemComponent) npcState.Owner.GetComponent<ILocationItemComponent>();
      if (component == null)
      {
        Debug.LogWarning((object) (GameObject.name + ": location component not found"));
        return;
      }
      indoor = component.IsIndoor;
    }
    NPCStateHelper.SetAgentAreaMask(agent, indoor);
    agent.enabled = true;
    if ((UnityEngine.Object) rigidbody != (UnityEngine.Object) null)
    {
      rigidbodyWasGravity = rigidbody.useGravity;
      rigidbody.useGravity = false;
    }
    mode = ModeEnum.Transform;
    if (!((UnityEngine.Object) weaponService != (UnityEngine.Object) null))
      return;
    weaponService.Weapon = WeaponEnum.Unknown;
  }

  public void Activate(Quaternion rotation)
  {
    this.rotation = rotation;
    done = false;
    mode = ModeEnum.Quaternion;
    if (!((UnityEngine.Object) weaponService != (UnityEngine.Object) null))
      return;
    weaponService.Weapon = WeaponEnum.Unknown;
  }

  public void Shutdown()
  {
    if (failed)
      return;
    weaponService.Weapon = npcState.Weapon;
    if (!(bool) (UnityEngine.Object) rigidbody)
      return;
    rigidbody.useGravity = rigidbodyWasGravity;
  }

  public void OnAnimatorMove()
  {
    if (failed)
      return;
    behavior?.OnExternalAnimatorMove();
  }

  public void OnAnimatorEventEvent(string obj)
  {
    if (!failed)
      ;
  }

  public void Update()
  {
    if (failed || InstanceByRequest<EngineApplication>.Instance.IsPaused || done)
      return;
    Vector3 direction;
    if (mode == ModeEnum.Transform)
    {
      if ((UnityEngine.Object) target == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "null target");
        done = true;
        return;
      }
      direction = target.position - GameObject.transform.position;
    }
    else
    {
      if (mode != ModeEnum.Quaternion)
        throw new NotSupportedException();
      direction = rotation * Vector3.forward;
    }
    if ((UnityEngine.Object) behavior == (UnityEngine.Object) null)
      return;
    done = behavior.Rotate(direction);
  }

  public void OnLodStateChanged(bool enabled)
  {
  }

  private enum ModeEnum
  {
    Unknown,
    Transform,
    Quaternion,
  }
}
