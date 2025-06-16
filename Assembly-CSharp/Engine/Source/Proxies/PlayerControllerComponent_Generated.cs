// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.PlayerControllerComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Reputations;
using System;

#nullable disable
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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      PlayerControllerComponent_Generated componentGenerated = (PlayerControllerComponent_Generated) target2;
      CloneableObjectUtility.CopyListTo<ReputationInfo>(componentGenerated.reputations, this.reputations);
      componentGenerated.thresholdNearRegionsPositive = this.thresholdNearRegionsPositive;
      componentGenerated.thresholdNearRegionsNegative = this.thresholdNearRegionsNegative;
      componentGenerated.coefficientNearRegionsPositive = this.coefficientNearRegionsPositive;
      componentGenerated.coefficientNearRegionsNegative = this.coefficientNearRegionsNegative;
      componentGenerated.thresholdPlayerInfected = this.thresholdPlayerInfected;
      componentGenerated.thresholdRegionInfected = this.thresholdRegionInfected;
      CloneableObjectUtility.FillListTo<FractionEnum>(componentGenerated.dangerousFractions, this.dangerousFractions);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<ReputationInfo>(writer, "Reputations", this.reputations);
      DefaultDataWriteUtility.Write(writer, "ThresholdNearRegionsPositive", this.thresholdNearRegionsPositive);
      DefaultDataWriteUtility.Write(writer, "ThresholdNearRegionsNegative", this.thresholdNearRegionsNegative);
      DefaultDataWriteUtility.Write(writer, "CoefficientNearRegionsPositive", this.coefficientNearRegionsPositive);
      DefaultDataWriteUtility.Write(writer, "CoefficientNearRegionsNegative", this.coefficientNearRegionsNegative);
      DefaultDataWriteUtility.Write(writer, "ThresholdPlayerInfected", this.thresholdPlayerInfected);
      DefaultDataWriteUtility.Write(writer, "ThresholdRegionInfected", this.thresholdRegionInfected);
      DefaultDataWriteUtility.WriteListEnum<FractionEnum>(writer, "DangerousFractions", this.dangerousFractions);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.reputations = DefaultDataReadUtility.ReadListSerialize<ReputationInfo>(reader, "Reputations", this.reputations);
      this.thresholdNearRegionsPositive = DefaultDataReadUtility.Read(reader, "ThresholdNearRegionsPositive", this.thresholdNearRegionsPositive);
      this.thresholdNearRegionsNegative = DefaultDataReadUtility.Read(reader, "ThresholdNearRegionsNegative", this.thresholdNearRegionsNegative);
      this.coefficientNearRegionsPositive = DefaultDataReadUtility.Read(reader, "CoefficientNearRegionsPositive", this.coefficientNearRegionsPositive);
      this.coefficientNearRegionsNegative = DefaultDataReadUtility.Read(reader, "CoefficientNearRegionsNegative", this.coefficientNearRegionsNegative);
      this.thresholdPlayerInfected = DefaultDataReadUtility.Read(reader, "ThresholdPlayerInfected", this.thresholdPlayerInfected);
      this.thresholdRegionInfected = DefaultDataReadUtility.Read(reader, "ThresholdRegionInfected", this.thresholdRegionInfected);
      this.dangerousFractions = DefaultDataReadUtility.ReadListEnum<FractionEnum>(reader, "DangerousFractions", this.dangerousFractions);
    }
  }
}
