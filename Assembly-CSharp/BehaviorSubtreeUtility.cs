using BehaviorDesigner.Runtime;
using UnityEngine;

public static class BehaviorSubtreeUtility {
	public static BehaviorTree GetCharacterSubtree(GameObject character) {
		if (character == null) {
			Debug.LogError("character == null");
			return null;
		}

		var components = character.GetComponents<BehaviorTree>();
		if (components.Length < 2) {
			PrepareCharacter(character);
			components = character.GetComponents<BehaviorTree>();
		}

		return components.Length > 1 ? components[1] : null;
	}

	public static void SetCharacterSubtree(BehaviorTree tree, ExternalBehaviorTree newTree) {
		if (tree == null)
			return;
		var name = tree.ExternalBehaviorTree?.name;
		if (newTree != null)
			tree.enabled = true;
		var startWhenEnabled = tree.StartWhenEnabled;
		if (newTree == null)
			tree.StartWhenEnabled = false;
		tree.ExternalBehaviorTree = newTree;
		tree.StartWhenEnabled = startWhenEnabled;
		if (!(newTree == null))
			return;
		tree.enabled = false;
	}

	private static void PrepareCharacter(GameObject character) {
		var components = character.GetComponents<BehaviorTree>();
		if (components.Length != 1)
			return;
		var behaviorTree1 = components[0];
		var behaviorTree2 = character.AddComponent<BehaviorTree>();
		behaviorTree2.StartWhenEnabled = behaviorTree1.StartWhenEnabled;
		behaviorTree2.PauseWhenDisabled = behaviorTree1.PauseWhenDisabled;
		behaviorTree2.RestartWhenComplete = behaviorTree1.RestartWhenComplete;
		behaviorTree2.ResetValuesOnRestart = behaviorTree1.ResetValuesOnRestart;
	}
}