// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.HerbRootsTemplate_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Crowds;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (HerbRootsTemplate))]
  public class HerbRootsTemplate_Generated : 
    HerbRootsTemplate,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      HerbRootsTemplate_Generated instance = Activator.CreateInstance<HerbRootsTemplate_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      HerbRootsTemplate_Generated templateGenerated = (HerbRootsTemplate_Generated) target2;
      templateGenerated.Template = this.Template;
      templateGenerated.Weight = this.Weight;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<IEntity>(writer, "Template", this.Template);
      DefaultDataWriteUtility.Write(writer, "Weight", this.Weight);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Template = UnityDataReadUtility.Read<IEntity>(reader, "Template", this.Template);
      this.Weight = DefaultDataReadUtility.Read(reader, "Weight", this.Weight);
    }

    public void StateSave(IDataWriter writer)
    {
    }

    public void StateLoad(IDataReader reader, Type type)
    {
    }
  }
}
