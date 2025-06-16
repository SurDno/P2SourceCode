using UnityEngine;

namespace RootMotion
{
  public class TriggerEventBroadcaster : MonoBehaviour
  {
    public GameObject target;

    private void OnTriggerEnter(Collider collider)
    {
      if (!((Object) this.target != (Object) null))
        return;
      this.target.SendMessage(nameof (OnTriggerEnter), (object) collider, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerStay(Collider collider)
    {
      if (!((Object) this.target != (Object) null))
        return;
      this.target.SendMessage(nameof (OnTriggerStay), (object) collider, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerExit(Collider collider)
    {
      if (!((Object) this.target != (Object) null))
        return;
      this.target.SendMessage(nameof (OnTriggerExit), (object) collider, SendMessageOptions.DontRequireReceiver);
    }
  }
}
