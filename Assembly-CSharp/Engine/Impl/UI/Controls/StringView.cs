// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.StringView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
