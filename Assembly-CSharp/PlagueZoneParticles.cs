using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using UnityEngine;

public class PlagueZoneParticles : MonoBehaviour, IUpdatable
{
  [SerializeField]
  private float maxRate;
  [SerializeField]
  private LayerMask collisionLayers;
  [SerializeField]
  private ParticleSystem particleSystem2;
  private float prevLevel = -1f;

  private void Awake()
  {
    InstanceByRequest<UpdateService>.Instance.PlagueZoneUpdater.AddUpdatable((IUpdatable) this);
  }

  private void OnDestroy()
  {
    InstanceByRequest<UpdateService>.Instance.PlagueZoneUpdater.RemoveUpdatable((IUpdatable) this);
  }

  public void ComputeUpdate()
  {
    float num = 0.0f;
    Vector3 position = this.transform.position;
    position.y += 100f;
    if (!Physics.Raycast(position, Vector3.down, 100f, (int) this.collisionLayers, QueryTriggerInteraction.Ignore))
      num = PlagueZone.GetLevel(new Vector2(this.transform.position.x, this.transform.position.z));
    if ((double) this.prevLevel == (double) num)
      return;
    this.prevLevel = num;
    this.particleSystem2.emission.rateOverTime = new ParticleSystem.MinMaxCurve(this.maxRate * num);
    ServiceCache.OptimizationService.FrameHasSpike = true;
  }
}
