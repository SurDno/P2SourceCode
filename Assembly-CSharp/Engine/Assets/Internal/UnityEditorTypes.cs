using System;

namespace Engine.Assets.Internal;

public static class UnityEditorTypes {
	public static readonly Type AnimatorControllerToolType =
		Type.GetType("UnityEditor.Graphs.AnimatorControllerTool, UnityEditor.Graphs");

	public static readonly Type AnimationStateMachineGraphType =
		Type.GetType("UnityEditor.Graphs.AnimationStateMachine.Graph, UnityEditor.Graphs");

	public static readonly Type EditorGUIType = Type.GetType("UnityEditor.EditorGUI, UnityEditor");
}