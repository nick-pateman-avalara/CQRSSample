using CQRSAPI.Extensions;
using CQRSAPI.Models;
using CQRSAPI.RequestHandlers;
using CQRSAPI.Requests;
using CQRSAPI.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CQRSAPI.xUnitTests
{

    public class PeopleCqrsTests
    {

        private Random _rnd;
        private PeopleContext _peopleContext;
        private Person _created;

        private void Initialise()
        {
            _rnd = new Random(Environment.TickCount);

            DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
            {
                { "Data Source", @"." },
                { "Initial Catalog", "CQRSAPI" },
                { "Integrated Security", "true" }
            };

            DbContextOptionsBuilder<PeopleContext> optionsBuilder = new DbContextOptionsBuilder<PeopleContext>();
            optionsBuilder.UseSqlServer(dbConnectionStringBuilder.ToString());
            PeopleContext peopleContext = new PeopleContext(optionsBuilder.Options);
            _peopleContext = peopleContext;
        }

        [Fact]
        public async Task Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord()
        {
            Initialise();

            Person person = new Person()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Age = _rnd.Next(1, 101)
            };

            CreatePersonRequestHandler handler = new CreatePersonRequestHandler(
                _peopleContext,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            _created = createdPerson.Result;
            Assert.True(createdPerson.Success);
        }

        [Fact]
        public async Task Given_CreatePersonCqrsRequest_When_FirstNameIsMissing_Then_FailureShouldBeReturnedWithModelErrorList()
        {
            Initialise();

            Person person = new Person()
            {
                FirstName = String.Empty,
                LastName = Guid.NewGuid().ToString(),
                Age = _rnd.Next(1, 101)
            };

            CreatePersonRequestHandler handler = new CreatePersonRequestHandler(
                _peopleContext,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            List<ModelError> errors = createdPerson.Errors;
            Assert.True(!createdPerson.Success);
            Assert.True(errors != null);
            Assert.True(errors.Count == 1);
            Assert.Contains("FirstName", errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Given_CreatePersonCqrsRequest_When_LastNameIsInvalidLength_Then_FailureShouldBeReturnedWithModelErrorList()
        {
            Initialise();

            Person person = new Person()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = new string('a', 101),
                Age = _rnd.Next(1, 101)
            };

            CreatePersonRequestHandler handler = new CreatePersonRequestHandler(
                _peopleContext,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            List<ModelError> errors = createdPerson.Errors;
            Assert.True(!createdPerson.Success);
            Assert.True(errors != null);
            Assert.True(errors.Count == 1);
            Assert.Contains("LastName", errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Given_CreatePersonCqrsRequest_When_AgeIsLessThan1_Then_FailureShouldBeReturnedWithModelErrorList()
        {
            Initialise();

            Person person = new Person()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Age = 0
            };

            CreatePersonRequestHandler handler = new CreatePersonRequestHandler(
                _peopleContext,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            List<ModelError> errors = createdPerson.Errors;
            Assert.True(!createdPerson.Success);
            Assert.True(errors != null);
            Assert.True(errors.Count == 1);
            Assert.Contains("Age", errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Given_CreatePersonCqrsRequest_When_AgeIsGreaterThan100_Then_FailureShouldBeReturnedWithModelErrorList()
        {
            Initialise();

            Person person = new Person()
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Age = 101
            };

            CreatePersonRequestHandler handler = new CreatePersonRequestHandler(
                _peopleContext,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            List<ModelError> errors = createdPerson.Errors;
            Assert.True(!createdPerson.Success);
            Assert.True(errors != null);
            Assert.True(errors.Count == 1);
            Assert.Contains("Age", errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Given_GetPersonCqrsRequest_When_ProvidedWithAnExistingPersonId_Then_RecordShouldBeReturnedWithSuccess()
        {
            Initialise();

            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            Assert.True(
                _created != null, 
                "Unit test cannot be run before creating a user.");

            GetPersonRequestHandler handler = new GetPersonRequestHandler(_peopleContext);
            Person getPerson = await handler.Handle(new GetPersonRequest() { PersonId = _created.PersonId }, new CancellationToken());

            Assert.True(getPerson != null);
            Assert.True(getPerson.IsEqual(_created));
        }

        [Fact]
        public async Task Given_GetPersonCqrsRequest_When_ProvidedWithAnInvalidPersonId_Then_FailureShouldBeReturned()
        {
            Initialise();

            GetPersonRequestHandler handler = new GetPersonRequestHandler(_peopleContext);
            Person getPerson = await handler.Handle(new GetPersonRequest() { PersonId = 0 }, new CancellationToken());

            Assert.True(getPerson == null);
        }

        [Fact]
        public async Task Given_GetAllPeopleCqrsRequest_When_RecordsAreInTheDatabase_Then_SuccessShouldBeReturnedWithAllPeopleRecords()
        {
            Initialise();

            //Make sure we have at least 1 record in the DB
            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            GetAllPeopleRequestHandler handler = new GetAllPeopleRequestHandler(_peopleContext);
            List<Person> people = await handler.Handle(new GetAllPeopleRequest(), new CancellationToken());

            Assert.True(people != null);
            Assert.True(people.Count > 0);
        }

        [Fact]
        public async Task Given_DeletePersonCqrsRequest_When_ProvidedWithAnExistingPersonId_Then_RecordShouldBeReturnedWithSuccess()
        {
            Initialise();

            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            Assert.True(
                _created != null,
                "Unit test cannot be run before creating a user.");

            DeletePersonRequestHandler handler = new DeletePersonRequestHandler(_peopleContext);
            bool deleted = await handler.Handle(new DeletePersonRequest() { PersonId = _created.PersonId }, new CancellationToken());

            Assert.True(deleted);
        }

        [Fact]
        public async Task Given_DeletePersonCqrsRequest_When_ProvidedWithAnInvalidPersonId_Then_RecordShouldBeReturnedWithSuccess()
        {
            Initialise();

            DeletePersonRequestHandler handler = new DeletePersonRequestHandler(_peopleContext);
            bool deleted = await handler.Handle(new DeletePersonRequest() { PersonId = 0 }, new CancellationToken());

            Assert.False(deleted);
        }


    }

}
