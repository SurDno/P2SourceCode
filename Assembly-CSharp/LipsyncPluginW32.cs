rt("UnityLipsync")]
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
