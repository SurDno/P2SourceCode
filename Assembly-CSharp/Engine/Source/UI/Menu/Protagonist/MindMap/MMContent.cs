using Engine.Common.MindMap;
using Engine.Common.Types;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System;

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
