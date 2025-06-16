// Decompiled with JetBrains decompiler
// Type: Engine.Common.Commons.Converters.EngineDataWriteUtility
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Cofe.Serializations.Data;
using Engine.Common.Types;
using System;

#nullable disable
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
