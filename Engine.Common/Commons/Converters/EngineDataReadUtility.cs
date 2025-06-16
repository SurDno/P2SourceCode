using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common.Types;

namespace Engine.Common.Commons.Converters
{
  public static class EngineDataReadUtility
  {
    public static Position Read(IDataReader reader, string name, Position value)
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? Position.Zero : new Position(DefaultDataReadUtility.Read(child, "X", value.X), DefaultDataReadUtility.Read(child, "Y", value.Y));
    }

    public static LocalizedText Read(IDataReader reader, string name, LocalizedText value)
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? LocalizedText.Empty : new LocalizedText(DefaultConverter.ParseUlong(child.Read()));
    }
  }
}
