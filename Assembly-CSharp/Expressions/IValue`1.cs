// Decompiled with JetBrains decompiler
// Type: Expressions.IValue`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons.Effects;

#nullable disable
namespace Expressions
{
  public interface IValue<T> where T : struct
  {
    string ValueView { get; }

    string TypeView { get; }

    T GetValue(IEffect context);
  }
}
