using Inspectors;
using System;
using UnityEngine;

public class MeleeWeaponEffect : MonoBehaviour
{
  private static MaterialPropertyBlock propertyBlock = (MaterialPropertyBlock) null;
  private static int propertyID;
  [SerializeField]
  private EnemyBase attacker;
  [SerializeField]
  private WeaponEnum weapon;
  [SerializeField]
  private ParticleSystem particles;
  [SerializeField]
  private Renderer splatRenderer;
  [SerializeField]
  private float splatBuildUp;
  [SerializeField]
  private float splatFadeSpeed;
  private float splatLevel = 0.0f;

  [Inspected(Mode = ExecuteMode.Runtime)]
  private void OnAttackHit()
  {
    this.SetSplatLevel(this.splatLevel + this.splatBuildUp);
    this.particles?.Play();
  }

  private void OnAttackHit(WeaponEnum weapon)
  {
    if (weapon != this.weapon)
      return;
    this.OnAttackHit();
  }

  private void OnDestroy()
  {
    this.attacker.PunchHitEvent -= new Action<WeaponEnum>(this.OnAttackHit);
  }

  private void OnDisable() => this.SetSplatLevel(0.0f);

  private void SetSplatLevel(float value)
  {
    value = Mathf.Clamp(value, 0.0f, 3f);
    if ((double) value == (double) this.splatLevel)
      return;
    this.splatLevel = value;
    if ((UnityEngine.Object) this.splatRenderer == (UnityEngine.Object) null)
      return;
    if (MeleeWeaponEffect.propertyBlock == null)
    {
      MeleeWeaponEffect.propertyBlock = new MaterialPropertyBlock();
      MeleeWeaponEffect.propertyID = Shader.PropertyToID("_GlobalSplatColor");
    }
    if ((double) value == 0.0)
    {
      this.splatRenderer.SetPropertyBlock((MaterialPropertyBlock) null);
    }
    else
    {
      Color color = new Color(Mathf.Clamp01(value), Mathf.Clamp01(value - 1f), Mathf.Clamp01(value - 2f));
      color = color.gamma;
      MeleeWeaponEffect.propertyBlock.SetColor(MeleeWeaponEffect.propertyID, color);
      this.splatRenderer.SetPropertyBlock(MeleeWeaponEffect.propertyBlock);
    }
  }

  private void Start() => this.attacker.PunchHitEvent += new Action<WeaponEnum>(this.OnAttackHit);

  private void Update()
  {
    if ((double) this.splatLevel == 0.0)
      return;
    this.SetSplatLevel(this.splatLevel - Time.deltaTime * this.splatFadeSpeed);
  }
}
