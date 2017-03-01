using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Rikka.TelegramBotCore;

namespace Rikka.Tsab2.Core.Services
{
    public interface IBotActionsService
    {
        IEnumerable<IBotAction> Actions { get; }
    }

    public class BotActionsService: IBotActionsService
    {
        private readonly IServiceProvider _serviceProvider;
        public BotActionsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private IEnumerable<IBotAction> _actions;
        public IEnumerable<IBotAction> Actions => _actions ?? (_actions = _getActions());

        private IEnumerable<IBotAction> _getActions()
        {
            var assembly = typeof(BotActionsService).GetTypeInfo().Assembly;
            var exports = assembly.GetExportedTypes();
            var marked = exports
                .Where(t => t.GetTypeInfo().GetCustomAttribute<BotActionAttribute>() != null);
            var actionTypes = marked
                .Where(t => typeof(IBotAction).IsAssignableFrom(t));
            foreach (var type in actionTypes)
            {
                var constructor = type.GetConstructors()[0];
                var ctorParams = constructor.GetParameters();
                var args = new object[ctorParams.Length];
                for (var i = 0; i < ctorParams.Length; i++)
                {
                    var param = ctorParams[i];
                    args[i] = _serviceProvider.GetService(param.ParameterType);
                }
                var action = (IBotAction)Activator.CreateInstance(type, args);
                yield return action;
            }
        }
    }
}
