// Decompiled with JetBrains decompiler
// Type: ImageAlphaRaycastEnabled
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
[RequireComponent(typeof (Image))]
public class ImageAlphaRaycastEnabled : MonoBehaviour
{
  private void Start() => this.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.25f;
}
