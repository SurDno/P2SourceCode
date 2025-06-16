// Decompiled with JetBrains decompiler
// Type: RootMotion.LargeHeader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace RootMotion
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class LargeHeader : PropertyAttribute
  {
    public string name;
    public string color = "white";

    public LargeHeader(string name)
    {
      this.name = name;
      this.color = "white";
    }

    public LargeHeader(string name, string color)
    {
      this.name = name;
      this.color = color;
    }
  }
}
