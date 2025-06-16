// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.PreInfectionTrialAbilityController_Generated
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
  [FactoryProxy(typeof (PreInfectionTrialAbilityController))]
  public class PreInfectionTrialAbilityController_Generated : 
    PreInfectionTrialAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PreInfectionTrialAbilityController_Generated instance = Activator.CreateInstance<PreInfectionTrialAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      PreInfectionTrialAbilityController_Generated controllerGenerated = (PreInfectionTrialAbilityController_Generated) target2;
      controllerGenerated.thresholdValue = this.thresholdValue;
      controllerGenerated.breakCoeficient1 = this.breakCoeficient1;
      controllerGenerated.breakCoeficient2 = this.breakCoeficient2;
      controllerGenerated.preinfectionCheckInterval = this.preinfectionCheckInterval;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "ThresholdValue", this.thresholdValue);
      DefaultDataWriteUtility.Write(writer, "BeakCoeficient1", this.breakCoeficient1);
      DefaultDataWriteUtility.Write(writer, "BeakCoeficient2", this.breakCoeficient2);
      DefaultDataWriteUtility.Write(writer, "PreinfectionCheckInterval", this.preinfectionCheckInterval);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.thresholdValue = DefaultDataReadUtility.Read(reader, "ThresholdValue", this.thresholdValue);
      this.breakCoeficient1 = DefaultDataReadUtility.Read(reader, "BeakCoeficient1", this.breakCoeficient1);
      this.breakCoeficient2 = DefaultDataReadUtility.Read(reader, "BeakCoeficient2", this.breakCoeficient2);
      this.preinfectionCheckInterval = DefaultDataReadUtility.Read(reader, "PreinfectionCheckInterval", this.preinfectionCheckInterval);
    }
  }
}
