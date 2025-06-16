// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.CreateBombExplosionEffect_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (CreateBombExplosionEffect))]
  public class CreateBombExplosionEffect_Generated : 
    CreateBombExplosionEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      CreateBombExplosionEffect_Generated instance = Activator.CreateInstance<CreateBombExplosionEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CreateBombExplosionEffect_Generated explosionEffectGenerated = (CreateBombExplosionEffect_Generated) target2;
      explosionEffectGenerated.queue = this.queue;
      explosionEffectGenerated.template = this.template;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      UnityDataWriteUtility.Write<IEntity>(writer, "Template", this.template);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.template = UnityDataReadUtility.Read<IEntity>(reader, "Template", this.template);
    }
  }
}
