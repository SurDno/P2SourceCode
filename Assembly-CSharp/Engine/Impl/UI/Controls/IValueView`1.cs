// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.IValueView`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public interface IValueView<T>
  {
    void SetValue(int id, T value, bool instant);

    T GetValue(int id);
  }
}
