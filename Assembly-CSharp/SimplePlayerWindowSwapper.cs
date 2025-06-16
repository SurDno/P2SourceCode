using System;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu;
using Engine.Source.UI;
using InputServices;

public class SimplePlayerWindowSwapper
{
  private static IWindow lastOpenedPlayerWindow;
  private static Type lastOpenedPlayerWindowType;
  private static Type bufferedPlayerWindowType;
  private static IWindow bufferedPlayerWindow;
  private static bool isSessionStateSubscribed;

  public static IWindow LastOpenedPlayerWindow
  {
    get => lastOpenedPlayerWindow;
    private set => lastOpenedPlayerWindow = value;
  }

  public static Type LastOpenedPlayerWindowType
  {
    get
    {
      return lastOpenedPlayerWindowType == null ? typeof (IInventoryWindow) : lastOpenedPlayerWindowType;
    }
    private set => lastOpenedPlayerWindowType = value;
  }

  public static bool CanPushLastWindow
  {
    get => ((UIWindow) LastOpenedPlayerWindow).IsWindowAvailable;
  }

  public static void SetLastOpenedPlayerWindow<T>(IWindow window) where T : class, IWindow
  {
    LastOpenedPlayerWindow = window;
    LastOpenedPlayerWindowType = typeof (T);
    if (isSessionStateSubscribed)
      return;
    InputService.Instance.OnSessionStateChanged += OnChangeSessionReset;
    isSessionStateSubscribed = true;
  }

  public static void SetNotificationTempWindow(NotificationEnum type)
  {
    if (type == NotificationEnum.None)
    {
      if (bufferedPlayerWindow != null && bufferedPlayerWindowType != null)
      {
        LastOpenedPlayerWindow = bufferedPlayerWindow;
        LastOpenedPlayerWindowType = bufferedPlayerWindowType;
      }
      bufferedPlayerWindow = null;
      bufferedPlayerWindowType = null;
    }
    else
    {
      bufferedPlayerWindow = LastOpenedPlayerWindow;
      bufferedPlayerWindowType = LastOpenedPlayerWindowType;
      switch (type - 1025)
      {
        case NotificationEnum.None:
          LastOpenedPlayerWindowType = typeof (IMapWindow);
          break;
        case (NotificationEnum) 1:
          LastOpenedPlayerWindowType = typeof (IMMWindow);
          break;
        case (NotificationEnum) 3:
          LastOpenedPlayerWindowType = typeof (IBoundCharactersWindow);
          break;
      }
      if (LastOpenedPlayerWindowType != bufferedPlayerWindowType)
        LastOpenedPlayerWindow = ServiceLocator.GetService<UIService>().Get(LastOpenedPlayerWindowType);
      else
        SetNotificationTempWindow(NotificationEnum.None);
    }
  }

  private static void OnChangeSessionReset()
  {
    lastOpenedPlayerWindow = null;
    LastOpenedPlayerWindowType = null;
  }

  public static bool CanCallLastPlayerWindow()
  {
    return LastOpenedPlayerWindow != null && CanPushLastWindow;
  }

  public static bool CallLastPlayerWindow()
  {
    if (!CanCallLastPlayerWindow())
      return false;
    ServiceLocator.GetService<UIService>().Push(LastOpenedPlayerWindowType);
    return true;
  }
}
