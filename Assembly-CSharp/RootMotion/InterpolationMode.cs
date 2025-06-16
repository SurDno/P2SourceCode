// Decompiled with JetBrains decompiler
// Type: RootMotion.InterpolationMode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace RootMotion
{
  [Serializable]
  public enum InterpolationMode
  {
    None,
    InOutCubic,
    InOutQuintic,
    InOutSine,
    InQuintic,
    InQuartic,
    InCubic,
    InQuadratic,
    InElastic,
    InElasticSmall,
    InElasticBig,
    InSine,
    InBack,
    OutQuintic,
    OutQuartic,
    OutCubic,
    OutInCubic,
    OutInQuartic,
    OutElastic,
    OutElasticSmall,
    OutElasticBig,
    OutSine,
    OutBack,
    OutBackCubic,
    OutBackQuartic,
    BackInCubic,
    BackInQuartic,
  }
}
