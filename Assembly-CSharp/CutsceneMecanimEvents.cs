// Decompiled with JetBrains decompiler
// Type: CutsceneMecanimEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class CutsceneMecanimEvents : MonoBehaviour
{
  public event Action<string> OnEndAnimationEnd;

  public void AnimationEnd(string name)
  {
    Action<string> onEndAnimationEnd = this.OnEndAnimationEnd;
    if (onEndAnimationEnd == null)
      return;
    onEndAnimationEnd(name);
  }
}
