using System;
using System.IO;
using Cofe.Serializations.Converters;
using Engine.Common;

namespace Engine.Source.Connections
{
  [Serializable]
  public struct IEntitySerializable : IEngineSerializable
  {
    [SerializeField]
    private string id;

    public Guid Id
    {
      get => DefaultConverter.ParseGuid(id);
      set => id = value != Guid.Empty ? value.ToString() : "";
    }

    public Type Type => typeof (IEntity);

    public IEntity Value
    {
      get => TemplateUtility.GetTemplate<IEntity>(Id);
      set => Id = value != null ? value.Id : Guid.Empty;
    }

    public override int GetHashCode() => string.IsNullOrEmpty(id) ? 0 : id.GetHashCode();

    public override bool Equals(object a)
    {
      return a is IEntitySerializable ientitySerializable && this == ientitySerializable;
    }

    public static bool operator ==(IEntitySerializable a, IEntitySerializable b) => a.id == b.id;

    public static bool operator !=(IEntitySerializable a, IEntitySerializable b) => !(a == b);

    public override string ToString()
    {
      IEntity entity = Value;
      return entity != null ? Path.GetFileNameWithoutExtension(entity.Source) : "";
    }
  }
}
