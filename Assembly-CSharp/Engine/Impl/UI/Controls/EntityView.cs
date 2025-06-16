using Engine.Common;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

public abstract class EntityView : MonoBehaviour {
	public abstract IEntity Value { get; set; }

	public virtual void SkipAnimation() { }
}