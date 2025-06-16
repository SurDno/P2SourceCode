// Decompiled with JetBrains decompiler
// Type: DontLoad
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

#nullable disable
public class DontLoad : MonoBehaviour, IEntityAttachable
{
  [Inspected]
  private StaticModelComponent model;

  public void Attach(IEntity owner)
  {
    this.model = owner.GetComponent<StaticModelComponent>();
    if (this.model == null)
      return;
    this.model.NeedLoad = false;
  }

  public void Detach() => this.model = (StaticModelComponent) null;
}
