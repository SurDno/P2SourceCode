// Decompiled with JetBrains decompiler
// Type: TumbaLightTrigger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Audio;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

#nullable disable
public class TumbaLightTrigger : MonoBehaviour
{
  [Inspected]
  private bool enabledLights;

  private GameObject GetPlayerGameObject()
  {
    return ((IEntityView) ServiceLocator.GetService<ISimulation>().Player)?.GameObject;
  }

  private void OnEnable() => this.EnabledLights(false);

  private void OnDisable()
  {
    this.enabledLights = false;
    this.EnabledLights(false);
  }

  private void OnTriggerEnter(Collider other)
  {
    if (this.enabledLights)
      return;
    AudioSource component = this.GetComponent<AudioSource>();
    if ((Object) component != (Object) null)
      component.PlayAndCheck();
    else
      Debug.LogWarning((object) "No audio source", (Object) this.gameObject);
    GameObject playerGameObject = this.GetPlayerGameObject();
    if ((Object) playerGameObject == (Object) null || !((Object) other.gameObject == (Object) playerGameObject))
      return;
    this.enabledLights = true;
    this.EnabledLights(true);
  }

  private void EnabledLights(bool enable)
  {
    foreach (Behaviour componentsInChild in this.gameObject.GetComponentsInChildren<Light>())
      componentsInChild.enabled = enable;
  }
}
