// Decompiled with JetBrains decompiler
// Type: Water
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class Water : MonoBehaviour
{
  private Vector2 _uvOffset = Vector2.zero;
  private Renderer _renderer;
  private Material material;

  private void Start()
  {
    this._renderer = this.GetComponent<Renderer>();
    this.material = this._renderer.materials[0];
  }

  private void Update()
  {
    this._uvOffset += new Vector2(0.051f, 0.091f) * Time.deltaTime;
    this.material.SetTextureOffset("_MainTex", this._uvOffset);
  }
}
