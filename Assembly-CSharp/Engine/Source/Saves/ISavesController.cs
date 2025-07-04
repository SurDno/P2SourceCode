﻿using System.Collections;
using System.Xml;
using Cofe.Serializations.Data;
using Engine.Source.Services.Saves;

namespace Engine.Source.Saves
{
  public interface ISavesController
  {
    IEnumerator Load(IErrorLoadingHandler errorHandler);

    IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler);

    void Unload();

    void Save(IDataWriter element, string context);
  }
}
