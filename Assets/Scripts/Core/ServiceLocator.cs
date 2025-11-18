using UnityEngine;
using System.Collections.Generic;

namespace SmolTheftAuto.Core
{
    // Service locator pattern for decoupled service registration and access
    // Allows systems to find services without direct dependencies
    public static class ServiceLocator
    {
        private static Dictionary<System.Type, object> services = new Dictionary<System.Type, object>();

        // Register a service
        public static void Register<T>(T service) where T : class
        {
            services[typeof(T)] = service;
        }

        // Unregister a service
        public static void Unregister<T>() where T : class
        {
            services.Remove(typeof(T));
        }

        // Get a service
        public static T Get<T>() where T : class
        {
            if (services.TryGetValue(typeof(T), out object service))
            {
                return service as T;
            }
            return null;
        }

        // Check if a service is registered
        public static bool IsRegistered<T>() where T : class
        {
            return services.ContainsKey(typeof(T));
        }

        // Clear all services (useful for scene transitions)
        public static void Clear()
        {
            services.Clear();
        }
    }
}

