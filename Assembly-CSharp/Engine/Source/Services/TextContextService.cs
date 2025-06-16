// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.TextContextService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Converters;
using Engine.Common.Services;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (ITextContextService), typeof (TextContextService)})]
  public class TextContextService : ITextContextService
  {
    private Dictionary<string, string> contexts = new Dictionary<string, string>();

    public void SetInt(string tag, int value)
    {
      this.contexts[tag] = DefaultConverter.ToString(value);
    }

    public string ComputeText(string source)
    {
      foreach (KeyValuePair<string, string> context in this.contexts)
        source = source.Replace(context.Key, context.Value);
      return source;
    }
  }
}
