using UnityEngine;

[CreateAssetMenu(fileName = "Plague Cloud Particles Settings", menuName = "Plague/Cloud Particles Settings")]
public class PlagueCloudParticlesSettings : ScriptableObject
{
  public float EmissionRadius = 2f;
  [Range(0.0f, 1f)]
  public float Emission = 1f;
  public float LifeTime = 10f;
  public Vector3 Acceleration = Vector3.down;
  public float Drag = 5f;
  [Space]
  public float NoiseFrequency = 0.0f;
  public float NoiseAmplitude = 0.0f;
  public float NoiseMotion = 0.0f;
  [Space]
  public float Gravity = 0.0f;
  public float GravitySphereRadius = 0.5f;
  public float GravityFadeRadius = 3f;
  public float GravityMovementPrediction = 0.0f;
  [Space]
  public Texture2D Pattern = (Texture2D) null;
  public Vector2 PatternSize = Vector2.one;
  public float PatternPlaneForce = 0.0f;
  public float PatternOrthogonalForce = 0.0f;
  public float PatternRandomForce = 0.0f;
}
