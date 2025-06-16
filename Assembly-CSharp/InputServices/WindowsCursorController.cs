
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
        X = x;
        Y = y;
      }
    }
  }
}
