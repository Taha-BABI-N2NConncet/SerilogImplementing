using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Threading;

namespace SerilogImplementing.Logger
{
    public class BackgroundQueueLogger
    {
        public delegate void logMethod();
        private static ConcurrentQueue<logMethod> loggingQueue = new ConcurrentQueue<logMethod>();
        private static CancellationTokenSource m_cancellationTokenSource = new CancellationTokenSource();
        private static object _lock = new object();
        private static ManualResetEvent manualReset = new ManualResetEvent(false);

        public static void StartProcessLogging()
        {
            var ltoken = m_cancellationTokenSource.Token;
            var lProcessLoggingThrd = new Thread(ProcessLogging);
            lProcessLoggingThrd.IsBackground = true;
            lProcessLoggingThrd.Start(ltoken);
        }
        public static void AddLoggingTaskToQueue(logMethod logMethod) 
        {
            Expression<logMethod> expression = Expression.Lambda<logMethod>
            (Expression.Call(
                Expression.Constant(logMethod.Target),
                logMethod.Method
            ));
            loggingQueue.Enqueue(expression.Compile());
            manualReset.Set();

        }
        private static void ProcessLogging(object obj)
        {
            while (!((CancellationToken)obj).IsCancellationRequested)
            {
                try
                {
                    logMethod logMethod;
                    int lCount = loggingQueue.Count;
                    if (lCount > 0)
                    {
                        lock (_lock)
                        {
                            for (int i = 0; i < lCount; i++)
                            {
                                var lSuccess = loggingQueue.TryDequeue(out logMethod);
                                if (lSuccess)
                                {
                                    logMethod.Invoke();
                                }
                            }
                        }
                    }
                    else
                        manualReset.WaitOne();
                }
                catch (Exception ex)
                {
                    //m_ATPLogger?.Fatal($"ProcessNewAckMsgRecv, {ex.Message}");
                }
            }
        }
    }
}
