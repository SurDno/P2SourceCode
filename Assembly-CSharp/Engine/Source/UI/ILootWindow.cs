﻿using Engine.Common.Components;

namespace Engine.Source.UI
{
  public interface ILootWindow : IWindow
  {
    IStorageComponent Actor { get; set; }

    IStorageComponent Target { get; set; }
  }
}
