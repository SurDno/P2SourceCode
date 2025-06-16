// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.IValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace Engine.Source.Settings
{
  public interface IValue<T>
  {
    T Value { get; set; }

    T DefaultValue { get; }

    T MinValue { get; }

    T MaxValue { get; }
  }
}
