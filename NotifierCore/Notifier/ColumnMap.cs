using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.ViewModel;

namespace NotifierCore.Notifier
{
    public class ColumnMap : BindableBase
    {
        //<CheckBox IsChecked="{Binding Image}" Content="Image"></CheckBox>
        //    <CheckBox IsChecked="{Binding Name}" Content="Name"></CheckBox>
        //    <CheckBox IsChecked="{Binding Rarity}" Content="Rarity"></CheckBox>
        //    <CheckBox IsChecked="{Binding Supply}" Content="Supply"></CheckBox>
        //    <CheckBox IsChecked="{Binding SupplyRecent}" Content="Recent Supply"></CheckBox>
        //    <CheckBox IsChecked="{Binding SupplyRecentIndex}" Content="Recent Supply Index"></CheckBox>
        //    <CheckBox IsChecked="{Binding Buy}" Content="Buy"></CheckBox>
        //    <CheckBox IsChecked="{Binding Demand}" Content="Demand"></CheckBox>
        //    <CheckBox IsChecked="{Binding DemandRecent}" Content="Recent Demand"></CheckBox>
        //    <CheckBox IsChecked="{Binding DemandRecentIndex}" Content="Recent Demand Index"></CheckBox>
        //    <CheckBox IsChecked="{Binding MarginGold}" Content="Margin Gold"></CheckBox>
        //    <CheckBox IsChecked="{Binding MarginPercent}" Content="Margin %"></CheckBox>
        //    <CheckBox IsChecked="{Binding CopyIngameLink}" Content="Copy Ingame Link"></CheckBox>
        //    <CheckBox IsChecked="{Binding CopyName}" Content="Copy Name"></CheckBox>
        //    <CheckBox IsChecked="{Binding Remove}" Content="Remove"></CheckBox>

        bool _image = true;
        public bool Image
        {
            get { return _image; }
            set { _image = value; OnPropertyChanged("Image"); }
        }

        bool _name = true;
        public bool Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        bool _rarity = true;
        public bool Rarity
        {
            get { return _rarity; }
            set { _rarity = value; OnPropertyChanged("Rarity"); }
        }

        bool _supply = true;
        public bool Supply
        {
            get { return _supply; }
            set { _supply = value; OnPropertyChanged("Supply"); }
        }

        bool _supplyRecent = true;
        public bool SupplyRecent
        {
            get { return _supplyRecent; }
            set { _supplyRecent = value; OnPropertyChanged("SupplyRecent"); }
        }

        bool _supplyRecentIndex = true;
        public bool SupplyRecentIndex
        {
            get { return _supplyRecentIndex; }
            set { _supplyRecentIndex = value; OnPropertyChanged("SupplyRecentIndex"); }
        }

        bool _buy = true;
        public bool Buy
        {
            get { return _buy; }
            set { _buy = value; OnPropertyChanged("Buy"); }
        }

        bool _demand = true;
        public bool Demand
        {
            get { return _demand; }
            set { _demand = value; OnPropertyChanged("Demand"); }
        }

        bool _demandRecent = true;
        public bool DemandRecent
        {
            get { return _demandRecent; }
            set { _demandRecent = value; OnPropertyChanged("DemandRecent"); }
        }

        bool _demandRecentIndex = true;
        public bool DemandRecentIndex
        {
            get { return _demandRecentIndex; }
            set { _demandRecentIndex = value; OnPropertyChanged("DemandRecentIndex"); }
        }

        bool _marginGold = true;
        public bool MarginGold
        {
            get { return _marginGold; }
            set { _marginGold = value; OnPropertyChanged("MarginGold"); }
        }

        bool _marginPercent = true;
        public bool MarginPercent
        {
            get { return _marginPercent; }
            set { _marginPercent = value; OnPropertyChanged("MarginPercent"); }
        }

        bool _copyIngameLink = true;
        public bool CopyIngameLink
        {
            get { return _copyIngameLink; }
            set { _copyIngameLink = value; OnPropertyChanged("CopyIngameLink"); }
        }

        bool _copyName = true;
        public bool CopyName
        {
            get { return _copyName; }
            set { _copyName = value; OnPropertyChanged("CopyName"); }
        }

        bool _remove = true;
        public bool Remove
        {
            get { return _remove; }
            set { _remove = value; OnPropertyChanged("Remove"); }
        }

        bool _add = true;
        public bool Add
        {
            get { return _add; }
            set
            {
                _add = value;
                OnPropertyChanged("Add");
            }
        }
    }
}
