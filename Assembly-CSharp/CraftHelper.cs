using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons.Parameters;
using Engine.Source.Components.Utilities;
using Engine.Source.Connections;

public class CraftHelper {
	public static IStorableComponent GetCraftResult(
		List<IStorableComponent> ingredients,
		out CraftRecipe recipe) {
		foreach (var recipe1 in ScriptableObjectInstance<CraftRecipesData>.Instance.Data
			         .Where(o => o != null && o.Recipes != null).SelectMany(o => o.Recipes)) {
			var craftResult = CheckRecipe(recipe1, ingredients);
			if (craftResult != null) {
				recipe = recipe1;
				return craftResult;
			}
		}

		recipe = null;
		return null;
	}

	public static IStorableComponent CheckRecipe(
		CraftRecipe recipe,
		List<IStorableComponent> ingredients) {
		var recipeIngredients = new List<IStorableComponent>();
		var componentFromEntity1 = GetStorableComponentFromEntity(recipe.Component1);
		if (componentFromEntity1 != null)
			recipeIngredients.Add(componentFromEntity1);
		var componentFromEntity2 = GetStorableComponentFromEntity(recipe.Component2);
		if (componentFromEntity2 != null)
			recipeIngredients.Add(componentFromEntity2);
		var componentFromEntity3 = GetStorableComponentFromEntity(recipe.Component3);
		if (componentFromEntity3 != null)
			recipeIngredients.Add(componentFromEntity3);
		if (ingredients.Count != recipeIngredients.Count)
			return null;
		var list = ingredients.ToList();
		for (var i = 0; i < recipeIngredients.Count; i++) {
			var storableComponent = list.Find(x =>
				StorageUtility.GetItemId(x.Owner) == StorageUtility.GetItemId(recipeIngredients[i].Owner));
			if (storableComponent == null)
				return null;
			list.Remove(storableComponent);
		}

		return GetStorableComponentFromEntity(recipe.Result);
	}

	public static IStorableComponent GetStorableComponentFromEntity(IEntitySerializable e) {
		return e.Value != null ? e.Value.GetComponent<IStorableComponent>() : null;
	}

	public static IParameter<TimeSpan> GetCraftTimeParameter(IStorableComponent item) {
		return ParametersUtility.GetParameter<TimeSpan>(item, ParameterNameEnum.CraftTime);
	}
}