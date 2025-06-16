using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
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
    gate = owner.GetComponent<DoorComponent>();
    if (gate == null)
      return;
    gate.OnInvalidate += OnInvalidate;
    OnInvalidate(gate);
  }

  public void Detach()
  {
    if (gate == null)
      return;
    gate.OnInvalidate -= OnInvalidate;
    gate = null;
  }

  private void OnInvalidate(IDoorComponent sender)
  {
    if (marked != null && marked.activeSelf != sender.Marked.Value)
      marked.SetActive(sender.Marked.Value);
    if (bolted != null && bolted.activeSelf != sender.Bolted.Value)
      bolted.SetActive(sender.Bolted.Value);
    if (!(navigation != null))
      return;
    bool flag = sender.LockState.Value != 0;
    if (navigation.activeSelf != flag)
      navigation.SetActive(flag);
  }
}
