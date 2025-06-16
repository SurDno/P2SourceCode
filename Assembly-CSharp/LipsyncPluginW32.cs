using System.Runtime.InteropServices;
using System.Text;

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
    int lipsyncArtCount = _GetLipsyncArtCount();
    if (lipsyncArtCount == 0)
    {
      phns = null;
      return 0;
    }
    phns = new phone_weight[lipsyncArtCount];
    for (int i = 0; i < lipsyncArtCount; ++i)
    {
      StringBuilder outFileBuffer = new StringBuilder(10);
      phns[i] = new phone_weight();
      phns[i].weight = _GetArticulationItem(i, outFileBuffer);
      phns[i].phn = outFileBuffer.ToString();
    }
    return lipsyncArtCount;
  }

  public static bool SelectDiskAudioFile(out string sFile)
  {
    StringBuilder outFileBuffer = new StringBuilder(512);
    if (_OpenAudioFile(outFileBuffer))
    {
      sFile = outFileBuffer.ToString();
      return true;
    }
    sFile = "";
    return false;
  }
}
