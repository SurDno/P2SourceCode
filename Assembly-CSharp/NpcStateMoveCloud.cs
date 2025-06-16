// Decompiled with JetBrains decompiler
// Type: NpcStateMoveCloud
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Components;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class NpcStateMoveCloud : INpcState
{
  private Pivot pivot;
  private NpcState npcState;
  [Inspected]
  private NpcStateMoveCloud.StateEnum state = NpcStateMoveCloud.StateEnum.Moving;
  [Inspected]
  private NavMeshAgent agent;
  [Inspected]
  private Vector3 destination;
  private int prevAreaMask;
  private bool agentWasEnabled;
  private float speed = 2f;

  public GameObject GameObject { get; private set; }

  [Inspected]
  public NpcStateStatusEnum Status { get; private set; }

  public NpcStateMoveCloud(NpcState npcState, Pivot pivot)
  {
    this.npcState = npcState;
    this.pivot = pivot;
    this.GameObject = npcState.gameObject;
  }

  public void Activate(Vector3 destination)
  {
    this.Status = NpcStateStatusEnum.Running;
    this.agent = this.pivot.GetAgent();
    if ((Object) this.agent != (Object) null)
    {
      this.speed = this.agent.speed;
      this.agentWasEnabled = this.agent.enabled;
      this.agent.enabled = false;
    }
    this.destination = destination;
    this.state = NpcStateMoveCloud.StateEnum.Moving;
  }

  public void Shutdown()
  {
    if (!((Object) this.agent != (Object) null))
      return;
    this.agent.enabled = this.agentWasEnabled;
  }

  public void OnAnimatorMove()
  {
  }

  public void OnAnimatorEventEvent(string obj)
  {
  }

  public void Update()
  {
    if (InstanceByRequest<EngineApplication>.Instance.IsPaused)
      return;
    if (this.state == NpcStateMoveCloud.StateEnum.Moving)
      this.OnUpdateMove();
    if (this.state != NpcStateMoveCloud.StateEnum.Done)
      return;
    this.Status = NpcStateStatusEnum.Success;
  }

  public void OnUpdateMove()
  {
    Vector3 vector3 = this.destination - this.GameObject.transform.position;
    float magnitude = vector3.magnitude;
    float num = 0.2f;
    this.GameObject.transform.position += vector3.normalized * this.speed * Time.deltaTime;
    if ((double) magnitude >= (double) num)
      return;
    this.state = NpcStateMoveCloud.StateEnum.Done;
  }

  public void OnLodStateChanged(bool enabled)
  {
  }

  private enum StateEnum
  {
    Moving,
    Done,
  }
}
