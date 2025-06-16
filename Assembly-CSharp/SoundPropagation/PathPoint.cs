// Decompiled with JetBrains decompiler
// Type: SoundPropagation.PathPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace SoundPropagation
{
  public struct PathPoint
  {
    public Vector3 Position;
    public Vector3 Direction;
    public float StepLength;
    public SPPortal Portal;
    public SPCell Cell;
  }
}
