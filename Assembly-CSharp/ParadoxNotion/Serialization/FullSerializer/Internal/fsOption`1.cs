// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.fsOption`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public struct fsOption<T>
  {
    private bool _hasValue;
    private T _value;
    public static fsOption<T> Empty;

    public bool HasValue => this._hasValue;

    public bool IsEmpty => !this._hasValue;

    public T Value
    {
      get
      {
        if (this.IsEmpty)
          throw new InvalidOperationException("fsOption is empty");
        return this._value;
      }
    }

    public fsOption(T value)
    {
      this._hasValue = true;
      this._value = value;
    }
  }
}
