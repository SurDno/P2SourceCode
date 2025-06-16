// Decompiled with JetBrains decompiler
// Type: ScriptableObjectInstanceAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ScriptableObjectInstanceAttribute : Attribute
{
  public string Path { get; private set; }

  public ScriptableObjectInstanceAttribute(string path) => this.Path = path;
}
