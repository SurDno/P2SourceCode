// Decompiled with JetBrains decompiler
// Type: Engine.Common.Commons.GroupAttribute
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;

#nullable disable
namespace Engine.Common.Commons
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class GroupAttribute : Attribute
  {
    public string Value { get; private set; }

    public GroupAttribute(string value) => this.Value = value;
  }
}
