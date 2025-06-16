// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.FocusModeEnabledNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class FocusModeEnabledNode : FlowControlNode
  {
    [Port("Enabled")]
    public bool IsEnabled()
    {
      QuestCompassService service = ServiceLocator.GetService<QuestCompassService>();
      return service != null && service.IsEnabled;
    }
  }
}
