using InputServices;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class TooltipTextStringView : StringView
  {
    [SerializeField]
    private TextMeshProUGUI _textMeshPro;
    [SerializeField]
    private UnityEngine.UI.Text _text;

    public override void SkipAnimation()
    {
    }

    protected override void ApplyStringValue()
    {
      if ((Object) this._textMeshPro == (Object) null)
        this._textMeshPro = this.gameObject?.GetComponentInChildren<TextMeshProUGUI>();
      if ((Object) this._text == (Object) null)
        this._text = this.gameObject?.GetComponentInChildren<UnityEngine.UI.Text>();
      string str = this.StringValue;
      if (string.IsNullOrEmpty(str))
        return;
      Match match1 = (Match) null;
      if (Regex.IsMatch(str, "<keyboard>(?<value>((.*?)))</keyboard>"))
        match1 = Regex.Match(str, "<keyboard>(?<value>((.*?)))</keyboard>");
      Match match2 = (Match) null;
      if (Regex.IsMatch(str, string.Format("<joystick={0}>(?<value>((.*?)))</joystick>", (object) JoystickLayoutSwitcher.Instance.GetCurrentLayoutIndex())))
        match2 = Regex.Match(str, string.Format("<joystick={0}>(?<value>((.*?)))</joystick>", (object) JoystickLayoutSwitcher.Instance.GetCurrentLayoutIndex()));
      if (match1 != null)
        str = match1.Groups["value"].Value;
      if ((Object) this._textMeshPro != (Object) null)
      {
        Regex regex = new Regex("(?<tag>(<b><color(.*?)>)(?<value>((.*?)))(</color></b>))");
        if (InputService.Instance.JoystickUsed && regex.IsMatch(str))
        {
          if (match2 != null)
            str = match2.Groups["value"].Value;
          foreach (Match match3 in regex.Matches(str))
          {
            GroupCollection groups = match3.Groups;
            if (groups["tag"].Success)
              str = str.Replace(groups["tag"].Value, "{0}");
            if (groups["value"].Success)
              str = str.Replace("{0}", string.Format("<sprite name=\"{0}\">", (object) groups["value"].Value));
          }
        }
        this.ShowSingle(str);
      }
      else
        this.ShowSingle(str);
    }

    private void ShowSingle(string text)
    {
      if ((Object) this._textMeshPro != (Object) null)
      {
        this._textMeshPro.gameObject?.SetActive(true);
        if ((Object) this._text != (Object) null)
          this._text.gameObject?.SetActive(false);
        this._textMeshPro.text = text;
      }
      else
      {
        if (!((Object) this._text != (Object) null))
          return;
        this._text.gameObject?.SetActive(true);
        this._text.text = text;
      }
    }
  }
}
