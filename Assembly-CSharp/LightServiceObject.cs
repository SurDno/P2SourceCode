using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;

public class LightServiceObject : MonoBehaviour
{
  [SerializeField]
  private float visibilityRadius = 5f;

  public float VisibilityRadius => visibilityRadius;

  private void OnEnable() => ServiceLocator.GetService<LightService>()?.RegisterLight(this, true);

  private void OnDisable() => ServiceLocator.GetService<LightService>()?.RegisterLight(this, false);
}
