// Decompiled with JetBrains decompiler
// Type: BehaviorDesigner.Runtime.Tasks.RequiredComponentAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace BehaviorDesigner.Runtime.Tasks
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class RequiredComponentAttribute : Attribute
  {
    private readonly Type mComponentType;

    public Type ComponentType => this.mComponentType;

    public RequiredComponentAttribute(Type componentType) => this.mComponentType = componentType;
  }
}
