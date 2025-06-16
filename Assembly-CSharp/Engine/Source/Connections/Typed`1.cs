// Decompiled with JetBrains decompiler
// Type: Engine.Source.Connections.Typed`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Inspectors;
using System;

#nullable disable
namespace Engine.Source.Connections
{
  public struct Typed<T> where T : class, IObject
  {
    private Guid id;

    [Inspected]
    public Guid Id => this.id;

    public Typed(Guid id) => this.id = id;

    public T Value
    {
      get => TemplateUtility.GetTemplate<T>(this.Id);
      set => this.id = (object) value != null ? value.Id : Guid.Empty;
    }

    public override int GetHashCode() => this.id.GetHashCode();

    public override bool Equals(object a) => a is Typed<T> typed && this == typed;

    public static bool operator ==(Typed<T> a, Typed<T> b) => a.Id == b.Id;

    public static bool operator !=(Typed<T> a, Typed<T> b) => a.Id != b.Id;
  }
}
