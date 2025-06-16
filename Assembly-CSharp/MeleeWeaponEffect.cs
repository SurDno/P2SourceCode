using Inspectors;
using UnityEngine;

public class MeleeWeaponEffect : MonoBehaviour {
	private static MaterialPropertyBlock propertyBlock;
	private static int propertyID;
	[SerializeField] private EnemyBase attacker;
	[SerializeField] private WeaponEnum weapon;
	[SerializeField] private ParticleSystem particles;
	[SerializeField] private Renderer splatRenderer;
	[SerializeField] private float splatBuildUp;
	[SerializeField] private float splatFadeSpeed;
	private float splatLevel;

	[Inspected(Mode = ExecuteMode.Runtime)]
	private void OnAttackHit() {
		SetSplatLevel(splatLevel + splatBuildUp);
		particles?.Play();
	}

	private void OnAttackHit(WeaponEnum weapon) {
		if (weapon != this.weapon)
			return;
		OnAttackHit();
	}

	private void OnDestroy() {
		attacker.PunchHitEvent -= OnAttackHit;
	}

	private void OnDisable() {
		SetSplatLevel(0.0f);
	}

	private void SetSplatLevel(float value) {
		value = Mathf.Clamp(value, 0.0f, 3f);
		if (value == (double)splatLevel)
			return;
		splatLevel = value;
		if (splatRenderer == null)
			return;
		if (propertyBlock == null) {
			propertyBlock = new MaterialPropertyBlock();
			propertyID = Shader.PropertyToID("_GlobalSplatColor");
		}

		if (value == 0.0)
			splatRenderer.SetPropertyBlock(null);
		else {
			var color = new Color(Mathf.Clamp01(value), Mathf.Clamp01(value - 1f), Mathf.Clamp01(value - 2f));
			color = color.gamma;
			propertyBlock.SetColor(propertyID, color);
			splatRenderer.SetPropertyBlock(propertyBlock);
		}
	}

	private void Start() {
		attacker.PunchHitEvent += OnAttackHit;
	}

	private void Update() {
		if (splatLevel == 0.0)
			return;
		SetSplatLevel(splatLevel - Time.deltaTime * splatFadeSpeed);
	}
}