using System;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI
{
  [RequireComponent(typeof (RectTransform))]
  [Serializable]
  public class BaseGraphic : Graphic, IDisposable
  {
    protected GameObject gameObject2;
    protected bool isDisposed;
    protected RectTransform transform2;

    public virtual string Name
    {
      get => this.gameObject2.name;
      set => this.gameObject2.name = value;
    }

    public RectTransform Transform
    {
      get => this.transform2;
      set => this.transform2 = value;
    }

    public BaseGraphic Parent
    {
      get => this.transform2.parent.GetComponent<BaseGraphic>();
      set
      {
        if ((UnityEngine.Object) value == (UnityEngine.Object) null)
          this.transform2.SetParent((UnityEngine.Transform) null);
        else
          this.transform2.SetParent((UnityEngine.Transform) value.transform2, false);
      }
    }

    public Vector2 Pivot
    {
      get => this.transform2.pivot;
      set => this.transform2.pivot = value;
    }

    public Vector2 Anchor
    {
      get => this.transform2.anchorMax;
      set
      {
        this.transform2.anchorMax = value;
        this.transform2.anchorMin = value;
      }
    }

    public Quaternion Rotation
    {
      get => this.transform2.rotation;
      set => this.transform2.rotation = value;
    }

    public Vector2 Position
    {
      get => (Vector2) this.transform2.position;
      set => this.transform2.position = (Vector3) value;
    }

    public Vector2 LocalPosition
    {
      get => (Vector2) this.transform2.localPosition;
      set => this.transform2.localPosition = (Vector3) value;
    }

    public Rect Rectangle => this.transform2.rect;

    public bool IsEnabled
    {
      get => this.gameObject2.activeSelf;
      set
      {
        this.gameObject2.SetActive(value);
        if (value)
        {
          Action<BaseGraphic> enableEvent = this.Enable_Event;
          if (enableEvent == null)
            return;
          enableEvent(this);
        }
        else
        {
          Action<BaseGraphic> disableEvent = this.Disable_Event;
          if (disableEvent != null)
            disableEvent(this);
        }
      }
    }

    public void Dispose()
    {
      if (this.isDisposed || (UnityEngine.Object) this == (UnityEngine.Object) null)
        return;
      this.isDisposed = true;
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }

    [field: NonSerialized]
    public event Action<BaseGraphic> Enable_Event;

    [field: NonSerialized]
    public event Action<BaseGraphic> Disable_Event;

    protected override void Awake()
    {
      this.gameObject2 = this.gameObject;
      this.transform2 = this.gameObject.GetComponent<RectTransform>();
    }
  }
}
