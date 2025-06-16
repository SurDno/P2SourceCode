// Decompiled with JetBrains decompiler
// Type: CraftRecipe
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Connections;
using System;
using UnityEngine;

#nullable disable
[Serializable]
public class CraftRecipe
{
  [SerializeField]
  public IEntitySerializable Component1;
  [SerializeField]
  public IEntitySerializable Component2;
  [SerializeField]
  public IEntitySerializable Component3;
  [SerializeField]
  public IEntitySerializable Result;
  [SerializeField]
  public float SuccessChance;
  [SerializeField]
  public float Time;
}
