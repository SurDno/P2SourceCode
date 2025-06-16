using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SteamNative;

namespace Facepunch.Steamworks;

public class Inventory : IDisposable {
	public Action OnUpdate;
	public Item[] Items;
	public byte[] SerializedItems;
	public DateTime SerializedExpireTime;
	internal uint LastTimestamp;
	internal SteamInventory inventory;
	private Stopwatch fetchRetryTimer;
	public Definition[] Definitions;

	private bool IsServer { get; set; }

	internal Inventory(BaseSteamworks steamworks, SteamInventory c, bool server) {
		IsServer = server;
		inventory = c;
		Result.Pending = new Dictionary<int, Result>();
		inventory.LoadItemDefinitions();
		FetchItemDefinitions();
		if (server)
			return;
		SteamInventoryResultReady_t.RegisterCallback(steamworks, onResultReady);
		SteamInventoryFullUpdate_t.RegisterCallback(steamworks, onFullUpdate);
		Refresh();
	}

	private void onFullUpdate(SteamInventoryFullUpdate_t data, bool error) {
		if (error)
			return;
		var r = new Result(this, data.Handle, false);
		r.Fill();
		onResult(r, true);
	}

	private void onResultReady(SteamInventoryResultReady_t data, bool error) {
		if (!Result.Pending.ContainsKey(data.Handle))
			return;
		var r = Result.Pending[data.Handle];
		r.OnSteamResult(data, error);
		if (!error && data.Esult == SteamNative.Result.OK)
			onResult(r, false);
		Result.Pending.Remove(data.Handle);
	}

	private void onResult(Result r, bool serialize) {
		if (r.IsSuccess) {
			if (serialize) {
				if (r.Timestamp < LastTimestamp)
					return;
				SerializedItems = r.Serialize();
				SerializedExpireTime = DateTime.Now.Add(TimeSpan.FromMinutes(60.0));
			}

			LastTimestamp = r.Timestamp;
			ApplyResult(r);
		}

		r.Dispose();
		r = null;
	}

	internal void ApplyResult(Result r) {
		if (IsServer || !r.IsSuccess || r.Items == null)
			return;
		if (Items == null)
			Items = new Item[0];
		Items = Items.Union(r.Items).Distinct().Where(x => !r.Removed.Contains(x)).Where(x => !r.Consumed.Contains(x))
			.ToArray();
		var onUpdate = OnUpdate;
		if (onUpdate != null)
			onUpdate();
	}

	public void Dispose() {
		inventory = null;
		Items = null;
		SerializedItems = null;
		Result.Pending = null;
	}

	[Obsolete("No longer required, will be removed in a later version")]
	public void PlaytimeHeartbeat() { }

	public void Refresh() {
		if (IsServer)
			return;
		SteamInventoryResult_t pResultHandle = 0;
		if (inventory.GetAllItems(ref pResultHandle) && pResultHandle != -1)
			return;
		Console.WriteLine("GetAllItems failed!?");
	}

	public Definition CreateDefinition(int id) {
		return new Definition(inventory, id);
	}

	internal void FetchItemDefinitions() {
		var itemDefinitionIds = inventory.GetItemDefinitionIDs();
		if (itemDefinitionIds == null)
			return;
		Definitions = itemDefinitionIds.Select(x => CreateDefinition(x)).ToArray();
		foreach (var definition in Definitions)
			definition.Link(Definitions);
	}

	public void Update() {
		if (Definitions != null || (fetchRetryTimer != null && fetchRetryTimer.Elapsed.TotalSeconds < 10.0))
			return;
		if (fetchRetryTimer == null)
			fetchRetryTimer = Stopwatch.StartNew();
		fetchRetryTimer.Reset();
		fetchRetryTimer.Start();
		FetchItemDefinitions();
		inventory.LoadItemDefinitions();
	}

	public static float PriceCategoryToFloat(string price) {
		if (string.IsNullOrEmpty(price))
			return 0.0f;
		price = price.Replace("1;VLV", "");
		var result = 0;
		return !int.TryParse(price, out result) ? 0.0f : int.Parse(price) / 100f;
	}

	public Definition FindDefinition(int DefinitionId) {
		return Definitions == null ? null : Definitions.FirstOrDefault(x => x.Id == DefinitionId);
	}

	public unsafe Result Deserialize(byte[] data, int dataLength = -1) {
		if (dataLength == -1)
			dataLength = data.Length;
		SteamInventoryResult_t pOutResultHandle = -1;
		fixed (byte* pBuffer = data) {
			if (!inventory.DeserializeResult(ref pOutResultHandle, (IntPtr)pBuffer, (uint)dataLength, false) ||
			    pOutResultHandle == -1)
				return null;
			var result = new Result(this, pOutResultHandle, false);
			result.Fill();
			return result;
		}
	}

	public Result CraftItem(Item[] list, Definition target) {
		SteamInventoryResult_t pResultHandle = -1;
		var pArrayGenerate = new SteamItemDef_t[1] {
			new() { Value = target.Id }
		};
		var punArrayGenerateQuantity = new uint[1] { 1U };
		var array1 = list.Select<Item, SteamItemInstanceID_t>(x => x.Id).ToArray();
		var array2 = list.Select(x => 1U).ToArray();
		return !inventory.ExchangeItems(ref pResultHandle, pArrayGenerate, punArrayGenerateQuantity, 1U, array1, array2,
			(uint)array1.Length)
			? null
			: new Result(this, pResultHandle, true);
	}

	public Result CraftItem(Item.Amount[] list, Definition target) {
		SteamInventoryResult_t pResultHandle = -1;
		var pArrayGenerate = new SteamItemDef_t[1] {
			new() { Value = target.Id }
		};
		var punArrayGenerateQuantity = new uint[1] { 1U };
		var array1 = list.Select<Item.Amount, SteamItemInstanceID_t>(x => x.Item.Id).ToArray();
		var array2 = list.Select(x => (uint)x.Quantity).ToArray();
		return !inventory.ExchangeItems(ref pResultHandle, pArrayGenerate, punArrayGenerateQuantity, 1U, array1, array2,
			(uint)array1.Length)
			? null
			: new Result(this, pResultHandle, true);
	}

	public Result SplitStack(Item item, int quantity = 1) {
		SteamInventoryResult_t pResultHandle = -1;
		return !inventory.TransferItemQuantity(ref pResultHandle, item.Id, (uint)quantity, ulong.MaxValue)
			? null
			: new Result(this, pResultHandle, true);
	}

	public Result Stack(Item source, Item dest, int quantity = 1) {
		SteamInventoryResult_t pResultHandle = -1;
		return !inventory.TransferItemQuantity(ref pResultHandle, source.Id, (uint)quantity, dest.Id)
			? null
			: new Result(this, pResultHandle, true);
	}

	public Result GenerateItem(Definition target, int amount) {
		SteamInventoryResult_t pResultHandle = -1;
		var pArrayItemDefs = new SteamItemDef_t[1] {
			new() { Value = target.Id }
		};
		var punArrayQuantity = new uint[1] { (uint)amount };
		return !inventory.GenerateItems(ref pResultHandle, pArrayItemDefs, punArrayQuantity, 1U)
			? null
			: new Result(this, pResultHandle, true);
	}

	public class Definition {
		internal SteamInventory inventory;
		private Dictionary<string, string> customProperties;

		public int Id { get; private set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string IconUrl { get; set; }

		public string IconLargeUrl { get; set; }

		public string Type { get; set; }

		public string ExchangeSchema { get; set; }

		public Recipe[] Recipes { get; set; }

		public Recipe[] IngredientFor { get; set; }

		public DateTime Created { get; set; }

		public DateTime Modified { get; set; }

		public string PriceRaw { get; set; }

		public double PriceDollars { get; set; }

		public bool Marketable { get; set; }

		public bool IsGenerator => Type == "generator";

		internal Definition(SteamInventory i, int id) {
			inventory = i;
			Id = id;
			SetupCommonProperties();
		}

		public void SetProperty(string name, string value) {
			if (customProperties == null)
				customProperties = new Dictionary<string, string>();
			if (!customProperties.ContainsKey(name))
				customProperties.Add(name, value);
			else
				customProperties[name] = value;
		}

		public T GetProperty<T>(string name) {
			var stringProperty = GetStringProperty(name);
			if (string.IsNullOrEmpty(stringProperty))
				return default;
			try {
				return (T)Convert.ChangeType(stringProperty, typeof(T));
			} catch (Exception ex) {
				return default;
			}
		}

		public string GetStringProperty(string name) {
			var pchValueBuffer = string.Empty;
			if (customProperties != null && customProperties.ContainsKey(name))
				return customProperties[name];
			return !inventory.GetItemDefinitionProperty(Id, name, out pchValueBuffer) ? string.Empty : pchValueBuffer;
		}

		public bool GetBoolProperty(string name) {
			var stringProperty = GetStringProperty(name);
			return stringProperty.Length != 0 && stringProperty[0] != '0' && stringProperty[0] != 'F' &&
			       stringProperty[0] != 'f';
		}

		internal void SetupCommonProperties() {
			Name = GetStringProperty("name");
			Description = GetStringProperty("description");
			Created = GetProperty<DateTime>("timestamp");
			Modified = GetProperty<DateTime>("modified");
			ExchangeSchema = GetStringProperty("exchange");
			IconUrl = GetStringProperty("icon_url");
			IconLargeUrl = GetStringProperty("icon_url_large");
			Type = GetStringProperty("type");
			PriceRaw = GetStringProperty("price_category");
			Marketable = GetBoolProperty("marketable");
			if (string.IsNullOrEmpty(PriceRaw))
				return;
			PriceDollars = PriceCategoryToFloat(PriceRaw);
		}

		public void TriggerItemDrop() {
			SteamInventoryResult_t pResultHandle = 0;
			inventory.TriggerItemDrop(ref pResultHandle, Id);
			inventory.DestroyResult(pResultHandle);
		}

		internal void Link(Definition[] definitions) {
			LinkExchange(definitions);
		}

		private void LinkExchange(Definition[] definitions) {
			if (string.IsNullOrEmpty(ExchangeSchema))
				return;
			Recipes = ExchangeSchema.Split(new char[1] {
				';'
			}, StringSplitOptions.RemoveEmptyEntries).Select(x => Recipe.FromString(x, definitions, this)).ToArray();
		}

		internal void InRecipe(Recipe r) {
			if (IngredientFor == null)
				IngredientFor = new Recipe[0];
			IngredientFor = new List<Recipe>(IngredientFor) {
				r
			}.ToArray();
		}
	}

	public struct Recipe {
		public Definition Result;
		public Ingredient[] Ingredients;

		internal static Recipe FromString(
			string part,
			Definition[] definitions,
			Definition Result) {
			var r = new Recipe();
			r.Result = Result;
			var source = part.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			r.Ingredients = source.Select(x => Ingredient.FromString(x, definitions)).Where(x => x.DefinitionId != 0)
				.ToArray();
			foreach (var ingredient in r.Ingredients)
				if (ingredient.Definition != null)
					ingredient.Definition.InRecipe(r);
			return r;
		}

		public struct Ingredient {
			public int DefinitionId;
			public Definition Definition;
			public int Count;

			internal static Ingredient FromString(
				string part,
				Definition[] definitions) {
				var i = new Ingredient();
				i.Count = 1;
				try {
					if (part.Contains('x')) {
						var length = part.IndexOf('x');
						var result = 0;
						if (int.TryParse(part.Substring(length + 1), out result))
							i.Count = result;
						part = part.Substring(0, length);
					}

					i.DefinitionId = int.Parse(part);
					i.Definition = definitions.FirstOrDefault(x => x.Id == i.DefinitionId);
				} catch (Exception ex) {
					return i;
				}

				return i;
			}
		}
	}

	public struct Item : IEquatable<Item> {
		public ulong Id;
		public int Quantity;
		public int DefinitionId;
		public Definition Definition;
		public bool TradeLocked;

		public bool Equals(Item other) {
			return Equals(other, this);
		}

		public override bool Equals(object obj) {
			return obj != null && !(GetType() != obj.GetType()) && (long)((Item)obj).Id == (long)Id;
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public static bool operator ==(Item c1, Item c2) {
			return c1.Equals(c2);
		}

		public static bool operator !=(Item c1, Item c2) {
			return !c1.Equals(c2);
		}

		public struct Amount {
			public Item Item;
			public int Quantity;
		}
	}

	public class Result : IDisposable {
		internal static Dictionary<int, Result> Pending;
		internal Inventory inventory;
		public Action<Result> OnResult;
		protected bool _gotResult;

		private SteamInventoryResult_t Handle { get; set; }

		public Item[] Items { get; internal set; }

		public Item[] Removed { get; internal set; }

		public Item[] Consumed { get; internal set; }

		public bool IsPending {
			get {
				if (_gotResult)
					return false;
				if (Status() != Callbacks.Result.OK)
					return Status() == Callbacks.Result.Pending;
				Fill();
				return false;
			}
		}

		internal uint Timestamp { get; private set; }

		internal bool IsSuccess {
			get {
				if (Items != null)
					return true;
				return Handle != -1 && Status() == Callbacks.Result.OK;
			}
		}

		internal Callbacks.Result Status() {
			return Handle == -1
				? Callbacks.Result.InvalidParam
				: (Callbacks.Result)inventory.inventory.GetResultStatus(Handle);
		}

		internal Result(Inventory inventory, int Handle, bool pending) {
			if (pending)
				Pending.Add(Handle, this);
			this.Handle = Handle;
			this.inventory = inventory;
		}

		internal void Fill() {
			if (_gotResult || Items != null || Status() != Callbacks.Result.OK)
				return;
			_gotResult = true;
			Timestamp = inventory.inventory.GetResultTimestamp(Handle);
			var resultItems = inventory.inventory.GetResultItems(Handle);
			if (resultItems == null)
				return;
			Items = resultItems.Where(x => (x.Flags & 256) != 256 && (x.Flags & 512) != 512).Select(x => new Item {
				Quantity = x.Quantity,
				Id = x.ItemId,
				DefinitionId = x.Definition,
				TradeLocked = (x.Flags & 1U) > 0U,
				Definition = inventory.FindDefinition(x.Definition)
			}).ToArray();
			Removed = resultItems.Where(x => (x.Flags & 256U) > 0U).Select(x => new Item {
				Quantity = x.Quantity,
				Id = x.ItemId,
				DefinitionId = x.Definition,
				TradeLocked = (x.Flags & 1U) > 0U,
				Definition = inventory.FindDefinition(x.Definition)
			}).ToArray();
			Consumed = resultItems.Where(x => (x.Flags & 512U) > 0U).Select(x => new Item {
				Quantity = x.Quantity,
				Id = x.ItemId,
				DefinitionId = x.Definition,
				TradeLocked = (x.Flags & 1U) > 0U,
				Definition = inventory.FindDefinition(x.Definition)
			}).ToArray();
			if (OnResult == null)
				return;
			OnResult(this);
		}

		internal void OnSteamResult(SteamInventoryResultReady_t data, bool error) {
			if (data.Esult != SteamNative.Result.OK || error)
				return;
			Fill();
		}

		internal unsafe byte[] Serialize() {
			uint punOutBufferSize = 0;
			if (!inventory.inventory.SerializeResult(Handle, IntPtr.Zero, out punOutBufferSize))
				return null;
			var numArray = new byte[(int)punOutBufferSize];
			fixed (byte* pOutBuffer = numArray) {
				if (!inventory.inventory.SerializeResult(Handle, (IntPtr)pOutBuffer, out punOutBufferSize))
					return null;
			}

			return numArray;
		}

		public void Dispose() {
			inventory.inventory.DestroyResult(Handle);
			Handle = -1;
			inventory = null;
		}
	}
}