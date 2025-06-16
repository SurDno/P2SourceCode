using Cofe.Utility;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Services.Inputs;
using Engine.Source.Settings.External;
using Inspectors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputServices
{
  public class InputService : IUpdatable
  {
    private static InputService instance;
    private Dictionary<string, AxisBind> axes = new Dictionary<string, AxisBind>();
    private Dictionary<string, DateTime> buttons = new Dictionary<string, DateTime>();
    private HashSet<string> buttonsPressed = new HashSet<string>();
    private HashSet<string> holdButtonsPressed = new HashSet<string>();
    private JoystickLayout layout = (JoystickLayout) null;
    private TimeSpan holdDelay;
    private string currentJoystick;
    private float delayTime = 2f;
    private float currentTime;
    private bool _joystickUsed;

    public static InputService Instance
    {
      get
      {
        if (InputService.instance == null)
        {
          InputService.instance = new InputService();
          InputService.instance.Initialise();
        }
        return InputService.instance;
      }
    }

    public event Action OnSessionStateChanged;

    public void ChangeGameSession()
    {
      Action sessionStateChanged = this.OnSessionStateChanged;
      if (sessionStateChanged == null)
        return;
      sessionStateChanged();
    }

    public event Action<bool> onJoystickUsedChanged;

    [Inspected]
    public bool JoystickUsed
    {
      get => this._joystickUsed;
      set
      {
        if (this._joystickUsed == value)
          return;
        this._joystickUsed = value;
        ICursorController instance = CursorService.Instance;
        Vector2 position = instance.Position;
        Vector2 vector2 = (value ? Vector2.zero : new Vector2((float) Screen.width, (float) Screen.height) * 0.5f) - position;
        instance.Move(vector2.x, vector2.y);
        BaseInputModule currentInputModule = EventSystem.current.currentInputModule;
        currentInputModule.DeactivateModule();
        currentInputModule.ActivateModule();
        instance.Visible = !value && instance.Free;
        Action<bool> joystickUsedChanged = this.onJoystickUsedChanged;
        if (joystickUsedChanged != null)
          joystickUsedChanged(value);
      }
    }

    [Inspected]
    public bool JoystickPresent => this.GetJoystickName() != "";

    private string GetJoystickName()
    {
      foreach (string joystickName in Input.GetJoystickNames())
      {
        if (joystickName != "")
          return joystickName;
      }
      return "";
    }

    [Inspected]
    public JoystickLayout Layout => this.layout;

    public float GetAxis(string name)
    {
      AxisBind axisBind;
      if (this.axes.TryGetValue(name, out axisBind))
      {
        float num = Input.GetAxisRaw(axisBind.Axis);
        if (axisBind.Normalize)
          num = (float) ((double) num * 0.5 + 0.5);
        if ((double) num > (double) axisBind.Dead)
          return (float) (((double) num - (double) axisBind.Dead) / (1.0 - (double) axisBind.Dead));
        if ((double) num < -(double) axisBind.Dead)
          return (float) (((double) num + (double) axisBind.Dead) / (1.0 - (double) axisBind.Dead));
      }
      return 0.0f;
    }

    public bool GetButton(string name, bool hold)
    {
      DateTime dateTime;
      if (!this.buttons.TryGetValue(name, out dateTime))
        return false;
      return !hold || dateTime == DateTime.MinValue;
    }

    public bool GetButtonDown(string name, bool hold)
    {
      return hold ? this.holdButtonsPressed.Contains(name) : this.buttonsPressed.Contains(name);
    }

    public float GetHoldProgress(string name)
    {
      DateTime dateTime;
      return this.buttons.TryGetValue(name, out dateTime) && dateTime > DateTime.MinValue ? (float) (DateTime.UtcNow - dateTime).TotalSeconds / (float) this.holdDelay.TotalSeconds : 0.0f;
    }

    private void Initialise()
    {
      this.holdDelay = TimeSpan.FromSeconds((double) ExternalSettingsInstance<ExternalInputSettings>.Instance.HoldDelay);
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void ComputeUpdate()
    {
      if (!ExternalSettingsInstance<ExternalInputSettings>.Instance.UseJoystick)
        return;
      this.CheckJoystick();
      if (this.layout == null)
        return;
      this.ComputeButtons();
      this.ComputeHoldButtons();
    }

    private void CheckJoystick()
    {
      float realtimeSinceStartup = Time.realtimeSinceStartup;
      if ((double) this.currentTime + (double) this.delayTime > (double) realtimeSinceStartup)
        return;
      this.currentTime = realtimeSinceStartup;
      string joystickName = this.GetJoystickName();
      if (!(joystickName != this.currentJoystick))
        return;
      this.currentJoystick = joystickName;
      this.InitialiseJoystick();
    }

    private void ComputeButtons()
    {
      this.buttonsPressed.Clear();
      bool flag = false;
      foreach (AxisToButton axesToButton in this.layout.AxesToButtons)
      {
        float axis = this.GetAxis(axesToButton.Axis);
        if (axesToButton.Inverse)
          axis *= -1f;
        if (this.buttons.ContainsKey(axesToButton.Name))
        {
          if ((double) axis < (double) axesToButton.Min)
          {
            this.buttons.Remove(axesToButton.Name);
            flag = true;
          }
        }
        else if ((double) axis > (double) axesToButton.Max)
        {
          this.buttons.Add(axesToButton.Name, DateTime.UtcNow);
          this.buttonsPressed.Add(axesToButton.Name);
          flag = true;
        }
      }
      foreach (KeyToButton keysToButton in this.layout.KeysToButtons)
      {
        bool key = Input.GetKey((KeyCode) keysToButton.KeyCode);
        if (this.buttons.ContainsKey(keysToButton.Name))
        {
          if (!key)
          {
            this.buttons.Remove(keysToButton.Name);
            flag = true;
          }
        }
        else if (key)
        {
          this.buttons.Add(keysToButton.Name, DateTime.UtcNow);
          this.buttonsPressed.Add(keysToButton.Name);
          flag = true;
        }
      }
      foreach (AxisBind ax in this.layout.Axes)
      {
        float num = Input.GetAxisRaw(ax.Axis);
        if (ax.Normalize)
          num = (float) ((double) num * 0.5 + 0.5);
        if ((double) num > (double) ax.Dead || (double) num < -(double) ax.Dead)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return;
      this.JoystickUsed = true;
    }

    private void ComputeHoldButtons()
    {
      this.holdButtonsPressed.Clear();
label_1:
      foreach (KeyValuePair<string, DateTime> button in this.buttons)
      {
        DateTime dateTime = button.Value;
        if (!(dateTime == DateTime.MinValue) && DateTime.UtcNow - dateTime > this.holdDelay)
        {
          this.holdButtonsPressed.Add(button.Key);
          this.buttons[button.Key] = DateTime.MinValue;
          goto label_1;
        }
      }
    }

    private void InitialiseJoystick()
    {
      this.layout = (JoystickLayout) null;
      this.axes.Clear();
      if (this.currentJoystick.IsNullOrEmpty())
        return;
      string currentJoystick = this.currentJoystick;
      this.layout = currentJoystick.IndexOf("xbox", StringComparison.InvariantCultureIgnoreCase) == -1 ? (!(currentJoystick == "Wireless Controller") ? ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Layouts[2] : ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Layouts[1]) : ExternalSettingsInstance<ExternalGameActionSettings>.Instance.Layouts[0];
      foreach (AxisBind ax in this.layout.Axes)
        this.axes[ax.Name] = ax;
    }
  }
}
