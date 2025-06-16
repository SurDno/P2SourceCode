using UnityEngine;

public class RandomMaterial : MonoBehaviour
{
  public Renderer targetRenderer;
  public Material[] materials;

  public void Start() => this.ChangeMaterial();

  public void ChangeMaterial()
  {
    this.targetRenderer.sharedMaterial = this.materials[Random.Range(0, this.materials.Length)];
  }
}
