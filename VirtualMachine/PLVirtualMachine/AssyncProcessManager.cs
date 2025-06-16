using PLVirtualMachine.Dynamic;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine
{
  public static class AssyncProcessManager
  {
    private static List<IAssyncUpdateable> updateableObjectsList = new List<IAssyncUpdateable>();

    public static void RegistrAssyncUpdateableObject(IAssyncUpdateable updateableObj)
    {
      AssyncProcessManager.updateableObjectsList.Add(updateableObj);
    }

    public static void Update(TimeSpan delta)
    {
      for (int index = 0; index < AssyncProcessManager.updateableObjectsList.Count; ++index)
      {
        if (AssyncProcessManager.updateableObjectsList[index].Active)
          AssyncProcessManager.updateableObjectsList[index].Update(delta);
      }
    }

    public static void Clear()
    {
      for (int index = 0; index < AssyncProcessManager.updateableObjectsList.Count; ++index)
        AssyncProcessManager.updateableObjectsList[index].Clear();
      AssyncProcessManager.updateableObjectsList.Clear();
    }
  }
}
