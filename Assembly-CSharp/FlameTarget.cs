using System.Runtime.CompilerServices;

public class FlameTarget : MonoBehaviour, IFlamable
{
  [SpecialName]
  GameObject IFlamable.get_gameObject() => this.gameObject;
}
