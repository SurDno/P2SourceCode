using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PlayerControllerComponent))]
  public class PlayerControllerComponent_Generated : 
    PlayerControllerComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PlayerControllerComponent_Generated instance = Activator.CreateInstance<PlayerControllerComponent_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      PlayerControllerComponent_Generated componentGenerated = (PlayerControllerComponent_Generated) target2;
      CloneableObjectUtility.CopyListTo(componentGenerated.reputations, reputations);
      componentGenerated.thresholdNearRegionsPositive = thresholdNearRegionsPositive;
      componentGenerated.thresholdNearRegionsNegative = thresholdNearRegionsNegative;
      componentGenerated.coefficientNearRegionsPositive = coefficientNearRegionsPositive;
      componentGenerated.coefficientNearRegionsNegative = coefficientNearRegionsNegative;
      componentGenerated.thresholdPlayerInfected = thresholdPlayerInfected;
      componentGenerated.thresholdRegionInfected = thresholdRegionInfected;
      CloneableObjectUtility.FillListTo(componentGenerated.dangerousFractions, dangerousFractions);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize(writer, "Reputations", reputations);
      DefaultDataWriteUtility.Write(writer, "ThresholdNearRegionsPositive", thresholdNearRegionsPositive);
      DefaultDataWriteUtility.Write(writer, "ThresholdNearRegionsNegative", thresholdNearRegionsNegative);
      DefaultDataWriteUtility.Write(writer, "CoefficientNearRegionsPositive", coefficientNearRegionsPositive);
      DefaultDataWriteUtility.Write(writer, "CoefficientNearRegionsNegative", coefficientNearRegionsNegative);
      DefaultDataWriteUtility.Write(writer, "ThresholdPlayerInfected", thresholdPlayerInfected);
      DefaultDataWriteUtility.Write(writer, "ThresholdRegionInfected", thresholdRegionInfected);
      DefaultDataWriteUtility.WriteListEnum(writer, "DangerousFractions", dangerousFractions);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      reputations = DefaultDataReadUtility.ReadListSerialize(reader, "Reputations", reputations);
      thresholdNearRegionsPositive = DefaultDataReadUtility.Read(reader, "ThresholdNearRegionsPositive", thresholdNearRegionsPositive);
      thresholdNearRegionsNegative = DefaultDataReadUtility.Read(reader, "ThresholdNearRegionsNegative", thresholdNearRegionsNegative);
      coefficientNearRegionsPositive = DefaultDataReadUtility.Read(reader, "CoefficientNearRegionsPositive", coefficientNearRegionsPositive);
      coefficientNearRegionsNegative = DefaultDataReadUtility.Read(reader, "CoefficientNearRegionsNegative", coefficientNearRegionsNegative);
      thresholdPlayerInfected = DefaultDataReadUtility.Read(reader, "ThresholdPlayerInfected", thresholdPlayerInfected);
      thresholdRegionInfected = DefaultDataReadUtility.Read(reader, "ThresholdRegionInfected", thresholdRegionInfected);
      dangerousFractions = DefaultDataReadUtility.ReadListEnum(reader, "DangerousFractions", dangerousFractions);
    }
  }
}
