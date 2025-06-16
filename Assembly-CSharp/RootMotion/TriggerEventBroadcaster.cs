namespace RootMotion
{
  public class TriggerEventBroadcaster : MonoBehaviour
  {
    public GameObject target;

    private void OnTriggerEnter(Collider collider)
    {
      if (!((Object) target != (Object) null))
        return;
      target.SendMessage(nameof (OnTriggerEnter), (object) collider, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerStay(Collider collider)
    {
      if (!((Object) target != (Object) null))
        return;
      target.SendMessage(nameof (OnTriggerStay), (object) collider, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerExit(Collider collider)
    {
      if (!((Object) target != (Object) null))
        return;
      target.SendMessage(nameof (OnTriggerExit), (object) collider, SendMessageOptions.DontRequireReceiver);
    }
  }
}
