using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.Commands;
using LibraryBase.Wpf.Event;
using LibraryBase.Wpf.ViewModel;
using Microsoft.Win32;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class FileSaveViewModel : BindableBase
    {
        public FileSaveViewModel()
        {

        }

        RelayCommand _openDialogCommand;

        SaveFileDialog _saveFileDialog = new SaveFileDialog();
        
        String _filter = "";
        String _selectedPath = "";

        public event EventHandler<EventArgs<FileSaveViewModel>> PathSelected;

        public RelayCommand OpenDialogCommand
        {
            get
            {
                if(_openDialogCommand == null)
                    _openDialogCommand = new RelayCommand(param =>
                    {
                        _saveFileDialog = new SaveFileDialog();
                        _saveFileDialog.Filter = Filter;
                        _saveFileDialog.FileName = SelectedPath;
                        _saveFileDialog.FileOk -= _saveFileDialog_FileOk;
                        _saveFileDialog.FileOk += _saveFileDialog_FileOk;
                        _saveFileDialog.ShowDialog();
                    });
                return _openDialogCommand;
            }
        }

        void _saveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveFileDialog tempDialog = (SaveFileDialog)sender;
            tempDialog.FileOk -= _saveFileDialog_FileOk;
            SelectedPath = tempDialog.FileName;
        }

        public String SelectedPath
        {
            get { return _selectedPath; }
            set 
            {
                if (_selectedPath != value)
                {
                    _selectedPath = value;
                    OnPropertyChanged("SelectedPath");
                    if (PathSelected != null)
                    {
                        PathSelected(this, new EventArgs<FileSaveViewModel>(this));
                    }
                }
            }
        }

        public String Filter
        {
            get { return _filter; }
            set 
            { 
                _filter = value;
                OnPropertyChanged("Filter");
            }
        }
    }
}
