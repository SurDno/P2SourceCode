// Decompiled with JetBrains decompiler
// Type: Engine.Common.Services.IFactory
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using System;

#nullable disable
namespace Engine.Common.Services
{
  public interface IFactory
  {
    T Create<T>() where T : class;

    T Create<T>(Guid id) where T : class;

    T Instantiate<T>(T template) where T : class;

    T Instantiate<T>(T template, Guid id) where T : class;
  }
}
