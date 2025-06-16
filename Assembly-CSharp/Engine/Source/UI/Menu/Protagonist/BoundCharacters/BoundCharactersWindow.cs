using Engine.Common.Services;
using Engine.Impl.UI.Menu;
using Engine.Impl.UI.Menu.Main;
using Engine.Source.Services;
using Engine.Source.Services.Inputs;
using Engine.Source.Utility;
using System;
using System.Collections;

namespace Engine.Source.UI.Menu.Protagonist.BoundCharacters
{
  public class BoundCharactersWindow : 
    CancelableSimpleWindow,
    IBoundCharactersWindow,
    IWindow,
    IPauseMenu
  {
    protected override void OnDisable()
    {
      PlayerUtility.ShowPlayerHands(true);
      ServiceLocator.GetService<GameActionService>().RemoveListener(GameActionType.GenericPlayerMenu, new GameActionHandle(((UIWindow) this).CancelListener));
      base.OnDisable();
    }

    protected override void OnEnable()
    {
      base.OnEnable();
      ServiceLocator.GetService<GameActionService>().AddListener(GameActionType.GenericPlayerMenu, new GameActionHandle(((UIWindow) this).CancelListener), true);
      PlayerUtility.ShowPlayerHands(false);
    }

    protected override void RegisterLayer()
    {
      this.RegisterLayer<IBoundCharactersWindow>((IBoundCharactersWindow) this);
    }

    public override Type GetWindowType() => typeof (IBoundCharactersWindow);

    public override IEnumerator OnOpened()
    {
      SimplePlayerWindowSwapper.SetLastOpenedPlayerWindow<IBoundCharactersWindow>((IWindow) this);
      return base.OnOpened();
    }

    public override bool IsWindowAvailable
    {
      get => !ServiceLocator.GetService<InterfaceBlockingService>().BlockBoundsInterface;
    }
  }
}
