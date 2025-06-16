// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.Engine.EffectContextDetectTypeValueAssignment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Inspectors;

#nullable disable
namespace Engine.Source.Effects.Engine
{
  [TypeName(TypeName = "[a = b] : DetectType", MenuItem = "a = b/DetectType")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class EffectContextDetectTypeValueAssignment : EffectContextValueAssignment<DetectType>
  {
    protected override DetectType Compute(DetectType a, DetectType b) => b;

    public override string ValueView
    {
      get
      {
        return (this.a != null ? this.a.ValueView : "null") + " = " + (this.b != null ? this.b.ValueView : "null");
      }
    }

    public override string TypeView
    {
      get
      {
        return (this.a != null ? this.a.TypeView : "null") + " = " + (this.b != null ? this.b.TypeView : "null");
      }
    }
  }
}
