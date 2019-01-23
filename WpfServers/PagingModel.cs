using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WpfServers.Notification;
using System.Diagnostics;

namespace WpfServers
{
    public delegate void PagedEventHandler();
    public class PagingModel<T> : NotificationObject
    {
        #region ---变量
        public event PagedEventHandler PagedEvent;

        private ObservableCollection<T> dataSource;
        /// <summary>
        /// 数据源
        /// </summary>
        public ObservableCollection<T> DataSource
        {
            private get { return dataSource; }
            set { dataSource = value; }
        }

        private ObservableCollection<T> showdataSource;
        /// <summary>
        /// 显示到界面的数据
        /// </summary>
        public ObservableCollection<T> ShowDataSource
        {
            get { return showdataSource; }
            set { showdataSource = value; RaisePropertyChanged("ShowDataSource"); }
        }

        int pageSize;//每页多少条记录

        int pageCount;
        /// <summary>
        /// 页数
        /// </summary>
        public int PageCount
        {
            get { return pageCount == 0 ? 1 :pageCount; }
            set { pageCount = value; RaisePropertyChanged("PageCount"); }
        }

        int currentIndex;
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentIndex
        {
            get { return currentIndex; }
            set { currentIndex = value;RaisePropertyChanged("CurrentIndex"); }
        }

        int jumpIndex;
        /// <summary>
        /// 跳转页
        /// </summary>
        public int JumpIndex
        {
            get { return jumpIndex == 0 ? 1 : jumpIndex; }
            set
            {
                jumpIndex = value;
                RaisePropertyChanged("JumpIndex");
                JumpPageData(jumpIndex);
            }
        }
        #endregion

        #region ---构造函数
        public PagingModel()
        {
            CurrentIndex = 1;
        }

        public PagingModel(ObservableCollection<T> dataSource,int pageSize):this()
        {
            this.DataSource = dataSource;
            this.pageSize = pageSize;
            this.PageCount = dataSource.Count / this.pageSize;
            if (dataSource.Count >= pageSize)
            {
                this.PageCount += (dataSource.Count % this.pageSize) != 0 ? 1 : 0;
            }
        }
        #endregion

        #region ---分页
        public void GetPageData(JumpOperation jo)
        {
            this.PageCount = dataSource.Count / this.pageSize;
            if (dataSource.Count >= pageSize)
            {
                this.PageCount += (dataSource.Count % this.pageSize) != 0 ? 1 : 0;
            }
            switch (jo)
            {
                case JumpOperation.GoHome:
                    CurrentIndex = 1;
                    break;
                case JumpOperation.GoPrevious:
                    if (CurrentIndex > 1) { CurrentIndex -= 1; }
                    break;
                case JumpOperation.GoNext:
                    if(CurrentIndex < PageCount) { CurrentIndex += 1; }
                    break;
                case JumpOperation.GoEnd:
                    CurrentIndex = PageCount;
                    break;
                case JumpOperation.Refresh:
                    break;
            }
            Paging();
        }

        public void JumpPageData(int index)
        {
            this.PageCount = dataSource.Count / this.pageSize;
            if (dataSource.Count >= pageSize)
            {
                this.PageCount += (dataSource.Count % this.pageSize) != 0 ? 1 : 0;
            }
            if (index > pageCount || index <= 0) return;
            CurrentIndex = index;
            Paging();
        }

        private void Paging()
        {
            ObservableCollection<T> listPageData = new ObservableCollection<T>();
            try
            {
                int pageCountTo = pageSize;
                if (pageCountTo == CurrentIndex && DataSource.Count % pageSize > 0)
                {
                    pageCountTo = DataSource.Count % pageSize;
                }
                if (null != DataSource)
                {
                    for (int i = 0; i < pageCountTo; i++)
                    {
                        if ((CurrentIndex - 1) * pageSize + i < DataSource.Count)
                        {
                            listPageData.Add(DataSource[(CurrentIndex - 1) * pageSize + i]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch { return; }
            ShowDataSource = listPageData;
            if (PagedEvent != null) PagedEvent();
        }
        #endregion

        #region ---刷新
        public void Refresh()
        {
            GetPageData(JumpOperation.Refresh);
        }
        #endregion
    }
    public enum JumpOperation
    {
        GoHome = 0,
        GoPrevious = 1,
        GoNext = 2,
        GoEnd = 3,
        Refresh = 4
    }
}
