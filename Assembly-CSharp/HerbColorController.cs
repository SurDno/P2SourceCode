// Decompiled with JetBrains decompiler
// Type: HerbColorController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using UnityEngine;

#nullable disable
public class HerbColorController : MonoBehaviour
{
  [SerializeField]
  private float maxDistance = 15f;
  [SerializeField]
  private Color emissionColor;
  [SerializeField]
  private float colorChangeSpeed = 0.1f;
  private MaterialPropertyBlock block;
  private float currentAlpha = 0.0f;

  private void Start() => this.CountColor();

  private void Update()
  {
    IEntity player = ServiceLocator.GetService<ISimulation>().Player;
    this.currentAlpha = Mathf.MoveTowards(this.currentAlpha, (double) (this.transform.position - (player != null ? ((IEntityView) player).Position : Vector3.zero)).magnitude < (double) this.maxDistance ? 1f : 0.0f, this.colorChangeSpeed * Time.deltaTime);
    this.CountColor();
    this.GetComponent<MeshRenderer>().SetPropertyBlock((double) this.currentAlpha > 0.0 ? this.block : (MaterialPropertyBlock) null);
  }

  private void CountColor()
  {
    this.emissionColor.a = Mathf.Lerp(0.0f, Mathf.Clamp01(Mathf.PingPong(Time.time, 2f) - 0.5f), this.currentAlpha);
    if (this.block == null)
      this.block = new MaterialPropertyBlock();
    this.block.SetColor("_EmissionColor", this.emissionColor);
  }
}
