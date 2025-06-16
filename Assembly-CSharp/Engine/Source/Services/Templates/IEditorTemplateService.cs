// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Templates.IEditorTemplateService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using System;

#nullable disable
namespace Engine.Source.Services.Templates
{
  public interface IEditorTemplateService
  {
    IObject GetTemplate(Type type, Guid id);

    T GetTemplate<T>(Guid id) where T : class, IObject;

    T CreateTemplate<T>(string path, Guid id) where T : class, IObject;

    void DeleteTemplate(IObject template);

    void SetDirty(IObject template);

    void SaveTemplates();

    void SaveTemplate(IObject template);

    void Cleanup();
  }
}
