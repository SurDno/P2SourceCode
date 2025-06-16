// Decompiled with JetBrains decompiler
// Type: StateSetters.StateSetterAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using System;

#nullable disable
namespace StateSetters
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class StateSetterAttribute : TypeAttribute
  {
    private string id;

    public StateSetterAttribute(string id) => this.id = id;

    public override void ComputeType(Type type)
    {
      StateSetterService.Register(this.id, (IStateSetterItemController) Activator.CreateInstance(type));
    }
  }
}
