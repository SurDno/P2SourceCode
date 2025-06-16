// Decompiled with JetBrains decompiler
// Type: KnifePostProcessEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[RequireComponent(typeof (AudioSource))]
public class KnifePostProcessEffect : MonoBehaviour
{
  [SerializeField]
  private Texture scarTexture;
  [SerializeField]
  private Color bloodColor;
  private AudioSource audioSource;
  private List<KnifePostProcessEffect.Scar> scars = new List<KnifePostProcessEffect.Scar>();
  private Material material;
  private KnifePostProcessEffect.Scar zero;

  [Inspected]
  public void AddRandomScar()
  {
    Vector3 vector3_1 = this.gameObject.transform.right - this.gameObject.transform.up;
    Vector3 vector3_2 = this.gameObject.transform.position + this.gameObject.transform.forward + Vector3.up;
  }

  public void AddScar(
    Vector3 velocityInWorldSpace,
    Vector3 positionInWorldSpace,
    float strength,
    float time)
  {
    Vector3 vector3 = this.gameObject.transform.InverseTransformVector(velocityInWorldSpace);
    float magnitude = vector3.magnitude;
    float num = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
    Vector3 viewportPoint = this.gameObject.GetComponent<Camera>().WorldToViewportPoint(positionInWorldSpace);
    if ((double) viewportPoint.z < 0.0)
      viewportPoint *= -1f;
    Vector2 vector2 = new Vector2(Mathf.Clamp(viewportPoint.x, 0.1f, 0.9f), Mathf.Clamp(viewportPoint.y, 0.1f, 0.9f));
    this.scars.Add(new KnifePostProcessEffect.Scar()
    {
      Position = vector2,
      Scale = 1f,
      Rotation = -num,
      Strength = strength,
      Time = time,
      TimeLeft = time
    });
  }

  public void AddScar(Vector2 position, float scale, float rotation, float strength, float time)
  {
    this.scars.Add(new KnifePostProcessEffect.Scar()
    {
      Position = position,
      Scale = scale,
      Rotation = rotation,
      Strength = strength,
      Time = time,
      TimeLeft = time
    });
  }

  private void Awake()
  {
    this.audioSource = this.GetComponent<AudioSource>();
    this.material = new Material(Shader.Find("Hidden/KnifePostProcessEffectShader"));
    this.zero = new KnifePostProcessEffect.Scar()
    {
      Position = Vector2.zero,
      Scale = 1f,
      Rotation = 0.0f,
      Strength = 0.0f,
      Time = 1f,
      TimeLeft = 0.0f
    };
    this.material.SetTexture("_ScarTex", this.scarTexture);
    this.material.SetColor("_BloodColor", this.bloodColor);
  }

  private void OnRenderImage(RenderTexture src, RenderTexture dest)
  {
    this.material.SetFloat("_Aspect", (float) src.width / (float) src.height);
    Graphics.Blit((Texture) src, dest, this.material);
  }

  private void OnPreRender()
  {
    int index = 0;
    while (index < this.scars.Count)
    {
      if ((double) this.scars[index].TimeLeft < (double) Time.deltaTime)
      {
        this.scars.RemoveAt(index);
      }
      else
      {
        this.scars[index].TimeLeft -= Time.deltaTime;
        ++index;
      }
    }
    while (this.scars.Count < 3)
      this.scars.Add(this.zero);
    this.material.SetFloat("_StrengthScar1", this.scars[0].Strength * this.scars[0].TimeLeft / this.scars[0].Time);
    this.material.SetFloat("_StrengthScar2", this.scars[1].Strength * this.scars[1].TimeLeft / this.scars[1].Time);
    this.material.SetFloat("_StrengthScar3", this.scars[2].Strength * this.scars[2].TimeLeft / this.scars[2].Time);
    this.material.SetVector("_PositionScar1", new Vector4()
    {
      x = this.scars[0].Position.x,
      y = this.scars[0].Position.y,
      z = Mathf.Sin(this.scars[0].Rotation * ((float) Math.PI / 180f)) / this.scars[0].Scale,
      w = Mathf.Cos(this.scars[0].Rotation * ((float) Math.PI / 180f)) / this.scars[0].Scale
    });
    this.material.SetVector("_PositionScar2", new Vector4()
    {
      x = this.scars[1].Position.x,
      y = this.scars[1].Position.y,
      z = Mathf.Sin(this.scars[1].Rotation * ((float) Math.PI / 180f)) / this.scars[1].Scale,
      w = Mathf.Cos(this.scars[1].Rotation * ((float) Math.PI / 180f)) / this.scars[1].Scale
    });
    this.material.SetVector("_PositionScar3", new Vector4()
    {
      x = this.scars[2].Position.x,
      y = this.scars[2].Position.y,
      z = Mathf.Sin(this.scars[2].Rotation * ((float) Math.PI / 180f)) / this.scars[2].Scale,
      w = Mathf.Cos(this.scars[2].Rotation * ((float) Math.PI / 180f)) / this.scars[2].Scale
    });
  }

  public class Scar
  {
    public Vector2 Position;
    public float Scale;
    public float Rotation;
    public float Strength;
    public float Time;
    public float TimeLeft;
  }
}
