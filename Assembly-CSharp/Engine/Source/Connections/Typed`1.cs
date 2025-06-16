using Engine.Common;
using Inspectors;
using System;

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
