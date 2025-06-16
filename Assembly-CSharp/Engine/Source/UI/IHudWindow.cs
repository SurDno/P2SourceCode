// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.IHudWindow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Source.Services.Notifications;

#nullable disable
namespace Engine.Source.UI
{
  public interface IHudWindow : IWindow
  {
    InteractableWindow InteractableInterface { get; }

    INotification Create(NotificationEnum type);

    void SetVisibility(bool visible, bool ignoreTextNotifications = false);
  }
}
