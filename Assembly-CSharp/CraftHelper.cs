// Decompiled with JetBrains decompiler
// Type: CraftHelper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
public class CraftHelper
{
  public static IStorableComponent GetCraftResult(
    List<IStorableComponent> ingredients,
    out CraftRecipe recipe)
  {
    foreach (CraftRecipe recipe1 in ((IEnumerable<CraftRecipesInfoData>) ScriptableObjectInstance<CraftRecipesData>.Instance.Data).Where<CraftRecipesInfoData>((Func<CraftRecipesInfoData, bool>) (o => (UnityEngine.Object) o != (UnityEngine.Object) null && o.Recipes != null)).SelectMany<CraftRecipesInfoData, CraftRecipe>((Func<CraftRecipesInfoData, IEnumerable<CraftRecipe>>) (o => (IEnumerable<CraftRecipe>) o.Recipes)))
    {
      IStorableComponent craftResult = CraftHelper.CheckRecipe(recipe1, ingredients);
      if (craftResult != null)
      {
        recipe = recipe1;
        return craftResult;
      }
    }
    recipe = (CraftRecipe) null;
    return (IStorableComponent) null;
  }

  public static IStorableComponent CheckRecipe(
    CraftRecipe recipe,
    List<IStorableComponent> ingredients)
  {
    List<IStorableComponent> recipeIngredients = new List<IStorableComponent>();
    IStorableComponent componentFromEntity1 = CraftHelper.GetStorableComponentFromEntity(recipe.Component1);
    if (componentFromEntity1 != null)
      recipeIngredients.Add(componentFromEntity1);
    IStorableComponent componentFromEntity2 = CraftHelper.GetStorableComponentFromEntity(recipe.Component2);
    if (componentFromEntity2 != null)
      recipeIngredients.Add(componentFromEntity2);
    IStorableComponent componentFromEntity3 = CraftHelper.GetStorableComponentFromEntity(recipe.Component3);
    if (componentFromEntity3 != null)
      recipeIngredients.Add(componentFromEntity3);
    if (ingredients.Count != recipeIngredients.Count)
      return (IStorableComponent) null;
    List<IStorableComponent> list = ingredients.ToList<IStorableComponent>();
    for (int i = 0; i < recipeIngredients.Count; i++)
    {
      IStorableComponent storableComponent = list.Find((Predicate<IStorableComponent>) (x => StorageUtility.GetItemId(x.Owner) == StorageUtility.GetItemId(recipeIngredients[i].Owner)));
      if (storableComponent == null)
        return (IStorableComponent) null;
      list.Remove(storableComponent);
    }
    return CraftHelper.GetStorableComponentFromEntity(recipe.Result);
  }

  public static IStorableComponent GetStorableComponentFromEntity(IEntitySerializable e)
  {
    return e.Value != null ? e.Value.GetComponent<IStorableComponent>() : (IStorableComponent) null;
  }

  public static IParameter<TimeSpan> GetCraftTimeParameter(IStorableComponent item)
  {
    return ParametersUtility.GetParameter<TimeSpan>((IComponent) item, ParameterNameEnum.CraftTime);
  }
}
