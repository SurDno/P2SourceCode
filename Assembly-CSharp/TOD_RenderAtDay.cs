using UnityEngine;

[RequireComponent(typeof (Renderer))]
public class TOD_RenderAtDay : MonoBehaviour
{
  private Renderer rendererComponent;

  protected void Start()
  {
    this.rendererComponent = this.GetComponent<Renderer>();
    this.rendererComponent.enabled = TOD_Sky.Instance.IsDay;
  }

  protected void Update() => this.rendererComponent.enabled = TOD_Sky.Instance.IsDay;
}
