using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using LibraryBase.Wpf.Commands;
using LibraryBase.Wpf.Event;
using LibraryBase.Wpf.ViewModel;

namespace ZicoresTradingPostNotifier.ViewModel
{
    public class PagerViewModel : BindableBase
    {
        int _currentPage = 0;

        public event EventHandler<EventArgs<int>> RequestSelectPage;
        public event EventHandler<EventArgs> RequestNext;
        public event EventHandler<EventArgs> RequestPrevious;

        RelayCommand _selectPageCommand;
        RelayCommand _nextCommand;
        RelayCommand _previousCommand;

        // ----------------------------------------

        public RelayCommand SelectPageCommand
        {
            get
            {
                if (_selectPageCommand == null)
                    _selectPageCommand = new RelayCommand(param => SelectPage(param));
                return _selectPageCommand;
            }
        }

        public void SelectPage(object page)
        {
            if (RequestSelectPage != null)
                RequestSelectPage(this, new EventArgs<int>((int)page));
        }

        // ----------------------------------------

        public RelayCommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                    _nextCommand = new RelayCommand(param => Next());
                return _nextCommand;
            }
        }

        public void Next()
        {
            if (RequestNext != null)
                RequestNext(this, new EventArgs());
        }

        // ----------------------------------------

        public RelayCommand PreviousCommand
        {
            get
            {
                if (_previousCommand == null)
                    _previousCommand = new RelayCommand(param => Previous());
                return _previousCommand;
            }
        }

        public void Previous()
        {
            if (RequestPrevious != null)
                RequestPrevious(this, new EventArgs());
        }

        // ----------------------------------------

        public PagerViewModel()
        {
            for (int i = 0; i < 5; i++)
            {
                Pages.Add(new Page());
            }
        }

        // ----------------------------------------

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        ObservableCollection<Page> _pages = new ObservableCollection<Page>();
        public ObservableCollection<Page> Pages
        {
            get { return _pages; }
            set { _pages = value; }
        }

        public void Setup(int total, int offset, int itemsPerPage)
        {
            int remain = total - offset;
            int pagesMax = (int)Math.Ceiling((float)total / (float)itemsPerPage);
            int pagesRemain = (int)Math.Ceiling((float)remain / (float)itemsPerPage);
            int currentPage = (int)((float)offset / (float)itemsPerPage) + 1;

            for (int i = 0; i < 5; i++)
            {
                Page p = Pages[4 - i];

                int tempIndex = 4 - i;

                if (pagesMax >= 5)
                {
                    tempIndex = i - 2;
                    if (currentPage == 1)
                        tempIndex -= 2;
                    if (currentPage == 2)
                        tempIndex -= 1;

                    if (currentPage == pagesMax - 1)
                        tempIndex += 1;
                    if (currentPage == pagesMax)
                        tempIndex += 2;

                    p.Value = (currentPage - tempIndex);
                }
                else
                {
                    p.Value = tempIndex + 1;
                }

                if (p.Value > pagesMax)
                {
                    p.IsDisabled = true;
                }
                else
                {
                    p.IsDisabled = false;
                }

                if (p.Value == currentPage)
                {
                    p.IsSelected = true;
                }
                else
                {
                    p.IsSelected = false;
                }
            }

            this.CurrentPage = currentPage;
        }
    }
}
