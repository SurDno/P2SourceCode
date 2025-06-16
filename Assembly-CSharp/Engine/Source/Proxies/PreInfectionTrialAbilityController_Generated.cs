using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Abilities.Controllers;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      PreInfectionTrialAbilityController_Generated controllerGenerated = (PreInfectionTrialAbilityController_Generated) target2;
      controllerGenerated.thresholdValue = thresholdValue;
      controllerGenerated.breakCoeficient1 = breakCoeficient1;
      controllerGenerated.breakCoeficient2 = breakCoeficient2;
      controllerGenerated.preinfectionCheckInterval = preinfectionCheckInterval;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "ThresholdValue", thresholdValue);
      DefaultDataWriteUtility.Write(writer, "BeakCoeficient1", breakCoeficient1);
      DefaultDataWriteUtility.Write(writer, "BeakCoeficient2", breakCoeficient2);
      DefaultDataWriteUtility.Write(writer, "PreinfectionCheckInterval", preinfectionCheckInterval);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      thresholdValue = DefaultDataReadUtility.Read(reader, "ThresholdValue", thresholdValue);
      breakCoeficient1 = DefaultDataReadUtility.Read(reader, "BeakCoeficient1", breakCoeficient1);
      breakCoeficient2 = DefaultDataReadUtility.Read(reader, "BeakCoeficient2", breakCoeficient2);
      preinfectionCheckInterval = DefaultDataReadUtility.Read(reader, "PreinfectionCheckInterval", preinfectionCheckInterval);
    }
  }
}
