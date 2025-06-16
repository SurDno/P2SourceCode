using System;
using Engine.Common.Components.Parameters;

namespace Engine.Source.Commons.Parameters
{
  public abstract class ParameterListener
  {
    private IChangeParameterListener[] listeners;

    protected void ChangeParameterInvoke(IParameter parameter)
    {
      if (listeners == null)
        return;
      foreach (IChangeParameterListener listener in listeners)
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
      if (listeners == null)
      {
        listeners = new IChangeParameterListener[1]
        {
          listener
        };
      }
      else
      {
        if (Array.IndexOf(listeners, listener) != -1)
          return;
        Array.Resize(ref listeners, listeners.Length + 1);
        listeners[listeners.Length - 1] = listener;
      }
    }

    public void RemoveListener(IChangeParameterListener listener)
    {
      int index = listeners != null ? Array.IndexOf(listeners, listener) : -1;
      if (index == -1)
        return;
      if (listeners.Length == 1)
      {
        listeners = null;
      }
      else
      {
        listeners[index] = listeners[listeners.Length - 1];
        Array.Resize(ref listeners, listeners.Length - 1);
      }
    }
  }
}
