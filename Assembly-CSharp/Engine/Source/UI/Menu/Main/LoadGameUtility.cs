// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Main.LoadGameUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Main;
using Engine.Source.Commons;
using Engine.Source.Services;
using Engine.Source.Services.Profiles;
using System.Collections;

#nullable disable
namespace Engine.Source.UI.Menu.Main
{
  public static class LoadGameUtility
  {
    public static IEnumerator StartGameWithSave(string saveName)
    {
      ProfilesService profiles = ServiceLocator.GetService<ProfilesService>();
      LoadWindow.Instance.GameDay = ProfilesUtility.GetGameDay(saveName);
      LoadWindow.Instance.Show = true;
      UIService uiService = ServiceLocator.GetService<UIService>();
      uiService.Pop();
      while (uiService.IsTransition)
        yield return (object) null;
      GameLauncher gameLauncher = ServiceLocator.GetService<GameLauncher>();
      gameLauncher.StartGameWithSave(saveName);
    }

    public static IEnumerator StartNewGame(string projectName = "")
    {
      LoadWindow.Instance.GameDay = 0;
      LoadWindow.Instance.Show = true;
      UIService uiService = ServiceLocator.GetService<UIService>();
      uiService.Pop();
      while (uiService.IsTransition)
        yield return (object) null;
      InstanceByRequest<GameDataService>.Instance.SetCurrentGameData(projectName);
      ProfilesService profiles = ServiceLocator.GetService<ProfilesService>();
      profiles.GenerateNewProfile(InstanceByRequest<GameDataService>.Instance.GetCurrentGameData().GameName);
      GameLauncher gameLauncher = ServiceLocator.GetService<GameLauncher>();
      gameLauncher.StartNewGame();
    }

    public static IEnumerator RestartGameWithSave(string saveName)
    {
      ProfilesService profiles = ServiceLocator.GetService<ProfilesService>();
      LoadWindow.Instance.GameDay = ProfilesUtility.GetGameDay(saveName);
      LoadWindow.Instance.Show = true;
      UIService uiService = ServiceLocator.GetService<UIService>();
      uiService.Pop();
      while (uiService.IsTransition)
        yield return (object) null;
      uiService.Pop();
      while (uiService.IsTransition)
        yield return (object) null;
      GameLauncher gameLauncher = ServiceLocator.GetService<GameLauncher>();
      VirtualMachineController vm = ServiceLocator.GetService<VirtualMachineController>();
      if (vm.IsLoaded)
        gameLauncher.RestartGameWithSave(saveName);
      else
        gameLauncher.StartGameWithSave(saveName);
    }
  }
}
