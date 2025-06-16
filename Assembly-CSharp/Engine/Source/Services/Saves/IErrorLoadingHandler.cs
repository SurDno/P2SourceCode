// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Saves.IErrorLoadingHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Engine.Source.Services.Saves
{
  public interface IErrorLoadingHandler
  {
    string ErrorLoading { get; }

    bool HasErrorLoading { get; }

    void LogError(string text);

    void LogException(Exception e);
  }
}
