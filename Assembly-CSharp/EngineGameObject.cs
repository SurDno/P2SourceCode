// Decompiled with JetBrains decompiler
// Type: EngineGameObject
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

#nullable disable
[DisallowMultipleComponent]
public class EngineGameObject : MonoBehaviour, IEntityAttachable
{
  [Inspected]
  public IEntity Owner { get; private set; }

  public void Attach(IEntity owner) => this.Owner = owner;

  public void Detach() => this.Owner = (IEntity) null;
}
