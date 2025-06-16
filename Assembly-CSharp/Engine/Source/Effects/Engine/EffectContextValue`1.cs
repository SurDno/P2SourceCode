// Decompiled with JetBrains decompiler
// Type: Engine.Source.Effects.Engine.EffectContextValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Generator;
using Engine.Source.Commons.Effects;
using Engine.Source.Components;
using Expressions;
using Inspectors;

#nullable disable
namespace Engine.Source.Effects.Engine
{
  public abstract class EffectContextValue<T> : IValueSetter<T>, IValue<T> where T : struct
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected EffectContextEnum effectContext;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterNameEnum parameterName;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected ParameterDataEnum parameterData;

    public T GetValue(IEffect context)
    {
      IEntity entity = ExpressionEffectUtility.GetEntity(this.effectContext, context);
      if (entity != null)
      {
        ParametersComponent component = entity.GetComponent<ParametersComponent>();
        if (component != null)
        {
          IParameter<T> byName = component.GetByName<T>(this.parameterName);
          if (byName != null)
          {
            if (this.parameterData == ParameterDataEnum.BaseValue)
              return byName.BaseValue;
            if (this.parameterData == ParameterDataEnum.Value)
              return byName.Value;
            if (this.parameterData == ParameterDataEnum.MaxValue)
              return byName.MaxValue;
            if (this.parameterData == ParameterDataEnum.MinValue)
              return byName.MinValue;
          }
        }
      }
      return default (T);
    }

    public void SetValue(T value, IEffect context)
    {
      IEntity entity = ExpressionEffectUtility.GetEntity(this.effectContext, context);
      if (entity == null)
        return;
      ParametersComponent component = entity.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<T> byName = component.GetByName<T>(this.parameterName);
        if (byName != null)
        {
          if (this.parameterData == ParameterDataEnum.BaseValue)
            byName.BaseValue = value;
          else if (this.parameterData == ParameterDataEnum.Value)
            byName.Value = value;
          else if (this.parameterData == ParameterDataEnum.MaxValue)
            byName.MaxValue = value;
          else if (this.parameterData == ParameterDataEnum.MinValue)
            byName.MinValue = value;
        }
      }
    }

    public string ValueView
    {
      get
      {
        return this.effectContext.ToString() + "." + this.parameterName.ToString() + "." + this.parameterData.ToString();
      }
    }

    public string TypeView => TypeUtility.GetTypeName(this.GetType());
  }
}
