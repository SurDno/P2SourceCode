using UnityEngine;

public class TOD_LoadSkyFromFile : MonoBehaviour {
	public TOD_Sky sky;
	public TextAsset textAsset;

	protected void Start() {
		if (!(bool)(Object)sky)
			sky = TOD_Sky.Instance;
		if (!(bool)(Object)textAsset)
			return;
		sky.LoadParameters(textAsset.text);
	}
}