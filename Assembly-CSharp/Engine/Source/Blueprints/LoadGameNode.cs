using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.Services.Profiles;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class LoadGameNode : FlowControlNode
  {
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      GameLauncher service = ServiceLocator.GetService<GameLauncher>();
      string lastSaveName = ServiceLocator.GetService<ProfilesService>().GetLastSaveName();
      if (ProfilesUtility.IsSaveExist(lastSaveName))
        service.RestartGameWithSave(lastSaveName);
      else
        Debug.LogError((object) ("Save file name not found : " + lastSaveName));
      this.output.Call();
    }
  }
}
