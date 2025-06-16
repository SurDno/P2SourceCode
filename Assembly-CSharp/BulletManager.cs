using UnityEngine;

public class BulletManager : MonoBehaviour
{
  public GameObject bullet1;
  public GameObject bullet2;
  public GameObject bullet3;
  public GameObject bulletHands;

  public void Bullets_Reset()
  {
    bulletHands.SetActive(true);
    bullet1.SetActive(false);
    bullet2.SetActive(false);
    bullet3.SetActive(false);
  }

  public void Bullet_1_Show()
  {
    bullet1.SetActive(true);
    bulletHands.SetActive(false);
  }

  public void Unhide_Hands_Bullet_1() => bulletHands.SetActive(true);

  public void Bullet_2_Show()
  {
    bullet2.SetActive(true);
    bulletHands.SetActive(false);
  }

  public void Unhide_Hands_Bullet_2() => bulletHands.SetActive(true);

  public void Bullet_3_Show()
  {
    bullet3.SetActive(true);
    bulletHands.SetActive(false);
  }
}
