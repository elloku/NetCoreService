using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mediinfo.Infrastructure.Core.EventBus
{
    public static class MessageEventBus
    {
        public static event Action<MessageEventArgs> Handle;
        public static event Action<IMQMessageEventArgs> SendMQ;
        public static ConcurrentQueue<MessageEventArgs> Args = new ConcurrentQueue<MessageEventArgs>();
        public static List<MessageEventArgs> ArgsWait = new List<MessageEventArgs>();

        static MessageEventBus()
        {
            new System.Threading.Thread(() => {
                while (true)
                {
                    if (Args.Count > 0)
                    {
                        MessageEventArgs args = null;
                        Args.TryDequeue(out args);
                        if (args != null)
                        {
                            try
                            {
                                Handle?.Invoke(args);

                                if (SendMQ != null)
                                {
                                    var types = args.GetType().GetInterfaces();
                                    if (types != null && types.Length > 0)
                                    {
                                        if (types[0].Name == typeof(IMQMessageEventArgs).Name)
                                        {
                                            SendMQ((IMQMessageEventArgs)args);
                                        }
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1024);
                    }
                }
            }).Start();
        }

        public static void Add<T>(T arg) where T : MessageEventArgs, new()
        {
            Args.Enqueue(arg);
        }

        public static void AddWaitCommit<T>(T arg) where T : MessageEventArgs, new()
        {
            ArgsWait.Add(arg);
        }

        public static void SendForCommit()
        {
            if (SendMQ != null)
            {
                ArgsWait.ForEach(o =>
                {
                    Args.Enqueue(o);
                });
            }
            ArgsWait.Clear();
        } 
    }
}
