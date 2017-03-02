using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Rikka.Tsab2.Core.Services
{
    public interface IWorkerService
    {
        void StartListener();
        void RegisterWorker<TWorker, T>(string name) where TWorker : IWorker<T>;
        void PushMessage(JobMessage message);
    }

    public class WorkerService:IDisposable, IWorkerService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly BlockingCollection<JobMessage> _queue;
        private readonly Dictionary<string,WorkerInfo> _workers;
        private Thread _listenerThread;

        public WorkerService(ILogger<WorkerService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _queue = new BlockingCollection<JobMessage>();
            _workers = new Dictionary<string, WorkerInfo>();
        }

        public void StartListener()
        {
            if (_listenerThread != null)
                return;
            _listenerThread = new Thread(Listener);
            _listenerThread.Start();
        }
        public void RegisterWorker<TWorker,T>(string name) where TWorker: IWorker<T>
        {
            var parameterType = typeof(T);
            var workerType = typeof(TWorker);
            var method = workerType.GetMethod("Invoke", new [] { parameterType });
            var worker = new WorkerInfo(name,workerType, parameterType, method);
            _workers.Add(name,worker);
        }


        public void PushMessage(JobMessage message)
        {
            _queue.TryAdd(message);
        }

        private object _createWorkerInstance(WorkerInfo info)
        {
            var constructor = info. WorkerType.GetConstructors()[0];
            var ctorParams = constructor.GetParameters();
            var args = new object[ctorParams.Length];
            for (var i = 0; i < ctorParams.Length; i++)
            {
                var param = ctorParams[i];
                args[i] = _serviceProvider.GetService(param.ParameterType);
            }
            var worker = Activator.CreateInstance(info.WorkerType, args);
            return worker;
        }

        private void _ivokeWorker(JobMessage message,WorkerInfo info,object worker, object parameter)
        {
            try
            {
                info.Method.Invoke(worker, new[] { parameter });
            }
            catch (Exception exception)
            {
                _logger.LogError(new EventId(0), exception, $"Worker '{message.Name}' throws an error on '{message.Id}' message");
            }
        }

        protected void Listener()
        {
            while (true)
            {
                var message = _queue.Take();
                if (!_workers.ContainsKey(message.Name))
                    continue;
                var info = _workers[message.Name];
                var worker = _createWorkerInstance(info);
                _ivokeWorker(message, info, worker, message.Parameter);
            }
        }

        public void Dispose()
        {
            
        }
    }

    public class WorkerInfo
    {
        public WorkerInfo()
        {
            
        }

        public WorkerInfo(string name, Type workerType, Type parameterType, MethodInfo method)
        {
            Name = name;
            WorkerType = workerType;
            ParameterType = parameterType;
            Method = method;
        }
        public string Name { get; }
        public Type ParameterType { get; }
        public MethodInfo Method { get; }
        public Type WorkerType { get; }        
    }
    public interface IWorker<T>
    {
        void Invoke(T message);
    }

    public class JobMessage
    {
        public JobMessage()
        {
            Id = Guid.NewGuid();
        }

        public JobMessage(string name, object parameter)
        {
            Id = Guid.NewGuid();
            Name = name;
            Parameter = parameter;
        }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public object Parameter { get; set; }
    }
}
