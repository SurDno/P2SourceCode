// Decompiled with JetBrains decompiler
// Type: Facepunch.Steamworks.Inventory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using SteamNative;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#nullable disable
namespace Facepunch.Steamworks
{
  public class Inventory : IDisposable
  {
    public Action OnUpdate;
    public Inventory.Item[] Items;
    public byte[] SerializedItems;
    public DateTime SerializedExpireTime;
    internal uint LastTimestamp = 0;
    internal SteamInventory inventory;
    private Stopwatch fetchRetryTimer;
    public Inventory.Definition[] Definitions;

    private bool IsServer { get; set; }

    internal Inventory(BaseSteamworks steamworks, SteamInventory c, bool server)
    {
      this.IsServer = server;
      this.inventory = c;
      Inventory.Result.Pending = new Dictionary<int, Inventory.Result>();
      this.inventory.LoadItemDefinitions();
      this.FetchItemDefinitions();
      if (server)
        return;
      SteamInventoryResultReady_t.RegisterCallback(steamworks, new Action<SteamInventoryResultReady_t, bool>(this.onResultReady));
      SteamInventoryFullUpdate_t.RegisterCallback(steamworks, new Action<SteamInventoryFullUpdate_t, bool>(this.onFullUpdate));
      this.Refresh();
    }

    private void onFullUpdate(SteamInventoryFullUpdate_t data, bool error)
    {
      if (error)
        return;
      Inventory.Result r = new Inventory.Result(this, data.Handle, false);
      r.Fill();
      this.onResult(r, true);
    }

    private void onResultReady(SteamInventoryResultReady_t data, bool error)
    {
      if (!Inventory.Result.Pending.ContainsKey(data.Handle))
        return;
      Inventory.Result r = Inventory.Result.Pending[data.Handle];
      r.OnSteamResult(data, error);
      if (!error && data.Esult == SteamNative.Result.OK)
        this.onResult(r, false);
      Inventory.Result.Pending.Remove(data.Handle);
    }

    private void onResult(Inventory.Result r, bool serialize)
    {
      if (r.IsSuccess)
      {
        if (serialize)
        {
          if (r.Timestamp < this.LastTimestamp)
            return;
          this.SerializedItems = r.Serialize();
          this.SerializedExpireTime = DateTime.Now.Add(TimeSpan.FromMinutes(60.0));
        }
        this.LastTimestamp = r.Timestamp;
        this.ApplyResult(r);
      }
      r.Dispose();
      r = (Inventory.Result) null;
    }

    internal void ApplyResult(Inventory.Result r)
    {
      if (this.IsServer || !r.IsSuccess || r.Items == null)
        return;
      if (this.Items == null)
        this.Items = new Inventory.Item[0];
      this.Items = ((IEnumerable<Inventory.Item>) this.Items).Union<Inventory.Item>((IEnumerable<Inventory.Item>) r.Items).Distinct<Inventory.Item>().Where<Inventory.Item>((Func<Inventory.Item, bool>) (x => !((IEnumerable<Inventory.Item>) r.Removed).Contains<Inventory.Item>(x))).Where<Inventory.Item>((Func<Inventory.Item, bool>) (x => !((IEnumerable<Inventory.Item>) r.Consumed).Contains<Inventory.Item>(x))).ToArray<Inventory.Item>();
      Action onUpdate = this.OnUpdate;
      if (onUpdate != null)
        onUpdate();
    }

    public void Dispose()
    {
      this.inventory = (SteamInventory) null;
      this.Items = (Inventory.Item[]) null;
      this.SerializedItems = (byte[]) null;
      Inventory.Result.Pending = (Dictionary<int, Inventory.Result>) null;
    }

    [Obsolete("No longer required, will be removed in a later version")]
    public void PlaytimeHeartbeat()
    {
    }

    public void Refresh()
    {
      if (this.IsServer)
        return;
      SteamInventoryResult_t pResultHandle = (SteamInventoryResult_t) 0;
      if (this.inventory.GetAllItems(ref pResultHandle) && (int) pResultHandle != -1)
        return;
      Console.WriteLine("GetAllItems failed!?");
    }

    public Inventory.Definition CreateDefinition(int id)
    {
      return new Inventory.Definition(this.inventory, id);
    }

    internal void FetchItemDefinitions()
    {
      SteamItemDef_t[] itemDefinitionIds = this.inventory.GetItemDefinitionIDs();
      if (itemDefinitionIds == null)
        return;
      this.Definitions = ((IEnumerable<SteamItemDef_t>) itemDefinitionIds).Select<SteamItemDef_t, Inventory.Definition>((Func<SteamItemDef_t, Inventory.Definition>) (x => this.CreateDefinition((int) x))).ToArray<Inventory.Definition>();
      foreach (Inventory.Definition definition in this.Definitions)
        definition.Link(this.Definitions);
    }

    public void Update()
    {
      if (this.Definitions != null || this.fetchRetryTimer != null && this.fetchRetryTimer.Elapsed.TotalSeconds < 10.0)
        return;
      if (this.fetchRetryTimer == null)
        this.fetchRetryTimer = Stopwatch.StartNew();
      this.fetchRetryTimer.Reset();
      this.fetchRetryTimer.Start();
      this.FetchItemDefinitions();
      this.inventory.LoadItemDefinitions();
    }

    public static float PriceCategoryToFloat(string price)
    {
      if (string.IsNullOrEmpty(price))
        return 0.0f;
      price = price.Replace("1;VLV", "");
      int result = 0;
      return !int.TryParse(price, out result) ? 0.0f : (float) int.Parse(price) / 100f;
    }

    public Inventory.Definition FindDefinition(int DefinitionId)
    {
      return this.Definitions == null ? (Inventory.Definition) null : ((IEnumerable<Inventory.Definition>) this.Definitions).FirstOrDefault<Inventory.Definition>((Func<Inventory.Definition, bool>) (x => x.Id == DefinitionId));
    }

    public unsafe Inventory.Result Deserialize(byte[] data, int dataLength = -1)
    {
      if (dataLength == -1)
        dataLength = data.Length;
      SteamInventoryResult_t pOutResultHandle = (SteamInventoryResult_t) -1;
      fixed (byte* pBuffer = data)
      {
        if (!this.inventory.DeserializeResult(ref pOutResultHandle, (IntPtr) (void*) pBuffer, (uint) dataLength, false) || (int) pOutResultHandle == -1)
          return (Inventory.Result) null;
        Inventory.Result result = new Inventory.Result(this, (int) pOutResultHandle, false);
        result.Fill();
        return result;
      }
    }

    public Inventory.Result CraftItem(Inventory.Item[] list, Inventory.Definition target)
    {
      SteamInventoryResult_t pResultHandle = (SteamInventoryResult_t) -1;
      SteamItemDef_t[] pArrayGenerate = new SteamItemDef_t[1]
      {
        new SteamItemDef_t() { Value = target.Id }
      };
      uint[] punArrayGenerateQuantity = new uint[1]{ 1U };
      SteamItemInstanceID_t[] array1 = ((IEnumerable<Inventory.Item>) list).Select<Inventory.Item, SteamItemInstanceID_t>((Func<Inventory.Item, SteamItemInstanceID_t>) (x => (SteamItemInstanceID_t) x.Id)).ToArray<SteamItemInstanceID_t>();
      uint[] array2 = ((IEnumerable<Inventory.Item>) list).Select<Inventory.Item, uint>((Func<Inventory.Item, uint>) (x => 1U)).ToArray<uint>();
      return !this.inventory.ExchangeItems(ref pResultHandle, pArrayGenerate, punArrayGenerateQuantity, 1U, array1, array2, (uint) array1.Length) ? (Inventory.Result) null : new Inventory.Result(this, (int) pResultHandle, true);
    }

    public Inventory.Result CraftItem(Inventory.Item.Amount[] list, Inventory.Definition target)
    {
      SteamInventoryResult_t pResultHandle = (SteamInventoryResult_t) -1;
      SteamItemDef_t[] pArrayGenerate = new SteamItemDef_t[1]
      {
        new SteamItemDef_t() { Value = target.Id }
      };
      uint[] punArrayGenerateQuantity = new uint[1]{ 1U };
      SteamItemInstanceID_t[] array1 = ((IEnumerable<Inventory.Item.Amount>) list).Select<Inventory.Item.Amount, SteamItemInstanceID_t>((Func<Inventory.Item.Amount, SteamItemInstanceID_t>) (x => (SteamItemInstanceID_t) x.Item.Id)).ToArray<SteamItemInstanceID_t>();
      uint[] array2 = ((IEnumerable<Inventory.Item.Amount>) list).Select<Inventory.Item.Amount, uint>((Func<Inventory.Item.Amount, uint>) (x => (uint) x.Quantity)).ToArray<uint>();
      return !this.inventory.ExchangeItems(ref pResultHandle, pArrayGenerate, punArrayGenerateQuantity, 1U, array1, array2, (uint) array1.Length) ? (Inventory.Result) null : new Inventory.Result(this, (int) pResultHandle, true);
    }

    public Inventory.Result SplitStack(Inventory.Item item, int quantity = 1)
    {
      SteamInventoryResult_t pResultHandle = (SteamInventoryResult_t) -1;
      return !this.inventory.TransferItemQuantity(ref pResultHandle, (SteamItemInstanceID_t) item.Id, (uint) quantity, (SteamItemInstanceID_t) ulong.MaxValue) ? (Inventory.Result) null : new Inventory.Result(this, (int) pResultHandle, true);
    }

    public Inventory.Result Stack(Inventory.Item source, Inventory.Item dest, int quantity = 1)
    {
      SteamInventoryResult_t pResultHandle = (SteamInventoryResult_t) -1;
      return !this.inventory.TransferItemQuantity(ref pResultHandle, (SteamItemInstanceID_t) source.Id, (uint) quantity, (SteamItemInstanceID_t) dest.Id) ? (Inventory.Result) null : new Inventory.Result(this, (int) pResultHandle, true);
    }

    public Inventory.Result GenerateItem(Inventory.Definition target, int amount)
    {
      SteamInventoryResult_t pResultHandle = (SteamInventoryResult_t) -1;
      SteamItemDef_t[] pArrayItemDefs = new SteamItemDef_t[1]
      {
        new SteamItemDef_t() { Value = target.Id }
      };
      uint[] punArrayQuantity = new uint[1]{ (uint) amount };
      return !this.inventory.GenerateItems(ref pResultHandle, pArrayItemDefs, punArrayQuantity, 1U) ? (Inventory.Result) null : new Inventory.Result(this, (int) pResultHandle, true);
    }

    public class Definition
    {
      internal SteamInventory inventory;
      private Dictionary<string, string> customProperties;

      public int Id { get; private set; }

      public string Name { get; set; }

      public string Description { get; set; }

      public string IconUrl { get; set; }

      public string IconLargeUrl { get; set; }

      public string Type { get; set; }

      public string ExchangeSchema { get; set; }

      public Inventory.Recipe[] Recipes { get; set; }

      public Inventory.Recipe[] IngredientFor { get; set; }

      public DateTime Created { get; set; }

      public DateTime Modified { get; set; }

      public string PriceRaw { get; set; }

      public double PriceDollars { get; set; }

      public bool Marketable { get; set; }

      public bool IsGenerator => this.Type == "generator";

      internal Definition(SteamInventory i, int id)
      {
        this.inventory = i;
        this.Id = id;
        this.SetupCommonProperties();
      }

      public void SetProperty(string name, string value)
      {
        if (this.customProperties == null)
          this.customProperties = new Dictionary<string, string>();
        if (!this.customProperties.ContainsKey(name))
          this.customProperties.Add(name, value);
        else
          this.customProperties[name] = value;
      }

      public T GetProperty<T>(string name)
      {
        string stringProperty = this.GetStringProperty(name);
        if (string.IsNullOrEmpty(stringProperty))
          return default (T);
        try
        {
          return (T) Convert.ChangeType((object) stringProperty, typeof (T));
        }
        catch (Exception ex)
        {
          return default (T);
        }
      }

      public string GetStringProperty(string name)
      {
        string pchValueBuffer = string.Empty;
        if (this.customProperties != null && this.customProperties.ContainsKey(name))
          return this.customProperties[name];
        return !this.inventory.GetItemDefinitionProperty((SteamItemDef_t) this.Id, name, out pchValueBuffer) ? string.Empty : pchValueBuffer;
      }

      public bool GetBoolProperty(string name)
      {
        string stringProperty = this.GetStringProperty(name);
        return stringProperty.Length != 0 && stringProperty[0] != '0' && stringProperty[0] != 'F' && stringProperty[0] != 'f';
      }

      internal void SetupCommonProperties()
      {
        this.Name = this.GetStringProperty("name");
        this.Description = this.GetStringProperty("description");
        this.Created = this.GetProperty<DateTime>("timestamp");
        this.Modified = this.GetProperty<DateTime>("modified");
        this.ExchangeSchema = this.GetStringProperty("exchange");
        this.IconUrl = this.GetStringProperty("icon_url");
        this.IconLargeUrl = this.GetStringProperty("icon_url_large");
        this.Type = this.GetStringProperty("type");
        this.PriceRaw = this.GetStringProperty("price_category");
        this.Marketable = this.GetBoolProperty("marketable");
        if (string.IsNullOrEmpty(this.PriceRaw))
          return;
        this.PriceDollars = (double) Inventory.PriceCategoryToFloat(this.PriceRaw);
      }

      public void TriggerItemDrop()
      {
        SteamInventoryResult_t pResultHandle = (SteamInventoryResult_t) 0;
        this.inventory.TriggerItemDrop(ref pResultHandle, (SteamItemDef_t) this.Id);
        this.inventory.DestroyResult(pResultHandle);
      }

      internal void Link(Inventory.Definition[] definitions) => this.LinkExchange(definitions);

      private void LinkExchange(Inventory.Definition[] definitions)
      {
        if (string.IsNullOrEmpty(this.ExchangeSchema))
          return;
        this.Recipes = ((IEnumerable<string>) this.ExchangeSchema.Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, Inventory.Recipe>((Func<string, Inventory.Recipe>) (x => Inventory.Recipe.FromString(x, definitions, this))).ToArray<Inventory.Recipe>();
      }

      internal void InRecipe(Inventory.Recipe r)
      {
        if (this.IngredientFor == null)
          this.IngredientFor = new Inventory.Recipe[0];
        this.IngredientFor = new List<Inventory.Recipe>((IEnumerable<Inventory.Recipe>) this.IngredientFor)
        {
          r
        }.ToArray();
      }
    }

    public struct Recipe
    {
      public Inventory.Definition Result;
      public Inventory.Recipe.Ingredient[] Ingredients;

      internal static Inventory.Recipe FromString(
        string part,
        Inventory.Definition[] definitions,
        Inventory.Definition Result)
      {
        Inventory.Recipe r = new Inventory.Recipe();
        r.Result = Result;
        string[] source = part.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
        r.Ingredients = ((IEnumerable<string>) source).Select<string, Inventory.Recipe.Ingredient>((Func<string, Inventory.Recipe.Ingredient>) (x => Inventory.Recipe.Ingredient.FromString(x, definitions))).Where<Inventory.Recipe.Ingredient>((Func<Inventory.Recipe.Ingredient, bool>) (x => x.DefinitionId != 0)).ToArray<Inventory.Recipe.Ingredient>();
        foreach (Inventory.Recipe.Ingredient ingredient in r.Ingredients)
        {
          if (ingredient.Definition != null)
            ingredient.Definition.InRecipe(r);
        }
        return r;
      }

      public struct Ingredient
      {
        public int DefinitionId;
        public Inventory.Definition Definition;
        public int Count;

        internal static Inventory.Recipe.Ingredient FromString(
          string part,
          Inventory.Definition[] definitions)
        {
          Inventory.Recipe.Ingredient i = new Inventory.Recipe.Ingredient();
          i.Count = 1;
          try
          {
            if (part.Contains<char>('x'))
            {
              int length = part.IndexOf('x');
              int result = 0;
              if (int.TryParse(part.Substring(length + 1), out result))
                i.Count = result;
              part = part.Substring(0, length);
            }
            i.DefinitionId = int.Parse(part);
            i.Definition = ((IEnumerable<Inventory.Definition>) definitions).FirstOrDefault<Inventory.Definition>((Func<Inventory.Definition, bool>) (x => x.Id == i.DefinitionId));
          }
          catch (Exception ex)
          {
            return i;
          }
          return i;
        }
      }
    }

    public struct Item : IEquatable<Inventory.Item>
    {
      public ulong Id;
      public int Quantity;
      public int DefinitionId;
      public Inventory.Definition Definition;
      public bool TradeLocked;

      public bool Equals(Inventory.Item other) => object.Equals((object) other, (object) this);

      public override bool Equals(object obj)
      {
        return obj != null && !(this.GetType() != obj.GetType()) && (long) ((Inventory.Item) obj).Id == (long) this.Id;
      }

      public override int GetHashCode() => this.Id.GetHashCode();

      public static bool operator ==(Inventory.Item c1, Inventory.Item c2) => c1.Equals(c2);

      public static bool operator !=(Inventory.Item c1, Inventory.Item c2) => !c1.Equals(c2);

      public struct Amount
      {
        public Inventory.Item Item;
        public int Quantity;
      }
    }

    public class Result : IDisposable
    {
      internal static Dictionary<int, Inventory.Result> Pending;
      internal Inventory inventory;
      public Action<Inventory.Result> OnResult;
      protected bool _gotResult = false;

      private SteamInventoryResult_t Handle { get; set; }

      public Inventory.Item[] Items { get; internal set; }

      public Inventory.Item[] Removed { get; internal set; }

      public Inventory.Item[] Consumed { get; internal set; }

      public bool IsPending
      {
        get
        {
          if (this._gotResult)
            return false;
          if (this.Status() != Facepunch.Steamworks.Callbacks.Result.OK)
            return this.Status() == Facepunch.Steamworks.Callbacks.Result.Pending;
          this.Fill();
          return false;
        }
      }

      internal uint Timestamp { get; private set; }

      internal bool IsSuccess
      {
        get
        {
          if (this.Items != null)
            return true;
          return (int) this.Handle != -1 && this.Status() == Facepunch.Steamworks.Callbacks.Result.OK;
        }
      }

      internal Facepunch.Steamworks.Callbacks.Result Status()
      {
        return (int) this.Handle == -1 ? Facepunch.Steamworks.Callbacks.Result.InvalidParam : (Facepunch.Steamworks.Callbacks.Result) this.inventory.inventory.GetResultStatus(this.Handle);
      }

      internal Result(Inventory inventory, int Handle, bool pending)
      {
        if (pending)
          Inventory.Result.Pending.Add(Handle, this);
        this.Handle = (SteamInventoryResult_t) Handle;
        this.inventory = inventory;
      }

      internal void Fill()
      {
        if (this._gotResult || this.Items != null || this.Status() != Facepunch.Steamworks.Callbacks.Result.OK)
          return;
        this._gotResult = true;
        this.Timestamp = this.inventory.inventory.GetResultTimestamp(this.Handle);
        SteamItemDetails_t[] resultItems = this.inventory.inventory.GetResultItems(this.Handle);
        if (resultItems == null)
          return;
        this.Items = ((IEnumerable<SteamItemDetails_t>) resultItems).Where<SteamItemDetails_t>((Func<SteamItemDetails_t, bool>) (x => ((int) x.Flags & 256) != 256 && ((int) x.Flags & 512) != 512)).Select<SteamItemDetails_t, Inventory.Item>((Func<SteamItemDetails_t, Inventory.Item>) (x => new Inventory.Item()
        {
          Quantity = (int) x.Quantity,
          Id = x.ItemId,
          DefinitionId = x.Definition,
          TradeLocked = ((uint) x.Flags & 1U) > 0U,
          Definition = this.inventory.FindDefinition(x.Definition)
        })).ToArray<Inventory.Item>();
        this.Removed = ((IEnumerable<SteamItemDetails_t>) resultItems).Where<SteamItemDetails_t>((Func<SteamItemDetails_t, bool>) (x => ((uint) x.Flags & 256U) > 0U)).Select<SteamItemDetails_t, Inventory.Item>((Func<SteamItemDetails_t, Inventory.Item>) (x => new Inventory.Item()
        {
          Quantity = (int) x.Quantity,
          Id = x.ItemId,
          DefinitionId = x.Definition,
          TradeLocked = ((uint) x.Flags & 1U) > 0U,
          Definition = this.inventory.FindDefinition(x.Definition)
        })).ToArray<Inventory.Item>();
        this.Consumed = ((IEnumerable<SteamItemDetails_t>) resultItems).Where<SteamItemDetails_t>((Func<SteamItemDetails_t, bool>) (x => ((uint) x.Flags & 512U) > 0U)).Select<SteamItemDetails_t, Inventory.Item>((Func<SteamItemDetails_t, Inventory.Item>) (x => new Inventory.Item()
        {
          Quantity = (int) x.Quantity,
          Id = x.ItemId,
          DefinitionId = x.Definition,
          TradeLocked = ((uint) x.Flags & 1U) > 0U,
          Definition = this.inventory.FindDefinition(x.Definition)
        })).ToArray<Inventory.Item>();
        if (this.OnResult == null)
          return;
        this.OnResult(this);
      }

      internal void OnSteamResult(SteamInventoryResultReady_t data, bool error)
      {
        if (data.Esult != SteamNative.Result.OK || error)
          return;
        this.Fill();
      }

      internal unsafe byte[] Serialize()
      {
        uint punOutBufferSize = 0;
        if (!this.inventory.inventory.SerializeResult(this.Handle, IntPtr.Zero, out punOutBufferSize))
          return (byte[]) null;
        byte[] numArray = new byte[(int) punOutBufferSize];
        fixed (byte* pOutBuffer = numArray)
        {
          if (!this.inventory.inventory.SerializeResult(this.Handle, (IntPtr) (void*) pOutBuffer, out punOutBufferSize))
            return (byte[]) null;
        }
        return numArray;
      }

      public void Dispose()
      {
        this.inventory.inventory.DestroyResult(this.Handle);
        this.Handle = (SteamInventoryResult_t) -1;
        this.inventory = (Inventory) null;
      }
    }
  }
}
