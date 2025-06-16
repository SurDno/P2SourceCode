// Decompiled with JetBrains decompiler
// Type: CurrentGameNameView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using UnityEngine;

#nullable disable
public class CurrentGameNameView : MonoBehaviour
{
  [SerializeField]
  private StringView view;

  private void OnEnable()
  {
    this.view.StringValue = InstanceByRequest<GameDataService>.Instance.GetCurrentGameData().GameName;
  }
}
