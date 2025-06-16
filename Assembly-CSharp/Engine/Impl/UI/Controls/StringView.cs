using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class StringView : MonoBehaviour
  {
    [SerializeField]
    private string stringValue;

    public string StringValue
    {
      get => this.stringValue;
      set
      {
        if (this.stringValue == value)
          return;
        this.stringValue = value;
        this.ApplyStringValue();
      }
    }

    private void OnValidate()
    {
      if (Application.isPlaying)
        return;
      this.ApplyStringValue();
      this.SkipAnimation();
    }

    protected abstract void ApplyStringValue();

    public abstract void SkipAnimation();
  }
}
