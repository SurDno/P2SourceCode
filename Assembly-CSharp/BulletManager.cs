// Decompiled with JetBrains decompiler
// Type: BulletManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class BulletManager : MonoBehaviour
{
  public GameObject bullet1;
  public GameObject bullet2;
  public GameObject bullet3;
  public GameObject bulletHands;

  public void Bullets_Reset()
  {
    this.bulletHands.SetActive(true);
    this.bullet1.SetActive(false);
    this.bullet2.SetActive(false);
    this.bullet3.SetActive(false);
  }

  public void Bullet_1_Show()
  {
    this.bullet1.SetActive(true);
    this.bulletHands.SetActive(false);
  }

  public void Unhide_Hands_Bullet_1() => this.bulletHands.SetActive(true);

  public void Bullet_2_Show()
  {
    this.bullet2.SetActive(true);
    this.bulletHands.SetActive(false);
  }

  public void Unhide_Hands_Bullet_2() => this.bulletHands.SetActive(true);

  public void Bullet_3_Show()
  {
    this.bullet3.SetActive(true);
    this.bulletHands.SetActive(false);
  }
}
