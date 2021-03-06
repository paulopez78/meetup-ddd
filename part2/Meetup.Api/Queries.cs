using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Meetup.Domain;

namespace Meetup.Api
{
    public static class Queries
    {
        public static async Task<TReadModel> Query<TReadModel, TProjection>(this IDocumentStore eventStore, Guid id)
            where TProjection : IProjection<TReadModel>, new()
        {
            using var session = eventStore.OpenSession();
            var stream = await session.Events.FetchStreamAsync(id);

            return new TProjection().Project(stream.Select(@event => @event.Data).ToArray());
        }

        public static async Task<TReadModel> Query<TReadModel>(this IDocumentStore eventStore, Guid id)
        {
            using var session = eventStore.OpenSession();
            return await session.LoadAsync<TReadModel>(id);
        }
    }
}