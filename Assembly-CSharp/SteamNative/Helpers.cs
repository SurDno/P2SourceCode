using System.Text;

namespace SteamNative;

internal static class Helpers {
	private static StringBuilder[] StringBuilderPool;
	private static int StringBuilderPoolIndex;

	public static StringBuilder TakeStringBuilder() {
		if (StringBuilderPool == null) {
			StringBuilderPool = new StringBuilder[8];
			for (var index = 0; index < StringBuilderPool.Length; ++index)
				StringBuilderPool[index] = new StringBuilder(4096);
		}

		++StringBuilderPoolIndex;
		if (StringBuilderPoolIndex >= StringBuilderPool.Length)
			StringBuilderPoolIndex = 0;
		StringBuilderPool[StringBuilderPoolIndex].Capacity = 4096;
		StringBuilderPool[StringBuilderPoolIndex].Length = 0;
		return StringBuilderPool[StringBuilderPoolIndex];
	}
}