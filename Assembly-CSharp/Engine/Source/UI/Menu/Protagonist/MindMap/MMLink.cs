// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Menu.Protagonist.MindMap.MMLink
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.MindMap;
using Engine.Impl.Services.Factories;
using Inspectors;

#nullable disable
namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  [Factory(typeof (IMMLink))]
  public class MMLink : IMMLink
  {
    [Inspected]
    public IMMNode Origin { get; set; }

    [Inspected]
    public IMMNode Target { get; set; }

    [Inspected]
    public MMLinkKind Kind { get; set; }
  }
}
