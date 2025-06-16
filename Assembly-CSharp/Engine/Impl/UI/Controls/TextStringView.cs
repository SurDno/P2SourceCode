using System.Text.RegularExpressions;
using InputServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class TextStringView : StringView
  {
    [SerializeField]
    private TextMeshProUGUI textMeshProComponent;
    [SerializeField]
    private Text textComponent;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyStringValue()
    {
      if (textMeshProComponent == null)
        textMeshProComponent = gameObject?.GetComponentInChildren<TextMeshProUGUI>();
      if (textComponent == null)
        textComponent = gameObject?.GetComponentInChildren<Text>();
      if (textMeshProComponent != null)
      {
        if (textComponent != null)
          textComponent.gameObject?.SetActive(false);
        string input = StringValue;
        Regex regex = new Regex("(?<tag>(<b><color(.*?)>)(?<value>((.*?)))(</color></b>))");
        if (!string.IsNullOrEmpty(input) && InputService.Instance.JoystickUsed && regex.IsMatch(input))
        {
          foreach (Match match in regex.Matches(input))
          {
            GroupCollection groups = match.Groups;
            if (groups["tag"].Success)
              input = input.Replace(groups["tag"].Value, "{0}");
            if (groups["value"].Success)
              input = input.Replace("{0}", string.Format("<sprite name=\"{0}\">", groups["value"].Value));
          }
        }
        textMeshProComponent.text = input;
      }
      else
      {
        if (!(textComponent != null))
          return;
        textComponent.gameObject?.SetActive(true);
        textComponent.text = StringValue;
      }
    }
  }
}
