// Decompiled with JetBrains decompiler
// Type: Engine.Common.IEntity
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;
using System.Collections.Generic;

#nullable disable
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
