using UnityEngine;

[RequireComponent(typeof (BoxCollider))]
[AddComponentMenu("Image Effects/Amplify Color Volume")]
public class AmplifyColorVolume : AmplifyColorVolumeBase
{
  private void OnTriggerEnter(Collider other)
  {
    AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
    if (!((Object) component != (Object) null) || !component.OwnerEffect.UseVolumes || ((int) component.OwnerEffect.VolumeCollisionMask & 1 << this.gameObject.layer) == 0)
      return;
    component.OwnerEffect.EnterVolume((AmplifyColorVolumeBase) this);
  }

  private void OnTriggerExit(Collider other)
  {
    AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
    if (!((Object) component != (Object) null) || !component.OwnerEffect.UseVolumes || ((int) component.OwnerEffect.VolumeCollisionMask & 1 << this.gameObject.layer) == 0)
      return;
    component.OwnerEffect.ExitVolume((AmplifyColorVolumeBase) this);
  }
}
