using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/* ==================================================================================================
 * based on Exrin thread helper with some improvements
 * ================================================================================================*/
namespace XDemo.Core.Shared
{
    public static class ThreadHelper
    {
        private static SynchronizationContext _uiContext = null;
        private static bool _initialized = false;
        public static void Init(SynchronizationContext uiContext)
        {
            if(_initialized)
                throw new InvalidOperationException("You must call ThreadHelper.Init(context) once.");
            _uiContext = uiContext;
            _initialized = true;
        }

        /// <summary>
        /// Determines if the current synchronization context is the UI Thread
        /// </summary>
        /// <param name="context">SynchronizationContext you want to compare with the UIThread SynchronizationContext</param>
        /// <returns>true or false</returns>
        public static bool IsOnUIThread(SynchronizationContext context)
        {
            return context == _uiContext;
        }

        /// <summary>
        /// Will run the Func on the UIThread asynchronously.
        /// PERFORMANCE: Do not use in a loop. Context switching can cause significant performance degradation. Call this as infrequently as possible.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task RunOnUIThreadAsync(Func<Task> action)
        {
            if (_uiContext == null)
                throw new Exception($"You must call {nameof(ThreadHelper)}.{nameof(Init)}(context) before calling this method.");

            if (SynchronizationContext.Current == _uiContext || SynchronizationContext.Current is ExclusiveSynchronizationContext)
                await action();
            else
                await RunOnUIThreadHelper(action);
        }
        /// <summary>
        /// Will run the Func on the UIThread asynchronously.
        /// PERFORMANCE: Do not use in a loop. Context switching can cause significant performance degradation. Call this as infrequently as possible.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task<T> RunOnUIThreadAsync<T>(Func<Task<T>> action)
        {
            if (_uiContext == null)
                throw new Exception($"You must call {nameof(ThreadHelper)}.{nameof(Init)}(context) before calling this method.");

            return SynchronizationContext.Current == _uiContext || SynchronizationContext.Current is ExclusiveSynchronizationContext
                ? await action()
                : await RunOnUIThreadHelper(action);
        }
        /// <summary>
        /// Will run the Action on the UI Thread.
        /// PERFORMANCE: Do not use in a loop. Context switching can cause significant performance degradation. Call this as infrequently as possible.
        /// </summary>
        /// <param name="action"></param>
        public static void RunOnUIThread(Action action)
        {
            if (_uiContext == null)
                throw new Exception($"You must call {nameof(ThreadHelper)}.{nameof(Init)}(context) before calling this method.");

            if (SynchronizationContext.Current == _uiContext || SynchronizationContext.Current is ExclusiveSynchronizationContext)
                action();
            else
                RunOnUIThreadHelper(action).Wait(); // I can wait because I am not on the same thread.
        }

        /// <summary>
        /// Will run the Func on the UI Thread.
        /// PERFORMANCE: Do not use in a loop. Context switching can cause significant performance degradation. Call this as infrequently as possible.
        /// </summary>
        /// <param name="action"></param>
        public static void RunOnUIThread(Func<Task> action)
        {
            if (_uiContext == null)
                throw new Exception($"You must call {nameof(ThreadHelper)}.{nameof(Init)}(context) before calling this method.");

            if (SynchronizationContext.Current == _uiContext || SynchronizationContext.Current is ExclusiveSynchronizationContext)
            {
                RunSync(action);
            }
            else
                RunOnUIThreadHelper(action).Wait(); // I can wait because I am not on the same thread.
        }

        private static Task RunOnUIThreadHelper(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            _uiContext.Post((e) =>
            {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, null);

            return tcs.Task;
        }

        private static Task RunOnUIThreadHelper(Func<Task> action)
        {
            var tcs = new TaskCompletionSource<bool>();
            _uiContext.Post((e) =>
            {
                try
                {
                    action().ContinueWith(t =>
                    {
                        tcs.SetResult(true);
                    });
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, null);

            return tcs.Task;
        }
        private static Task<T> RunOnUIThreadHelper<T>(Func<Task<T>> action)
        {
            var tcs = new TaskCompletionSource<T>();

            _uiContext.Post((e) =>
            {
                try
                {
                    action().ContinueWith(t =>
                    {
                        tcs.SetResult(t.Result);
                    });
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, null);

            return tcs.Task;
        }

        private static void RunSync(Func<Task> task)
        {
            var oldContext = SynchronizationContext.Current;
            var synch = new ExclusiveSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(synch);
            synch.Post(async _ =>
            {
                try
                {
                    await task();
                }
                catch (Exception e)
                {
                    synch.InnerException = e;
                    throw;
                }
                finally
                {
                    synch.EndMessageLoop();
                }
            }, null);
            synch.BeginMessageLoop();

            SynchronizationContext.SetSynchronizationContext(oldContext);
        }


        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private bool done;
            public Exception InnerException { get; set; }
            readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
            readonly Queue<Tuple<SendOrPostCallback, object>> items = new Queue<Tuple<SendOrPostCallback, object>>();

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("Cannot send to the same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                lock (items)
                {
                    items.Enqueue(Tuple.Create(d, state));
                }
                workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!done)
                {
                    Tuple<SendOrPostCallback, object> task = null;
                    lock (items)
                    {
                        if (items.Count > 0)
                        {
                            task = items.Dequeue();
                        }
                    }
                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // the method threw an exeption
                        {
                            throw new AggregateException("ThreadHelper.Run method threw an exception.", InnerException);
                        }
                    }
                    else
                    {
                        workItemsWaiting.WaitOne();
                    }
                }
            }

            public override SynchronizationContext CreateCopy()
            {
                return this;
            }
        }
    }
}
