using Engine.Common;
using Engine.Impl.UI.Controls;
using Engine.Source.Audio;
using System;
using System.Globalization;
using UnityEngine;

[Serializable]
public class SubtitlesItem
{
  private readonly SubtitlesView subtitlesView;
  private string text;
  private float startTime;
  private float endTime;
  private GameObject view;
  private ProgressView fadeView;
  private float partStartTime;
  private float partEndTime;
  private int partIndex;
  private AudioState audioState;
  private float progress;

  public IEntity Actor { get; private set; }

  public bool Ended { get; private set; }

  public void End()
  {
    this.text = (string) null;
    this.ReleaseView();
    this.Actor = (IEntity) null;
    this.Ended = true;
  }

  private void ReleaseView()
  {
    if (!((UnityEngine.Object) this.view != (UnityEngine.Object) null))
      return;
    this.subtitlesView.ReleaseLineView(this.view);
    this.view = (GameObject) null;
    this.fadeView = (ProgressView) null;
  }

  private void ShowPart()
  {
    this.ReleaseView();
    int num1 = int.MaxValue;
    float num2 = float.MaxValue;
    int num3 = this.text.IndexOf("<split=", this.partIndex);
    if (num3 != -1)
    {
      int startIndex = num3 + 7;
      int num4 = this.text.IndexOf('>', startIndex);
      if (num4 != -1)
      {
        string s = this.text.Substring(startIndex, num4 - startIndex);
        float result = float.MaxValue;
        if (float.TryParse(s, NumberStyles.Float, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
        {
          num1 = num4 + 1;
          num2 = this.startTime + result;
        }
      }
    }
    string str;
    if (num1 == int.MaxValue)
    {
      str = this.partIndex == 0 ? this.text : this.text.Substring(this.partIndex);
      num2 = this.endTime;
    }
    else
      str = this.text.Substring(this.partIndex, num3 - this.partIndex).Trim();
    if (str.Length != 0)
    {
      this.view = this.subtitlesView.CreateLineView();
      this.view.GetComponent<StringView>().StringValue = str;
      this.fadeView = this.view.GetComponent<ProgressView>();
    }
    this.partStartTime = this.partEndTime;
    this.partIndex = num1;
    this.partEndTime = num2;
  }

  public void Start(IEntity actor, string text, AudioState audioState, UnityEngine.Object context)
  {
    this.audioState = audioState;
    this.text = text;
    this.Actor = actor;
    float length = audioState.AudioSource.clip.length;
    this.progress = 0.0f;
    this.startTime = 0.0f;
    this.endTime = length;
    this.partIndex = 0;
    this.partStartTime = this.startTime;
    this.partEndTime = this.startTime;
    this.Ended = false;
  }

  public SubtitlesItem(SubtitlesView subtitlesView) => this.subtitlesView = subtitlesView;

  public void Update()
  {
    if (this.Ended)
      return;
    if (this.audioState.Complete)
    {
      this.End();
    }
    else
    {
      this.progress = this.audioState.AudioSource.time;
      if ((double) this.progress >= (double) this.endTime)
      {
        this.End();
      }
      else
      {
        while ((double) this.progress >= (double) this.partEndTime)
          this.ShowPart();
        if ((UnityEngine.Object) this.view != (UnityEngine.Object) null && this.view.activeSelf == this.audioState.Pause)
          this.view.SetActive(!this.audioState.Pause);
        if (!((UnityEngine.Object) this.fadeView != (UnityEngine.Object) null))
          return;
        this.fadeView.Progress = (double) this.subtitlesView.FadeTime > 0.0 ? Mathf.Clamp01(Mathf.Min(this.progress - this.partStartTime, this.partEndTime - this.progress) / this.subtitlesView.FadeTime) : 1f;
      }
    }
  }
}
