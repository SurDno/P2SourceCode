// Decompiled with JetBrains decompiler
// Type: LipsyncPluginW32
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Runtime.InteropServices;
using System.Text;

#nullable disable
public class LipsyncPluginW32
{
  [DllImport("UnityLipsync")]
  private static extern bool _OpenAudioFile(StringBuilder outFileBuffer);

  [DllImport("UnityLipsync")]
  public static extern bool _StartAudioPlayback(string audioFile);

  [DllImport("UnityLipsync")]
  public static extern bool _IsPlayingAudio();

  [DllImport("UnityLipsync")]
  public static extern int _GetMilliAudioTime();

  [DllImport("UnityLipsync")]
  public static extern void _StopPlayback();

  [DllImport("UnityLipsync")]
  public static extern void _StartRecording();

  [DllImport("UnityLipsync")]
  public static extern void _StopRecording();

  [DllImport("UnityLipsync")]
  public static extern bool _IsRecording();

  [DllImport("UnityLipsync")]
  public static extern void _CloseMicrophone();

  [DllImport("UnityLipsync")]
  private static extern int _GetLipsyncArtCount();

  [DllImport("UnityLipsync")]
  private static extern float _GetArticulationItem(int i, StringBuilder outFileBuffer);

  public static int GetLipsync(out phone_weight[] phns)
  {
    int lipsyncArtCount = LipsyncPluginW32._GetLipsyncArtCount();
    if (lipsyncArtCount == 0)
    {
      phns = (phone_weight[]) null;
      return 0;
    }
    phns = new phone_weight[lipsyncArtCount];
    for (int i = 0; i < lipsyncArtCount; ++i)
    {
      StringBuilder outFileBuffer = new StringBuilder(10);
      phns[i] = new phone_weight();
      phns[i].weight = LipsyncPluginW32._GetArticulationItem(i, outFileBuffer);
      phns[i].phn = outFileBuffer.ToString();
    }
    return lipsyncArtCount;
  }

  public static bool SelectDiskAudioFile(out string sFile)
  {
    StringBuilder outFileBuffer = new StringBuilder(512);
    if (LipsyncPluginW32._OpenAudioFile(outFileBuffer))
    {
      sFile = outFileBuffer.ToString();
      return true;
    }
    sFile = "";
    return false;
  }
}
