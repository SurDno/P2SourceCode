// Decompiled with JetBrains decompiler
// Type: SubtitlesViewAnchor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class SubtitlesViewAnchor : MonoBehaviour
{
  private static SubtitlesView viewInstance = (SubtitlesView) null;
  [SerializeField]
  private SubtitlesView prefab;

  private void OnEnable()
  {
    if ((Object) SubtitlesViewAnchor.viewInstance == (Object) null)
      SubtitlesViewAnchor.viewInstance = Object.Instantiate<SubtitlesView>(this.prefab, this.transform, false);
    else
      SubtitlesViewAnchor.viewInstance.transform.SetParent(this.transform, false);
  }
}
