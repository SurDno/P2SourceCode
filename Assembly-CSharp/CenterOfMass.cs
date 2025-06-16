using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
  public Vector3 Offset;

  private void Start() => GetComponent<Rigidbody>().centerOfMass += Offset;
}
