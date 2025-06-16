// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.ParameterListener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components.Parameters;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons.Parameters
{
  public abstract class ParameterListener
  {
    private IChangeParameterListener[] listeners;

    protected void ChangeParameterInvoke(IParameter parameter)
    {
      if (this.listeners == null)
        return;
      foreach (IChangeParameterListener listener in this.listeners)
      {
        try
        {
          listener.OnParameterChanged(parameter);
        }
        catch (Exception ex)
        {
          Debug.LogException(ex);
        }
      }
    }

    public void AddListener(IChangeParameterListener listener)
    {
      if (this.listeners == null)
      {
        this.listeners = new IChangeParameterListener[1]
        {
          listener
        };
      }
      else
      {
        if (Array.IndexOf<IChangeParameterListener>(this.listeners, listener) != -1)
          return;
        Array.Resize<IChangeParameterListener>(ref this.listeners, this.listeners.Length + 1);
        this.listeners[this.listeners.Length - 1] = listener;
      }
    }

    public void RemoveListener(IChangeParameterListener listener)
    {
      int index = this.listeners != null ? Array.IndexOf<IChangeParameterListener>(this.listeners, listener) : -1;
      if (index == -1)
        return;
      if (this.listeners.Length == 1)
      {
        this.listeners = (IChangeParameterListener[]) null;
      }
      else
      {
        this.listeners[index] = this.listeners[this.listeners.Length - 1];
        Array.Resize<IChangeParameterListener>(ref this.listeners, this.listeners.Length - 1);
      }
    }
  }
}
