using System.Collections.Generic;
using Engine.Common;

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
    if (entity == null || !targets.Add(entity) || !((Object) interior != (Object) null))
      return;
    interior.Invalidate(targets);
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.layer != ScriptableObjectInstance<GameSettingsData>.Instance.DynamicLayer.GetIndex())
      return;
    IEntity entity = EntityUtility.GetEntity(other.gameObject);
    if (entity == null || !targets.Remove(entity) || !((Object) interior != (Object) null))
      return;
    interior.Invalidate(targets);
  }
}
