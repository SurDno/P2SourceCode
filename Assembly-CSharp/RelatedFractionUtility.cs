// Decompiled with JetBrains decompiler
// Type: RelatedFractionUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using System.Collections.Generic;

#nullable disable
public static class RelatedFractionUtility
{
  public static List<FractionEnum> GetFraction(IEntity entity, FractionRelationEnum relation)
  {
    if (entity != null)
    {
      ParametersComponent component = entity.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<FractionEnum> byName = component.GetByName<FractionEnum>(ParameterNameEnum.Fraction);
        if (byName != null)
        {
          FractionSettings fractionSettings = RelatedFractionUtility.GetFractionSettings(byName);
          if (fractionSettings != null && fractionSettings.Relations != null)
          {
            FractionRelationGroup fractionRelationGroup = RelatedFractionUtility.GetFractionRelationGroup(relation, fractionSettings);
            if (fractionRelationGroup != null)
              return fractionRelationGroup.Fractions;
          }
        }
      }
    }
    return (List<FractionEnum>) null;
  }

  private static FractionRelationGroup GetFractionRelationGroup(
    FractionRelationEnum relation,
    FractionSettings fractionSettings)
  {
    for (int index = 0; index < fractionSettings.Relations.Count; ++index)
    {
      FractionRelationGroup relation1 = fractionSettings.Relations[index];
      if (relation1.Relation == relation)
        return relation1;
    }
    return (FractionRelationGroup) null;
  }

  private static FractionSettings GetFractionSettings(IParameter<FractionEnum> fraction)
  {
    for (int index = 0; index < ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Count; ++index)
    {
      FractionSettings fraction1 = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions[index];
      if (fraction1.Name == fraction.Value)
        return fraction1;
    }
    return (FractionSettings) null;
  }
}
