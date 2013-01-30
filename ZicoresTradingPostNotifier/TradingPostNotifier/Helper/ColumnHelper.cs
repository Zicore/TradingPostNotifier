using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Scraper.Notifier;
using ZicoresTradingPostNotifier.Controls;
using ZicoresTradingPostNotifier.ViewModel;

namespace ZicoresTradingPostNotifier.Helper
{
    public class ColumnHelper
    {
        public ColumnHelper()
        {

        }

        String key = "";
        GridViewColumnCollection _gridViewColumns;
        Config config;

        public Config Config
        {
            get { return config; }
            set { config = value; }
        }

        private void UpdateIndex(String identifier, int index)
        {
            var newCol = Config.Columns[key].FirstOrDefault(x => x.Identifier == identifier);
            if (newCol != null)
            {
                newCol.NewIndex = index;
            }
            else
            {
                newCol = new ColumnInfo() { Identifier = identifier, NewIndex = index };
                Config.Columns[key].Add(newCol);
            }
        }

        private void Update(String identifier, int index, int width)
        {
            if (width > 1200)
            {
                width = 140;
            }

            var newCol = Config.Columns[key].FirstOrDefault(x => x.Identifier == identifier);
            if (newCol != null)
            {
                if (width != 0)
                {
                    newCol.Width = width;
                }
                newCol.NewIndex = index;
            }
            else
            {
                if (width != 0)
                {
                    newCol = new ColumnInfo() { Identifier = identifier, Width = width, NewIndex = index };
                }
                else
                {
                    newCol = new ColumnInfo() { Identifier = identifier, NewIndex = index };
                }

                Config.Columns[key].Add(newCol);
            }
        }

        public void Handle(ListView list, String key)
        {
            if (list.View is GridView)
            {
                Handle(((GridView)list.View).Columns, key);
            }
        }

        public void Handle(TreeListView list, String key)
        {
            Handle(list.Columns, key);
        }

        public void Handle(GridViewColumnCollection gridViewColumns, String key)
        {
            this._gridViewColumns = gridViewColumns;
            this.key = key;
            Config.Loading += Config_Loading;
            Config.Saving += Config_Saving;
        }

        void Config_Saving(object sender, EventArgs e)
        {
            if (Config != null)
            {
                if (_gridViewColumns != null)
                {
                    for (int i = 0; i < _gridViewColumns.Count; i++)
                    {
                        var col = _gridViewColumns[i] as GridViewColumnEx;
                        if (col != null)
                        {
                            int width = (int)col.ActualWidth;
                            Update(col.Identifier, _gridViewColumns.IndexOf(col), width);
                        }
                    }
                }
            }
        }

        private int FindIndex(GridViewColumnCollection columns, String id)
        {
            int index = -1;

            for (int i = 0; i < columns.Count; i++)
            {
                GridViewColumnEx col = columns[i] as GridViewColumnEx;
                if (col != null)
                {
                    if (col.Identifier == id)
                    {
                        return columns.IndexOf(col);
                    }
                }
            }

            return index;
        }

        public void LoadColumns()
        {
            if (!Config.Columns.ContainsKey(key))
            {
                Config.Columns[key] = new List<ColumnInfo>();
            }

            if (Config.Columns.ContainsKey(key))
            {
                if (Config.ResetColumns.Contains(key))
                {
                    Config.ResetColumns.Remove(key);
                    Config.Columns[key].Clear();
                }

                for (int i = 0; i < Config.Columns[key].Count; i++)
                {
                    var item = Config.Columns[key][i];

                    int index = FindIndex(_gridViewColumns, item.Identifier);
                    if (index >= 0)
                    {
                        int width = item.Width;
                        if (width >= 0)
                        {
                            _gridViewColumns[index].Width = width;
                            if (item.NewIndex >= 0)
                            {
                                if (item.NewIndex < _gridViewColumns.Count)
                                {
                                    _gridViewColumns.Move(index, item.NewIndex);
                                }
                                else
                                {
                                    Config.Columns[key].Remove(item);
                                    i--;
                                }
                            }
                        }
                    }
                }
            }
        }

        void Config_Loading(object sender, EventArgs e)
        {
            this.Config = (Config)sender;
            LoadColumns();
        }
    }
}
