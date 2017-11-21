namespace BibleTraining.Api.Person
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Address;
    using Email;
    using Entities;
    using Improving.Highway.Data.Scope.Repository;
    using Miruken;
    using Miruken.Callback;
    using Miruken.Callback.Policy;
    using Miruken.Map;
    using Miruken.Mediate;
    using Phone;
    using Queries;

    public abstract class PersonAggregateHandlerBase : PipelineHandler,
        IGlobalMiddleware<UpdatePerson, PersonData>,
        IGlobalMiddleware<RemovePerson, PersonData>
    {
        protected readonly IRepository<IBibleTrainingDomain> _repository;

        protected PersonAggregateHandlerBase(
            IRepository<IBibleTrainingDomain> repository)
        {
            _repository = repository;
        }

        public int? Order { get; set; } = Stage.Validation - 1;

        protected abstract Task<Person> Person(int? id, IHandler composer);

        public async Task<PersonData> Begin(
            int? id, IHandler composer, NextDelegate<Task<PersonData>> next)
        {
            using (var scope = _repository.Scopes.Create())
            {
                var person = await Person(id, composer);
                var result = await next();
                await scope.SaveChangesAsync();

                result.RowVersion = person?.RowVersion;
                return result;
            }
        }

        public virtual Task<object[]> GetUpdateRelationships(
            UpdatePerson request, Person person, IHandler composer)
        {
            return Task.FromResult(Array.Empty<object>());
        }

        [Mediates]
        public async Task<PersonData> Create(
            CreatePerson request, IHandler composer,
            StashOf<Person> personStash)
        {
            using(var scope = _repository.Scopes.Create())
            {
                var person = personStash.Value =
                    composer.Proxy<IMapping>().Map<Person>(request.Resource);
                person.Created = DateTime.Now;
                _repository.Context.Add(person);

                var relationships = new List<object>();

                var emails = request.Resource.Emails;
                if (emails != null)
                {
                    var adds = emails.Where(x => !x.Id.HasValue).ToArray();
                    relationships.AddRange(adds.Select(add => new CreateEmail(add)));
                }

                var addresses = request.Resource.Addresses;
                if (addresses != null)
                {
                    var adds = addresses.Where(x => !x.Id.HasValue).ToArray();
                    relationships.AddRange(adds.Select(add => new CreateAddress(add)));
                }

                var phones = request.Resource.Phones;
                if (phones != null)
                {
                    var adds = phones.Where(x => !x.Id.HasValue).ToArray();
                    relationships.AddRange(adds.Select(add => new CreatePhone(add)));
                }

                foreach (var relationship in relationships)
                    await composer.Send(relationship);

                var data = new PersonData();
                await scope.SaveChangesAsync((dbScope, count) =>
                {
                    data.Id         = person.Id;
                    data.RowVersion = person.RowVersion;
                });

                return data;
            }
        }

        [Mediates]
        public async Task<PersonResult> Get(GetPeople message, IHandler composer)
        {
            using (_repository.Scopes.CreateReadOnly())
            {
                var people = (await _repository
                    .FindAsync(new GetPeopleById(message.Ids)
                      {
                         IncludePhones    = message.IncludePhones,
                         IncludeEmails    = message.IncludeEmails,
                         IncludeAddresses = message.IncludeAddresses
                      }))
                    .Select(x => composer.Proxy<IMapping>().Map<PersonData>(x))
                    .ToArray();

                return new PersonResult
                {
                    People = people
                };
            }
        }

        public async Task<PersonData> Next(
            UpdatePerson request, MethodBinding method, 
            IHandler composer, NextDelegate<Task<PersonData>> next)
        {
            return await Begin(request.Resource.Id, composer, next);
        }

        [Mediates]
        public async Task<PersonData> Update(
            UpdatePerson request, IHandler composer,
            StashOf<Person> personStash)
        {
            var person = await Person(request.Resource.Id, composer);
            composer.Proxy<IMapping>().MapInto(request.Resource, person);
            personStash.Value = person;

            var relationships = await GetUpdateRelationships(request, person, composer);

            foreach (var relationship in relationships)
                await composer.Send(relationship);

            return new PersonData
            {
                Id = request.Resource.Id
            };
        }

        public async Task<PersonData> Next(
            RemovePerson request, MethodBinding method, 
            IHandler composer, NextDelegate<Task<PersonData>> next)
        {
            return await Begin(request.Resource.Id, composer, next);
        }

        [Mediates]
        public async Task<PersonData> Remove(RemovePerson request, IHandler composer)
        {
            var person = await Person(request.Resource.Id, composer);
            _repository.Context.Remove(person);

            return new PersonData
            {
                Id         = person.Id,
                RowVersion = person.RowVersion
            };
        }
    }
}
