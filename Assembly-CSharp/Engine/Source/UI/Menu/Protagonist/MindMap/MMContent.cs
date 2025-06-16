// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.MindMap.MMContent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.MindMap;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System;

#nullable disable
namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  [Factory(typeof (IMMContent))]
  public class MMContent : IMMContent, IIdSetter
  {
    [Inspected]
    public Guid Id { get; set; }

    [Inspected]
    public LocalizedText Description { get; set; }

    [Inspected]
    public MMContentKind Kind { get; set; }

    [Inspected]
    public IMMPlaceholder Placeholder { get; set; }
  }
}
