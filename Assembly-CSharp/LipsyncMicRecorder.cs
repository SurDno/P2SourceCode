using System.Collections.Generic;
using Engine.Source.Audio;
using UnityEngine;

public class LipsyncMicRecorder : MonoBehaviour
{
  public AudioSource src;
  public TextAsset Hmm;
  private string m_anno = "";
  public int latency = 50;
  public int markerWindow = 125;
  private bool LipsyncAfterRecord = true;
  private int m_maxSeconds = 1;
  private State state = State.off;
  private int m_cursor;
  private List<float[]> m_RecordBuffers = [];
  private string deviceName;

  public event StateChanged StateChangeListeners;

  private void Start()
  {
    foreach (string device in Microphone.devices)
      Debug.Log("Microphone " + device);
    if (Microphone.devices.Length != 0)
      deviceName = Microphone.devices[0];
    LipsyncPlugin.LoadSpeechHmm(Hmm);
  }

  private void OnDestroy() => StopPlugin(true);

  private void StopPlugin(bool bAudio)
  {
    LipsyncPlugin.LipRTStop();
    LipsyncPlugin.CancelLipsync();
    src.Stop();
  }

  public State GetState => state;

  public bool isRealtime => state == State.rec_speech_realtime;

  public string Anno => m_anno;

  public bool isRecording => state == State.rec_speech || state == State.rec_speech_realtime;

  public float[] PCM
  {
    get
    {
      int length = 0;
      foreach (float[] recordBuffer in m_RecordBuffers)
        length += recordBuffer.Length;
      float[] pcm = new float[length];
      int num = 0;
      foreach (float[] recordBuffer in m_RecordBuffers)
      {
        for (int index = 0; index < recordBuffer.Length; ++index)
          pcm[index + num] = recordBuffer[index];
        num += recordBuffer.Length;
      }
      return pcm;
    }
  }

  public int sampleRate => src.clip == null ? 22050 : src.clip.frequency;

  public int numChannels => src.clip == null ? 1 : src.clip.channels;

  public void StartRecording(bool bRealtime, bool bLipsyncWhenFinished)
  {
    if (state == State.lipsync)
      StopPlugin(true);
    LipsyncAfterRecord = bLipsyncWhenFinished;
    m_RecordBuffers.Clear();
    src.mute = true;
    src.clip = Microphone.Start(deviceName, true, m_maxSeconds, 44100);
    if (src.clip == null)
      return;
    src.loop = true;
    do
      ;
    while (Microphone.GetPosition(deviceName) <= 0);
    src.PlayAndCheck();
    if (bRealtime && src.clip != null)
    {
      LipsyncPlugin.SetRtLatency(latency);
      LipsyncPlugin.SetRtArticWindowMilli(markerWindow);
      LipsyncPlugin.LipRTStart(src.clip.frequency, src.clip.channels);
    }
    if (bRealtime)
      ChangeStateTo(State.rec_speech_realtime);
    else
      ChangeStateTo(State.rec_speech);
  }

  public void Stop(bool bRunLipsync)
  {
    if (state == State.rec_speech || state == State.rec_speech_realtime)
    {
      Microphone.End(deviceName);
      src.loop = false;
      if (isRealtime)
        LipsyncPlugin.LipRTStop();
      if (bRunLipsync && LipsyncAfterRecord)
      {
        CreateClipFromBuffers();
        LipsyncPlugin.StartLipsyncFromBuffer(PCM, sampleRate, numChannels);
        ChangeStateTo(State.lipsync);
      }
      else
        ChangeStateTo(State.off);
    }
    else
    {
      if (state != State.lipsync)
        return;
      LipsyncPlugin.CancelLipsync();
      ChangeStateTo(State.cancel_lipsync);
    }
  }

  public void LipsyncAudioClip(AudioClip theClip)
  {
    Stop(false);
    float[] numArray = new float[theClip.samples * theClip.channels];
    theClip.GetData(numArray, 0);
    LipsyncPlugin.StartLipsyncFromBuffer(numArray, theClip.frequency, theClip.channels);
    ChangeStateTo(State.lipsync);
  }

  private void CreateClipFromBuffers()
  {
    if (src.clip == null)
      return;
    float[] pcm = PCM;
    src.clip = AudioClip.Create("Microphone", pcm.Length / src.clip.channels, src.clip.channels, src.clip.frequency, false, false);
    src.clip.SetData(pcm, 0);
  }

  private void Update()
  {
    if (state == State.rec_speech || state == State.rec_speech_realtime)
    {
      int position = Microphone.GetPosition(deviceName);
      if (position < m_cursor)
      {
        float[] audioBuffer1 = GetAudioBuffer(m_cursor, src.clip.samples);
        float[] audioBuffer2 = GetAudioBuffer(0, position);
        AddBuffer(audioBuffer1);
        AddBuffer(audioBuffer2);
        m_cursor = position;
      }
      else
      {
        if (position <= m_cursor)
          return;
        AddBuffer(GetAudioBuffer(m_cursor, position));
        m_cursor = position;
      }
    }
    else if (state == State.lipsync && !LipsyncPlugin.IsLipsyncing())
    {
      m_anno = LipsyncPlugin.GetLipsyncAnnoResult();
      ChangeStateTo(State.off);
    }
    else
    {
      if (state != State.cancel_lipsync || LipsyncPlugin.IsLipsyncing())
        return;
      m_anno = "";
      ChangeStateTo(State.off);
    }
  }

  private float[] GetAudioBuffer(int startPos, int endPos)
  {
    float[] data = new float[(endPos - startPos) * src.clip.channels];
    src.clip.GetData(data, startPos);
    return data;
  }

  private void AddBuffer(float[] buffer)
  {
    if (LipsyncAfterRecord)
      m_RecordBuffers.Add(buffer);
    if (!isRealtime)
      return;
    LipsyncPlugin.LipRTAddBuffer(buffer);
  }

  private void ChangeStateTo(State newState)
  {
    State state = this.state;
    this.state = newState;
    if (StateChangeListeners == null)
      return;
    StateChangeListeners(state, newState);
  }

  public delegate void StateChanged(State prev, State cur);

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
