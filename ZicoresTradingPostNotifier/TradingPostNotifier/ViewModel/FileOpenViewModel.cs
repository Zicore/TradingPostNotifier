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
    public class FileOpenViewModel : BindableBase
    {
        public FileOpenViewModel()
        {

        }

        RelayCommand _openDialogCommand;

        OpenFileDialog _openFileDialog = new OpenFileDialog();
        
        String _filter = "";
        String _selectedPath = "";

        public event EventHandler<EventArgs<FileOpenViewModel>> PathSelected;

        public RelayCommand OpenDialogCommand
        {
            get
            {
                if(_openDialogCommand == null)
                    _openDialogCommand = new RelayCommand(param =>
                    {
                        _openFileDialog = new OpenFileDialog();
                        _openFileDialog.Filter = Filter;
                        _openFileDialog.FileName = SelectedPath;
                        _openFileDialog.FileOk -= _saveFileDialog_FileOk;
                        _openFileDialog.FileOk += _saveFileDialog_FileOk;
                        _openFileDialog.ShowDialog();
                    });
                return _openDialogCommand;
            }
        }

        void _saveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OpenFileDialog tempDialog = (OpenFileDialog)sender;
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
                        PathSelected(this, new EventArgs<FileOpenViewModel>(this));
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
