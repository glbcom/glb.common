using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Glb.Common.Settings;
using Glb.Common.Inerfaces;

namespace Glb.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                if (configuration != null)
                {
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                    if (mongoDbSettings != null && serviceSettings != null)
                    {
                        var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                        return mongoClient.GetDatabase(serviceSettings.ServiceName);
                    }
                    else
                    {
                        throw new System.Exception("mongoDbSettings or serviceSettings is null");
                    }
                }
                else
                {
                    throw new System.Exception("Configuration is null");
                }

            });

            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
            where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var database = serviceProvider.GetService<IMongoDatabase>();
                if (database != null)
                {
                    return new MongoRepository<T>(database, collectionName);
                }
                else
                {
                    throw new System.Exception("database is null");
                }

            });

            return services;
        }
    }
}