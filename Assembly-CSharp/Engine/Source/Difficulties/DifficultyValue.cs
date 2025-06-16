// Decompiled with JetBrains decompiler
// Type: Engine.Source.Difficulties.DifficultyValue
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Settings;

#nullable disable
namespace Engine.Source.Difficulties
{
  public class DifficultyValue : IValue<float>
  {
    public float Value { get; set; }

    public float DefaultValue { get; set; }

    public float MinValue { get; set; }

    public float MaxValue { get; set; }
  }
}
