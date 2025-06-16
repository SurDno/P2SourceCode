using UnityEngine;

[CreateAssetMenu(fileName = "StrongAttack", menuName = "Pathologic2/States/Strong attack", order = 101)]
public class StrongAttackDescription : ScriptableObject {
	[Tooltip("Время атаки")] public float AttackTime = 7.5f;
}