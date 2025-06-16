// Decompiled with JetBrains decompiler
// Type: EnumFlagAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class EnumFlagAttribute : PropertyAttribute
{
  public System.Type EnumType { get; set; }

  public EnumFlagAttribute(System.Type enumType) => this.EnumType = enumType;
}
