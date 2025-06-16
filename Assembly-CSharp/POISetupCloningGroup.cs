using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class POISetupCloningGroup {
	[SerializeField] public POISetup Base;
	[SerializeField] public List<POISetup> Clones;
}