using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meetup.Domain;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Meetup.Api
{
    public class AttendantsRepository
    {
        private readonly IMongoCollection<AttendantsMongoDocument> _collection;

        public AttendantsRepository(IConfiguration config)
        {
            var connectionString = config["mongo"] ?? "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            _collection = client.GetDatabase("netcorebcn").GetCollection<AttendantsMongoDocument>("attendants");
        }

        public async Task<AttendantsReadModel> Get(Guid meetupId)
        {
            var doc = await _collection.Find<AttendantsMongoDocument>(doc => doc.MeetupId == meetupId).FirstOrDefaultAsync();

            if (doc == null)
            {
                return null;
            }

            return new AttendantsReadModel
            {
                MeetupId = doc.MeetupId,
                MeetupCapacity = doc.MeetupCapacity,
                Waiting = doc.Waiting,
                NotGoing = doc.NotGoing,
                Going = doc.Going
            };
        }

        public async Task Save(AttendantsReadModel readModel)
        {
            await _collection.ReplaceOneAsync(doc => doc.MeetupId == readModel.MeetupId, new AttendantsMongoDocument
            {
                MeetupId = readModel.MeetupId,
                MeetupCapacity = readModel.MeetupCapacity,
                Waiting = readModel.Waiting,
                NotGoing = readModel.NotGoing,
                Going = readModel.Going
            }, new UpdateOptions
            {
                IsUpsert = true
            });
        }
    }

    public class AttendantsMongoDocument
    {
        [BsonId]
        public Guid MeetupId { get; set; }
        public int MeetupCapacity { get; set; }
        public List<Guid> Waiting { get; internal set; } = new List<Guid>();
        public List<Guid> Going { get; internal set; } = new List<Guid>();
        public List<Guid> NotGoing { get; internal set; } = new List<Guid>();
    }
}