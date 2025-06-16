// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.LoadGameNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.Services.Profiles;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
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
