// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.TextNotification
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

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
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class TextNotification : UIControl, INotification
  {
    [SerializeField]
    private float fade;
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioMixerGroup mixer;
    [SerializeField]
    private StringView textView;
    [SerializeField]
    private StringView optionalHeaderView;
    private CanvasGroup canvasGroup;
    private UIService ui;
    private bool play;
    private float alpha = -1f;
    private bool shutdown = false;
    private float timer = 0.0f;
    private bool useTimeout = false;
    private object[] vals = (object[]) null;

    private CanvasGroup CanvasGroup
    {
      get
      {
        if ((UnityEngine.Object) this.canvasGroup == (UnityEngine.Object) null)
          this.canvasGroup = this.GetComponent<CanvasGroup>();
        return this.canvasGroup;
      }
    }

    [Inspected]
    public bool Complete { get; private set; }

    [Inspected]
    public NotificationEnum Type { get; private set; }

    private void Update()
    {
      if (!(this.ui.Active is HudWindow))
        return;
      if (!this.play)
      {
        this.Play();
        this.play = true;
      }
      if (this.useTimeout)
      {
        this.timer -= Time.deltaTime;
        if ((double) this.timer <= 0.0)
        {
          this.Complete = true;
          this.useTimeout = false;
        }
      }
      if (this.shutdown)
      {
        this.alpha -= Time.deltaTime / this.fade;
        if ((double) this.alpha <= 0.0)
        {
          JoystickLayoutSwitcher.Instance.OnLayoutChanged -= new Action<JoystickLayoutSwitcher.KeyLayouts>(this.ChangeLayout);
          InputService.Instance.onJoystickUsedChanged -= new Action<bool>(this.OnJoystick);
          ServiceLocator.GetService<LocalizationService>().LocalizationChanged -= new Action(this.ChangeLanguage);
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
        else
          this.CanvasGroup.alpha = this.alpha;
      }
      else
      {
        if ((double) this.alpha >= 1.0)
          return;
        this.alpha += Time.deltaTime / this.fade;
        if ((double) this.alpha > 1.0)
          this.alpha = 1f;
        this.CanvasGroup.alpha = this.alpha;
      }
    }

    protected override void Awake()
    {
      base.Awake();
      this.ui = ServiceLocator.GetService<UIService>();
      this.alpha = 0.0f;
      this.CanvasGroup.alpha = this.alpha;
    }

    private void Append(StringBuilder sb, string s)
    {
      if (string.IsNullOrEmpty(s))
        return;
      if (sb.Length != 0)
      {
        char c1 = sb[sb.Length - 1];
        char c2 = s[0];
        if (!char.IsWhiteSpace(c1) && !char.IsWhiteSpace(c2) && !char.IsPunctuation(c2))
          sb.Append(' ');
      }
      sb.Append(s);
    }

    private string Concat(string a, string b)
    {
      if (!string.IsNullOrEmpty(a) && !string.IsNullOrEmpty(b))
      {
        char c1 = a[a.Length - 1];
        char c2 = b[0];
        if (!char.IsWhiteSpace(c1) && !char.IsWhiteSpace(c2) && !char.IsPunctuation(c2))
          return a + " " + b;
      }
      return a + b;
    }

    private void Play()
    {
      if ((UnityEngine.Object) this.clip == (UnityEngine.Object) null || (UnityEngine.Object) this.mixer == (UnityEngine.Object) null)
        return;
      SoundUtility.PlayAudioClip2D(this.clip, this.mixer, 1f, 0.0f, context: this.gameObject.GetFullName());
    }

    public void ResetTooltip()
    {
      CoroutineService.Instance.WaitFrame(1, (Action) (() =>
      {
        this.timer = 0.0f;
        this.useTimeout = false;
        if (this.vals == null)
          return;
        this.BuildNotification(this.Type, this.vals);
      }));
    }

    public void OnJoystick(bool joystick) => this.ResetTooltip();

    public void ChangeLayout(JoystickLayoutSwitcher.KeyLayouts layout) => this.ResetTooltip();

    private void ChangeLanguage() => this.ResetTooltip();

    private void BuildNotification(NotificationEnum type, object[] values)
    {
      this.Type = type;
      this.vals = values;
      string text1 = (string) null;
      string text2 = (string) null;
      object input = values.Length != 0 ? values[0] : (object) null;
      object obj = values.Length > 1 ? values[1] : (object) null;
      this.useTimeout = false;
      switch (input)
      {
        case string _:
          if (Regex.IsMatch((string) input, "(?<value>({(.*?))})"))
          {
            string tag = Regex.Match((string) input, "(?<value>({(.*?))})").Groups["value"].Value;
            if (!string.IsNullOrEmpty(tag))
            {
              text1 = ServiceLocator.GetService<LocalizationService>().GetText(tag);
              break;
            }
            break;
          }
          text1 = (string) input;
          break;
        case LocalizedText text3:
          text1 = ServiceLocator.GetService<LocalizationService>().GetText(text3);
          break;
        case List<string> _:
          List<string> stringList = (List<string>) input;
          if (stringList.Count > 0)
            text2 = stringList[0];
          StringBuilder sb1 = new StringBuilder();
          for (int index = 1; index < stringList.Count; ++index)
            this.Append(sb1, stringList[index]);
          text1 = sb1.ToString();
          break;
        case List<LocalizedText> _:
          LocalizationService service = ServiceLocator.GetService<LocalizationService>();
          List<LocalizedText> localizedTextList = (List<LocalizedText>) input;
          if (localizedTextList.Count > 0)
            text2 = service.GetText(localizedTextList[0]);
          StringBuilder sb2 = new StringBuilder();
          for (int index = 1; index < localizedTextList.Count; ++index)
            this.Append(sb2, service.GetText(localizedTextList[index]));
          text1 = sb2.ToString();
          break;
      }
      if (string.IsNullOrEmpty(text1) && string.IsNullOrEmpty(text2))
      {
        Debug.LogWarning((object) ("Notification : " + type.ToString() + " : No text. Expects value[0] as string, LocalizedText, List<string> or List<LocalizedText>"));
        this.textView.StringValue = (string) null;
        if (!((UnityEngine.Object) this.optionalHeaderView != (UnityEngine.Object) null))
          return;
        this.optionalHeaderView.StringValue = (string) null;
      }
      else
      {
        string a = TextHelper.ReplaceTags(text2, "<b><color=#e4b450>", "</color></b>");
        string source = TextHelper.ReplaceTags(text1, "<b><color=#e4b450>", "</color></b>");
        string b = ServiceLocator.GetService<TextContextService>().ComputeText(source);
        if ((UnityEngine.Object) this.optionalHeaderView != (UnityEngine.Object) null)
        {
          this.optionalHeaderView.StringValue = a;
          this.optionalHeaderView.gameObject.SetActive(!string.IsNullOrEmpty(a));
        }
        else
          b = this.Concat(a, b);
        this.textView.StringValue = b;
        if (obj == null || !(obj is float))
          return;
        float num = (float) obj;
        if ((double) num > 0.0)
        {
          this.useTimeout = true;
          this.timer = num;
        }
      }
    }

    public void Initialise(NotificationEnum type, object[] values)
    {
      JoystickLayoutSwitcher.Instance.OnLayoutChanged += new Action<JoystickLayoutSwitcher.KeyLayouts>(this.ChangeLayout);
      InputService.Instance.onJoystickUsedChanged += new Action<bool>(this.OnJoystick);
      ServiceLocator.GetService<LocalizationService>().LocalizationChanged += new Action(this.ChangeLanguage);
      this.BuildNotification(type, values);
    }

    public void Shutdown() => this.shutdown = true;

    private void ApplyValue<T>(ref T result, object[] values, int index)
    {
      if (index >= values.Length)
        return;
      object obj1 = values[index];
      if (obj1 == null || !(obj1 is T obj2))
        return;
      result = obj2;
    }
  }
}
