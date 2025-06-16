// Decompiled with JetBrains decompiler
// Type: OpenByDynamic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
