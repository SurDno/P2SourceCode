// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SingleColorView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public abstract class SingleColorView : MonoBehaviour, IValueView<Color>
  {
    [SerializeField]
    private Color value = Color.white;

    protected Color GetValue() => this.value;

    public Color GetValue(int id) => this.value;

    public void SetValue(int id, Color value, bool instant)
    {
      if (!instant && this.value == value)
        return;
      this.value = value;
      this.ApplyValue(instant);
    }

    protected abstract void ApplyValue(bool instant);
  }
}
