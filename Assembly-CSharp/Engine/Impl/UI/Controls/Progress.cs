using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls;

public class Progress : UIControl {
	[SerializeField] [FormerlySerializedAs("_Image")]
	private GameObject image;

	public Material Material { get; protected set; }

	public float Value {
		get => Material == null ? 0.0f : Material.GetFloat("_Progress");
		set {
			if (Material == null)
				return;
			Material.SetFloat("_Progress", value);
		}
	}

	public bool IsVisible {
		get => gameObject.activeSelf;
		set => gameObject.SetActive(value);
	}

	protected override void Awake() {
		base.Awake();
		if (image == null)
			return;
		var component = image.GetComponent<RawImage>();
		if (component == null)
			return;
		Material = Instantiate(component.material);
		component.material = Material;
	}
}