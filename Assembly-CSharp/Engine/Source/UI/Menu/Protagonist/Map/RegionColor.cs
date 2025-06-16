using System;
using UnityEngine;

namespace Engine.Source.UI.Menu.Protagonist.Map;

[Serializable]
public class RegionColor {
	public int Level;
	public Color FarColor = Color.white;
	public Color NearColor = Color.white;
}