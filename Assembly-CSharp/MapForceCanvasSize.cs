// Decompiled with JetBrains decompiler
// Type: MapForceCanvasSize
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class MapForceCanvasSize : MonoBehaviour
{
  [SerializeField]
  private Vector2 size = new Vector2(1920f, 1080f);

  private void OnEnable()
  {
    RectTransform transform = (RectTransform) this.transform;
    transform.anchorMin = Vector2.zero;
    transform.anchorMax = Vector2.zero;
    transform.pivot = Vector2.zero;
    transform.anchoredPosition = Vector2.zero;
    transform.sizeDelta = this.size;
  }
}
