using System;
using UnityEngine;

namespace Engine.Source.Services.Consoles.Binds;

public class ThrowScript : MonoBehaviour {
	private void Start() {
		throw new Exception();
	}

	private void Update() {
		if (!(gameObject != null))
			return;
		Destroy(gameObject);
	}
}