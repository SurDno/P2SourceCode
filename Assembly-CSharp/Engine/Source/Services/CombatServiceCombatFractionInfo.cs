// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.CombatServiceCombatFractionInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Services
{
  public class CombatServiceCombatFractionInfo
  {
    private FractionEnum fraction;
    public List<CombatServiceCharacterInfo> Characters;
    private List<CombatServiceCharacterInfo> personalFractionEnemies = new List<CombatServiceCharacterInfo>();
    public List<FractionEnum> AttackFractions = new List<FractionEnum>();
    public List<FractionEnum> FearFractions = new List<FractionEnum>();
    private List<FractionEnum> FearOnAttackFractions = new List<FractionEnum>();
    public List<FractionEnum> HelpInCombatFractions = new List<FractionEnum>();
    public List<FractionEnum> AskForHelpFractions = new List<FractionEnum>();

    public FractionEnum Fraction => this.fraction;

    public List<CombatServiceCharacterInfo> PersonalFractionEnemies => this.personalFractionEnemies;

    public CombatServiceCombatFractionInfo(FractionEnum fraction)
    {
      this.fraction = fraction;
      FractionSettings fractionSettings = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Find((Predicate<FractionSettings>) (x => x.Name == fraction));
      if (fractionSettings == null)
        return;
      FractionRelationGroup fractionRelationGroup1 = fractionSettings.Relations.Find((Predicate<FractionRelationGroup>) (x => x.Relation == FractionRelationEnum.AttackOnSee));
      if (fractionRelationGroup1 != null)
        this.AttackFractions.AddRange((IEnumerable<FractionEnum>) fractionRelationGroup1.Fractions);
      FractionRelationGroup fractionRelationGroup2 = fractionSettings.Relations.Find((Predicate<FractionRelationGroup>) (x => x.Relation == FractionRelationEnum.FearOnSee));
      if (fractionRelationGroup2 != null)
        this.FearFractions.AddRange((IEnumerable<FractionEnum>) fractionRelationGroup2.Fractions);
      FractionRelationGroup fractionRelationGroup3 = fractionSettings.Relations.Find((Predicate<FractionRelationGroup>) (x => x.Relation == FractionRelationEnum.FearOnAttack));
      if (fractionRelationGroup3 != null)
        this.FearOnAttackFractions.AddRange((IEnumerable<FractionEnum>) fractionRelationGroup3.Fractions);
      FractionRelationGroup fractionRelationGroup4 = fractionSettings.Relations.Find((Predicate<FractionRelationGroup>) (x => x.Relation == FractionRelationEnum.HelpInCombat));
      if (fractionRelationGroup4 != null)
        this.HelpInCombatFractions.AddRange((IEnumerable<FractionEnum>) fractionRelationGroup4.Fractions);
      foreach (FractionSettings fraction1 in ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions)
      {
        if (fraction1.Relations.Exists((Predicate<FractionRelationGroup>) (x => x.Relation == FractionRelationEnum.HelpInCombat && x.Fractions.Contains(fraction))))
          this.AskForHelpFractions.Add(fraction1.Name);
      }
    }

    public bool ChangeOnHostileAction(
      CombatServiceCharacterInfo enemy,
      CombatServiceCharacterInfo reporter)
    {
      if (enemy == null || reporter == null || enemy.IsCombatIgnored || enemy.IsImmortal && !enemy.IsPlayer || reporter.IsCombatIgnored || enemy == reporter)
        return false;
      if (!this.HelpInCombatFractions.Contains(reporter.Fraction))
      {
        if (!this.Characters.Contains(reporter) || reporter.PersonalFearEnemies.Contains(enemy) || reporter.PersonalAttackEnemies.Contains(enemy))
          return false;
        if (this.FearOnAttackFractions.Contains(enemy.Fraction))
        {
          reporter.PersonalFearEnemies.Add(enemy);
          reporter.RecountEnemies();
          return true;
        }
        reporter.PersonalAttackEnemies.Add(enemy);
        return true;
      }
      if (this.Fraction == enemy.Fraction || this.FearFractions.Contains(enemy.Fraction) || this.AttackFractions.Contains(enemy.Fraction))
        return false;
      if (this.FearOnAttackFractions.Contains(enemy.Fraction))
      {
        this.FearFractions.Add(enemy.Fraction);
        return true;
      }
      this.AttackFractions.Add(enemy.Fraction);
      return true;
    }
  }
}
