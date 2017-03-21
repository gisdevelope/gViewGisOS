using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using gView.Framework.system;

namespace gView.MapServer.Instance.Threading
{
    internal class QueuedThread<T> : IDisposable
    {
        internal delegate void QueuedThreadTarget(T parameter);
        internal delegate void QueueThreadFinished(QueuedThread<T> sender);

        internal QueueThreadFinished _callback = null;
        internal QueuedThreadTarget _target = null;
        internal T _parameter;
        internal ManualResetEvent _resetEvent = null;

        public QueuedThread(QueuedThreadTarget Target, T Parameter, QueueThreadFinished callback, ManualResetEvent resetEvent)
        {
            _target = Target;
            _parameter = Parameter;
            _callback = callback;
            _resetEvent = resetEvent;
        }

        public void Start()
        {
            try
            {
                if (_target != null)
                {
                    _target(_parameter);
                }
                RunAsyncCallback();
                if (_resetEvent != null)
                {
                    _resetEvent.Set();
                    _resetEvent = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(loggingMethod.error,
                    "Thread Error: " + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
            finally
            {
                if (_resetEvent != null)
                    _resetEvent.Set();
            }
        }

        private void RunAsyncCallback()
        {
            try
            {
                if (_callback != null)
                    _callback.BeginInvoke(this, null, null);
            }
            catch (Exception ex)
            {
                Logger.Log(loggingMethod.error,
                    "Thread Error: " + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }
        private void RunCallback()
        {
            try
            {
                if (_callback != null)
                {
                    _callback(this);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(loggingMethod.error,
                    "Thread Error: " + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        #region IDisposable Member

        public void Dispose()
        {
            try
            {
                if (_resetEvent != null)
                    _resetEvent.Set();
            }
            catch (Exception ex)
            {
                Logger.Log(loggingMethod.error,
                     "Disposing QueuedThread Error: " + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);
            }
        }

        #endregion
    }

    internal class ThreadQueue<T>
    {
        private Queue<QueuedThread<T>> _queue;
        private int _maxThreads = 2, _currentThreads = 0, _queueLength = 100;
        private static Object lockThis = new Object();

        public ThreadQueue(int maxThreads, int queueLength)
        {
            _queue = new Queue<QueuedThread<T>>();
            _maxThreads = maxThreads;
            _queueLength = queueLength;
        }

        public bool AddQueuedThread(QueuedThread<T>.QueuedThreadTarget Target, T Parameter, ManualResetEvent resetEvent)
        {
            try
            {
                if (_queue.Count >= _queueLength) return false;

                QueuedThread<T> thread = new QueuedThread<T>(Target, Parameter, this.QueuedThreadFinshed, resetEvent);

                lock (lockThis)
                {
                    _queue.Enqueue(thread);
                }

                if (_currentThreads < _maxThreads) TryStartThread();
                return true;
            }
            catch (Exception ex)
            {
                if (Functions.log_errors)
                    Logger.Log(loggingMethod.error, "ThreadQueue.AddQueuedThread: " + ex.Message);
                return false;
            }
        }

        public bool AddQueuedThreadSync(QueuedThread<T>.QueuedThreadTarget Target, T Parameter)
        {
            try
            {
                if (_queue.Count >= _queueLength)
                    return false;

                ManualResetEvent resetEvent = new ManualResetEvent(false);
                QueuedThread<T> thread = new QueuedThread<T>(Target, Parameter, this.QueuedThreadFinshed, resetEvent);
                lock (lockThis)
                {
                    _queue.Enqueue(thread);
                }

                if (_currentThreads < _maxThreads)
                    TryStartThread();

                resetEvent.WaitOne();

                return true;
            }
            catch (Exception ex)
            {
                if (Functions.log_errors)
                    Logger.Log(loggingMethod.error, "ThreadQueue.AddQueuedThreadSync: " + ex.Message);
                return false;
            }
        }

        public void QueuedThreadFinshed(QueuedThread<T> sender)
        {
            try
            {
                if (_currentThreads > 0)  // ist 0, wenn zwischendurch Clear durgef�hrt wird
                {
                    _currentThreads--;
                    if (_currentThreads == 0)
                    {
                        // wenn alle fertig sind k�nnen neue gestartet werden...
                        // (wenn jedesmal aufgerufen wird, wird ein schon lage gestarteteter
                        // immer wieder blokiert
                        // so werden alte einmal fertig gemacht
                        TryStartThread();
                    }
                }
            }
            catch (Exception ex)
            {
                if (Functions.log_errors)
                    Logger.Log(loggingMethod.error, "ThreadQueue.QueuedThreadFinshed: " + ex.Message);
            }
        }

        public void Clear()
        {
            try
            {
                _queue.Clear();
                _currentThreads = 0;
            }
            catch (Exception ex)
            {
                if (Functions.log_errors)
                    Logger.Log(loggingMethod.error, "ThreadQueue.Clear: " + ex.Message);
            }
        }

        private void TryStartThread()
        {
            try
            {
                lock (lockThis)
                {
                    if (_currentThreads >= _maxThreads) return;
                    int count = Math.Min(_maxThreads - _currentThreads, _queue.Count);

                    for (int i = 0; i < count; i++)
                    {
                        QueuedThread<T> queuedThread = _queue.Dequeue();
                        try
                        {
                            if (queuedThread == null)
                                throw new ArgumentException("Thread Object in Queue is NULL");

                            Thread thread = new Thread(new ThreadStart(queuedThread.Start));
                            _currentThreads++;
                            thread.Start();
                        }
                        catch (Exception ex)
                        {
                            Logger.Log(loggingMethod.error,
                                "Starting Thread Error: " + ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace);

                            if (queuedThread != null)
                                queuedThread.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (Functions.log_errors)
                    Logger.Log(loggingMethod.error, "ThreadQueue.TryStartThread: " + ex.Message);
            }
        }

        public int CurrentThreads
        {
            get
            {
                return _currentThreads;
            }
        }
        public int ThreadsInQueue
        {
            get { return _queue.Count; }
        }
    }
}
