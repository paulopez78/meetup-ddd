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
    public class MeetupRepository
    {
        private readonly IMongoCollection<MeetupMongoDocument> _collection;

        public MeetupRepository(IConfiguration config)
        {
            var connectionString = config["mongo"] ?? "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            _collection = client.GetDatabase("netcorebcn").GetCollection<MeetupMongoDocument>("meetup");
        }

        public async Task<MeetupAggregate> Get(Guid meetupId)
        {
            var doc = await _collection.Find<MeetupMongoDocument>(doc => doc.Id == meetupId).FirstOrDefaultAsync();

            if (doc == null)
            {
                return null;
            }

            return new MeetupAggregate(
                MeetupId.From(doc.Id),
                new MeetupTitle(doc.Title),
                ValidatedLocation.Parse(doc.Location),
                NumberOfSeats.Parse(doc.NumberOfSeats),
                doc.State.ToLower() switch
                {
                    "published" => MeetupState.Published,
                    "canceled" => MeetupState.Canceled,
                    "closed" => MeetupState.Closed,
                    "created" => MeetupState.Created,
                    _ => MeetupState.Created
                },
                doc.Going.ToDictionary(x => new MemberId(x.Key), v => v.Value),
                doc.NotGoing.ToDictionary(x => new MemberId(x.Key), v => v.Value));
        }

        public async Task Save(MeetupAggregate meetup)
        {
            await _collection.ReplaceOneAsync(doc => doc.Id == meetup.Id, new MeetupMongoDocument
            {
                Id = meetup.Id,
                Title = meetup.Title,
                Location = meetup.Location,
                NumberOfSeats = meetup.NumberOfSeats,
                State = meetup.State.ToString(),
                Going = meetup.Going.ToDictionary(x => x.Key.Value, y => y.Value),
                NotGoing = meetup.NotGoing.ToDictionary(x => x.Key.Value, y => y.Value)
            }, new UpdateOptions
            {
                IsUpsert = true
            });
        }
    }

    internal class MeetupMongoDocument
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public int NumberOfSeats { get; set; }
        public string State { get; set; }
        public Dictionary<Guid, DateTime> Going { get; set; }
        public Dictionary<Guid, DateTime> NotGoing { get; set; }
    }
}