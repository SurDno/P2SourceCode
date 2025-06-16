// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.GameDataService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Scripts.Data;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons
{
  public class GameDataService : InstanceByRequest<GameDataService>
  {
    private GameDataInfo currentGameData;

    public GameDataInfo GetCurrentGameData()
    {
      return this.currentGameData != null ? this.currentGameData : BuildSettingsUtility.GetDefaultGameData();
    }

    public void SetCurrentGameData(string projectName)
    {
      this.currentGameData = !projectName.IsNullOrEmpty() ? BuildSettingsUtility.GetGameData(projectName) : (GameDataInfo) null;
      Debug.Log((object) ("GameDataService : SetCurrentGameData : " + projectName));
    }
  }
}
