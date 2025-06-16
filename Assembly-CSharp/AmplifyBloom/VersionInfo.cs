// Decompiled with JetBrains decompiler
// Type: AmplifyBloom.VersionInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace AmplifyBloom
{
  [Serializable]
  public class VersionInfo
  {
    public const byte Major = 1;
    public const byte Minor = 1;
    public const byte Release = 1;
    private static string StageSuffix = "_dev001";
    [SerializeField]
    private int m_major;
    [SerializeField]
    private int m_minor;
    [SerializeField]
    private int m_release;

    public static string StaticToString()
    {
      return string.Format("{0}.{1}.{2}", (object) (byte) 1, (object) (byte) 1, (object) (byte) 1) + VersionInfo.StageSuffix;
    }

    public override string ToString()
    {
      return string.Format("{0}.{1}.{2}", (object) this.m_major, (object) this.m_minor, (object) this.m_release) + VersionInfo.StageSuffix;
    }

    public int Number => this.m_major * 100 + this.m_minor * 10 + this.m_release;

    private VersionInfo()
    {
      this.m_major = 1;
      this.m_minor = 1;
      this.m_release = 1;
    }

    private VersionInfo(byte major, byte minor, byte release)
    {
      this.m_major = (int) major;
      this.m_minor = (int) minor;
      this.m_release = (int) release;
    }

    public static VersionInfo Current() => new VersionInfo((byte) 1, (byte) 1, (byte) 1);

    public static bool Matches(VersionInfo version)
    {
      return 1 == version.m_major && 1 == version.m_minor && 1 == version.m_release;
    }
  }
}
