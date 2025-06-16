using System.Collections.Generic;
using UnityEngine;

namespace Cinemachine.Utility
{
  public class CinemachineGameWindowDebug
  {
    private static HashSet<Object> mClients;

    public static void ReleaseScreenPos(Object client)
    {
      if (CinemachineGameWindowDebug.mClients == null || !CinemachineGameWindowDebug.mClients.Contains(client))
        return;
      CinemachineGameWindowDebug.mClients.Remove(client);
    }

    public static Rect GetScreenPos(Object client, string text, GUIStyle style)
    {
      if (CinemachineGameWindowDebug.mClients == null)
        CinemachineGameWindowDebug.mClients = new HashSet<Object>();
      if (!CinemachineGameWindowDebug.mClients.Contains(client))
        CinemachineGameWindowDebug.mClients.Add(client);
      Vector2 position = new Vector2(0.0f, 0.0f);
      Vector2 size = style.CalcSize(new GUIContent(text));
      if (CinemachineGameWindowDebug.mClients != null)
      {
        foreach (Object mClient in CinemachineGameWindowDebug.mClients)
        {
          if (!(mClient == client))
            position.y += size.y;
          else
            break;
        }
      }
      return new Rect(position, size);
    }
  }
}
