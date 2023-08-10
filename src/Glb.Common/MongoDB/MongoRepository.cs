using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Glb.Common.Inerfaces;
using System.Linq;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Glb.Common.Exceptions;

namespace Glb.Common.MongoDB
{

    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;
        private readonly ILogger<T>? logger;
        private readonly AsyncRetryPolicy retryPolicy;


        public MongoRepository(IMongoDatabase database, string collectionName,ILogger<T>? logger)
        {
            dbCollection = database.GetCollection<T>(collectionName);
            this.logger=logger;
            // Define a retry policy: wait and retry up to 5 times, 
            // waiting for 2, 4, 8, 16 and 32 seconds between attempts
            retryPolicy = Policy
            .Handle<NoDocumentsModifiedException>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

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

        public async Task<T> CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await dbCollection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);

            ReplaceOneResult result = await dbCollection.ReplaceOneAsync(filter, entity);
            if (result.ModifiedCount == 0) 
            {
                // Log: No documents were updated
                if(logger!=null){
                logger.LogCritical("No documents were updated for Id:{Id}",entity.Id);
                }
            }
            else if (result.ModifiedCount > 1) 
            {
                // Log: More than one document were updated. This is not expected in a ReplaceOne operation.
            }          
            return entity;
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