// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Consoles.Binds.BindVmDataConsoleCommands
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services.Saves;
using System.Collections;

#nullable disable
namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public class BindVmDataConsoleCommands
  {
    [ConsoleCommand("load_vm_data")]
    private static string LoadVmData(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length != 1 || parameters.Length == 1 && parameters[0].Value == "?")
        return command + " project_name";
      VirtualMachineController service = ServiceLocator.GetService<VirtualMachineController>();
      if (service == null)
        return "VirtualMachineController not found";
      IVirtualMachine virtualMachine = service.VirtualMachine;
      if (virtualMachine == null)
        return "IVirtualMachine not found";
      if (virtualMachine.IsDataLoaded)
        virtualMachine.UnloadData();
      string projectName = parameters[0].Value;
      DefaultErrorLoadingHandler errorHandler = new DefaultErrorLoadingHandler();
      CoroutineService.Instance.Route(BindVmDataConsoleCommands.Load(projectName, service, (IErrorLoadingHandler) errorHandler));
      return errorHandler.HasErrorLoading ? "Error : " + errorHandler.ErrorLoading : command + " " + projectName;
    }

    private static IEnumerator Load(
      string projectName,
      VirtualMachineController virtualMachineController,
      IErrorLoadingHandler errorHandler)
    {
      yield return (object) virtualMachineController.LoadData(projectName, errorHandler);
    }

    [ConsoleCommand("unload_vm_data")]
    private static string UnloadVmData(string command, ConsoleParameter[] parameters)
    {
      if (parameters.Length != 0 || parameters.Length == 1 && parameters[0].Value == "?")
        return command;
      VirtualMachineController service = ServiceLocator.GetService<VirtualMachineController>();
      if (service == null)
        return "VirtualMachineController not found";
      IVirtualMachine virtualMachine = service.VirtualMachine;
      if (virtualMachine == null)
        return "IVirtualMachine not found";
      if (virtualMachine.IsDataLoaded)
        virtualMachine.UnloadData();
      return command;
    }
  }
}
