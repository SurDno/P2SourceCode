using System;
using System.Globalization;
using Engine.Common;
using Engine.Impl.UI.Controls;
using Engine.Source.Audio;
using UnityEngine;
using Object = UnityEngine.Object;

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
    text = null;
    ReleaseView();
    Actor = null;
    Ended = true;
  }

  private void ReleaseView()
  {
    if (!(view != null))
      return;
    subtitlesView.ReleaseLineView(view);
    view = null;
    fadeView = null;
  }

  private void ShowPart()
  {
    ReleaseView();
    int num1 = int.MaxValue;
    float num2 = float.MaxValue;
    int num3 = text.IndexOf("<split=", partIndex);
    if (num3 != -1)
    {
      int startIndex = num3 + 7;
      int num4 = text.IndexOf('>', startIndex);
      if (num4 != -1)
      {
        string s = text.Substring(startIndex, num4 - startIndex);
        float result = float.MaxValue;
        if (float.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out result))
        {
          num1 = num4 + 1;
          num2 = startTime + result;
        }
      }
    }
    string str;
    if (num1 == int.MaxValue)
    {
      str = partIndex == 0 ? text : text.Substring(partIndex);
      num2 = endTime;
    }
    else
      str = text.Substring(partIndex, num3 - partIndex).Trim();
    if (str.Length != 0)
    {
      view = subtitlesView.CreateLineView();
      view.GetComponent<StringView>().StringValue = str;
      fadeView = view.GetComponent<ProgressView>();
    }
    partStartTime = partEndTime;
    partIndex = num1;
    partEndTime = num2;
  }

  public void Start(IEntity actor, string text, AudioState audioState, Object context)
  {
    this.audioState = audioState;
    this.text = text;
    Actor = actor;
    float length = audioState.AudioSource.clip.length;
    progress = 0.0f;
    startTime = 0.0f;
    endTime = length;
    partIndex = 0;
    partStartTime = startTime;
    partEndTime = startTime;
    Ended = false;
  }

  public SubtitlesItem(SubtitlesView subtitlesView) => this.subtitlesView = subtitlesView;

  public void Update()
  {
    if (Ended)
      return;
    if (audioState.Complete)
    {
      End();
    }
    else
    {
      progress = audioState.AudioSource.time;
      if (progress >= (double) endTime)
      {
        End();
      }
      else
      {
        while (progress >= (double) partEndTime)
          ShowPart();
        if (view != null && view.activeSelf == audioState.Pause)
          view.SetActive(!audioState.Pause);
        if (!(fadeView != null))
          return;
        fadeView.Progress = subtitlesView.FadeTime > 0.0 ? Mathf.Clamp01(Mathf.Min(progress - partStartTime, partEndTime - progress) / subtitlesView.FadeTime) : 1f;
      }
    }
  }
}
