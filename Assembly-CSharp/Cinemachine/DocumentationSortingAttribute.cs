// Decompiled with JetBrains decompiler
// Type: Cinemachine.DocumentationSortingAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace Cinemachine
{
  [DocumentationSorting(0.0f, DocumentationSortingAttribute.Level.Undoc)]
  public sealed class DocumentationSortingAttribute : Attribute
  {
    public float SortOrder { get; private set; }

    public DocumentationSortingAttribute.Level Category { get; private set; }

    public DocumentationSortingAttribute(
      float sortOrder,
      DocumentationSortingAttribute.Level category)
    {
      this.SortOrder = sortOrder;
      this.Category = category;
    }

    public enum Level
    {
      Undoc,
      API,
      UserRef,
    }
  }
}
