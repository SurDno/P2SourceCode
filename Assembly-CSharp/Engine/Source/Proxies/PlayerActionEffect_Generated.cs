// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.PlayerActionEffect_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlayerActionEffect))]
  public class PlayerActionEffect_Generated : 
    PlayerActionEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlayerActionEffect_Generated instance = Activator.CreateInstance<PlayerActionEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      PlayerActionEffect_Generated actionEffectGenerated = (PlayerActionEffect_Generated) target2;
      actionEffectGenerated.queue = this.queue;
      actionEffectGenerated.action = this.action;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.WriteEnum<ActionEnum>(writer, "Action", this.action);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.action = DefaultDataReadUtility.ReadEnum<ActionEnum>(reader, "Action");
    }
  }
}
