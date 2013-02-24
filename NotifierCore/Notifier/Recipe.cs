using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.ViewModel;

namespace NotifierCore.Notifier
{
    public enum RecipeWay
    {
        Buy,
        Craft
    }

    public class Recipe : HotItem
    {

        public Recipe()
        {

        }

        int _createdItemId = 0;
        public int CreatedItemId
        {
            get { return _createdItemId; }
            set { _createdItemId = value; }
        }

        int _type = 0;
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }

        int _rating = 0;
        public int Rating
        {
            get { return _rating; }
            set { _rating = value; }
        }

        int _id = 0;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        int _externalId = 0;
        public int ExternalId
        {
            get { return _externalId; }
            set { _externalId = value; }
        }

        int _itemId;
        public int ItemId
        {
            get { return _itemId; }
            set { _itemId = value; }
        }

        bool _preSelected = false;

        bool _isRoot = false;
        public bool IsRoot
        {
            get { return _isRoot; }
            set
            {
                _isRoot = value;
                OnPropertyChanged("IsRoot");
            }
        }

        public Recipe(int dataId, int quantity, bool register = true)
            : base(dataId)
        {
            this.Quantity = quantity;
            if (register)
            {
                HotItemController.Self.RegisterRecipeItem(this);
            }
        }

        bool _calculate = false;
        public bool Calculate
        {
            get { return _calculate; }
            set { _calculate = value; }
        }

        int _sellPriceCosts = 0;
        public int SellPriceCosts
        {
            get { return _sellPriceCosts; }
            set
            {
                _sellPriceCosts = value;
                OnPropertyChanged("SellPriceCosts");
                OnPropertyChanged("SellPriceCostsMoney");
            }
        }

        int _craftPriceCosts = 0;
        public int CraftPriceCosts
        {
            get { return _craftPriceCosts; }
            set
            {
                _craftPriceCosts = value;
                OnPropertyChanged("CraftPriceCosts");
                OnPropertyChanged("CraftPriceCostsMoney");
            }
        }

        public Money SellPriceCostsMoney
        {
            get { return new Money(0, 0, SellPriceCosts); }
        }

        public Money CraftPriceCostsMoney
        {
            get { return new Money(0, 0, CraftPriceCosts); }
        }

        public Money ListingFeeMoney
        {
            get { return new Money(0, 0, (int)Math.Floor(SellPrice * 0.05)); }
        }

        public Money SaleFeeMoney
        {
            get { return new Money(0, 0, (int)Math.Floor(SellPrice * 0.10)); }
        }

        public Money ProfitMoney
        {
            get { return new Money(0, 0, (int)Math.Floor(SellPrice * 0.85) - CraftPriceCosts); }
        }

        RecipeWay _way = RecipeWay.Craft;
        public RecipeWay Way
        {
            get { return _way; }
            set
            {
                _way = value;
                OnPropertyChanged("Way");
                OnPropertyChanged("IsCraftBestWay");
                OnPropertyChanged("IsBuyBestWay");
            }
        }

        RecipeWay _bestWay = RecipeWay.Craft;
        public RecipeWay BestWay
        {
            get { return _bestWay; }
            set
            {
                _bestWay = value;
                OnPropertyChanged("BestWay");
                OnPropertyChanged("IsCraftBestWay");
                OnPropertyChanged("IsBuyBestWay");
            }
        }

        public bool IsCraftBestWay
        {
            get
            {
                return Way == RecipeWay.Craft;
            }
            set
            {
                if (value)
                    Way = RecipeWay.Craft;
                else
                    Way = RecipeWay.Buy;
                IsAutomated = false;
            }
        }

        public bool IsBuyBestWay
        {
            get
            {
                return Way == RecipeWay.Buy;
            }
            set
            {
                if (value)
                    Way = RecipeWay.Buy;
                else
                    Way = RecipeWay.Craft;
                IsAutomated = false;
            }
        }

        ObservableCollection<Recipe> _shoppingList = new ObservableCollection<Recipe>();
        public ObservableCollection<Recipe> ShoppingList
        {
            get { return _shoppingList; }
            set
            {
                _shoppingList = value;
                OnPropertyChanged("ShoppingList");
            }
        }

        Recipe _item;
        public Recipe Item
        {
            get { return _item; }
            set { _item = value; }
        }

        bool _isAutomated = true;
        public bool IsAutomated
        {
            get { return _isAutomated; }
            set
            {
                _isAutomated = value;
                OnPropertyChanged("IsAutomated");
            }
        }

        private void CalculateCosts()
        {
            SellPriceCosts = 0;
            CraftPriceCosts = 0;

            SellPriceCosts = Quantity * SellPrice;

            foreach (Recipe r in RecipeItems)
            {
                r.CalculateCosts();
                if (r.Way == RecipeWay.Buy)
                {
                    CraftPriceCosts += r.SellPriceCosts;
                }
                else
                {
                    CraftPriceCosts += r.CraftPriceCosts;
                }
            }

            if (CraftPriceCosts != 0 && CraftPriceCosts < SellPriceCosts)
            {
                //Way = RecipeWay.Craft;
                BestWay = RecipeWay.Craft;
            }
            else
            {
                //Way = RecipeWay.Buy;
                BestWay = RecipeWay.Buy;
            }

            if (!_preSelected || IsAutomated)
            {
                Way = BestWay;
            }

            //OnPropertyChanged("CraftPriceCosts");
            //OnPropertyChanged("SellPriceCosts");
            //OnPropertyChanged("CraftPriceCostsMoney");
            //OnPropertyChanged("SellPriceCostsMoney");
            OnPropertyChanged("BestWay");
            OnPropertyChanged("IsCraftBestWay");
            OnPropertyChanged("IsBuyBestWay");
            OnPropertyChanged("ProfitMoney");
            OnPropertyChanged("ListingFeeMoney");
            OnPropertyChanged("SaleFeeMoney");
            OnPropertyChanged("IsAutomated");
            OnPropertyChanged("MarginMoney");
            OnPropertyChanged("MarginPercent");

            var shoppingList = CalculateShoppingList(new List<Recipe>());
            ShoppingList = new ObservableCollection<Recipe>(shoppingList);
            OnPropertyChanged("ShoppingList");
            _preSelected = true;
        }

        private List<Recipe> CalculateShoppingList(List<Recipe> list)
        {
            if (Way == RecipeWay.Buy)
            {
                var r = list.FirstOrDefault(x => x.DataId == DataId);
                if (r == null)
                {
                    list.Add(new Recipe(this.DataId, this.Quantity, false)
                    {
                        MarketItem = this.MarketItem,
                        SellPriceCosts = this.SellPriceCosts,
                        CraftPriceCosts = this.CraftPriceCosts,
                    });
                }
                else
                {
                    r.Quantity += Quantity;
                    r.SellPriceCosts = r.Quantity * SellPrice;
                }

            }
            else
            {
                foreach (Recipe r in RecipeItems)
                {
                    r.CalculateShoppingList(list);
                }
            }

            return list;
        }

        public override long SaleVolume
        {
            get
            {
                return MarketItem.SaleVolume;
            }
            set
            {
                base.SaleVolume = value;
                MarketItem.SaleVolume = value;
            }
        }

        public override long BuyVolume
        {
            get
            {
                return MarketItem.BuyVolume;
            }
            set
            {
                base.BuyVolume = value;
                MarketItem.BuyVolume = value;
            }
        }

        public override int BuyPrice
        {
            get
            {
                return MarketItem.BuyPrice;
            }
            set
            {
                base.BuyPrice = value;
                MarketItem.BuyPrice = value;
            }
        }

        public override int SellPrice
        {
            get
            {
                return MarketItem.SellPrice;
            }
            set
            {
                base.SellPrice = value;
                MarketItem.SellPrice = value;
            }
        }

        public override string Name
        {
            get
            {
                return MarketItem.Name;
            }
            set
            {
                base.Name = value;
                MarketItem.Name = value;
            }
        }

        public override string Image
        {
            get
            {
                return MarketItem.Image;
            }
            set
            {
                base.Image = value;
                MarketItem.Image = value;
            }
        }

        public override string ImgUri
        {
            get
            {
                return MarketItem.ImgUri;
            }
            set
            {
                base.ImgUri = value;
                MarketItem.ImgUri = value;
            }
        }

        public override Money MarginMoney
        {
            get
            {
                int price = CraftPriceCosts;
                if (Way == RecipeWay.Buy)
                    price = SellPriceCosts;

                int margin = (int)(Math.Floor(SellMoney.TotalCopper * 0.85 - price));
                return new Money(0, 0, margin);
            }
        }

        public override string MarginPercent
        {
            get
            {
                int price = CraftPriceCosts;
                if (Way == RecipeWay.Buy)
                    price = SellPriceCosts;

                int margin = (int)(Math.Floor(SellMoney.TotalCopper * 0.85 - price));
                float percent = 0.0f;
                if (BuyMoney.TotalCopper == 0)
                {
                    percent = 0;
                }
                else
                {
                    percent = (float)margin / (float)price * 100.0f;
                }
                return String.Format("{0:0.00}%", percent);
            }
        }

        ObservableCollection<Recipe> _recipeItems = new ObservableCollection<Recipe>();
        public ObservableCollection<Recipe> RecipeItems
        {
            get { return _recipeItems; }
            set
            {
                _recipeItems = value;
                OnPropertyChanged("RecipeItems");
            }
        }

        public void BuildRecipe(HotItemController c)
        {
            this.IsExpanded = true;
            if (c.ItemIdToDataId.ContainsKey(CreatedItemId))
            {
                Name = "loading...";
                this.DataId = c.ItemIdToDataId[CreatedItemId];
            }

            if (!IsRoot)
            {
                if (c.CreatedIdToRecipe.ContainsKey(ItemId))
                {
                    List<Recipe> tempList = new List<Recipe>();
                    for (int i = 0; i < c.CreatedIdToRecipe[ItemId].RecipeItems.Count; i++)
                    {
                        var r = c.CreatedIdToRecipe[ItemId].RecipeItems[i];
                        Recipe recipe = new Recipe(r.DataId, r.Quantity * this.Quantity);
                        recipe.ItemId = r.ItemId;
                        recipe.CreatedItemId = r.CreatedItemId;
                        recipe.RecipeItems = r.RecipeItems;
                        recipe.Name = "loading...";
                        tempList.Add(recipe);
                    }
                    RecipeItems = new ObservableCollection<Recipe>(tempList);
                }
            }

            c.RegisterRecipeItem(this);
            for (int i = 0; i < RecipeItems.Count; i++)
            {
                RecipeItems[i].BuildRecipe(c);
            }
        }

        public void UnbuildRecipe(HotItemController c)
        {
            c.UnregisterRecipeItem(DataId);
            for (int i = 0; i < RecipeItems.Count; i++)
            {
                RecipeItems[i].UnbuildRecipe(c);
            }
        }

        public Recipe Add(Recipe recipe)
        {
            RecipeItems.Add(recipe);
            return recipe;
        }

        public Recipe AddDeep(int dataId, int quantity)
        {
            Recipe r = new Recipe(dataId, quantity);
            RecipeItems.Add(r);
            return r;
        }

        public Recipe Add(int dataId, int quantity)
        {
            Recipe r = new Recipe(dataId, quantity);
            RecipeItems.Add(r);
            return this;
        }

        public override void Update()
        {
            CalculateCosts();
            base.Update();

            SellPrice = SellPrice;
            BuyPrice = BuyPrice;
            SaleVolume = SaleVolume;
            BuyVolume = BuyVolume;
            SellPriceCosts = SellPriceCosts;
            CraftPriceCosts = CraftPriceCosts;
        }

        public static Recipe CreateSample()
        {
            Recipe r = new Recipe(13901, 1) { IsRoot = true };
            r.AddDeep(12973, 1).AddDeep(19712, 6).AddDeep(19725, 18);

            r.AddDeep(13261, 1).AddDeep(19712, 2).AddDeep(19725, 6);

            var o = r.AddDeep(19922, 1);
            var o2 = o.AddDeep(12988, 5);
            o2.AddDeep(19685, 15).AddDeep(19701, 30);
            o2.AddDeep(19712, 10).AddDeep(19725, 30);

            o.AddDeep(19721, 5);
            o.AddDeep(24289, 5);

            return r;
        }
    }
}
