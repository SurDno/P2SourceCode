public static class Fight {
	private static FightDescription description;

	public static FightDescription Description {
		get {
			if (description == null)
				description = ScriptableObjectInstance<FightSettingsData>.Instance.Description;
			return description;
		}
	}
}