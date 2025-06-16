using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Engine.Impl.UI.Controls
{
  public class Progress : UIControl
  {
    [SerializeField]
    [FormerlySerializedAs("_Image")]
    private GameObject image = (GameObject) null;

    public Material Material { get; protected set; }

    public float Value
    {
      get => (Object) this.Material == (Object) null ? 0.0f : this.Material.GetFloat("_Progress");
      set
      {
        if ((Object) this.Material == (Object) null)
          return;
        this.Material.SetFloat("_Progress", value);
      }
    }

    public bool IsVisible
    {
      get => this.gameObject.activeSelf;
      set => this.gameObject.SetActive(value);
    }

    protected override void Awake()
    {
      base.Awake();
      if ((Object) this.image == (Object) null)
        return;
      RawImage component = this.image.GetComponent<RawImage>();
      if ((Object) component == (Object) null)
        return;
      this.Material = Object.Instantiate<Material>(component.material);
      component.material = this.Material;
    }
  }
}
