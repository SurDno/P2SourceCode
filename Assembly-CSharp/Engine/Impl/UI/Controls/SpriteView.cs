using UnityEngine;

namespace Engine.Impl.UI.Controls;

public abstract class SpriteView : MonoBehaviour {
	public abstract void SetValue(Sprite value, bool instant);

	public abstract Sprite GetValue();
}