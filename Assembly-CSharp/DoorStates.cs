using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorStates : MonoBehaviour, IEntityAttachable
{
  [SerializeField]
  private GameObject marked;
  [SerializeField]
  [FormerlySerializedAs("blocked")]
  private GameObject bolted;
  [SerializeField]
  private GameObject navigation;
  [Inspected]
  private DoorComponent gate;

  public void Attach(IEntity owner)
  {
    this.gate = owner.GetComponent<DoorComponent>();
    if (this.gate == null)
      return;
    this.gate.OnInvalidate += new Action<IDoorComponent>(this.OnInvalidate);
    this.OnInvalidate((IDoorComponent) this.gate);
  }

  public void Detach()
  {
    if (this.gate == null)
      return;
    this.gate.OnInvalidate -= new Action<IDoorComponent>(this.OnInvalidate);
    this.gate = (DoorComponent) null;
  }

  private void OnInvalidate(IDoorComponent sender)
  {
    if ((UnityEngine.Object) this.marked != (UnityEngine.Object) null && this.marked.activeSelf != sender.Marked.Value)
      this.marked.SetActive(sender.Marked.Value);
    if ((UnityEngine.Object) this.bolted != (UnityEngine.Object) null && this.bolted.activeSelf != sender.Bolted.Value)
      this.bolted.SetActive(sender.Bolted.Value);
    if (!((UnityEngine.Object) this.navigation != (UnityEngine.Object) null))
      return;
    bool flag = sender.LockState.Value != 0;
    if (this.navigation.activeSelf != flag)
      this.navigation.SetActive(flag);
  }
}
