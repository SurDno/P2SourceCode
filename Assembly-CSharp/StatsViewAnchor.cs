// Decompiled with JetBrains decompiler
// Type: StatsViewAnchor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class StatsViewAnchor : MonoBehaviour
{
  private static StatsView viewInstance = (StatsView) null;
  [SerializeField]
  private StatsView prefab;
  [SerializeField]
  private bool fullVersion;

  private void OnEnable()
  {
    if ((Object) StatsViewAnchor.viewInstance == (Object) null)
      StatsViewAnchor.viewInstance = Object.Instantiate<StatsView>(this.prefab, this.transform, false);
    else
      StatsViewAnchor.viewInstance.transform.SetParent(this.transform, false);
    StatsViewAnchor.viewInstance.SetFullVersion(this.fullVersion);
  }
}
