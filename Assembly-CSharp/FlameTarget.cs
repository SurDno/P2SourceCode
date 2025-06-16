using System.Runtime.CompilerServices;
using UnityEngine;

public class FlameTarget : MonoBehaviour, IFlamable
{
  [SpecialName]
  GameObject IFlamable.get_gameObject() => this.gameObject;
}
