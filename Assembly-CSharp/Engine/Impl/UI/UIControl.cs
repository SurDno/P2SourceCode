// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.UIControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI
{
  [RequireComponent(typeof (RectTransform))]
  public class UIControl : MonoBehaviour
  {
    private RectTransform transform;

    public RectTransform Transform
    {
      get
      {
        if ((Object) this.transform == (Object) null)
          this.transform = this.gameObject.GetComponent<RectTransform>();
        return this.transform;
      }
    }

    public Vector2 PivotedPosition
    {
      get
      {
        return (Vector2) this.Transform.position - Vector2.Scale(this.Transform.pivot, Vector2.Scale(this.Transform.sizeDelta, (Vector2) this.Transform.lossyScale));
      }
      set
      {
        this.Transform.position = (Vector3) (value + Vector2.Scale(this.Transform.pivot, Vector2.Scale(this.Transform.sizeDelta, (Vector2) this.Transform.lossyScale)));
      }
    }

    public bool IsEnabled
    {
      get => this.gameObject.activeSelf;
      set => this.gameObject.SetActive(value);
    }

    protected virtual void Awake()
    {
    }
  }
}
