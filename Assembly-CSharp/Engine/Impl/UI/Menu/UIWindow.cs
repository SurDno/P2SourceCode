// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.UIWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.UI;
using InputServices;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

#nullable disable
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
          Debug.LogError((object) ("Wrong Enabled : " + value.ToString() + " , info : " + ((object) this).GetType().Name + ":" + MethodBase.GetCurrentMethod().Name));
        Debug.Log((object) ObjectInfoUtility.GetStream().Append("Enabled : ").Append(value).Append(" , info : ").Append(TypeUtility.GetTypeName(((object) this).GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
        this.gameObject.SetActive(value);
      }
    }

    public event Action<IWindow> DisableWindowEvent;

    protected void RegisterLayer<T>(T layer) where T : class, IWindow
    {
      ServiceLocator.GetService<UIService>().RegisterLayer<T>(layer);
    }

    public virtual void Initialize()
    {
    }

    protected bool WithoutJoystickCancelListener(GameActionType type, bool down)
    {
      return !InputService.Instance.JoystickUsed && this.CancelListener(type, down);
    }

    protected bool CancelListener(GameActionType type, bool down)
    {
      if (!down)
        return false;
      UIWindow active = ServiceLocator.GetService<UIService>().Active;
      if ((UnityEngine.Object) active != (UnityEngine.Object) this)
      {
        Debug.LogError((object) ("Wrong state, active : " + ((UnityEngine.Object) active != (UnityEngine.Object) null ? ((object) active).GetType().Name : "null") + " , this : " + ((object) this).GetType().Name));
        return false;
      }
      ServiceLocator.GetService<UIService>().Pop();
      return true;
    }

    protected virtual void OnEnable()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(((object) this).GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
      this.StartCoroutine(this.AfterEnabled());
    }

    protected virtual void OnDisable()
    {
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(TypeUtility.GetTypeName(((object) this).GetType())).Append(":").Append(MethodBase.GetCurrentMethod().Name).Append("\n").GetStackTrace());
      Action<IWindow> disableWindowEvent = this.DisableWindowEvent;
      if (disableWindowEvent != null)
        disableWindowEvent((IWindow) this);
      InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
    }

    protected virtual IEnumerator AfterEnabled()
    {
      yield return (object) new WaitForEndOfFrame();
      this.OnJoystick(InputService.Instance.JoystickUsed);
    }

    protected virtual void OnJoystick(bool joystick)
    {
    }

    public virtual System.Type GetWindowType() => typeof (UIWindow);

    public virtual IEnumerator OnOpened()
    {
      this.IsEnabled = true;
      yield break;
    }

    public virtual IEnumerator OnClosed()
    {
      this.IsEnabled = false;
      yield break;
    }

    public virtual bool IsWindowAvailable => false;
  }
}
