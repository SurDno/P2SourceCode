using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;

public class GroupPoint : MonoBehaviour {
	private bool registered;

	private void OnEnable() {
		RegisterInService();
	}

	private void OnDisable() {
		var service = ServiceLocator.GetService<GroupPointsService>();
		if (service == null)
			return;
		registered = false;
		service.RemovePoint(this);
	}

	private void OnUpdate() {
		if (registered)
			return;
		RegisterInService();
	}

	private void RegisterInService() {
		var service = ServiceLocator.GetService<GroupPointsService>();
		if (service == null)
			return;
		registered = true;
		service.AddPoint(this);
	}
}