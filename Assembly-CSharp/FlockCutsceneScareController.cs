// Decompiled with JetBrains decompiler
// Type: FlockCutsceneScareController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class FlockCutsceneScareController : MonoBehaviour
{
  public LandingSpotController landingSpotController;

  private void OnEnable() => this.StartCoroutine(this.Scare());

  private IEnumerator Scare()
  {
    if ((Object) this.landingSpotController != (Object) null)
    {
      yield return (object) new WaitForEndOfFrame();
      this.landingSpotController.ScareAll();
    }
  }
}
