// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.BlueprintEffect_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (BlueprintEffect))]
  public class BlueprintEffect_Generated : 
    BlueprintEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      BlueprintEffect_Generated instance = Activator.CreateInstance<BlueprintEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      BlueprintEffect_Generated blueprintEffectGenerated = (BlueprintEffect_Generated) target2;
      blueprintEffectGenerated.name = this.name;
      blueprintEffectGenerated.queue = this.queue;
      blueprintEffectGenerated.blueprint = this.blueprint;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.name);
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      UnityDataWriteUtility.Write<GameObject>(writer, "Blueprint", this.blueprint);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.name = DefaultDataReadUtility.Read(reader, "Name", this.name);
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.blueprint = UnityDataReadUtility.Read<GameObject>(reader, "Blueprint", this.blueprint);
    }
  }
}
