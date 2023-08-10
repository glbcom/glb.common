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
            
            int retryCount=0;
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);

             await retryPolicy.ExecuteAsync(async () =>
            {
                try
                {
                    ReplaceOneResult result = await dbCollection.ReplaceOneAsync(filter, entity);
                    if (result.ModifiedCount < 1) 
                   {
                        // Log: No documents were updated
                        retryCount+=1;
                        if(logger!=null){
                        logger.LogError("No documents were updated for Id:{Id}, retry count:{retryCount}, ModifiedCount:{ModifiedCount}",entity.Id,retryCount,result.ModifiedCount);
                        }
                        throw new NoDocumentsModifiedException($"No documents were updated for Id:{entity.Id} retry count:{retryCount}");
                    }
                    else if (result.ModifiedCount > 1) 
                    {
                        // Log: More than one document were updated. This is not expected in a ReplaceOne operation.
                         if(logger!=null){
                        logger.LogError("More than one document were updated. This is not expected in a ReplaceOne operation");
                         }
                    } 
                    else 
                    {
                         if(logger!=null){
                            logger.LogInformation("document updated successfully for Id:{Id}",entity.Id);
                         } 
                    }
                
                }catch(Exception ex){
                      if(logger!=null){
                        logger.LogError(ex, "Unexpected error updating entity {EntityId}", entity.Id);
                      }
                }
            });         
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