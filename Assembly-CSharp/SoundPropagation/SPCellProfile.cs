using UnityEngine;

namespace SoundPropagation;

[CreateAssetMenu(fileName = "New Cell Profile", menuName = "Scriptable Objects/Sound Propagation Cell Profile")]
public class SPCellProfile : ScriptableObject {
	public Filtering FilteringPerMeter;

	public float OcclusionPerMeter => FilteringPerMeter.Occlusion;
}