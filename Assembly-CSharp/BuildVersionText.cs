using Engine.Source.Commons;
using UnityEngine;
using UnityEngine.UI;

public class BuildVersionText : MonoBehaviour {
	private void Start() {
		InstanceByRequest<LabelService>.Instance.OnInvalidate += OnInvalidate;
		OnInvalidate();
	}

	private void OnInvalidate() {
		GetComponent<Text>().text = InstanceByRequest<LabelService>.Instance.Label;
	}
}