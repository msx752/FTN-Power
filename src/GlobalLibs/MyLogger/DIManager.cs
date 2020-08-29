using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public static class DIManager
    {
        static Lazy<IServiceProvider> _serviceProvider;
        public static IServiceProvider Services
        {
            get
            {
                var val = _serviceProvider?.Value;
                if (val == null) return val;
                lock (val)
                    return val;
            }
            private set
            {
                _serviceProvider = new Lazy<IServiceProvider>(value);
            }
        }
        /// <summary>
        /// for .Net Core MVC
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void SetManualService(IServiceProvider serviceProvider)
        {
            if (Services != null)
                throw new InvalidOperationException("ServiceProvider already initiated");
            Services = serviceProvider;
        }
        /// <summary>
        /// Console Application
        /// </summary>
        /// <param name="actionServiceCollection"></param>
        public static void BuildService(Action<ServiceCollection> actionServiceCollection)
        {
            if (Services != null)
                throw new InvalidOperationException("ServiceProvider already initiated");
            var sc = new ServiceCollection();
            actionServiceCollection(sc);
            Services = sc.BuildServiceProvider();
        }
    }
}
