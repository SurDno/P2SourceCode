// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Effects.OriginalExperienceNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Services;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints.Effects
{
  [Category("Effects")]
  public class OriginalExperienceNode : FlowControlNode
  {
    [Port("Value")]
    private bool Value()
    {
      VirtualMachineController service = ServiceLocator.GetService<VirtualMachineController>();
      return service == null || service.OriginalExperienceSession;
    }
  }
}
