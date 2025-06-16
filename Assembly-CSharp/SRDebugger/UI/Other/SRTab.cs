// Decompiled with JetBrains decompiler
// Type: SRDebugger.UI.Other.SRTab
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SRDebugger.UI.Controls;
using SRF;
using UnityEngine;
using UnityEngine.Serialization;

#nullable disable
namespace SRDebugger.UI.Other
{
  public class SRTab : SRMonoBehaviour
  {
    public RectTransform HeaderExtraContent;
    public int SortIndex;
    [HideInInspector]
    public SRTabButton TabButton;
    [SerializeField]
    [FormerlySerializedAs("Title")]
    private string _title;
    [SerializeField]
    private DefaultTabs tab;

    public string Title => this._title;

    public DefaultTabs Tab => this.tab;
  }
}
