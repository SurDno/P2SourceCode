// Decompiled with JetBrains decompiler
// Type: Inspectors.SliderAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Inspectors
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class SliderAttribute : Attribute
  {
    public float Min { get; set; } = 0.0f;

    public float Max { get; set; } = 1f;
  }
}
