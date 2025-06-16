using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services.Inputs;
using Engine.Source.Services.Notifications;
using Engine.Source.UI;
using Engine.Source.Utility;
using InputServices;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay
{
  public class HudWindow : UIWindow, IHudWindow, IWindow
  {
    [SerializeField]
    private InteractableWindow interactableInterface;
    [SerializeField]
    private NotificationLayerView[] notificationLayers;
    [SerializeField]
    private EntityView[] playerEntityView;
    [SerializeField]
    private HideableView visibilityView;
    [SerializeField]
    private HideableView textNotificationsVisibility;

    public InteractableWindow InteractableInterface => interactableInterface;

    public override void Initialize()
    {
      RegisterLayer<IHudWindow>(this);
      base.Initialize();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      CursorService.Instance.Free = CursorService.Instance.Visible = false;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      foreach (EntityView entityView in playerEntityView)
        entityView.Value = player;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.MainMenu, MainMenuListener);
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.MainMenu, MainMenuListener);
      foreach (EntityView entityView in playerEntityView)
        entityView.Value = null;
      base.OnDisable();
    }

    private bool MainMenuListener(GameActionType type, bool down)
    {
      if (!down || !PlayerUtility.IsPlayerCanControlling)
        return false;
      ServiceLocator.GetService<UIService>().Push<IGameWindow>();
      return true;
    }

    public INotification Create(NotificationEnum type)
    {
      foreach (NotificationLayerView notificationLayer in notificationLayers)
      {
        INotification notification = notificationLayer.Create(type);
        if (notification != null)
          return notification;
      }
      return null;
    }

    public void SetVisibility(bool visible, bool ignoreTextNotifications)
    {
      visibilityView.Visible = visible;
      if (!isActiveAndEnabled)
        visibilityView.SkipAnimation();
      if (ignoreTextNotifications)
        return;
      textNotificationsVisibility.Visible = visible;
      if (!isActiveAndEnabled)
        textNotificationsVisibility.SkipAnimation();
    }
  }
}
