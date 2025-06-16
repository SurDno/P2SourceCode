// Decompiled with JetBrains decompiler
// Type: LandingSpot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

#nullable disable
public class LandingSpot : MonoBehaviour
{
  [HideInInspector]
  public FlockChild landingChild;
  [HideInInspector]
  public bool landing;
  private int lerpCounter;
  [HideInInspector]
  public LandingSpotController _controller;
  private bool _idle;
  public bool _gotcha;

  public void StraightenBird()
  {
    if ((double) this.landingChild.transform.eulerAngles.x == 0.0)
      return;
    this.landingChild.transform.eulerAngles = this.landingChild.transform.eulerAngles with
    {
      z = 0.0f
    };
  }

  public void RotateBird()
  {
    if (this._controller._randomRotate && this._idle)
      return;
    ++this.lerpCounter;
    Quaternion rotation = this.landingChild.transform.rotation;
    Vector3 eulerAngles = rotation.eulerAngles with
    {
      y = Mathf.LerpAngle(this.landingChild.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.y, (float) this.lerpCounter * Time.deltaTime * this._controller._landedRotateSpeed)
    };
    rotation.eulerAngles = eulerAngles;
    this.landingChild.transform.rotation = rotation;
  }

  public IEnumerator GetFlockChild(float minDelay, float maxDelay)
  {
    yield break;
  }

  public void InstantLand()
  {
  }

  public void ReleaseFlockChild()
  {
  }
}
