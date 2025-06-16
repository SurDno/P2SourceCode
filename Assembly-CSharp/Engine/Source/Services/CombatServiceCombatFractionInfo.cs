using System.Collections.Generic;
using Engine.Common.Commons;

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

    public FractionEnum Fraction => fraction;

    public List<CombatServiceCharacterInfo> PersonalFractionEnemies => personalFractionEnemies;

    public CombatServiceCombatFractionInfo(FractionEnum fraction)
    {
      this.fraction = fraction;
      FractionSettings fractionSettings = ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Find(x => x.Name == fraction);
      if (fractionSettings == null)
        return;
      FractionRelationGroup fractionRelationGroup1 = fractionSettings.Relations.Find(x => x.Relation == FractionRelationEnum.AttackOnSee);
      if (fractionRelationGroup1 != null)
        AttackFractions.AddRange(fractionRelationGroup1.Fractions);
      FractionRelationGroup fractionRelationGroup2 = fractionSettings.Relations.Find(x => x.Relation == FractionRelationEnum.FearOnSee);
      if (fractionRelationGroup2 != null)
        FearFractions.AddRange(fractionRelationGroup2.Fractions);
      FractionRelationGroup fractionRelationGroup3 = fractionSettings.Relations.Find(x => x.Relation == FractionRelationEnum.FearOnAttack);
      if (fractionRelationGroup3 != null)
        FearOnAttackFractions.AddRange(fractionRelationGroup3.Fractions);
      FractionRelationGroup fractionRelationGroup4 = fractionSettings.Relations.Find(x => x.Relation == FractionRelationEnum.HelpInCombat);
      if (fractionRelationGroup4 != null)
        HelpInCombatFractions.AddRange(fractionRelationGroup4.Fractions);
      foreach (FractionSettings fraction1 in ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions)
      {
        if (fraction1.Relations.Exists(x => x.Relation == FractionRelationEnum.HelpInCombat && x.Fractions.Contains(fraction)))
          AskForHelpFractions.Add(fraction1.Name);
      }
    }

    public bool ChangeOnHostileAction(
      CombatServiceCharacterInfo enemy,
      CombatServiceCharacterInfo reporter)
    {
      if (enemy == null || reporter == null || enemy.IsCombatIgnored || enemy.IsImmortal && !enemy.IsPlayer || reporter.IsCombatIgnored || enemy == reporter)
        return false;
      if (!HelpInCombatFractions.Contains(reporter.Fraction))
      {
        if (!Characters.Contains(reporter) || reporter.PersonalFearEnemies.Contains(enemy) || reporter.PersonalAttackEnemies.Contains(enemy))
          return false;
        if (FearOnAttackFractions.Contains(enemy.Fraction))
        {
          reporter.PersonalFearEnemies.Add(enemy);
          reporter.RecountEnemies();
          return true;
        }
        reporter.PersonalAttackEnemies.Add(enemy);
        return true;
      }
      if (Fraction == enemy.Fraction || FearFractions.Contains(enemy.Fraction) || AttackFractions.Contains(enemy.Fraction))
        return false;
      if (FearOnAttackFractions.Contains(enemy.Fraction))
      {
        FearFractions.Add(enemy.Fraction);
        return true;
      }
      AttackFractions.Add(enemy.Fraction);
      return true;
    }
  }
}
