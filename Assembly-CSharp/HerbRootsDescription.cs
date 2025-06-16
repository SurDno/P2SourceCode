using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "HerbRootsDescription", menuName = "Pathologic2/HerbRootsDescription", order = 101)]
public class HerbRootsDescription : ScriptableObject {
	[Header("Топология")] [Tooltip("На сколько нужно погрузить корень под землю (положительное значение)")]
	public float VerticalOffset = 1f;

	[Tooltip("Радиус триггера активации")] public float Radius = 5f;

	[Header("Геймплей")] [Tooltip("Время активации в игровых минутах")]
	public float activationStayTime = 5f;

	[Tooltip("время выползания корней из земли в реальных секундах")]
	public float activationTime = 10f;

	[Header("Звуки привлечения")] public AudioMixerGroup Mixer;
	public float AttractMinDistance = 1f;
	public float AttractMaxDistance = 50f;

	[Tooltip("3д звук, привлекающий внимание к корню")]
	public AudioClip AttractLoopSound;

	[Header("Звуки внутри триггера")] public float EnterTriggerMinDistance = 2f;
	public float EnterTriggerMaxDistance = 10f;
	[Tooltip("3д звук входа в триггер")] public AudioClip EnterTriggerOneshotSound;

	[Tooltip("3д звук стояния в триггере")]
	public AudioClip EnterTriggerLoopSound;

	[Tooltip("3д прорастания корня")] public AudioClip RootReleaseOneshotSound;
}