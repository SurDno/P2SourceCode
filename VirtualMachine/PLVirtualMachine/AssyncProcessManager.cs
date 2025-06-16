// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.AssyncProcessManager
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using PLVirtualMachine.Dynamic;
using System;
using System.Collections.Generic;

#nullable disable
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
