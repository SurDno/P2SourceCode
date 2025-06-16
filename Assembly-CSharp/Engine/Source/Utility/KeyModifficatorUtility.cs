namespace Engine.Source.Utility;

public static class KeyModifficatorUtility {
	public static bool HasValue(KeyModifficator source, KeyModifficator value) {
		return (source & value) != 0;
	}
}