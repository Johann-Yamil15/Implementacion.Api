namespace Implementacion.Api.Singleton
{
    public static class SingletonConstruct<T> where T : class
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance => _instance ??
            throw new InvalidOperationException(
                $"El singleton de {typeof(T).Name} no ha sido inicializado. Llama Initialize() primero.");

        public static void Initialize(params object[] args)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)Activator.CreateInstance(typeof(T), args)!;
                    }
                }
            }
        }
    }
}
