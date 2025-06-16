// Decompiled with JetBrains decompiler
// Type: SimplePlayerWindowSwapper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu;
using Engine.Source.UI;
using InputServices;
using System;

#nullable disable
public class SimplePlayerWindowSwapper
{
  private static IWindow lastOpenedPlayerWindow = (IWindow) null;
  private static Type lastOpenedPlayerWindowType = (Type) null;
  private static Type bufferedPlayerWindowType = (Type) null;
  private static IWindow bufferedPlayerWindow = (IWindow) null;
  private static bool isSessionStateSubscribed = false;

  public static IWindow LastOpenedPlayerWindow
  {
    get => SimplePlayerWindowSwapper.lastOpenedPlayerWindow;
    private set => SimplePlayerWindowSwapper.lastOpenedPlayerWindow = value;
  }

  public static Type LastOpenedPlayerWindowType
  {
    get
    {
      return SimplePlayerWindowSwapper.lastOpenedPlayerWindowType == (Type) null ? typeof (IInventoryWindow) : SimplePlayerWindowSwapper.lastOpenedPlayerWindowType;
    }
    private set => SimplePlayerWindowSwapper.lastOpenedPlayerWindowType = value;
  }

  public static bool CanPushLastWindow
  {
    get => ((UIWindow) SimplePlayerWindowSwapper.LastOpenedPlayerWindow).IsWindowAvailable;
  }

  public static void SetLastOpenedPlayerWindow<T>(IWindow window) where T : class, IWindow
  {
    SimplePlayerWindowSwapper.LastOpenedPlayerWindow = window;
    SimplePlayerWindowSwapper.LastOpenedPlayerWindowType = typeof (T);
    if (SimplePlayerWindowSwapper.isSessionStateSubscribed)
      return;
    InputService.Instance.OnSessionStateChanged += new Action(SimplePlayerWindowSwapper.OnChangeSessionReset);
    SimplePlayerWindowSwapper.isSessionStateSubscribed = true;
  }

  public static void SetNotificationTempWindow(NotificationEnum type)
  {
    if (type == NotificationEnum.None)
    {
      if (SimplePlayerWindowSwapper.bufferedPlayerWindow != null && SimplePlayerWindowSwapper.bufferedPlayerWindowType != (Type) null)
      {
        SimplePlayerWindowSwapper.LastOpenedPlayerWindow = SimplePlayerWindowSwapper.bufferedPlayerWindow;
        SimplePlayerWindowSwapper.LastOpenedPlayerWindowType = SimplePlayerWindowSwapper.bufferedPlayerWindowType;
      }
      SimplePlayerWindowSwapper.bufferedPlayerWindow = (IWindow) null;
      SimplePlayerWindowSwapper.bufferedPlayerWindowType = (Type) null;
    }
    else
    {
      SimplePlayerWindowSwapper.bufferedPlayerWindow = SimplePlayerWindowSwapper.LastOpenedPlayerWindow;
      SimplePlayerWindowSwapper.bufferedPlayerWindowType = SimplePlayerWindowSwapper.LastOpenedPlayerWindowType;
      switch (type - 1025)
      {
        case NotificationEnum.None:
          SimplePlayerWindowSwapper.LastOpenedPlayerWindowType = typeof (IMapWindow);
          break;
        case (NotificationEnum) 1:
          SimplePlayerWindowSwapper.LastOpenedPlayerWindowType = typeof (IMMWindow);
          break;
        case (NotificationEnum) 3:
          SimplePlayerWindowSwapper.LastOpenedPlayerWindowType = typeof (IBoundCharactersWindow);
          break;
      }
      if (SimplePlayerWindowSwapper.LastOpenedPlayerWindowType != SimplePlayerWindowSwapper.bufferedPlayerWindowType)
        SimplePlayerWindowSwapper.LastOpenedPlayerWindow = (IWindow) ServiceLocator.GetService<UIService>().Get(SimplePlayerWindowSwapper.LastOpenedPlayerWindowType);
      else
        SimplePlayerWindowSwapper.SetNotificationTempWindow(NotificationEnum.None);
    }
  }

  private static void OnChangeSessionReset()
  {
    SimplePlayerWindowSwapper.lastOpenedPlayerWindow = (IWindow) null;
    SimplePlayerWindowSwapper.LastOpenedPlayerWindowType = (Type) null;
  }

  public static bool CanCallLastPlayerWindow()
  {
    return SimplePlayerWindowSwapper.LastOpenedPlayerWindow != null && SimplePlayerWindowSwapper.CanPushLastWindow;
  }

  public static bool CallLastPlayerWindow()
  {
    if (!SimplePlayerWindowSwapper.CanCallLastPlayerWindow())
      return false;
    ServiceLocator.GetService<UIService>().Push(SimplePlayerWindowSwapper.LastOpenedPlayerWindowType);
    return true;
  }
}
