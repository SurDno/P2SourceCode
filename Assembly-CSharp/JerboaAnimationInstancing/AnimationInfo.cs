// Decompiled with JetBrains decompiler
// Type: JerboaAnimationInstancing.AnimationInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace JerboaAnimationInstancing
{
  public class AnimationInfo
  {
    public string animationName;
    public int animationNameHash;
    public int totalFrame;
    public int fps;
    public int animationIndex;
    public int textureIndex;
    public bool rootMotion;
    public WrapMode wrapMode;
    public Vector3[] velocity;
    public Vector3[] angularVelocity;
    public List<AnimationEvent> eventList;
  }
}
