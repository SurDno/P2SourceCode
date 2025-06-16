// Decompiled with JetBrains decompiler
// Type: CreditsScrolling
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using UnityEngine;

#nullable disable
public class CreditsScrolling : MonoBehaviour
{
  [SerializeField]
  private CreditsGenerator content;
  [SerializeField]
  private float speed = 1f;
  [SerializeField]
  private float edgePositions = 50f;
  private RectTransform canvas;

  private void Start()
  {
    this.canvas = (RectTransform) this.GetComponentInParent<Canvas>().transform;
    this.content.Position = -this.edgePositions;
  }

  private void Update()
  {
    float position = this.content.Position;
    float b = Mathf.Max(this.content.Size + this.canvas.sizeDelta.y + this.edgePositions, position);
    float num = Mathf.Min(position + Time.deltaTime * this.speed, b);
    this.content.Position = num;
    if ((double) num != (double) b)
      return;
    ServiceLocator.GetService<UIService>()?.Pop();
  }
}
