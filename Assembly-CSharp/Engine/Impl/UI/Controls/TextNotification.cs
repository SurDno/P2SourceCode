using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay;
using Engine.Source.Audio;
using Engine.Source.Services;
using Engine.Source.Services.Notifications;
using InputServices;
using Inspectors;
using UnityEngine;
using UnityEngine.Audio;

namespace Engine.Impl.UI.Controls;

public class TextNotification : UIControl, INotification {
	[SerializeField] private float fade;
	[SerializeField] private AudioClip clip;
	[SerializeField] private AudioMixerGroup mixer;
	[SerializeField] private StringView textView;
	[SerializeField] private StringView optionalHeaderView;
	private CanvasGroup canvasGroup;
	private UIService ui;
	private bool play;
	private float alpha = -1f;
	private bool shutdown;
	private float timer;
	private bool useTimeout;
	private object[] vals;

	private CanvasGroup CanvasGroup {
		get {
			if (canvasGroup == null)
				canvasGroup = GetComponent<CanvasGroup>();
			return canvasGroup;
		}
	}

	[Inspected] public bool Complete { get; private set; }

	[Inspected] public NotificationEnum Type { get; private set; }

	private void Update() {
		if (!(ui.Active is HudWindow))
			return;
		if (!play) {
			Play();
			play = true;
		}

		if (useTimeout) {
			timer -= Time.deltaTime;
			if (timer <= 0.0) {
				Complete = true;
				useTimeout = false;
			}
		}

		if (shutdown) {
			alpha -= Time.deltaTime / fade;
			if (alpha <= 0.0) {
				JoystickLayoutSwitcher.Instance.OnLayoutChanged -= ChangeLayout;
				InputService.Instance.onJoystickUsedChanged -= OnJoystick;
				ServiceLocator.GetService<LocalizationService>().LocalizationChanged -= ChangeLanguage;
				Destroy(gameObject);
			} else
				CanvasGroup.alpha = alpha;
		} else {
			if (alpha >= 1.0)
				return;
			alpha += Time.deltaTime / fade;
			if (alpha > 1.0)
				alpha = 1f;
			CanvasGroup.alpha = alpha;
		}
	}

	protected override void Awake() {
		base.Awake();
		ui = ServiceLocator.GetService<UIService>();
		alpha = 0.0f;
		CanvasGroup.alpha = alpha;
	}

	private void Append(StringBuilder sb, string s) {
		if (string.IsNullOrEmpty(s))
			return;
		if (sb.Length != 0) {
			var c1 = sb[sb.Length - 1];
			var c2 = s[0];
			if (!char.IsWhiteSpace(c1) && !char.IsWhiteSpace(c2) && !char.IsPunctuation(c2))
				sb.Append(' ');
		}

		sb.Append(s);
	}

	private string Concat(string a, string b) {
		if (!string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b)) {
			var c1 = a[a.Length - 1];
			var c2 = b[0];
			if (!char.IsWhiteSpace(c1) && !char.IsWhiteSpace(c2) && !char.IsPunctuation(c2))
				return a + " " + b;
		}

		return a + b;
	}

	private void Play() {
		if (clip == null || mixer == null)
			return;
		SoundUtility.PlayAudioClip2D(clip, mixer, 1f, 0.0f, context: gameObject.GetFullName());
	}

	public void ResetTooltip() {
		CoroutineService.Instance.WaitFrame(1, (Action)(() => {
			timer = 0.0f;
			useTimeout = false;
			if (vals == null)
				return;
			BuildNotification(Type, vals);
		}));
	}

	public void OnJoystick(bool joystick) {
		ResetTooltip();
	}

	public void ChangeLayout(JoystickLayoutSwitcher.KeyLayouts layout) {
		ResetTooltip();
	}

	private void ChangeLanguage() {
		ResetTooltip();
	}

	private void BuildNotification(NotificationEnum type, object[] values) {
		Type = type;
		vals = values;
		string text1 = null;
		string text2 = null;
		var input = values.Length != 0 ? values[0] : null;
		var obj = values.Length > 1 ? values[1] : null;
		useTimeout = false;
		switch (input) {
			case string _:
				if (Regex.IsMatch((string)input, "(?<value>({(.*?))})")) {
					var tag = Regex.Match((string)input, "(?<value>({(.*?))})").Groups["value"].Value;
					if (!string.IsNullOrEmpty(tag))
						text1 = ServiceLocator.GetService<LocalizationService>().GetText(tag);
					break;
				}

				text1 = (string)input;
				break;
			case LocalizedText text3:
				text1 = ServiceLocator.GetService<LocalizationService>().GetText(text3);
				break;
			case List<string> _:
				var stringList = (List<string>)input;
				if (stringList.Count > 0)
					text2 = stringList[0];
				var sb1 = new StringBuilder();
				for (var index = 1; index < stringList.Count; ++index)
					Append(sb1, stringList[index]);
				text1 = sb1.ToString();
				break;
			case List<LocalizedText> _:
				var service = ServiceLocator.GetService<LocalizationService>();
				var localizedTextList = (List<LocalizedText>)input;
				if (localizedTextList.Count > 0)
					text2 = service.GetText(localizedTextList[0]);
				var sb2 = new StringBuilder();
				for (var index = 1; index < localizedTextList.Count; ++index)
					Append(sb2, service.GetText(localizedTextList[index]));
				text1 = sb2.ToString();
				break;
		}

		if (string.IsNullOrEmpty(text1) && string.IsNullOrEmpty(text2)) {
			Debug.LogWarning("Notification : " + type +
			                 " : No text. Expects value[0] as string, LocalizedText, List<string> or List<LocalizedText>");
			textView.StringValue = null;
			if (!(optionalHeaderView != null))
				return;
			optionalHeaderView.StringValue = null;
		} else {
			var a = TextHelper.ReplaceTags(text2, "<b><color=#e4b450>", "</color></b>");
			var source = TextHelper.ReplaceTags(text1, "<b><color=#e4b450>", "</color></b>");
			var b = ServiceLocator.GetService<TextContextService>().ComputeText(source);
			if (optionalHeaderView != null) {
				optionalHeaderView.StringValue = a;
				optionalHeaderView.gameObject.SetActive(!string.IsNullOrEmpty(a));
			} else
				b = Concat(a, b);

			textView.StringValue = b;
			if (obj == null || !(obj is float))
				return;
			var num = (float)obj;
			if (num > 0.0) {
				useTimeout = true;
				timer = num;
			}
		}
	}

	public void Initialise(NotificationEnum type, object[] values) {
		JoystickLayoutSwitcher.Instance.OnLayoutChanged += ChangeLayout;
		InputService.Instance.onJoystickUsedChanged += OnJoystick;
		ServiceLocator.GetService<LocalizationService>().LocalizationChanged += ChangeLanguage;
		BuildNotification(type, values);
	}

	public void Shutdown() {
		shutdown = true;
	}

	private void ApplyValue<T>(ref T result, object[] values, int index) {
		if (index >= values.Length)
			return;
		var obj1 = values[index];
		if (obj1 == null || !(obj1 is T obj2))
			return;
		result = obj2;
	}
}