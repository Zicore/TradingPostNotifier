using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibraryBase.Wpf.Commands;
using LibraryBase.Wpf.Event;
using LibraryBase.Wpf.ViewModel;
using Newtonsoft.Json.Linq;
using NotifierCore.Crawler;
using NotifierCore.DataProvider;
using NotifierCore.Notifier;
using ZicoresTradingPostNotifier.Model;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class SearchViewModel : BindableBase
    {
        public SearchViewModel()
        {

        }

        public SearchViewModel(HotItemController hotItemController, MainWindowViewModel mainViewModel)
        {
            ParseCategories();
            ParseRarities();
            this._mainViewModel = mainViewModel;
            this._hotItemController = hotItemController;
            this._hotItemController.GuildWars2StatusChanged += _hotItemController_GuildWars2StatusChanged;
            this._hotItemController.SearchFinished += new EventHandler<EventArgs<SearchResult>>(HotItemController_SearchFinished);

            Pager.RequestNext += Pager_RequestNext;
            Pager.RequestPrevious += Pager_RequestPrevious;
            Pager.RequestSelectPage += Pager_RequestSelectPage;
        }

        void _hotItemController_GuildWars2StatusChanged(object sender, EventArgs<GuildWars2Status> e)
        {
            if (e.Value == GuildWars2Status.FoundKey)
            {
                OnPropertyChanged("AdvancedSearchVisiblity");
            }
        }

        void Pager_RequestSelectPage(object sender, EventArgs<int> e)
        {
            _hotItemController.SearchPage(e.Value);
        }

        void Pager_RequestPrevious(object sender, EventArgs e)
        {
            _hotItemController.SearchPreviousPage();
        }

        void Pager_RequestNext(object sender, EventArgs e)
        {
            _hotItemController.SearchNextPage();
        }

        public void SortClickRedirect(GridViewColumnHeader column)
        {
            if (column != null)
            {
                _hotItemController.EnableSorting(column.Column.Header.ToString());
            }
        }

        private ObservableCollection<HotItem> _searchedItems = new ObservableCollection<HotItem>();

        RelayCommand _searchCommand;
        RelayCommand _viewRecipeCommand;
        RelayCommand _searchNext;
        RelayCommand _searchPrevious;
        RelayCommand _selectPage;

        RelayCommand _buyOfferCommand;

        private String _searchString;
        HotItemController _hotItemController;
        MainWindowViewModel _mainViewModel;
        PagerViewModel _pager = new PagerViewModel();

        public PagerViewModel Pager
        {
            get { return _pager; }
            set
            {
                _pager = value;
                OnPropertyChanged("Pager");
            }
        }

        String jsonCategories = "{\"results\":[{\"id\":0,\"name\":\"Armor\",\"subtypes\":[{\"id\":0,\"name\":\"Coat\"},{\"id\":1,\"name\":\"Leggings\"},{\"id\":2,\"name\":\"Gloves\"},{\"id\":3,\"name\":\"Helm\"},{\"id\":4,\"name\":\"Aquatic Helm\"},{\"id\":5,\"name\":\"Boots\"},{\"id\":6,\"name\":\"Shoulders\"}]},{\"id\":2,\"name\":\"Bag\",\"subtypes\":[]},{\"id\":3,\"name\":\"Consumable\",\"subtypes\":[{\"id\":1,\"name\":\"Food\"},{\"id\":2,\"name\":\"Generic\"},{\"id\":5,\"name\":\"Transmutation\"},{\"id\":6,\"name\":\"Unlock\"}]},{\"id\":4,\"name\":\"Container\",\"subtypes\":[{\"id\":0,\"name\":\"Default\"},{\"id\":1,\"name\":\"Gift Box\"}]},{\"id\":5,\"name\":\"Crafting Material\",\"subtypes\":[]},{\"id\":6,\"name\":\"Gathering\",\"subtypes\":[{\"id\":0,\"name\":\"Foraging\"},{\"id\":1,\"name\":\"Logging\"},{\"id\":2,\"name\":\"Mining\"}]},{\"id\":7,\"name\":\"Gizmo\",\"subtypes\":[{\"id\":0,\"name\":\"Default\"},{\"id\":2,\"name\":\"Salvage\"}]},{\"id\":11,\"name\":\"Mini\",\"subtypes\":[]},{\"id\":13,\"name\":\"Tool\",\"subtypes\":[{\"id\":0,\"name\":\"[[Crafting]]\"},{\"id\":2,\"name\":\"Salvage\"}]},{\"id\":15,\"name\":\"Trinket\",\"subtypes\":[{\"id\":0,\"name\":\"Accessory\"},{\"id\":1,\"name\":\"Amulet\"},{\"id\":2,\"name\":\"Ring\"}]},{\"id\":16,\"name\":\"Trophy\",\"subtypes\":[]},{\"id\":17,\"name\":\"Upgrade Component\",\"subtypes\":[{\"id\":0,\"name\":\"Weapon\"},{\"id\":2,\"name\":\"Armor\"}]},{\"id\":18,\"name\":\"Weapon\",\"subtypes\":[{\"id\":0,\"name\":\"Sword\"},{\"id\":1,\"name\":\"Hammer\"},{\"id\":2,\"name\":\"Longbow\"},{\"id\":3,\"name\":\"Short Bow\"},{\"id\":4,\"name\":\"Axe\"},{\"id\":5,\"name\":\"Dagger\"},{\"id\":6,\"name\":\"Greatsword\"},{\"id\":7,\"name\":\"Mace\"},{\"id\":8,\"name\":\"Pistol\"},{\"id\":10,\"name\":\"Rifle\"},{\"id\":11,\"name\":\"Scepter\"},{\"id\":12,\"name\":\"Staff\"},{\"id\":13,\"name\":\"Focus\"},{\"id\":14,\"name\":\"Torch\"},{\"id\":15,\"name\":\"Warhorn\"},{\"id\":16,\"name\":\"Shield\"},{\"id\":19,\"name\":\"Spear\"},{\"id\":20,\"name\":\"Harpoon Gun\"},{\"id\":21,\"name\":\"Trident\"},{\"id\":22,\"name\":\"Toy\"}]}]}";
        String jsonRarities = "{\"results\":[{\"id\":0,\"name\":\"Junk\"},{\"id\":1,\"name\":\"Common\"},{\"id\":2,\"name\":\"Fine\"},{\"id\":3,\"name\":\"Masterwork\"},{\"id\":4,\"name\":\"Rare\"},{\"id\":5,\"name\":\"Exotic\"},{\"id\":6,\"name\":\"Ascended\"},{\"id\":7,\"name\":\"Legendary\"}]}";

        ObservableCollection<KeyValueString> _rarities = new ObservableCollection<KeyValueString>();

        public ObservableCollection<KeyValueString> Rarities
        {
            get { return _rarities; }
            set
            {
                _rarities = value;
                OnPropertyChanged("Rarities");
            }
        }
        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        public ObservableCollection<Category> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged("Categories");
            }
        }

        int _minLevel = 0;

        public int MinLevel
        {
            get { return _minLevel; }
            set
            {
                if (value != _minLevel)
                {
                    _minLevel = value;
                    if (_minLevel < 0)
                        _minLevel = 0;
                    if (_minLevel > 80)
                        _minLevel = 80;
                    OnPropertyChanged("MinLevel");
                }
            }
        }

        int _maxLevel = 80;

        public int MaxLevel
        {
            get { return _maxLevel; }
            set
            {
                if (value != _maxLevel)
                {
                    _maxLevel = value;
                    if (_maxLevel < 0)
                        _maxLevel = 0;
                    if (_maxLevel > 80)
                        _maxLevel = 80;
                    OnPropertyChanged("MaxLevel");
                }
            }
        }

        Category _selectedCategory;

        public Category SelectedCategory
        {
            get { return _selectedCategory; }
            set { _selectedCategory = value; OnPropertyChanged("SelectedCategory"); }
        }

        KeyValueString _selectedSubCategory;

        public KeyValueString SelectedSubCategory
        {
            get { return _selectedSubCategory; }
            set { _selectedSubCategory = value; OnPropertyChanged("SelectedSubCategory"); }
        }

        KeyValueString _selectedRarity;

        public KeyValueString SelectedRarity
        {
            get { return _selectedRarity; }
            set { _selectedRarity = value; OnPropertyChanged("SelectedRarity"); }
        }

        private void ParseCategories()
        {
            JObject json = JObject.Parse(jsonCategories);
            Categories.Add(new Category("*", "All"));
            for (int i = 0; i < json["results"].Count(); i++)
            {
                JToken t = json["results"][i];
                var k = new Category(t["id"].ToObject<String>(), t["name"].ToObject<String>());
                var subJson = json["results"][i]["subtypes"];
                k.Items.Add(new KeyValueString("*", "All"));
                for (int j = 0; j < subJson.Count(); j++)
                {
                    k.Items.Add(new KeyValueString(subJson[j]["id"].ToString(), subJson[j]["name"].ToString()));
                }

                Categories.Add(k);
            }
        }

        private void ParseRarities()
        {
            JObject json = JObject.Parse(jsonRarities);
            Rarities.Add(new KeyValueString("*", "All"));
            for (int i = 0; i < json["results"].Count(); i++)
            {
                JToken t = json["results"][i];
                Rarities.Add(new KeyValueString(t["id"].ToObject<String>(), t["name"].ToObject<String>()));
            }
        }

        public ICommand ViewRecipeCommand
        {
            get
            {
                if (_viewRecipeCommand == null)
                    _viewRecipeCommand = new RelayCommand(param => this.ViewRecipe(param));

                return _viewRecipeCommand;
            }
        }

        public void ViewRecipe(object param)
        {
            HotItem item = (HotItem)param;
            if (item != null && item.IsRecipeItem)
            {
                _mainViewModel.RecipeViewModel.ViewRecipe(item);
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new RelayCommand(param => this.Search());

                return _searchCommand;
            }
        }

        public String SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                OnPropertyChanged("SearchString");
            }
        }

        public Visibility AdvancedSearchVisiblity
        {
            get
            {
                return HotItemController.CurrentApi.IsAdvancedSearchSupported ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private bool _resultsVisible = true;
        public bool HasResults
        {
            get
            {
                return (_resultsVisible && SearchedItems != null && SearchedItems.Count > 0);
            }
            set
            {
                _resultsVisible = value;
            }
        }

        private void Search()
        {
            String rarity = null;
            String category = null;
            String subCategory = null;

            if (SelectedCategory != null)
            {
                category = SelectedCategory.Value.Key;
                if (category == "*")
                    category = null;
            }
            if (SelectedSubCategory != null)
            {
                subCategory = SelectedSubCategory.Key;
                if (subCategory == "*")
                    subCategory = null;
            }
            if (SelectedRarity != null)
            {
                rarity = SelectedRarity.Key;
                if (rarity == "*")
                    rarity = null;
            }

            SearchFilters f = new SearchFilters()
            {
                QueryString = SearchString,
                TypeId = category,
                SubTypeId = subCategory,
                Rarity = rarity,
                LevelMin = MinLevel.ToString(),
                LevelMax = MaxLevel.ToString(),
            };

            _hotItemController.Search(0, f);
        }

        public ObservableCollection<HotItem> SearchedItems
        {
            get { return _searchedItems; }
            set { _searchedItems = value; }
        }

        void HotItemController_SearchFinished(object sender, EventArgs<SearchResult> e)
        {
            if (e.Value.JsonResultType == JsonResultType.Search)
            {
                Pager.Setup(e.Value.Total, e.Value.Offset, HotItemController.CurrentApi.ItemsPerPage);

                foreach (HotItem item in e.Value.Items)
                {
                    Task.Factory.StartNew(() =>
                    {
                        item.Crawl();
                    });
                }
                MainWindowViewModel.Dispatcher.BeginInvoke((Action)delegate
                {
                    SearchedItems.Clear();
                    foreach (HotItem item in e.Value.Items)
                    {
                        SearchedItems.Add(item);
                    }
                    _resultsVisible = true;
                    OnPropertyChanged("HasResults");
                });
            }
        }
    }
}
