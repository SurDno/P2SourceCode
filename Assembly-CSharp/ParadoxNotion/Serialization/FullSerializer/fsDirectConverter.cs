using System;

namespace ParadoxNotion.Serialization.FullSerializer;

public abstract class fsDirectConverter : fsBaseConverter {
	public abstract Type ModelType { get; }
}