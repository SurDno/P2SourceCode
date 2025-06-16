// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.Engine.EffectContextValueAssignment`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Expressions;
using Inspectors;
using Scripts.Expressions.Commons;

#nullable disable
namespace Engine.Source.Effects.Engine
{
  public abstract class EffectContextValueAssignment<T> : IEffectValueSetter where T : struct
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Name = "a", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValueSetter<T> a;
    [DataReadProxy(MemberEnum.None, Name = "Source")]
    [DataWriteProxy(MemberEnum.None, Name = "Source")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Name = "b", Mutable = true, Mode = ExecuteMode.Edit)]
    protected IValue<T> b;
    private ExpressionViewWrapper view = new ExpressionViewWrapper();

    [Inspected(Mode = ExecuteMode.EditAndRuntime)]
    private ExpressionViewWrapper ValueViewWrapper
    {
      get
      {
        this.view.Value = this.ValueView;
        return this.view;
      }
    }

    public abstract string ValueView { get; }

    public abstract string TypeView { get; }

    protected abstract T Compute(T a, T b);

    public void Compute(IEffect context)
    {
      if (this.b == null || this.a == null)
        return;
      this.a.SetValue(this.Compute(this.a.GetValue(context), this.b.GetValue(context)), context);
    }
  }
}
