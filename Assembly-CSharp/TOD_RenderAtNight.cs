// Decompiled with JetBrains decompiler
// Type: TOD_RenderAtNight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[RequireComponent(typeof (Renderer))]
public class TOD_RenderAtNight : MonoBehaviour
{
  private Renderer rendererComponent;

  protected void Start()
  {
    this.rendererComponent = this.GetComponent<Renderer>();
    this.rendererComponent.enabled = TOD_Sky.Instance.IsNight;
  }

  protected void Update() => this.rendererComponent.enabled = TOD_Sky.Instance.IsNight;
}
