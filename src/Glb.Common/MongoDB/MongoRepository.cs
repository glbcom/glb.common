using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Glb.Common.Inerfaces;
using System.Linq;

namespace Glb.Common.MongoDB
{

    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).ToListAsync();
        }

        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await dbCollection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        public async Task SaveAll(List<T> lstEntity)
        {
            IEnumerable<T> lstInsert = lstEntity.Where(entity => entity.Id == Guid.Empty);
            await dbCollection.InsertManyAsync(lstEntity);

            IEnumerable<T> lstUpdate = lstEntity.Where(entity => entity.Id != Guid.Empty);
            // foreach (T entity in lstEntity) await UpdateAsync(entity);
            var listWrites = new List<WriteModel<T>>();
            foreach (T entity in lstUpdate)
            {
                var filterDefinition = Builders<T>.Filter.Eq(p => p.Id, new Guid());
                var updateDefinition = Builders<T>.Update.Set(p => p, entity);

                listWrites.Add(new UpdateOneModel<T>(filterDefinition, updateDefinition));
            }
            await dbCollection.BulkWriteAsync(listWrites);
        }

        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}