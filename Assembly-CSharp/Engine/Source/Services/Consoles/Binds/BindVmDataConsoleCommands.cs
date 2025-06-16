using System.Collections;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Services.Saves;

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
      CoroutineService.Instance.Route(Load(projectName, service, errorHandler));
      return errorHandler.HasErrorLoading ? "Error : " + errorHandler.ErrorLoading : command + " " + projectName;
    }

    private static IEnumerator Load(
      string projectName,
      VirtualMachineController virtualMachineController,
      IErrorLoadingHandler errorHandler)
    {
      yield return virtualMachineController.LoadData(projectName, errorHandler);
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
