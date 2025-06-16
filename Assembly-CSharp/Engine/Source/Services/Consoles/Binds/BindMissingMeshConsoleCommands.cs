using Cofe.Meta;
using UnityEngine;

namespace Engine.Source.Services.Consoles.Binds
{
  [Initialisable]
  public static class BindMissingMeshConsoleCommands
  {
    [ConsoleCommand("dump_missing_mesh")]
    private static string DumpMissingMesh(string command, ConsoleParameter[] parameters)
    {
      foreach (ParticleSystem context in Object.FindObjectsOfType<ParticleSystem>())
      {
        ParticleSystem.ShapeModule shape = context.shape;
        if (shape.shapeType == ParticleSystemShapeType.Mesh)
        {
          shape = context.shape;
          if ((Object) shape.mesh == (Object) null)
          {
            object[] objArray = new object[4]
            {
              (object) "Mesh not found, ParticleSystem , type : ",
              null,
              null,
              null
            };
            shape = context.shape;
            objArray[1] = (object) shape.shapeType;
            objArray[2] = (object) " , info : ";
            objArray[3] = (object) context.GetInfo();
            Debug.LogError((object) string.Concat(objArray), (Object) context);
          }
        }
        else
        {
          shape = context.shape;
          if (shape.shapeType == ParticleSystemShapeType.MeshRenderer)
          {
            shape = context.shape;
            if ((Object) shape.meshRenderer == (Object) null)
            {
              object[] objArray = new object[4]
              {
                (object) "Mesh not found, ParticleSystem , type : ",
                null,
                null,
                null
              };
              shape = context.shape;
              objArray[1] = (object) shape.shapeType;
              objArray[2] = (object) " , info : ";
              objArray[3] = (object) context.GetInfo();
              Debug.LogError((object) string.Concat(objArray), (Object) context);
            }
          }
          else
          {
            shape = context.shape;
            if (shape.shapeType == ParticleSystemShapeType.SkinnedMeshRenderer)
            {
              shape = context.shape;
              if ((Object) shape.skinnedMeshRenderer == (Object) null)
              {
                object[] objArray = new object[4]
                {
                  (object) "Mesh not found, ParticleSystem , type : ",
                  null,
                  null,
                  null
                };
                shape = context.shape;
                objArray[1] = (object) shape.shapeType;
                objArray[2] = (object) " , info : ";
                objArray[3] = (object) context.GetInfo();
                Debug.LogError((object) string.Concat(objArray), (Object) context);
              }
            }
          }
        }
      }
      foreach (MeshFilter context in Object.FindObjectsOfType<MeshFilter>())
      {
        if ((Object) context.mesh == (Object) null)
          Debug.LogError((object) ("Mesh not found, MeshFilter : " + context.GetInfo()), (Object) context);
      }
      return command;
    }
  }
}
