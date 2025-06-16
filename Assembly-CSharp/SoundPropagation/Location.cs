// Decompiled with JetBrains decompiler
// Type: SoundPropagation.Location
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace SoundPropagation
{
  public struct Location
  {
    public bool PathFound;
    public float PathLength;
    public Vector3 NearestCorner;
    public Filtering Filtering;
  }
}
