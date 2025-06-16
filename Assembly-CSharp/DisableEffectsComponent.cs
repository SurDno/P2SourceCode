// Decompiled with JetBrains decompiler
// Type: DisableEffectsComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
internal class DisableEffectsComponent : MonoBehaviour, IEntityAttachable
{
  public void Attach(IEntity owner)
  {
    EffectsComponent component = owner.GetComponent<EffectsComponent>();
    if (component == null)
      return;
    component.Disabled = true;
  }

  public void Detach()
  {
  }
}
