// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.DefaultAbilityController_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DefaultAbilityController))]
  public class DefaultAbilityController_Generated : 
    DefaultAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DefaultAbilityController_Generated instance = Activator.CreateInstance<DefaultAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((DefaultAbilityController) target2).active = this.active;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Active", this.active);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.active = DefaultDataReadUtility.Read(reader, "Active", this.active);
    }
  }
}
