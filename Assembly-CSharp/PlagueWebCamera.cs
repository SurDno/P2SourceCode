public class PlagueWebCamera : MonoBehaviour
{
  private void Update() => PlagueWeb.Instance.CameraPosition = this.transform.position;
}
