// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.SuokCircleAbilityController_Generated
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
  [FactoryProxy(typeof (SuokCircleAbilityController))]
  public class SuokCircleAbilityController_Generated : 
    SuokCircleAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      SuokCircleAbilityController_Generated instance = Activator.CreateInstance<SuokCircleAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((SuokCircleAbilityController) target2).state = this.state;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<SuokCircleTutorialStateEnum>(writer, "State", this.state);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.state = DefaultDataReadUtility.ReadEnum<SuokCircleTutorialStateEnum>(reader, "State");
    }
  }
}
