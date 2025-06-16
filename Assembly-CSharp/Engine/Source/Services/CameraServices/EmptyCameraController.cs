using Engine.Common;
using UnityEngine;

namespace Engine.Source.Services.CameraServices;

public class EmptyCameraController : ICameraController {
	public void Initialise() { }

	public void Shutdown() { }

	public void Update(IEntity target, GameObject gameObjectTarget) { }
}