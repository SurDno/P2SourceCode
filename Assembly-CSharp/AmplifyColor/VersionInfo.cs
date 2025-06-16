using System;

namespace AmplifyColor
{
  [Serializable]
  public class VersionInfo
  {
    public const byte Major = 1;
    public const byte Minor = 6;
    public const byte Release = 1;
    private static string StageSuffix = "_dev002";
    private static string TrialSuffix = "";
    [SerializeField]
    private int m_major;
    [SerializeField]
    private int m_minor;
    [SerializeField]
    private int m_release;

    public static string StaticToString()
    {
      return string.Format("{0}.{1}.{2}", (byte) 1, (byte) 6, (byte) 1) + StageSuffix + TrialSuffix;
    }

    public override string ToString()
    {
      return string.Format("{0}.{1}.{2}", m_major, m_minor, m_release) + StageSuffix + TrialSuffix;
    }

    public int Number => m_major * 100 + m_minor * 10 + m_release;

    private VersionInfo()
    {
      m_major = 1;
      m_minor = 6;
      m_release = 1;
    }

    private VersionInfo(byte major, byte minor, byte release)
    {
      m_major = major;
      m_minor = minor;
      m_release = release;
    }

    public static VersionInfo Current() => new VersionInfo(1, 6, 1);

    public static bool Matches(VersionInfo version)
    {
      return 1 == version.m_major && 6 == version.m_minor && 1 == version.m_release;
    }
  }
}
