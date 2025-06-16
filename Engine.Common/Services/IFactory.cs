using System;

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
