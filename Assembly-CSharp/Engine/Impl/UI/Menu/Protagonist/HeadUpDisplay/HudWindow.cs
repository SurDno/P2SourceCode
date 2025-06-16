// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay.HudWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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

#nullable disable
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

    public InteractableWindow InteractableInterface => this.interactableInterface;

    public override void Initialize()
    {
      this.RegisterLayer<IHudWindow>((IHudWindow) this);
      base.Initialize();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      CursorService.Instance.Free = CursorService.Instance.Visible = false;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      foreach (EntityView entityView in this.playerEntityView)
        entityView.Value = player;
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.MainMenu, new GameActionHandle(this.MainMenuListener));
    }

    protected override void OnDisable()
    {
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.MainMenu, new GameActionHandle(this.MainMenuListener));
      foreach (EntityView entityView in this.playerEntityView)
        entityView.Value = (IEntity) null;
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
      foreach (NotificationLayerView notificationLayer in this.notificationLayers)
      {
        INotification notification = notificationLayer.Create(type);
        if (notification != null)
          return notification;
      }
      return (INotification) null;
    }

    public void SetVisibility(bool visible, bool ignoreTextNotifications)
    {
      this.visibilityView.Visible = visible;
      if (!this.isActiveAndEnabled)
        this.visibilityView.SkipAnimation();
      if (ignoreTextNotifications)
        return;
      this.textNotificationsVisibility.Visible = visible;
      if (!this.isActiveAndEnabled)
        this.textNotificationsVisibility.SkipAnimation();
    }
  }
}
