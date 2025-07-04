﻿using System.Collections.Generic;
using ParadoxNotion.Design;

namespace FlowCanvas.Nodes
{
  [Category("Functions/Lists")]
  public class GetListItem<T> : PureFunctionNode<T, IList<T>, int>
  {
    public override T Invoke(IList<T> list, int index)
    {
      try
      {
        return list[index];
      }
      catch
      {
        return default (T);
      }
    }
  }
}
