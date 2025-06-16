// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.BindStorableConsoleCommands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindStorableConsoleCommands
  {
    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      ConsoleTargetService.AddTarget("-storable", (Func<string, object>) (value => (object) BindStorableConsoleCommands.Storables().FirstOrDefault<IStorableComponent>((Func<IStorableComponent, bool>) (o => o.Owner.Name == value))));
      EnumConsoleCommand.AddBind("-storable", (Func<string>) (() =>
      {
        List<IStorableComponent> list = BindStorableConsoleCommands.Storables().ToList<IStorableComponent>();
        list.Sort((Comparison<IStorableComponent>) ((a, b) => a.Owner.Name.CompareTo(b.Owner.Name)));
        string str = "\nStorables :\n";
        foreach (IStorableComponent storableComponent in list)
          str = str + storableComponent.Owner.Name + "\n";
        return str;
      }));
    }

    [ConsoleCommand("add_storable")]
    private static string StorableCommand(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length == 1 && parameters[0].Value == "?")
        return command + " [-player | -slot:index] -storable:name";
      IStorageComponent target1;
      IStorableComponent target2;
      if (parameters.Length == 1)
      {
        target1 = ConsoleTargetService.GetTarget(typeof (IStorageComponent), ConsoleParameter.Empty) as IStorageComponent;
        target2 = ConsoleTargetService.GetTarget(typeof (IStorableComponent), parameters[0]) as IStorableComponent;
      }
      else
      {
        if (parameters.Length != 2)
          return "Error parameter count";
        target1 = ConsoleTargetService.GetTarget(typeof (IStorageComponent), parameters[0]) as IStorageComponent;
        target2 = ConsoleTargetService.GetTarget(typeof (IStorableComponent), parameters[1]) as IStorableComponent;
      }
      if (target1 == null)
        return typeof (IStorageComponent).Name + " not found";
      if (target2 == null)
        return typeof (IStorableComponent).Name + " not found";
      IEntity entity = ServiceLocator.GetService<IFactory>().Instantiate<IEntity>(target2.Owner);
      ServiceLocator.GetService<ISimulation>().Add(entity, ServiceLocator.GetService<ISimulation>().Storables);
      IStorableComponent component = entity.GetComponent<IStorableComponent>();
      target1.AddItem(component, (IInventoryComponent) null);
      return "Add item : " + target2.Owner.Name + " , to storage : " + target1.Owner.Name;
    }

    private static IEnumerable<IStorableComponent> Storables()
    {
      return ServiceLocator.GetService<ITemplateService>().GetTemplates<IEntity>().Select<IEntity, IStorableComponent>((Func<IEntity, IStorableComponent>) (o => o.GetComponent<IStorableComponent>())).Where<IStorableComponent>((Func<IStorableComponent, bool>) (o => o != null));
    }
  }
}
