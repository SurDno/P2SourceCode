// Decompiled with JetBrains decompiler
// Type: LipsyncPlugin
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

#nullable disable
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
    GCHandle gcHandle = GCHandle.Alloc((object) pcm, GCHandleType.Pinned);
    int num = LipsyncPlugin.StartLipsyncFromBuffer(gcHandle.AddrOfPinnedObject(), pcm.Length, sampleRate, numChannels);
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

  public static int StartLipsync(string szFile) => LipsyncPlugin._StartLipsync(szFile);

  public static int StartTextBasedLipsync(string szFile, string sText)
  {
    return LipsyncPlugin._StartTextBasedLipsync(szFile, sText);
  }

  public static int LipsyncProgress() => LipsyncPlugin._LipsyncProgress();

  public static bool IsLipsyncing() => LipsyncPlugin._IsLipsyncing();

  public static void CancelLipsync() => LipsyncPlugin._CancelLipsync();

  public static string GetLipsyncAnnoResult()
  {
    int capacity = LipsyncPlugin._LipsyncAnnoResult((StringBuilder) null);
    if (capacity <= 0)
      return "error not yet finished";
    StringBuilder szResult = new StringBuilder(capacity);
    LipsyncPlugin._LipsyncAnnoResult(szResult);
    return szResult.ToString();
  }

  public static LipsyncPlugin.LipsyncSmooth Smoothness
  {
    get => (LipsyncPlugin.LipsyncSmooth) LipsyncPlugin._SetLipsyncSmooth(-1);
    set => LipsyncPlugin._SetLipsyncSmooth((int) value);
  }

  public static string GetProgressString()
  {
    int num = LipsyncPlugin.LipsyncProgress();
    if (num != LipsyncPlugin.iLastProg)
    {
      LipsyncPlugin.sProg = num.ToString();
      LipsyncPlugin.sProg += "%";
    }
    return LipsyncPlugin.sProg;
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
    GCHandle gcHandle = GCHandle.Alloc((object) pcm, GCHandleType.Pinned);
    LipsyncPlugin.LipRTAddBuffer(gcHandle.AddrOfPinnedObject(), pcm.Length);
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
    int articCount = LipsyncPlugin.LipRTGetArticCount();
    if (articCount == 0)
    {
      phns = (phone_weight[]) null;
      return 0;
    }
    phns = new phone_weight[articCount];
    for (int i = 0; i < articCount; ++i)
    {
      StringBuilder outFileBuffer = new StringBuilder(10);
      phns[i] = new phone_weight();
      phns[i].weight = LipsyncPlugin.LipRTGetArtItem(i, outFileBuffer);
      phns[i].phn = outFileBuffer.ToString();
    }
    return articCount;
  }

  public static void LoadSpeechHmm(TextAsset hmm)
  {
    byte[] bytes = hmm.bytes;
    GCHandle gcHandle = GCHandle.Alloc((object) bytes, GCHandleType.Pinned);
    LipsyncPlugin.LoadSpeechHmm(gcHandle.AddrOfPinnedObject(), bytes.Length);
    gcHandle.Free();
  }

  public enum LipsyncSmooth
  {
    Default,
    Tight,
    Tighter,
  }
}
