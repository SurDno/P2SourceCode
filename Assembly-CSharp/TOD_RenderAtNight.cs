using UnityEngine;

[RequireComponent(typeof (Renderer))]
public class TOD_RenderAtNight : MonoBehaviour
{
  private Renderer rendererComponent;

  protected void Start()
  {
    this.rendererComponent = this.GetComponent<Renderer>();
    this.rendererComponent.enabled = TOD_Sky.Instance.IsNight;
  }

  protected void Update() => this.rendererComponent.enabled = TOD_Sky.Instance.IsNight;
}
