// Decompiled with JetBrains decompiler
// Type: TOD_RangeAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class TOD_RangeAttribute : PropertyAttribute
{
  public float min;
  public float max;

  public TOD_RangeAttribute(float min, float max)
  {
    this.min = min;
    this.max = max;
  }
}
