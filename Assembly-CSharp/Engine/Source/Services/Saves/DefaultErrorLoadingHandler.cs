using System;
using UnityEngine;

namespace Engine.Source.Services.Saves
{
  public class DefaultErrorLoadingHandler : IErrorLoadingHandler
  {
    public string ErrorLoading { get; private set; }

    public bool HasErrorLoading => this.ErrorLoading != null;

    public void LogError(string text)
    {
      this.ErrorLoading = text;
      Debug.LogError((object) text);
    }

    public void LogException(Exception e)
    {
      this.ErrorLoading = e.ToString();
      Debug.LogException(e);
    }
  }
}
