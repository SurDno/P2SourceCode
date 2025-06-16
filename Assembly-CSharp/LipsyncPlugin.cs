rt("UnityLipsync")]
  private static extern void LoadSpeechHmm(IntPtr pData, int len);

  public static int LipRTGetLipsync(out phone_weight[] phns)
  {
    int articCount = LipsyncPlugin.LipRTGetArticCount();
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
