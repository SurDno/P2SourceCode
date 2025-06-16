// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.BindNpcConsoleCommands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Components;
using System;
using System.Linq;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindNpcConsoleCommands
  {
    [ConsoleCommand("create_npc")]
    private static string CreateNpc(string command, ConsoleParameter[] parameters)
    {
      return parameters.Length == 1 && parameters[0].Value == "?" ? command : BindNpcConsoleCommands.CreateNpc(new Guid("5718c1ab-f017-4b21-961b-fe00121f60b9"), new Guid("059ed7e4-e421-1e84-bbd1-41c4f51a80a0"), command);
    }

    [ConsoleCommand("create_npc2")]
    private static string CreateNpc2(string command, ConsoleParameter[] parameters)
    {
      return parameters.Length == 1 && parameters[0].Value == "?" ? command : BindNpcConsoleCommands.CreateNpc(new Guid("4273aa85-3d7b-4da4-695a-c3f397881eb2"), new Guid("059ed7e4-e421-1e84-bbd1-41c4f51a80a0"), command);
    }

    [ConsoleCommand("create_npc3")]
    private static string CreateNpc3(string command, ConsoleParameter[] parameters)
    {
      return parameters.Length == 1 && parameters[0].Value == "?" ? command : BindNpcConsoleCommands.CreateNpc(new Guid("db1b5913-5c15-2094-2b7e-b1b80444f996"), new Guid("059ed7e4-e421-1e84-bbd1-41c4f51a80a0"), command);
    }

    private static string CreateNpc(Guid entityId, Guid behaviorId, string command)
    {
      IEntity player = ServiceCache.Simulation.Player;
      if (player == null)
        return "Player not found";
      ITemplateService service = ServiceLocator.GetService<ITemplateService>();
      IBehaviorObject template = service.GetTemplate<IBehaviorObject>(behaviorId);
      IEntity entity = ServiceCache.Factory.Instantiate<IEntity>(service.GetTemplate<IEntity>(entityId));
      NavigationComponent component1 = entity.GetComponent<NavigationComponent>();
      if (component1 == null)
        return "NavigationComponent not found in npc";
      ServiceCache.Simulation.Add(entity, ServiceCache.Simulation.Hierarchy);
      DynamicModelComponent component2 = entity.GetComponent<DynamicModelComponent>();
      if (component2 == null)
        return "DynamicModelComponent not found in npc";
      component2.Model = component2.Models.FirstOrDefault<IModel>();
      component1.TeleportTo(player);
      BehaviorComponent component3 = entity.GetComponent<BehaviorComponent>();
      if (component3 == null)
        return "BehaviorComponent not found in npc";
      component3.BehaviorObject = template;
      return command;
    }
  }
}
