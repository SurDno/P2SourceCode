// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IMailComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Services.Mails;
using Engine.Common.Types;

#nullable disable
namespace Engine.Common.Components
{
  public interface IMailComponent : IComponent
  {
    MailStateEnum State { get; set; }

    LocalizedText Header { get; set; }

    LocalizedText Text { get; set; }
  }
}
