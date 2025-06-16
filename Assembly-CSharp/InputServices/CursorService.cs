// Decompiled with JetBrains decompiler
// Type: InputServices.CursorService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace InputServices
{
  public static class CursorService
  {
    private static ICursorController instance;

    public static ICursorController Instance
    {
      get
      {
        if (CursorService.instance == null)
          CursorService.instance = (ICursorController) new WindowsCursorController();
        return CursorService.instance;
      }
    }
  }
}
