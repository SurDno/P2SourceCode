using System;
using System.Collections;
using System.Reflection;
using Cofe.Utility;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;

namespace Engine.Impl.UI.Menu
{
  public abstract class UIWindow : MonoBehaviour, IWindow
  {
    public bool IsEnabled
    {
      get => this.gameObject.activeSelf;
      set
      {
        if (this.gameObject.activeSelf == value)
          Debug.LogError((object) ("Wrong Enabled : " + value + " , info : " + this.GetType().Name + ":" + MethodBase.GetCurrentMethod().Name));
        Debug.Log((object) ObjectInfoUtility.GetStream().Append("Enabled : ").Append(value).Append(" , info : ").Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
        this.gameObject.SetActive(value);
      }
    }

    public event Action<IWindow> DisableWindowEvent;

    protected void RegisterLayer<T>(T layer) where T : class, IWindow
    {
      ServiceLocator.GetService<UIService>().RegisterLayer(layer);
    }

    public virtual void Initialize()
    {
    }

    protected bool WithoutJoystickCancelListener(GameActionType type, bool down)
    {
      return !InputService.Instance.JoystickUsed && CancelListener(type, down);
    }

    protected bool CancelListener(GameActionType type, bool down)
    {
      if (!down)
        return false;
      UIWindow active = ServiceLocator.GetService<UIService>().Active;
      if ((UnityEngine.Object) active != (UnityEngine.Object) this)
      {
        Debug.LogError((object) ("Wrong state, active : " + ((UnityEngine.Object) active != (UnityEngine.Object) null ? active.GetType().Name : "null") + " , this : " + this.GetType().Name));
        return false;
      }
      ServiceLocator.GetService<UIService>().Pop();
      return true;
    }

    protected virtual void OnEnable()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      InputService.Instance.onJoystickUsedChanged += OnJoystick;
      this.StartCoroutine(AfterEnabled());
    }

    protected virtual void OnDisable()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      Action<IWindow> disableWindowEvent = DisableWindowEvent;
      if (disableWindowEvent != null)
        disableWindowEvent(this);
      InputService.Instance.onJoystickUsedChanged -= OnJoystick;
    }

    protected virtual IEnumerator AfterEnabled()
    {
      yield return (object) new WaitForEndOfFrame();
      OnJoystick(InputService.Instance.JoystickUsed);
    }

    protected virtual void OnJoystick(bool joystick)
    {
    }

    public virtual Type GetWindowType() => typeof (UIWindow);

    public virtual IEnumerator OnOpened()
    {
      IsEnabled = true;
      yield break;
    }

    public virtual IEnumerator OnClosed()
    {
      IsEnabled = false;
      yield break;
    }

    public virtual bool IsWindowAvailable => false;
  }
}
