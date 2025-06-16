using System.Text.RegularExpressions;
using InputServices;

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
      if ((Object) textMeshProComponent == (Object) null)
        textMeshProComponent = this.gameObject?.GetComponentInChildren<TextMeshProUGUI>();
      if ((Object) textComponent == (Object) null)
        textComponent = this.gameObject?.GetComponentInChildren<UnityEngine.UI.Text>();
      if ((Object) textMeshProComponent != (Object) null)
      {
        if ((Object) textComponent != (Object) null)
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
        if (!((Object) textComponent != (Object) null))
          return;
        textComponent.gameObject?.SetActive(true);
        textComponent.text = StringValue;
      }
    }
  }
}
