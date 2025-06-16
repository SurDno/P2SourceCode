// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.FloatArrayView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public abstract class FloatArrayView : MonoBehaviour
  {
    public abstract void SetValue(int index, float value);

    public abstract void GetValue(int index, out float value);

    public abstract void SkipAnimation();
  }
}
