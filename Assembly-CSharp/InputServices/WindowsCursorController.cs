// Decompiled with JetBrains decompiler
// Type: InputServices.WindowsCursorController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using Inspectors;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

#nullable disable
namespace InputServices
{
  public class WindowsCursorController : ICursorController, IUpdatable
  {
    private bool visible = true;
    private bool free = true;

    [DllImport("User32.dll")]
    public static extern long SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out WindowsCursorController.POINT lpPoint);

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
    public bool Free
    {
      get => this.free;
      set
      {
        this.free = value;
        this.UpdateCursor();
      }
    }

    public Vector2 Position => (Vector2) Input.mousePosition;

    public WindowsCursorController()
    {
      Cursor.SetCursor(ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultCursor, new Vector2(0.0f, 0.0f), CursorMode.Auto);
      InstanceByRequest<EngineApplication>.Instance.OnApplicationFocusEvent += new Action<bool>(this.OnApplicationFocus);
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Move(float diffX, float diffY)
    {
      WindowsCursorController.POINT lpPoint;
      WindowsCursorController.GetCursorPos(out lpPoint);
      lpPoint.X += (int) diffX;
      lpPoint.Y -= (int) diffY;
      WindowsCursorController.SetCursorPos(lpPoint.X, lpPoint.Y);
    }

    private void UpdateCursor()
    {
      Cursor.visible = this.visible && !InputService.Instance.JoystickUsed;
      CursorLockMode cursorLockMode = Cursor.visible || this.free && !InputService.Instance.JoystickUsed ? CursorLockMode.None : CursorLockMode.Locked;
      if (Cursor.lockState == cursorLockMode)
        return;
      Cursor.lockState = cursorLockMode;
    }

    private void OnApplicationFocus(bool focus)
    {
      if (!focus)
        return;
      this.UpdateCursor();
    }

    public void ComputeUpdate()
    {
      if (!Input.anyKeyDown)
        return;
      this.UpdateCursor();
    }

    public struct POINT
    {
      public int X;
      public int Y;

      public POINT(int x, int y)
      {
        this.X = x;
        this.Y = y;
      }
    }
  }
}
