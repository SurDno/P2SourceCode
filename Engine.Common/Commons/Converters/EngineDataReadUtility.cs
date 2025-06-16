// Decompiled with JetBrains decompiler
// Type: Engine.Common.Commons.Converters.EngineDataReadUtility
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Engine.Common.Types;

#nullable disable
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
