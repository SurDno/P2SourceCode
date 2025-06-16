// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.EntityByTag
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints.Effects
{
  [Category("Engine")]
  public class EntityByTag : FlowControlNode
  {
    [Port("Tag")]
    private ValueInput<string> tagInput;

    [Port("Value")]
    private IEntity Value() => RegisterComponent.GetByTag(this.tagInput.value);
  }
}
