// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Crowds.IndoorPointInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Inspectors;

#nullable disable
namespace Engine.Source.Components.Crowds
{
  public class IndoorPointInfo : PointInfo
  {
    [Inspected]
    public CrowdTemplateInfo TemplateInfo;
    [Inspected]
    public IEntity Template;
  }
}
