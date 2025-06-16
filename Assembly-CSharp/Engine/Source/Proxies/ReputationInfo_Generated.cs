using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Reputations;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ReputationInfo))]
  public class ReputationInfo_Generated : 
    ReputationInfo,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ReputationInfo_Generated instance = Activator.CreateInstance<ReputationInfo_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      ReputationInfo_Generated reputationInfoGenerated = (ReputationInfo_Generated) target2;
      reputationInfoGenerated.Action = Action;
      CloneableObjectUtility.FillListTo(reputationInfoGenerated.Fractions, Fractions);
      reputationInfoGenerated.Visible = Visible;
      reputationInfoGenerated.Invisible = Invisible;
      reputationInfoGenerated.AffectNearRegions = AffectNearRegions;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Action", Action);
      DefaultDataWriteUtility.WriteListEnum(writer, "Fractions", Fractions);
      DefaultDataWriteUtility.Write(writer, "Visible", Visible);
      DefaultDataWriteUtility.Write(writer, "Invisible", Invisible);
      DefaultDataWriteUtility.Write(writer, "AffectNearRegions", AffectNearRegions);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      Action = DefaultDataReadUtility.ReadEnum<ActionEnum>(reader, "Action");
      Fractions = DefaultDataReadUtility.ReadListEnum(reader, "Fractions", Fractions);
      Visible = DefaultDataReadUtility.Read(reader, "Visible", Visible);
      Invisible = DefaultDataReadUtility.Read(reader, "Invisible", Invisible);
      AffectNearRegions = DefaultDataReadUtility.Read(reader, "AffectNearRegions", AffectNearRegions);
    }
  }
}
