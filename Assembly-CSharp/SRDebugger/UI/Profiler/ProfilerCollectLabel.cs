using System;
using SRF;
using UnityEngine;
using UnityEngine.UI;

namespace SRDebugger.UI.Profiler;

public class ProfilerCollectLabel : SRMonoBehaviour {
	[SerializeField] private Text _text;
	private float updateFrequency = 1f;
	private float nextUpdate;

	private void Update() {
		if (Time.realtimeSinceStartup <= (double)nextUpdate)
			return;
		Refresh();
	}

	private void Refresh() {
		nextUpdate = Time.realtimeSinceStartup + updateFrequency;
		_text.text = "Collect : " + GC.CollectionCount(0);
	}
}