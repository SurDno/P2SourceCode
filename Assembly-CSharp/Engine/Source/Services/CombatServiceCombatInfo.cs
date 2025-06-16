using System.Collections.Generic;

namespace Engine.Source.Services;

public class CombatServiceCombatInfo {
	private static int lastId;
	public List<CombatServiceCharacterInfo> Characters;
	public List<CombatServiceCombatFractionInfo> Fractions;
	public CombatServiceCharacterStateEnum State;
	public List<CombatServiceEscapedCharacterInfo> EscapedCharacters;
	public int Id;

	public CombatServiceCombatInfo() {
		Id = lastId++;
	}
}