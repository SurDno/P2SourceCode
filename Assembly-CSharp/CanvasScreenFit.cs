// Decompiled with JetBrains decompiler
// Type: CanvasScreenFit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

#nullable disable
[RequireComponent(typeof (CanvasScaler))]
public class CanvasScreenFit : MonoBehaviour
{
  private void OnEnable() => this.UpdateCanvasScale();

  private void Update() => this.UpdateCanvasScale();

  private void UpdateCanvasScale()
  {
    float num = (float) ((double) Screen.width / 16.0 / ((double) Screen.height / 9.0));
    this.GetComponent<CanvasScaler>().matchWidthOrHeight = (double) num > 1.0 ? 1f : 0.0f;
  }
}
