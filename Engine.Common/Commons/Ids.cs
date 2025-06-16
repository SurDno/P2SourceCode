using System;

namespace Engine.Common.Commons;

public static class Ids {
	public static readonly Guid MainId = new("8eb1fde2-b043-2604-2a9b-3e007cfefb62");
	public static readonly Guid PathologicId = new("f07a62d3-6c61-4e82-b5be-713051f73f3f");
	public static readonly Guid HierarchyId = new("00000000-0000-0000-0000-000000000001");
	public static readonly Guid StorablesId = new("C4165512-B881-46B8-8F29-C5434FED2D77");
	public static readonly Guid ObjectsId = new("0299F8B5-5531-4290-8196-CEC3432212CC");
	public static readonly Guid OthersId = new("{85E1C900-E337-4F77-837B-9B071CAEADEE}");

	public static bool IsRoot(Guid id) {
		return id == HierarchyId || id == StorablesId || id == ObjectsId || id == OthersId;
	}
}