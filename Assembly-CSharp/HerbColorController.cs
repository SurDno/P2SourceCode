using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using UnityEngine;

public class HerbColorController : MonoBehaviour
{
  [SerializeField]
  private float maxDistance = 15f;
  [SerializeField]
  private Color emissionColor;
  [SerializeField]
  private float colorChangeSpeed = 0.1f;
  private MaterialPropertyBlock block;
  private float currentAlpha;

  private void Start() => CountColor();

  private void Update()
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    currentAlpha = Mathf.MoveTowards(currentAlpha, (transform.position - (player != null ? ((IEntityView) player).Position : Vector3.zero)).magnitude < (double) maxDistance ? 1f : 0.0f, colorChangeSpeed * Time.deltaTime);
    CountColor();
    GetComponent<MeshRenderer>().SetPropertyBlock(currentAlpha > 0.0 ? block : null);
  }

  private void CountColor()
  {
    emissionColor.a = Mathf.Lerp(0.0f, Mathf.Clamp01(Mathf.PingPong(Time.time, 2f) - 0.5f), currentAlpha);
    if (block == null)
      block = new MaterialPropertyBlock();
    block.SetColor("_EmissionColor", emissionColor);
  }
}
