// Decompiled with JetBrains decompiler
// Type: InputServices.VirtualCursorController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
namespace InputServices
{
  public class VirtualCursorController : ICursorController
  {
    private GameObject cursor;
    private RectTransform cursorTransform;
    private bool visible;

    [Inspected(Mutable = true)]
    public bool Visible
    {
      get => this.visible;
      set
      {
        this.visible = value;
        this.UpdateCursor();
      }
    }

    [Inspected(Mutable = true)]
    public bool Free { get; set; }

    [Inspected(Mutable = true)]
    public Vector2 Position { get; private set; }

    public VirtualCursorController()
    {
      InstanceByRequest<EngineApplication>.Instance.OnInitialized += new Action(this.OnInitialized);
    }

    private void OnInitialized()
    {
      this.Position = new Vector2((float) (Screen.width / 2), (float) (Screen.height / 2));
      this.cursor = ServiceLocator.GetService<UIService>().VirtualCursor;
      if (!((UnityEngine.Object) this.cursor != (UnityEngine.Object) null))
        return;
      this.cursorTransform = this.cursor.GetComponent<RectTransform>();
      this.UpdateCursor();
    }

    public void Move(float diffX, float diffY)
    {
      float num1 = this.Position.x + diffX;
      float num2 = this.Position.y - diffY;
      this.Position = new Vector2(Mathf.Clamp(num1, 0.0f, (float) Screen.width), Mathf.Clamp(num2, 0.0f, (float) Screen.height));
      this.UpdateCursor();
    }

    private void UpdateCursor()
    {
      if ((UnityEngine.Object) this.cursor == (UnityEngine.Object) null)
        return;
      if (this.cursor.activeSelf != this.Visible)
        this.cursor.SetActive(this.Visible && !InputService.Instance.JoystickUsed);
      this.cursorTransform.position = new Vector3(this.Position.x, this.Position.y, this.cursorTransform.position.z);
    }
  }
}
