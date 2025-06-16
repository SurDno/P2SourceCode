using System;
using System.Collections.Generic;

namespace Engine.Common
{
  public interface IEntity : IObject, IDisposable
  {
    string Context { get; set; }

    bool IsDisposed { get; }

    bool IsEnabled { get; set; }

    bool IsEnabledInHierarchy { get; }

    bool DontSave { get; }

    string HierarchyPath { get; }

    IEnumerable<IComponent> Components { get; }

    T GetComponent<T>() where T : class, IComponent;

    IComponent GetComponent(Type type);

    IEntity Parent { get; }

    IEnumerable<IEntity> Childs { get; }
  }
}
