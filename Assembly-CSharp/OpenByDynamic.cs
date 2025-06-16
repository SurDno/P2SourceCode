using Engine.Common;
using System.Collections.Generic;
using UnityEngine;

public class OpenByDynamic : MonoBehaviour
{
  private HashSet<IEntity> targets = new HashSet<IEntity>();
  [SerializeField]
  private InteriorDoor interior;

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.layer != ScriptableObjectInstance<GameSettingsData>.Instance.DynamicLayer.GetIndex())
      return;
    IEntity entity = EntityUtility.GetEntity(other.gameObject);
    if (entity == null || !this.targets.Add(entity) || !((Object) this.interior != (Object) null))
      return;
    this.interior.Invalidate(this.targets);
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.layer != ScriptableObjectInstance<GameSettingsData>.Instance.DynamicLayer.GetIndex())
      return;
    IEntity entity = EntityUtility.GetEntity(other.gameObject);
    if (entity == null || !this.targets.Remove(entity) || !((Object) this.interior != (Object) null))
      return;
    this.interior.Invalidate(this.targets);
  }
}
