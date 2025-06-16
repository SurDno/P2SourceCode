// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.MinAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace UnityEngine.PostProcessing
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class MinAttribute : PropertyAttribute
  {
    public readonly float min;

    public MinAttribute(float min) => this.min = min;
  }
}
