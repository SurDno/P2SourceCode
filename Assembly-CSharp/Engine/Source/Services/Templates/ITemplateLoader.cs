// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Templates.ITemplateLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Services.Templates
{
  public interface ITemplateLoader
  {
    int AsyncCount { get; }

    IEnumerator Load(Dictionary<Guid, IObject> items, Dictionary<Guid, string> names);
  }
}
