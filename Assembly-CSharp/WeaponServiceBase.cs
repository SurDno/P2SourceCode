using UnityEngine;

public class WeaponServiceBase : MonoBehaviour
{
  public virtual WeaponEnum Weapon { get; set; }

  public virtual Vector3 KnifeSpeed { get; }

  public virtual Vector3 KnifePosition { get; }
}
