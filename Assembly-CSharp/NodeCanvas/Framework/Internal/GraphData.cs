// Decompiled with JetBrains decompiler
// Type: NodeCanvas.Framework.Internal.GraphData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace NodeCanvas.Framework.Internal
{
  [Serializable]
  public class GraphData
  {
    public List<NodeCanvas.Framework.Node> nodes = new List<NodeCanvas.Framework.Node>();
    public List<Connection> connections = new List<Connection>();
  }
}
