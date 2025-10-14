using Implementacion.Api.Context;
using Implementacion.Api.Models;
using System.Collections.Concurrent;

namespace Implementacion.Api.Flyweight
{
    /// <summary>
    /// Flyweight Factory para Category:
    /// Mantiene instancias compartidas de categorías en memoria
    /// para evitar duplicar objetos en distintas operaciones.
    /// </summary>
    public class CategoryFlyweightFactory
    {
        private readonly AppDbContext _db;

        // Caches seguras para hilos (thread-safe)
        private readonly ConcurrentDictionary<int, Category> _cacheById = new();
        private readonly ConcurrentDictionary<string, Category> _cacheByName = new();

        // Constructor que recibe el contexto (usado al inicializar el Singleton)
        public CategoryFlyweightFactory(AppDbContext db)
        {
            _db = db;
        }

        // 🔹 Obtener o agregar por objeto Category
        public Category GetOrAdd(Category category)
        {
            if (category == null)
                return null;

            // Si ya existe en cache, devuelve la instancia compartida
            var existing = _cacheById.GetOrAdd(category.Id, category);
            _cacheByName.TryAdd(category.Name, existing);

            return existing;
        }

        // 🔹 Buscar por Id en cache
        public bool TryGetById(int id, out Category category)
        {
            return _cacheById.TryGetValue(id, out category);
        }

        // 🔹 Buscar o cargar desde la BD si no está en cache
        public Category GetOrAddByName(string name)
        {
            // Si ya está en cache, se devuelve inmediatamente
            if (_cacheByName.TryGetValue(name, out var cached))
                return cached;

            // Si no está, lo busca en BD
            var dbCategory = _db.Categories.FirstOrDefault(c => c.Name == name);
            if (dbCategory != null)
            {
                // Guarda en cache y devuelve
                _cacheById.TryAdd(dbCategory.Id, dbCategory);
                _cacheByName.TryAdd(dbCategory.Name, dbCategory);
                return dbCategory;
            }

            // Si no existe, se puede crear automáticamente
            var newCategory = new Category { Name = name };
            _db.Categories.Add(newCategory);
            _db.SaveChanges();

            // Registrar en cache
            _cacheById.TryAdd(newCategory.Id, newCategory);
            _cacheByName.TryAdd(newCategory.Name, newCategory);

            return newCategory;
        }
    }
}
