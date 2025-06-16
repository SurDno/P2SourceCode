using UnityEngine;

namespace Engine.Impl.UI.Controls;

public abstract class FloatArrayView : MonoBehaviour {
	public abstract void SetValue(int index, float value);

	public abstract void GetValue(int index, out float value);

	public abstract void SkipAnimation();
}