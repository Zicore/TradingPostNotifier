using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryBase.Wpf.ViewModel;
using Scraper.Notifier;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class RecipeViewModel : BindableBase
    {
        public RecipeViewModel(HotItemController hotItemController)
        {
            this.HotItemController = hotItemController;
            HotItemController.RecipesLoaded += HotItemController_RecipesLoaded;
        }

        void HotItemController_RecipesLoaded(object sender, EventArgs e)
        {

        }

        public void ViewRecipe(HotItem item)
        {
            Task.Factory.StartNew(() =>
            {
                int itemId = HotItemController.DataIdToRecipeId(item.DataId);
                if (itemId > 0)
                {
                    Recipe recipe = HotItemController.RecipeDB.Find(x => x.CreatedItemId == itemId);
                    Add(recipe);
                }
            });
        }

        public void Add(Recipe recipe)
        {
            if (recipe != null)
            {
                List<Recipe> tempList = new List<Recipe>();
                Clear();
                recipe.IsRoot = true;
                recipe.BuildRecipe(HotItemController);
                tempList.Add(recipe);
                Recipes = new ObservableCollection<Recipe>(tempList);
                SelectedRecipe = recipe;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Recipes.Count; i++)
            {
                Recipes[i].UnbuildRecipe(HotItemController);
            }
        }

        HotItemController _hotItemController;
        public HotItemController HotItemController
        {
            get { return _hotItemController; }
            set { _hotItemController = value; }
        }

        ObservableCollection<Recipe> _recipes = new ObservableCollection<Recipe>();
        public ObservableCollection<Recipe> Recipes
        {
            get { return _recipes; }
            set
            {
                _recipes = value;
                OnPropertyChanged("Recipes");
            }
        }

        Recipe _selectedRecipe;
        public Recipe SelectedRecipe
        {
            get { return _selectedRecipe; }
            set
            {
                _selectedRecipe = value;
                OnPropertyChanged("SelectedRecipe");
            }
        }
    }
}
