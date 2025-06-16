// Decompiled with JetBrains decompiler
// Type: LipsyncMicRecorder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Audio;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class LipsyncMicRecorder : MonoBehaviour
{
  public AudioSource src;
  public TextAsset Hmm;
  private string m_anno = "";
  public int latency = 50;
  public int markerWindow = 125;
  private bool LipsyncAfterRecord = true;
  private int m_maxSeconds = 1;
  private LipsyncMicRecorder.State state = LipsyncMicRecorder.State.off;
  private int m_cursor = 0;
  private List<float[]> m_RecordBuffers = new List<float[]>();
  private string deviceName = (string) null;

  public event LipsyncMicRecorder.StateChanged StateChangeListeners;

  private void Start()
  {
    foreach (string device in Microphone.devices)
      Debug.Log((object) ("Microphone " + device));
    if (Microphone.devices.Length != 0)
      this.deviceName = Microphone.devices[0];
    LipsyncPlugin.LoadSpeechHmm(this.Hmm);
  }

  private void OnDestroy() => this.StopPlugin(true);

  private void StopPlugin(bool bAudio)
  {
    LipsyncPlugin.LipRTStop();
    LipsyncPlugin.CancelLipsync();
    this.src.Stop();
  }

  public LipsyncMicRecorder.State GetState => this.state;

  public bool isRealtime => this.state == LipsyncMicRecorder.State.rec_speech_realtime;

  public string Anno => this.m_anno;

  public bool isRecording
  {
    get
    {
      return this.state == LipsyncMicRecorder.State.rec_speech || this.state == LipsyncMicRecorder.State.rec_speech_realtime;
    }
  }

  public float[] PCM
  {
    get
    {
      int length = 0;
      foreach (float[] recordBuffer in this.m_RecordBuffers)
        length += recordBuffer.Length;
      float[] pcm = new float[length];
      int num = 0;
      foreach (float[] recordBuffer in this.m_RecordBuffers)
      {
        for (int index = 0; index < recordBuffer.Length; ++index)
          pcm[index + num] = recordBuffer[index];
        num += recordBuffer.Length;
      }
      return pcm;
    }
  }

  public int sampleRate
  {
    get => (Object) this.src.clip == (Object) null ? 22050 : this.src.clip.frequency;
  }

  public int numChannels => (Object) this.src.clip == (Object) null ? 1 : this.src.clip.channels;

  public void StartRecording(bool bRealtime, bool bLipsyncWhenFinished)
  {
    if (this.state == LipsyncMicRecorder.State.lipsync)
      this.StopPlugin(true);
    this.LipsyncAfterRecord = bLipsyncWhenFinished;
    this.m_RecordBuffers.Clear();
    this.src.mute = true;
    this.src.clip = Microphone.Start(this.deviceName, true, this.m_maxSeconds, 44100);
    if ((Object) this.src.clip == (Object) null)
      return;
    this.src.loop = true;
    do
      ;
    while (Microphone.GetPosition(this.deviceName) <= 0);
    this.src.PlayAndCheck();
    if (bRealtime && (Object) this.src.clip != (Object) null)
    {
      LipsyncPlugin.SetRtLatency(this.latency);
      LipsyncPlugin.SetRtArticWindowMilli(this.markerWindow);
      LipsyncPlugin.LipRTStart(this.src.clip.frequency, this.src.clip.channels);
    }
    if (bRealtime)
      this.ChangeStateTo(LipsyncMicRecorder.State.rec_speech_realtime);
    else
      this.ChangeStateTo(LipsyncMicRecorder.State.rec_speech);
  }

  public void Stop(bool bRunLipsync)
  {
    if (this.state == LipsyncMicRecorder.State.rec_speech || this.state == LipsyncMicRecorder.State.rec_speech_realtime)
    {
      Microphone.End(this.deviceName);
      this.src.loop = false;
      if (this.isRealtime)
        LipsyncPlugin.LipRTStop();
      if (bRunLipsync && this.LipsyncAfterRecord)
      {
        this.CreateClipFromBuffers();
        LipsyncPlugin.StartLipsyncFromBuffer(this.PCM, this.sampleRate, this.numChannels);
        this.ChangeStateTo(LipsyncMicRecorder.State.lipsync);
      }
      else
        this.ChangeStateTo(LipsyncMicRecorder.State.off);
    }
    else
    {
      if (this.state != LipsyncMicRecorder.State.lipsync)
        return;
      LipsyncPlugin.CancelLipsync();
      this.ChangeStateTo(LipsyncMicRecorder.State.cancel_lipsync);
    }
  }

  public void LipsyncAudioClip(AudioClip theClip)
  {
    this.Stop(false);
    float[] numArray = new float[theClip.samples * theClip.channels];
    theClip.GetData(numArray, 0);
    LipsyncPlugin.StartLipsyncFromBuffer(numArray, theClip.frequency, theClip.channels);
    this.ChangeStateTo(LipsyncMicRecorder.State.lipsync);
  }

  private void CreateClipFromBuffers()
  {
    if ((Object) this.src.clip == (Object) null)
      return;
    float[] pcm = this.PCM;
    this.src.clip = AudioClip.Create("Microphone", pcm.Length / this.src.clip.channels, this.src.clip.channels, this.src.clip.frequency, false, false);
    this.src.clip.SetData(pcm, 0);
  }

  private void Update()
  {
    if (this.state == LipsyncMicRecorder.State.rec_speech || this.state == LipsyncMicRecorder.State.rec_speech_realtime)
    {
      int position = Microphone.GetPosition(this.deviceName);
      if (position < this.m_cursor)
      {
        float[] audioBuffer1 = this.GetAudioBuffer(this.m_cursor, this.src.clip.samples);
        float[] audioBuffer2 = this.GetAudioBuffer(0, position);
        this.AddBuffer(audioBuffer1);
        this.AddBuffer(audioBuffer2);
        this.m_cursor = position;
      }
      else
      {
        if (position <= this.m_cursor)
          return;
        this.AddBuffer(this.GetAudioBuffer(this.m_cursor, position));
        this.m_cursor = position;
      }
    }
    else if (this.state == LipsyncMicRecorder.State.lipsync && !LipsyncPlugin.IsLipsyncing())
    {
      this.m_anno = LipsyncPlugin.GetLipsyncAnnoResult();
      this.ChangeStateTo(LipsyncMicRecorder.State.off);
    }
    else
    {
      if (this.state != LipsyncMicRecorder.State.cancel_lipsync || LipsyncPlugin.IsLipsyncing())
        return;
      this.m_anno = "";
      this.ChangeStateTo(LipsyncMicRecorder.State.off);
    }
  }

  private float[] GetAudioBuffer(int startPos, int endPos)
  {
    float[] data = new float[(endPos - startPos) * this.src.clip.channels];
    this.src.clip.GetData(data, startPos);
    return data;
  }

  private void AddBuffer(float[] buffer)
  {
    if (this.LipsyncAfterRecord)
      this.m_RecordBuffers.Add(buffer);
    if (!this.isRealtime)
      return;
    LipsyncPlugin.LipRTAddBuffer(buffer);
  }

  private void ChangeStateTo(LipsyncMicRecorder.State newState)
  {
    LipsyncMicRecorder.State state = this.state;
    this.state = newState;
    if (this.StateChangeListeners == null)
      return;
    this.StateChangeListeners(state, newState);
  }

  public delegate void StateChanged(LipsyncMicRecorder.State prev, LipsyncMicRecorder.State cur);

  public enum State
  {
    off,
    listening,
    rec_speech,
    rec_speech_realtime,
    lipsync,
    cancel_lipsync,
  }
}
