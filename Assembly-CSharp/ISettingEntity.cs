// Decompiled with JetBrains decompiler
// Type: ISettingEntity
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
public interface ISettingEntity : ISelectable
{
  bool Interactable { get; set; }

  void OnSelect();

  void OnDeSelect();

  void IncrementValue();

  void DecrementValue();

  bool IsActive();
}
