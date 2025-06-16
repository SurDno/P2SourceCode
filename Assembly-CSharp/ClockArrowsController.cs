using System;
using Engine.Common.Services;
using UnityEngine;

public class ClockArrowsController : MonoBehaviour {
	private ITimeService timeService;

	private void Start() {
		timeService = ServiceLocator.GetService<ITimeService>();
	}

	private void Update() {
		if (timeService == null)
			return;
		var component = gameObject.GetComponent<Animator>();
		var gameTime = timeService.GameTime;
		double num1 = gameTime.Hours % 12;
		gameTime = timeService.GameTime;
		var num2 = gameTime.Minutes / 60.0;
		var num3 = num1 + num2;
		gameTime = timeService.GameTime;
		var num4 = gameTime.Seconds / 3600.0;
		var num5 = (float)(num3 + num4) / 12f;
		component.SetFloat("time", num5);
	}
}