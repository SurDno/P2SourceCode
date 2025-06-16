using UnityEngine;

public class Random_movement : MonoBehaviour
{
  private Vector3 dir;

  private void Start()
  {
    this.dir = Quaternion.Euler(0.0f, Random.Range(0.0f, 360f), 0.0f) * Vector3.forward;
  }

  private void FixedUpdate() => this.transform.Translate(this.dir / 20f);
}
