using Engine.Common.Services;
using Engine.Source.Commons;
using Inspectors;

public class TumbaLightTrigger : MonoBehaviour
{
  [Inspected]
  private bool enabledLights;

  private GameObject GetPlayerGameObject()
  {
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }

  private void OnEnable() => EnabledLights(false);

  private void OnDisable()
  {
    enabledLights = false;
    EnabledLights(false);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (enabledLights)
      return;
    AudioSource component = this.GetComponent<AudioSource>();
    if ((Object) component != (Object) null)
      component.PlayAndCheck();
    else
      Debug.LogWarning((object) "No audio source", (Object) this.gameObject);
    GameObject playerGameObject = GetPlayerGameObject();
    if ((Object) playerGameObject == (Object) null || !((Object) other.gameObject == (Object) playerGameObject))
      return;
    enabledLights = true;
    EnabledLights(true);
  }

  private void EnabledLights(bool enable)
  {
    foreach (Behaviour componentsInChild in this.gameObject.GetComponentsInChildren<Light>())
      componentsInChild.enabled = enable;
  }
}
