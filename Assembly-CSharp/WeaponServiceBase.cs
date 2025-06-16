// Decompiled with JetBrains decompiler
// Type: WeaponServiceBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WeaponServiceBase : MonoBehaviour
{
  public virtual WeaponEnum Weapon { get; set; }

  public virtual Vector3 KnifeSpeed { get; }

  public virtual Vector3 KnifePosition { get; }
}
