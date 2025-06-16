using System.Collections.Generic;
using Engine.Common.Commons;

namespace Engine.Source.Services;

public class CombatServiceCombatFractionInfo {
	private FractionEnum fraction;
	public List<CombatServiceCharacterInfo> Characters;
	private List<CombatServiceCharacterInfo> personalFractionEnemies = new();
	public List<FractionEnum> AttackFractions = new();
	public List<FractionEnum> FearFractions = new();
	private List<FractionEnum> FearOnAttackFractions = new();
	public List<FractionEnum> HelpInCombatFractions = new();
	public List<FractionEnum> AskForHelpFractions = new();

	public FractionEnum Fraction => fraction;

	public List<CombatServiceCharacterInfo> PersonalFractionEnemies => personalFractionEnemies;

	public CombatServiceCombatFractionInfo(FractionEnum fraction) {
		this.fraction = fraction;
		var fractionSettings =
			ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions.Find(x => x.Name == fraction);
		if (fractionSettings == null)
			return;
		var fractionRelationGroup1 =
			fractionSettings.Relations.Find(x => x.Relation == FractionRelationEnum.AttackOnSee);
		if (fractionRelationGroup1 != null)
			AttackFractions.AddRange(fractionRelationGroup1.Fractions);
		var fractionRelationGroup2 = fractionSettings.Relations.Find(x => x.Relation == FractionRelationEnum.FearOnSee);
		if (fractionRelationGroup2 != null)
			FearFractions.AddRange(fractionRelationGroup2.Fractions);
		var fractionRelationGroup3 =
			fractionSettings.Relations.Find(x => x.Relation == FractionRelationEnum.FearOnAttack);
		if (fractionRelationGroup3 != null)
			FearOnAttackFractions.AddRange(fractionRelationGroup3.Fractions);
		var fractionRelationGroup4 =
			fractionSettings.Relations.Find(x => x.Relation == FractionRelationEnum.HelpInCombat);
		if (fractionRelationGroup4 != null)
			HelpInCombatFractions.AddRange(fractionRelationGroup4.Fractions);
		foreach (var fraction1 in ScriptableObjectInstance<FractionsSettingsData>.Instance.Fractions)
			if (fraction1.Relations.Exists(x =>
				    x.Relation == FractionRelationEnum.HelpInCombat && x.Fractions.Contains(fraction)))
				AskForHelpFractions.Add(fraction1.Name);
	}

	public bool ChangeOnHostileAction(
		CombatServiceCharacterInfo enemy,
		CombatServiceCharacterInfo reporter) {
		if (enemy == null || reporter == null || enemy.IsCombatIgnored || (enemy.IsImmortal && !enemy.IsPlayer) ||
		    reporter.IsCombatIgnored || enemy == reporter)
			return false;
		if (!HelpInCombatFractions.Contains(reporter.Fraction)) {
			if (!Characters.Contains(reporter) || reporter.PersonalFearEnemies.Contains(enemy) ||
			    reporter.PersonalAttackEnemies.Contains(enemy))
				return false;
			if (FearOnAttackFractions.Contains(enemy.Fraction)) {
				reporter.PersonalFearEnemies.Add(enemy);
				reporter.RecountEnemies();
				return true;
			}

			reporter.PersonalAttackEnemies.Add(enemy);
			return true;
		}

		if (Fraction == enemy.Fraction || FearFractions.Contains(enemy.Fraction) ||
		    AttackFractions.Contains(enemy.Fraction))
			return false;
		if (FearOnAttackFractions.Contains(enemy.Fraction)) {
			FearFractions.Add(enemy.Fraction);
			return true;
		}

		AttackFractions.Add(enemy.Fraction);
		return true;
	}
}