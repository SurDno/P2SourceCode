using Engine.Common;
using UnityEngine;

namespace Engine.Source.Services.CameraServices;

public interface ICameraController {
	void Initialise();

	void Shutdown();

	void Update(IEntity target, GameObject gameObjectTarget);
}