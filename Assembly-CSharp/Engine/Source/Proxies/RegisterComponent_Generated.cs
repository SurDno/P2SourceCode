// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.RegisterComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RegisterComponent))]
  public class RegisterComponent_Generated : 
    RegisterComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RegisterComponent_Generated instance = Activator.CreateInstance<RegisterComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((RegisterComponent) target2).tag = this.tag;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Tag", this.tag);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.tag = DefaultDataReadUtility.Read(reader, "Tag", this.tag);
    }
  }
}
