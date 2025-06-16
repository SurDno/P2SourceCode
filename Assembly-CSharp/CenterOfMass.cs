public class CenterOfMass : MonoBehaviour
{
  public Vector3 Offset = new Vector3();

  private void Start() => this.GetComponent<Rigidbody>().centerOfMass += Offset;
}
