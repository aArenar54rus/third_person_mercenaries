using System;
using System.Collections.Generic;
using System.Linq;


namespace Arenar.SpawnerFactory
{
    public class CharacterKittyFactoryService : ISpawnerFactoriesService
    {
        private Dictionary<Type, IFactoryProvider<KittySpawnerElement>> kittySpawners;


        public void Initialize()
        {
            Type baseType = typeof(KittySpawnerFactoryProvider);
            Type[] implementations = GetImplementations(baseType);
            kittySpawners = new Dictionary<Type, IFactoryProvider<KittySpawnerElement>>();

            foreach (var implementation in implementations)
            {
                KittySpawnerFactoryProvider instance = (KittySpawnerFactoryProvider)Activator.CreateInstance(implementation);
                kittySpawners.Add(implementation, instance);
            }
        }

        public void DeInitialize() {}

        public virtual TFactoryProvider GetFactoryProvider<TFactoryProvider>()
            where TFactoryProvider : IFactoryProvider<ISpawnerElement>
        {
            return (TFactoryProvider)kittySpawners[typeof(TFactoryProvider)];
        }
        
        public virtual T GetWindow<T>() where T : KittySpawnerFactoryProvider
        {
            T window = (T)kittySpawners[typeof(T)];
            return window;
        }
        
        /// <summary>Returns non-abstract and non-generic implementations of type.</summary>
        private static Type[] GetImplementations(Type baseType)
        {
            bool ImplementationCondition(Type type) =>
                baseType.IsAssignableFrom(type)
                && type != baseType
                && !type.IsAbstract
                && !type.IsGenericType;

            return GetAllMatchingTypes(baseType, ImplementationCondition);
        }
        
        private static Type[] GetAllMatchingTypes(Type baseType, Func<Type, bool> predicate)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(predicate)
                .ToArray();
        }
    }
}