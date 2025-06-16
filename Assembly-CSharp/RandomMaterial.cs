// Decompiled with JetBrains decompiler
// Type: RandomMaterial
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class RandomMaterial : MonoBehaviour
{
  public Renderer targetRenderer;
  public Material[] materials;

  public void Start() => this.ChangeMaterial();

  public void ChangeMaterial()
  {
    this.targetRenderer.sharedMaterial = this.materials[Random.Range(0, this.materials.Length)];
  }
}
