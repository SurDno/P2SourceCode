// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ReputationInfo_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Reputations;
using System;

#nullable disable
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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ReputationInfo_Generated reputationInfoGenerated = (ReputationInfo_Generated) target2;
      reputationInfoGenerated.Action = this.Action;
      CloneableObjectUtility.FillListTo<FractionEnum>(reputationInfoGenerated.Fractions, this.Fractions);
      reputationInfoGenerated.Visible = this.Visible;
      reputationInfoGenerated.Invisible = this.Invisible;
      reputationInfoGenerated.AffectNearRegions = this.AffectNearRegions;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ActionEnum>(writer, "Action", this.Action);
      DefaultDataWriteUtility.WriteListEnum<FractionEnum>(writer, "Fractions", this.Fractions);
      DefaultDataWriteUtility.Write(writer, "Visible", this.Visible);
      DefaultDataWriteUtility.Write(writer, "Invisible", this.Invisible);
      DefaultDataWriteUtility.Write(writer, "AffectNearRegions", this.AffectNearRegions);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Action = DefaultDataReadUtility.ReadEnum<ActionEnum>(reader, "Action");
      this.Fractions = DefaultDataReadUtility.ReadListEnum<FractionEnum>(reader, "Fractions", this.Fractions);
      this.Visible = DefaultDataReadUtility.Read(reader, "Visible", this.Visible);
      this.Invisible = DefaultDataReadUtility.Read(reader, "Invisible", this.Invisible);
      this.AffectNearRegions = DefaultDataReadUtility.Read(reader, "AffectNearRegions", this.AffectNearRegions);
    }
  }
}
