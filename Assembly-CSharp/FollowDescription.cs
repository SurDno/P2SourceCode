using UnityEngine;

[CreateAssetMenu(fileName = "Follow", menuName = "Pathologic2/States/Follow", order = 101)]
public class FollowDescription : ScriptableObject {
	[Header("Передвижение")] [Tooltip("Если игрок удалился на эту дистанцию, но NPC переходит на бег.")]
	public float RunDistance = 5f;

	[Tooltip("Если игрок удалился на эту дистанцию, но NPC останавливается.")]
	public float StopDistance = 1.2f;

	[Header("Удары")] [Tooltip("Среднее время выбрасывания удара на месте.")]
	public float PunchCooldownTime = 0.5f;

	[Tooltip("Среднее время выбрасывания удара с шагом вперед.")]
	public float StepPunchCooldownTime = 0.75f;

	[Tooltip("Кулдаун бросания бомбы")] public float ThrowCooldownTime = 3f;

	[Tooltip("Среднее время выбрасывания удара с телеграфированием.")]
	public float TelegraphPunchCooldownTime = 1f;

	[Tooltip("Среднее время выбрасывания обманного движения без удара")]
	public float CheatCooldownTime = 0.5f;

	[Tooltip("Вероятность, что удар - обманка")]
	public float CheatProbability = 0.25f;

	[Header("Другое")] [Tooltip("С этой дистанции NPC атакует, может даже в движении.")]
	public float AttackDistance = 2.2f;

	[Tooltip("Толкает игрока если время в блоке больше чем это")]
	public float PushIfBlockTimeMoreThan = 2f;

	[Tooltip("Минимальное время которое должно пройти чтобы персонаж повторно кого-то сбил с ног")]
	public float KnockDownCooldownTime = 7f;
}