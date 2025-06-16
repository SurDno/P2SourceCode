using UnityEngine;

[CreateAssetMenu(fileName = "Plague Cloud Particles Settings", menuName = "Plague/Cloud Particles Settings")]
public class PlagueCloudParticlesSettings : ScriptableObject {
	public float EmissionRadius = 2f;
	[Range(0.0f, 1f)] public float Emission = 1f;
	public float LifeTime = 10f;
	public Vector3 Acceleration = Vector3.down;
	public float Drag = 5f;
	[Space] public float NoiseFrequency;
	public float NoiseAmplitude;
	public float NoiseMotion;
	[Space] public float Gravity;
	public float GravitySphereRadius = 0.5f;
	public float GravityFadeRadius = 3f;
	public float GravityMovementPrediction;
	[Space] public Texture2D Pattern;
	public Vector2 PatternSize = Vector2.one;
	public float PatternPlaneForce;
	public float PatternOrthogonalForce;
	public float PatternRandomForce;
}