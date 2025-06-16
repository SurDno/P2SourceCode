// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.SaveGameNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class SaveGameNode : FlowControlNode
  {
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In() => CoroutineService.Instance.Route(this.Save(this.output));

    private IEnumerator Save(FlowOutput output)
    {
      yield return (object) new WaitForEndOfFrame();
      SavesService saves = ServiceLocator.GetService<SavesService>();
      ProfilesService profiles = ServiceLocator.GetService<ProfilesService>();
      profiles.GenerateSaveName();
      string saveName = profiles.GetLastSaveName();
      saves.Save(saveName);
      output.Call();
    }
  }
}
