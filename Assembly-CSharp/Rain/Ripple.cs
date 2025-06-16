// Decompiled with JetBrains decompiler
// Type: Rain.Ripple
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Rain
{
  public class Ripple
  {
    private const float FadeSpeed = 0.5f;
    private const float ExpandSpeed = 0.5f;
    public Material material;
    public Ripple nextNode;
    private float _radius;
    private float _strength;

    public Ripple(
      Ripple currentRootNode,
      Material puddleMaterial,
      Vector3 worldPosition,
      float startRadius,
      float endRadius)
    {
      this.nextNode = currentRootNode;
      this.material = new Material(puddleMaterial);
      this.material.SetVector("_RippleOrigin", new Vector4(worldPosition.x, worldPosition.y, worldPosition.z, 0.0f));
      this._radius = startRadius;
      this._strength = (float) (((double) endRadius - (double) startRadius) * 0.5 / 0.5);
      this.UpdateMaterial();
    }

    public bool Update()
    {
      this._strength -= Time.deltaTime * 0.5f;
      if ((double) this._strength <= 0.0)
      {
        Object.Destroy((Object) this.material);
        return true;
      }
      this._radius += Time.deltaTime * 0.5f;
      this.UpdateMaterial();
      return false;
    }

    private void UpdateMaterial()
    {
      this.material.SetFloat("_RippleStrength", Mathf.Min(Mathf.Sqrt(this._strength), 1f));
      this.material.SetFloat("_RippleRadius", this._radius);
    }
  }
}
