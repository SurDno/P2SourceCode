// Decompiled with JetBrains decompiler
// Type: DynamicResolutionOutputCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
[RequireComponent(typeof (Camera))]
public class DynamicResolutionOutputCamera : MonoBehaviour
{
  [SerializeField]
  private RawImage view;

  private void OnPreCull()
  {
    RenderTexture targetTexture = DynamicResolution.Instance.GetTargetTexture();
    this.view.texture = (Texture) targetTexture;
    this.view.gameObject.SetActive((Object) targetTexture != (Object) null);
  }
}
