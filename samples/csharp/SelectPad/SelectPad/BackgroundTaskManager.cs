using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelectPad
{

    //BackgroundTaskManager class manages a new dispatcher instance for new threads
    public class BackgroundTaskManager : IDisposable
    {
        private System.Windows.Threading.Dispatcher _OwnerDispatcher;
        private System.Windows.Threading.Dispatcher _WorkerDispatcher;
        private System.Threading.Thread _WorkerThread;
        private Boolean _WorkerBusy;

        private System.Threading.EventWaitHandle _WorkerStarted = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset);

        public BackgroundTaskManager()
        {
            _OwnerDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            _WorkerThread = new System.Threading.Thread(new System.Threading.ThreadStart(WorkerStart));
            _WorkerThread.Name = "BackgroundTaskManager:" + DateTime.Now.Ticks.ToString();
            _WorkerThread.IsBackground = true;
            _WorkerThread.Start();

            _WorkerStarted.WaitOne();
        }

        public Boolean IsBusy
        {
            get { return _WorkerBusy; }
        }

        public System.Windows.Threading.Dispatcher Dispatcher
        {
            get
            {
                return _WorkerDispatcher;
            }
        }

        public System.Windows.Threading.Dispatcher OwnerDispatcher
        {
            get
            {
                return _OwnerDispatcher;
            }
        }

        private void WorkerStart()
        {
            _WorkerDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            _WorkerDispatcher.Hooks.DispatcherInactive += WorkDone;
            _WorkerDispatcher.Hooks.OperationPosted += WorkAdded;
            _WorkerStarted.Set();
            System.Windows.Threading.Dispatcher.Run();
        }

        private void WorkAdded(Object sender, System.Windows.Threading.DispatcherHookEventArgs e)
        {
            _WorkerBusy = true;
        }

        private void WorkDone(Object sender, EventArgs e)
        {
            _WorkerBusy = false;
        }

        public void Dispose()
        {
            if (_WorkerDispatcher != null)
            {
                _WorkerDispatcher.InvokeShutdown();
                _WorkerDispatcher = null;
            }
        }

    }
}
