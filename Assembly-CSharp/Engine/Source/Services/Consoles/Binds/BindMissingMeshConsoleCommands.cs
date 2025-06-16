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
          if (shape.mesh == null)
          {
            object[] objArray = new object[4]
            {
              "Mesh not found, ParticleSystem , type : ",
              null,
              null,
              null
            };
            shape = context.shape;
            objArray[1] = shape.shapeType;
            objArray[2] = " , info : ";
            objArray[3] = context.GetInfo();
            Debug.LogError(string.Concat(objArray), context);
          }
        }
        else
        {
          shape = context.shape;
          if (shape.shapeType == ParticleSystemShapeType.MeshRenderer)
          {
            shape = context.shape;
            if (shape.meshRenderer == null)
            {
              object[] objArray = new object[4]
              {
                "Mesh not found, ParticleSystem , type : ",
                null,
                null,
                null
              };
              shape = context.shape;
              objArray[1] = shape.shapeType;
              objArray[2] = " , info : ";
              objArray[3] = context.GetInfo();
              Debug.LogError(string.Concat(objArray), context);
            }
          }
          else
          {
            shape = context.shape;
            if (shape.shapeType == ParticleSystemShapeType.SkinnedMeshRenderer)
            {
              shape = context.shape;
              if (shape.skinnedMeshRenderer == null)
              {
                object[] objArray = new object[4]
                {
                  "Mesh not found, ParticleSystem , type : ",
                  null,
                  null,
                  null
                };
                shape = context.shape;
                objArray[1] = shape.shapeType;
                objArray[2] = " , info : ";
                objArray[3] = context.GetInfo();
                Debug.LogError(string.Concat(objArray), context);
              }
            }
          }
        }
      }
      foreach (MeshFilter context in Object.FindObjectsOfType<MeshFilter>())
      {
        if (context.mesh == null)
          Debug.LogError("Mesh not found, MeshFilter : " + context.GetInfo(), context);
      }
      return command;
    }
  }
}
