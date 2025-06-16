using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class LipsyncPlugin
{
  private static string sProg = "";
  private static int iLastProg = -1;

  [DllImport("UnityLipsync")]
  private static extern int _StartLipsync(string szFile);

  [DllImport("UnityLipsync")]
  private static extern int StartLipsyncFromBuffer(
    IntPtr floatArrayPtr,
    int len,
    int sampleRate,
    int numChannels);

  public static int StartLipsyncFromBuffer(float[] pcm, int sampleRate, int numChannels)
  {
    GCHandle gcHandle = GCHandle.Alloc(pcm, GCHandleType.Pinned);
    int num = StartLipsyncFromBuffer(gcHandle.AddrOfPinnedObject(), pcm.Length, sampleRate, numChannels);
    gcHandle.Free();
    return num;
  }

  [DllImport("UnityLipsync")]
  private static extern int _StartTextBasedLipsync(string szFile, string sText);

  [DllImport("UnityLipsync")]
  private static extern int _SetLipsyncSmooth(int smooth);

  [DllImport("UnityLipsync")]
  private static extern int _LipsyncProgress();

  [DllImport("UnityLipsync")]
  private static extern int _LipsyncAnnoResult(StringBuilder szResult);

  [DllImport("UnityLipsync")]
  private static extern bool _IsLipsyncing();

  [DllImport("UnityLipsync")]
  private static extern void _CancelLipsync();

  public static int StartLipsync(string szFile) => _StartLipsync(szFile);

  public static int StartTextBasedLipsync(string szFile, string sText)
  {
    return _StartTextBasedLipsync(szFile, sText);
  }

  public static int LipsyncProgress() => _LipsyncProgress();

  public static bool IsLipsyncing() => _IsLipsyncing();

  public static void CancelLipsync() => _CancelLipsync();

  public static string GetLipsyncAnnoResult()
  {
    int capacity = _LipsyncAnnoResult(null);
    if (capacity <= 0)
      return "error not yet finished";
    StringBuilder szResult = new StringBuilder(capacity);
    _LipsyncAnnoResult(szResult);
    return szResult.ToString();
  }

  public static LipsyncSmooth Smoothness
  {
    get => (LipsyncSmooth) _SetLipsyncSmooth(-1);
    set => _SetLipsyncSmooth((int) value);
  }

  public static string GetProgressString()
  {
    int num = LipsyncProgress();
    if (num != iLastProg)
    {
      sProg = num.ToString();
      sProg += "%";
    }
    return sProg;
  }

  [DllImport("UnityLipsync")]
  public static extern void SetRtLatency(int latency);

  [DllImport("UnityLipsync")]
  public static extern void SetRtArticWindowMilli(int ms);

  [DllImport("UnityLipsync")]
  public static extern void LipRTStart(int sampleRate, int numChannels);

  [DllImport("UnityLipsync")]
  public static extern void LipRTStop();

  [DllImport("UnityLipsync")]
  private static extern void LipRTAddBuffer(IntPtr pcm, int len);

  public static void LipRTAddBuffer(float[] pcm)
  {
    GCHandle gcHandle = GCHandle.Alloc(pcm, GCHandleType.Pinned);
    LipRTAddBuffer(gcHandle.AddrOfPinnedObject(), pcm.Length);
    gcHandle.Free();
  }

  [DllImport("UnityLipsync")]
  private static extern int LipRTGetArticCount();

  [DllImport("UnityLipsync")]
  private static extern float LipRTGetArtItem(int i, StringBuilder outFileBuffer);

  [DllImport("UnityLipsync")]
  private static extern void LoadSpeechHmm(IntPtr pData, int len);

  public static int LipRTGetLipsync(out phone_weight[] phns)
  {
    int articCount = LipRTGetArticCount();
    if (articCount == 0)
    {
      phns = null;
      return 0;
    }
    phns = new phone_weight[articCount];
    for (int i = 0; i < articCount; ++i)
    {
      StringBuilder outFileBuffer = new StringBuilder(10);
      phns[i] = new phone_weight();
      phns[i].weight = LipRTGetArtItem(i, outFileBuffer);
      phns[i].phn = outFileBuffer.ToString();
    }
    return articCount;
  }

  public static void LoadSpeechHmm(TextAsset hmm)
  {
    byte[] bytes = hmm.bytes;
    GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
    LoadSpeechHmm(gcHandle.AddrOfPinnedObject(), bytes.Length);
    gcHandle.Free();
  }

  public enum LipsyncSmooth
  {
    Default,
    Tight,
    Tighter,
  }
}
