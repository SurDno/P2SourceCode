﻿using System.Collections;
using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class SaveGameNode : FlowControlNode
  {
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In() => CoroutineService.Instance.Route(Save(output));

    private IEnumerator Save(FlowOutput output)
    {
      yield return new WaitForEndOfFrame();
      SavesService saves = ServiceLocator.GetService<SavesService>();
      ProfilesService profiles = ServiceLocator.GetService<ProfilesService>();
      profiles.GenerateSaveName();
      string saveName = profiles.GetLastSaveName();
      saves.Save(saveName);
      output.Call();
    }
  }
}
