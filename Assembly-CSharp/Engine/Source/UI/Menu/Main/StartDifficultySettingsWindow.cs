﻿using Engine.Impl.UI.Menu.Main;

namespace Engine.Source.UI.Menu.Main
{
  public class StartDifficultySettingsWindow : 
    CancelableSimpleWindow,
    IStartDifficultySettingsWindow,
    IWindow,
    IMainMenu,
    IPauseMenu
  {
    protected override void RegisterLayer()
    {
      RegisterLayer<IStartDifficultySettingsWindow>(this);
    }
  }
}
