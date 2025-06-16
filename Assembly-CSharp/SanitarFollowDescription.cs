using UnityEngine;

[CreateAssetMenu(fileName = "Sanitar follow", menuName = "Pathologic2/States/Sanitar follow", order = 101)]
public class SanitarFollowDescription : ScriptableObject {
	[Tooltip("NPC стремится сохранять эту дистанцию")]
	public float KeepDistance = 2f;

	[Tooltip("Если враг ближе, то НПС отступает")]
	public float RetreatDistance = 1f;

	[Tooltip("Если игрок удалился на эту дистанцию, то NPC переходит на бег.")]
	public float RunDistance = 5f;
}