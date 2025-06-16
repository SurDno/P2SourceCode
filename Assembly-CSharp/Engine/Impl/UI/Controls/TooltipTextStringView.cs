using System.Text.RegularExpressions;
using InputServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls;

public class TooltipTextStringView : StringView {
	[SerializeField] private TextMeshProUGUI _textMeshPro;
	[SerializeField] private Text _text;

	public override void SkipAnimation() { }

	protected override void ApplyStringValue() {
		if (_textMeshPro == null)
			_textMeshPro = gameObject?.GetComponentInChildren<TextMeshProUGUI>();
		if (_text == null)
			_text = gameObject?.GetComponentInChildren<Text>();
		var str = StringValue;
		if (string.IsNullOrEmpty(str))
			return;
		Match match1 = null;
		if (Regex.IsMatch(str, "<keyboard>(?<value>((.*?)))</keyboard>"))
			match1 = Regex.Match(str, "<keyboard>(?<value>((.*?)))</keyboard>");
		Match match2 = null;
		if (Regex.IsMatch(str,
			    string.Format("<joystick={0}>(?<value>((.*?)))</joystick>",
				    JoystickLayoutSwitcher.Instance.GetCurrentLayoutIndex())))
			match2 = Regex.Match(str,
				string.Format("<joystick={0}>(?<value>((.*?)))</joystick>",
					JoystickLayoutSwitcher.Instance.GetCurrentLayoutIndex()));
		if (match1 != null)
			str = match1.Groups["value"].Value;
		if (_textMeshPro != null) {
			var regex = new Regex("(?<tag>(<b><color(.*?)>)(?<value>((.*?)))(</color></b>))");
			if (InputService.Instance.JoystickUsed && regex.IsMatch(str)) {
				if (match2 != null)
					str = match2.Groups["value"].Value;
				foreach (Match match3 in regex.Matches(str)) {
					var groups = match3.Groups;
					if (groups["tag"].Success)
						str = str.Replace(groups["tag"].Value, "{0}");
					if (groups["value"].Success)
						str = str.Replace("{0}", string.Format("<sprite name=\"{0}\">", groups["value"].Value));
				}
			}

			ShowSingle(str);
		} else
			ShowSingle(str);
	}

	private void ShowSingle(string text) {
		if (_textMeshPro != null) {
			_textMeshPro.gameObject?.SetActive(true);
			if (_text != null)
				_text.gameObject?.SetActive(false);
			_textMeshPro.text = text;
		} else {
			if (!(_text != null))
				return;
			_text.gameObject?.SetActive(true);
			_text.text = text;
		}
	}
}