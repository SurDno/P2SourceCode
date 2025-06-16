using System;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer;

public abstract class fsDirectConverter<TModel> : fsDirectConverter {
	public override Type ModelType => typeof(TModel);

	public sealed override fsResult TrySerialize(
		object instance,
		out fsData serialized,
		Type storageType) {
		var dictionary = new Dictionary<string, fsData>();
		var fsResult = DoSerialize((TModel)instance, dictionary);
		serialized = new fsData(dictionary);
		return fsResult;
	}

	public sealed override fsResult TryDeserialize(
		fsData data,
		ref object instance,
		Type storageType) {
		fsResult fsResult1;
		if ((fsResult1 = fsResult.Success + CheckType(data, fsDataType.Object)).Failed)
			return fsResult1;
		var model = (TModel)instance;
		var fsResult2 = fsResult1 + DoDeserialize(data.AsDictionary, ref model);
		instance = model;
		return fsResult2;
	}

	protected abstract fsResult DoSerialize(TModel model, Dictionary<string, fsData> serialized);

	protected abstract fsResult DoDeserialize(Dictionary<string, fsData> data, ref TModel model);
}