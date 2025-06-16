using System.Runtime.InteropServices;
using Engine.Common;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

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
    public static extern bool GetCursorPos(out POINT lpPoint);

    [Inspected(Mutable = true)]
    public bool Visible
    {
      get => visible;
      set
      {
        visible = value;
        UpdateCursor();
      }
    }

    [Inspected(Mutable = true)]
    public bool Free
    {
      get => free;
      set
      {
        free = value;
        UpdateCursor();
      }
    }

    public Vector2 Position => Input.mousePosition;

    public WindowsCursorController()
    {
      Cursor.SetCursor(ScriptableObjectInstance<ResourceFromCodeData>.Instance.DefaultCursor, new Vector2(0.0f, 0.0f), CursorMode.Auto);
      InstanceByRequest<EngineApplication>.Instance.OnApplicationFocusEvent += OnApplicationFocus;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
    }

    public void Move(float diffX, float diffY)
    {
      POINT lpPoint;
      GetCursorPos(out lpPoint);
      lpPoint.X += (int) diffX;
      lpPoint.Y -= (int) diffY;
      SetCursorPos(lpPoint.X, lpPoint.Y);
    }

    private void UpdateCursor()
    {
      Cursor.visible = visible && !InputService.Instance.JoystickUsed;
      CursorLockMode cursorLockMode = Cursor.visible || free && !InputService.Instance.JoystickUsed ? CursorLockMode.None : CursorLockMode.Locked;
      if (Cursor.lockState == cursorLockMode)
        return;
      Cursor.lockState = cursorLockMode;
    }

    private void OnApplicationFocus(bool focus)
    {
      if (!focus)
        return;
      UpdateCursor();
    }

    public void ComputeUpdate()
    {
      if (!Input.anyKeyDown)
        return;
      UpdateCursor();
    }

    public struct POINT
    {
      public int X;
      public int Y;

      public POINT(int x, int y)
      {
        X = x;
        Y = y;
      }
    }
  }
}
