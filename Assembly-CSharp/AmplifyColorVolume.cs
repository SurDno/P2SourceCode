﻿using UnityEngine;

[RequireComponent(typeof (BoxCollider))]
[AddComponentMenu("Image Effects/Amplify Color Volume")]
public class AmplifyColorVolume : AmplifyColorVolumeBase
{
  private void OnTriggerEnter(Collider other)
  {
    AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
    if (!(component != null) || !component.OwnerEffect.UseVolumes || (component.OwnerEffect.VolumeCollisionMask & 1 << gameObject.layer) == 0)
      return;
    component.OwnerEffect.EnterVolume(this);
  }

  private void OnTriggerExit(Collider other)
  {
    AmplifyColorTriggerProxy component = other.GetComponent<AmplifyColorTriggerProxy>();
    if (!(component != null) || !component.OwnerEffect.UseVolumes || (component.OwnerEffect.VolumeCollisionMask & 1 << gameObject.layer) == 0)
      return;
    component.OwnerEffect.ExitVolume(this);
  }
}
