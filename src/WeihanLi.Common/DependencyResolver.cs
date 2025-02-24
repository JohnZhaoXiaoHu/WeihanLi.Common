﻿using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common;

/// <summary>
/// DependencyResolver
/// Service locator pattern
/// </summary>
public static class DependencyResolver
{
    public static IDependencyResolver Current { get; private set; } = new DefaultDependencyResolver();

    /// <summary>
    /// locker
    /// </summary>
    private static readonly object _lock = new();

    public static TService? ResolveService<TService>() => Current.ResolveService<TService>();

    public static TService ResolveRequiredService<TService>() => Current.ResolveRequiredService<TService>();

    public static IEnumerable<TService> ResolveServices<TService>() => Current.ResolveServices<TService>();

    public static bool TryInvoke<TService>(Action<TService> action) => Current.TryInvokeService(action);

    public static Task<bool> TryInvokeAsync<TService>(Func<TService, Task> action) => Current.TryInvokeServiceAsync(action);

    public static void SetDependencyResolver(IDependencyResolver dependencyResolver)
    {
        lock (_lock)
        {
            Current = dependencyResolver;
        }
    }

    public static void SetDependencyResolver(IServiceContainer serviceContainer) => SetDependencyResolver(new ServiceContainerDependencyResolver(serviceContainer));

    public static void SetDependencyResolver(IServiceProvider serviceProvider) => SetDependencyResolver(serviceProvider.GetService);

    public static void SetDependencyResolver(Func<Type, object?> getServiceFunc) => SetDependencyResolver(getServiceFunc, serviceType => (IEnumerable<object>)Guard.NotNull(getServiceFunc(typeof(IEnumerable<>).MakeGenericType(serviceType))));

    public static void SetDependencyResolver(Func<Type, object?> getServiceFunc, Func<Type, IEnumerable<object>> getServicesFunc) => SetDependencyResolver(new DelegateBasedDependencyResolver(getServiceFunc, getServicesFunc));

    public static void SetDependencyResolver(IServiceCollection services) => SetDependencyResolver(new ServiceCollectionDependencyResolver(services));

    private sealed class ServiceCollectionDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceCollectionDependencyResolver(IServiceCollection services)
        {
            _serviceProvider = services.BuildServiceProvider();
        }

        public object? GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceProvider.GetServices(serviceType);
        }

        public bool TryInvokeService<TService>(Action<TService> action)
        {
            Guard.NotNull(action, nameof(action));
            using var scope = _serviceProvider.CreateScope();
            var svc = scope.ServiceProvider.GetService(typeof(TService));
            if (svc is TService service)
            {
                action.Invoke(service);
                return true;
            }
            return false;
        }

        public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action)
        {
            Guard.NotNull(action, nameof(action));
            using var scope = _serviceProvider.CreateScope();
            var svc = scope.ServiceProvider.GetService(typeof(TService));
            if (svc is TService service)
            {
                await action.Invoke(service);
                return true;
            }
            return false;
        }
    }

    private sealed class DefaultDependencyResolver : IDependencyResolver
    {
        public object? GetService(Type serviceType)
        {
            // Since attempting to create an instance of an interface or an abstract type results in an exception, immediately return null
            // to improve performance and the debugging experience with first-chance exceptions enabled.
            if (serviceType.IsInterface || serviceType.IsAbstract)
            {
                return null;
            }
            try
            {
                return Activator.CreateInstance(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType) => Enumerable.Empty<object>();

        public bool TryInvokeService<TService>(Action<TService>? action)
        {
            var service = GetService(typeof(TService));
            if (null == service || action == null)
            {
                return false;
            }
            action.Invoke((TService)service);
            return true;
        }

        public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task>? action)
        {
            var service = GetService(typeof(TService));
            if (null == service || action == null)
            {
                return false;
            }
            await action.Invoke((TService)service);
            return true;
        }
    }

    private sealed class DelegateBasedDependencyResolver : IDependencyResolver
    {
        private readonly Func<Type, object?> _getService;
        private readonly Func<Type, IEnumerable<object>> _getServices;

        public DelegateBasedDependencyResolver(Func<Type, object?> getService, Func<Type, IEnumerable<object>> getServices)
        {
            _getService = getService;
            _getServices = getServices;
        }

        public object? GetService(Type serviceType)
        => _getService(serviceType);

        public IEnumerable<object> GetServices(Type serviceType)
            => _getServices(serviceType);

        public bool TryInvokeService<TService>(Action<TService>? action)
        {
            var svc = GetService(typeof(TService));
            if (action != null && svc is TService service)
            {
                action.Invoke(service);
                return true;
            }
            return false;
        }

        public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task>? action)
        {
            var svc = GetService(typeof(TService));
            if (action != null && svc is TService service)
            {
                await action.Invoke(service);
                return true;
            }
            return false;
        }
    }

    private sealed class ServiceContainerDependencyResolver : IDependencyResolver
    {
        private readonly IServiceContainer _rootContainer;

        public ServiceContainerDependencyResolver(IServiceContainer serviceContainer)
        {
            _rootContainer = serviceContainer;
        }

        public object? GetService(Type serviceType)
        {
            return _rootContainer.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return (IEnumerable<object>)Guard.NotNull(_rootContainer.GetService(typeof(IEnumerable<>).MakeGenericType(serviceType)));
        }

        public bool TryInvokeService<TService>(Action<TService> action)
        {
            Guard.NotNull(action, nameof(action));

            using var scope = _rootContainer.CreateScope();
            var svc = scope.GetService(typeof(TService));
            if (svc is TService service)
            {
                action.Invoke(service);
                return true;
            }
            return false;
        }

        public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action)
        {
            Guard.NotNull(action, nameof(action));
            using var scope = _rootContainer.CreateScope();
            var svc = scope.GetService(typeof(TService));
            if (svc is TService service)
            {
                await action.Invoke(service);
                return true;
            }
            return false;
        }
    }
}
