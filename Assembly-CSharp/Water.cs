using UnityEngine;

public class Water : MonoBehaviour
{
  private Vector2 _uvOffset = Vector2.zero;
  private Renderer _renderer;
  private Material material;

  private void Start()
  {
    this._renderer = this.GetComponent<Renderer>();
    this.material = this._renderer.materials[0];
  }

  private void Update()
  {
    this._uvOffset += new Vector2(0.051f, 0.091f) * Time.deltaTime;
    this.material.SetTextureOffset("_MainTex", this._uvOffset);
  }
}
