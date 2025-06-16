using UnityEngine;

namespace StateSetters;

public abstract class MaterialPropertyItemController : IStateSetterItemController {
	public void Apply(StateSetterItem item, float value) {
		var objectValue1 = item.ObjectValue1 as MeshRenderer;
		if (objectValue1 == null)
			return;
		var materialArray = Application.isPlaying ? objectValue1.materials : objectValue1.sharedMaterials;
		var intValue1 = item.IntValue1;
		if (intValue1 < 0 || intValue1 >= materialArray.Length)
			return;
		var intValue2 = item.IntValue2;
		if (intValue2 < 0 || intValue2 >= materialArray.Length)
			return;
		var stringValue1 = item.StringValue1;
		for (var index = intValue1; index <= intValue2; ++index) {
			var material = materialArray[index];
			SetParameter(item, material, stringValue1, value);
		}
	}

	protected abstract void SetParameter(
		StateSetterItem item,
		Material material,
		string parameter,
		float value);
}