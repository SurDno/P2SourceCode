using Cofe.Serializations.Data;
using Engine.Common.Types;
using System;

namespace Engine.Common.Commons.Converters
{
  public static class EngineDataWriteUtility
  {
    public static void Write(IDataWriter writer, string name, LocalizedText value)
    {
      DefaultDataWriteUtility.Write(writer, name, value.Id);
    }

    public static void Write(IDataWriter writer, string name, Position value)
    {
      writer.Begin(name, (Type) null, true);
      DefaultDataWriteUtility.Write(writer, "X", value.X);
      DefaultDataWriteUtility.Write(writer, "Y", value.Y);
      writer.End(name, true);
    }
  }
}
