using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WpfServers.Notification
{
    public class ThreadSafeCollection<T> : ObservableCollection<T>
    {
        private static object ThreadSafeLock = new object();
        public ThreadSafeCollection()
        {
            BindingOperations.EnableCollectionSynchronization(this, ThreadSafeLock);
        }
    }
}
