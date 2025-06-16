// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.LockPicking.LockPickingSettingsData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Source.UI.Menu.Protagonist.LockPicking
{
  [CreateAssetMenu(fileName = "Lock Picking Settings", menuName = "Data/Lock Picking Settings")]
  public class LockPickingSettingsData : ScriptableObject
  {
    [SerializeField]
    private LockPickingSettings settings;

    public LockPickingSettings Settings => this.settings;
  }
}
