using System;
using Engine.Source.Services.Inputs;
using UnityEngine;

namespace Engine.Impl.UI.Controls;

[Serializable]
public class Icon {
	public GameActionType iconType;
	public Sprite iconSprite;
}