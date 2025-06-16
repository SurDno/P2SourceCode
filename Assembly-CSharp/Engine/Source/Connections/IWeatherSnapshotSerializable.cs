using System;
using System.IO;
using Cofe.Serializations.Converters;
using Engine.Common.Weather;

namespace Engine.Source.Connections
{
  [Serializable]
  public struct IWeatherSnapshotSerializable : IEngineSerializable
  {
    [SerializeField]
    private string id;

    public Guid Id
    {
      get => DefaultConverter.ParseGuid(id);
      set => id = value != Guid.Empty ? value.ToString() : "";
    }

    public Type Type => typeof (IWeatherSnapshot);

    public IWeatherSnapshot Value
    {
      get => TemplateUtility.GetTemplate<IWeatherSnapshot>(Id);
      set => Id = value != null ? value.Id : Guid.Empty;
    }

    public override int GetHashCode() => string.IsNullOrEmpty(id) ? 0 : id.GetHashCode();

    public override bool Equals(object a)
    {
      return a is IWeatherSnapshotSerializable snapshotSerializable && this == snapshotSerializable;
    }

    public static bool operator ==(IWeatherSnapshotSerializable a, IWeatherSnapshotSerializable b)
    {
      return a.id == b.id;
    }

    public static bool operator !=(IWeatherSnapshotSerializable a, IWeatherSnapshotSerializable b)
    {
      return !(a == b);
    }

    public override string ToString()
    {
      IWeatherSnapshot weatherSnapshot = Value;
      return weatherSnapshot != null ? Path.GetFileNameWithoutExtension(weatherSnapshot.Source) : "";
    }
  }
}
