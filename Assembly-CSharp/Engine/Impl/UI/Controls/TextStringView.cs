using InputServices;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class TextStringView : StringView
  {
    [SerializeField]
    private TextMeshProUGUI textMeshProComponent;
    [SerializeField]
    private UnityEngine.UI.Text textComponent;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyStringValue()
    {
      if ((Object) this.textMeshProComponent == (Object) null)
        this.textMeshProComponent = this.gameObject?.GetComponentInChildren<TextMeshProUGUI>();
      if ((Object) this.textComponent == (Object) null)
        this.textComponent = this.gameObject?.GetComponentInChildren<UnityEngine.UI.Text>();
      if ((Object) this.textMeshProComponent != (Object) null)
      {
        if ((Object) this.textComponent != (Object) null)
          this.textComponent.gameObject?.SetActive(false);
        string input = this.StringValue;
        Regex regex = new Regex("(?<tag>(<b><color(.*?)>)(?<value>((.*?)))(</color></b>))");
        if (!string.IsNullOrEmpty(input) && InputService.Instance.JoystickUsed && regex.IsMatch(input))
        {
          foreach (Match match in regex.Matches(input))
          {
            GroupCollection groups = match.Groups;
            if (groups["tag"].Success)
              input = input.Replace(groups["tag"].Value, "{0}");
            if (groups["value"].Success)
              input = input.Replace("{0}", string.Format("<sprite name=\"{0}\">", (object) groups["value"].Value));
          }
        }
        this.textMeshProComponent.text = input;
      }
      else
      {
        if (!((Object) this.textComponent != (Object) null))
          return;
        this.textComponent.gameObject?.SetActive(true);
        this.textComponent.text = this.StringValue;
      }
    }
  }
}
