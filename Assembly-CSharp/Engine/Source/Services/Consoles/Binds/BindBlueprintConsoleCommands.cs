using Cofe.Meta;
using Cofe.Serializations.Converters;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Services;
using Engine.Source.Commons;
using FlowCanvas;
using NodeCanvas.Framework;
using System;
using System.Linq;
using UnityEngine;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public class BindBlueprintConsoleCommands
  {
    [Cofe.Meta.Initialise]
    private static void Initialise()
    {
      ConsoleTargetService.AddTarget("-blueprint", (Func<string, object>) (value => (object) ServiceLocator.GetService<ITemplateService>().GetTemplates<IBlueprintObject>().FirstOrDefault<IBlueprintObject>((Func<IBlueprintObject, bool>) (o => o.Name == value))));
      EnumConsoleCommand.AddBind("-blueprint", (Func<string>) (() =>
      {
        string str = "\nBlueprints :\n";
        foreach (IBlueprintObject template in ServiceLocator.GetService<ITemplateService>().GetTemplates<IBlueprintObject>())
          str = str + template.Name + "\n";
        return str;
      }));
    }

    [ConsoleCommand("create_blueprint")]
    private static string CreateBlueprint(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length == 1 && parameters[0].Value == "?")
        return command + " -slot:index -blueprint:name";
      int index = -1;
      if (parameters.Length != 2)
        return "Error parameters count";
      if (parameters[0].Parameter == "-slot")
        index = DefaultConverter.ParseInt(parameters[0].Value);
      if (!(ConsoleTargetService.GetTarget(typeof (IBlueprintObject), parameters[1]) is IBlueprintObject target))
        return "Blueprint not found";
      if (index < 0 || index >= ServiceLocator.GetService<SelectionService>().SelectionCount)
        return "Slot not found";
      GameObject gameObject = ((BlueprintObject) target).GameObject;
      if ((UnityEngine.Object) gameObject.GetComponent<FlowScriptController>() == (UnityEngine.Object) null)
        return typeof (FlowScriptController).Name + " not found";
      GameObject selection = UnityFactory.Instantiate(gameObject, "[Blueprints]");
      selection.name = gameObject.name;
      ServiceLocator.GetService<SelectionService>().SetSelection(index, (object) selection);
      selection.GetComponent<FlowScriptController>().StartBehaviour();
      return "Create blueprint : " + target.Name + " , in slot : " + (object) index;
    }

    [ConsoleCommand("blueprint_send_event")]
    private static string SendEvent(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length == 1 && parameters[0].Value == "?")
        return command + " -slot:index EventName";
      if (parameters.Length != 2)
        return "Error parameters count";
      GameObject target = ConsoleTargetService.GetTarget(typeof (GameObject), parameters[0]) as GameObject;
      string eventName = parameters[1].Value;
      if ((UnityEngine.Object) target == (UnityEngine.Object) null)
        return "Blueprint not found";
      FlowScriptController component = target.GetComponent<FlowScriptController>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        return typeof (FlowScriptController).Name + " not found";
      component.SendEvent(eventName);
      return "Blueprint send event : " + eventName;
    }

    [ConsoleCommand("blueprint_set_value")]
    private static string SetValue(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length == 0 || parameters.Length == 1 && parameters[0].Value == "?")
        return command + " -slot:index ValueName AnyTarget";
      if (parameters.Length != 3)
        return "Error parameters count";
      GameObject target1 = ConsoleTargetService.GetTarget(typeof (GameObject), parameters[0]) as GameObject;
      string name = parameters[1].Value;
      IObject target2 = ConsoleTargetService.GetTarget(typeof (IObject), parameters[2]) as IObject;
      if ((UnityEngine.Object) target1 == (UnityEngine.Object) null)
        return "Blueprint not found";
      Blackboard component = target1.GetComponent<Blackboard>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        return typeof (Blackboard).Name + " not found";
      component.SetValue(name, (object) target2);
      return "Blueprint set value : " + name;
    }
  }
}
