using SRDebugger.UI.Controls;
using SRF;
using UnityEngine;
using UnityEngine.Serialization;

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

    public string Title => _title;

    public DefaultTabs Tab => tab;
  }
}
