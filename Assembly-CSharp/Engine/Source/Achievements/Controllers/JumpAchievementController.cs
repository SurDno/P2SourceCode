using Engine.Common.Services;
using Engine.Source.Services.Inputs;

namespace Engine.Source.Achievements.Controllers;

[AchievementController("jump")]
public class JumpAchievementController : IAchievementController {
	private string id;

	public void Initialise(string id) {
		this.id = id;
		ServiceLocator.GetService<GameActionService>().OnGameAction += OnGameAction;
	}

	private void OnGameAction(GameActionType action) {
		if (action != GameActionType.Jump)
			return;
		ServiceLocator.GetService<AchievementService>().Unlock(id);
	}

	public void Terminate() {
		ServiceLocator.GetService<GameActionService>().OnGameAction -= OnGameAction;
	}
}