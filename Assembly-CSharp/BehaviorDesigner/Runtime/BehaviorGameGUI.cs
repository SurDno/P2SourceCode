using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace BehaviorDesigner.Runtime;

public class BehaviorGameGUI : MonoBehaviour {
	private BehaviorTreeManager behaviorManager;
	private Camera mainCamera;

	public void Start() {
		mainCamera = Camera.main;
	}

	public void OnGUI() {
		if (behaviorManager == null)
			behaviorManager = MonoBehaviourInstance<BehaviorTreeManager>.Instance;
		if (behaviorManager == null || mainCamera == null)
			return;
		var behaviorTrees = behaviorManager.BehaviorTrees;
		for (var index1 = 0; index1 < behaviorTrees.Count; ++index1) {
			var behaviorTreeClient = behaviorTrees[index1];
			var text = "";
			for (var index2 = 0; index2 < behaviorTreeClient.activeStack.Count; ++index2) {
				var active = behaviorTreeClient.activeStack[index2];
				if (active.Count != 0 && behaviorTreeClient.taskList[active.Peek()] is Action)
					text = text +
					       behaviorTreeClient.taskList[behaviorTreeClient.activeStack[index2].Peek()].FriendlyName +
					       (index2 < behaviorTreeClient.activeStack.Count - 1 ? "\n" : "");
			}

			var guiPoint =
				GUIUtility.ScreenToGUIPoint(
					Camera.main.WorldToScreenPoint(behaviorTreeClient.behavior.transform.position));
			var content = new GUIContent(text);
			var vector2 = GUI.skin.label.CalcSize(content);
			vector2.x += 14f;
			vector2.y += 5f;
			GUI.Box(
				new Rect(guiPoint.x - vector2.x / 2f, (float)(Screen.height - (double)guiPoint.y + vector2.y / 2.0),
					vector2.x, vector2.y), content);
		}
	}
}