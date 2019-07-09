using CQRSAPI.Data;
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
        private CqrsApiPeopleRepository _peopleRepository;
        private Person _created;

        private void Initialise()
        {
            _rnd = new Random(Environment.TickCount);

            DbConnectionStringBuilder dbConnectionStringBuilder = new DbConnectionStringBuilder
            {
                { "Data Source", @"." },
                { "Initial Catalog", "CQRSAPIDB" },
                { "Integrated Security", "true" }
            };
            _peopleRepository = new CqrsApiPeopleRepository(dbConnectionStringBuilder.ToString());
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
                _peopleRepository,
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
                _peopleRepository,
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
                _peopleRepository,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            Assert.True(!createdPerson.Success);
            Assert.True(createdPerson.Errors != null);
            Assert.True(createdPerson.Errors.Count == 1);
            Assert.Contains("LastName", createdPerson.Errors[0].ErrorMessage);
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
                _peopleRepository,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            Assert.True(!createdPerson.Success);
            Assert.True(createdPerson.Errors != null);
            Assert.True(createdPerson.Errors.Count == 1);
            Assert.Contains("Age", createdPerson.Errors[0].ErrorMessage);
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
                _peopleRepository,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            Assert.True(!createdPerson.Success);
            Assert.True(createdPerson.Errors != null);
            Assert.True(createdPerson.Errors.Count == 1);
            Assert.Contains("Age", createdPerson.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Given_GetPersonCqrsRequest_When_ProvidedWithAnExistingPersonId_Then_RecordShouldBeReturnedWithSuccess()
        {
            Initialise();

            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            Assert.True(
                _created != null, 
                "Unit test cannot be run before creating a user.");

            GetPersonRequestHandler handler = new GetPersonRequestHandler(_peopleRepository);
            Person getPerson = await handler.Handle(new GetPersonRequest() { Id = _created.Id }, new CancellationToken());

            Assert.True(getPerson != null);
            Assert.True(getPerson.IsEqual(_created));
        }

        [Fact]
        public async Task Given_GetPersonCqrsRequest_When_ProvidedWithAnInvalidPersonId_Then_FailureShouldBeReturned()
        {
            Initialise();

            GetPersonRequestHandler handler = new GetPersonRequestHandler(_peopleRepository);
            Person getPerson = await handler.Handle(new GetPersonRequest() { Id = 0 }, new CancellationToken());

            Assert.True(getPerson == null);
        }

        [Fact]
        public async Task Given_GetAllPeopleCqrsRequest_When_RecordsAreInTheDatabase_AndPaginationParametersAreValid_Then_SuccessShouldBeReturnedWithAllPeopleRecords()
        {
            Initialise();

            //Make sure we have at least 1 record in the DB
            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            GetAllPeopleRequestHandler handler = new GetAllPeopleRequestHandler(_peopleRepository);
            GetAllPeopleResponse getAllPeople = await handler.Handle(new GetAllPeopleRequest() { PageNumber = 1, PageSize = 50 }, new CancellationToken());

            Assert.True(getAllPeople.Success);
        }

        [Fact]
        public async Task Given_GetAllPeopleCqrsRequest_When_RecordsAreInTheDatabase_AndPageNumberIsInvalid_Then_SuccessShouldBeReturnedWithAllPeopleRecords()
        {
            Initialise();

            //Make sure we have at least 1 record in the DB
            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            GetAllPeopleRequestHandler handler = new GetAllPeopleRequestHandler(_peopleRepository);
            GetAllPeopleResponse getAllPeople = await handler.Handle(new GetAllPeopleRequest() { PageNumber = 0, PageSize = 50 }, new CancellationToken());

            Assert.False(getAllPeople.Success);
            Assert.True(getAllPeople.Errors != null);
            Assert.True(getAllPeople.Errors.Count == 1);
            Assert.Contains("PageNumber", getAllPeople.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Given_GetAllPeopleCqrsRequest_When_RecordsAreInTheDatabase_AndPageSizeIsInvalid_Then_SuccessShouldBeReturnedWithAllPeopleRecords()
        {
            Initialise();

            //Make sure we have at least 1 record in the DB
            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            GetAllPeopleRequestHandler handler = new GetAllPeopleRequestHandler(_peopleRepository);
            GetAllPeopleResponse getAllPeople = await handler.Handle(new GetAllPeopleRequest() { PageNumber = 1, PageSize = 0 }, new CancellationToken());

            Assert.False(getAllPeople.Success);
            Assert.True(getAllPeople.Errors != null);
            Assert.True(getAllPeople.Errors.Count == 1);
            Assert.Contains("PageSize", getAllPeople.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Given_DeletePersonCqrsRequest_When_ProvidedWithAnExistingPersonId_Then_RecordShouldBeReturnedWithSuccess()
        {
            Initialise();

            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            Assert.True(
                _created != null,
                "Unit test cannot be run before creating a user.");

            DeletePersonRequestHandler handler = new DeletePersonRequestHandler(_peopleRepository);
            bool deleted = await handler.Handle(new DeletePersonRequest() { Id = _created.Id }, new CancellationToken());

            Assert.True(deleted);
        }

        [Fact]
        public async Task Given_DeletePersonCqrsRequest_When_ProvidedWithAnInvalidPersonId_Then_RecordShouldBeReturnedWithSuccess()
        {
            Initialise();

            DeletePersonRequestHandler handler = new DeletePersonRequestHandler(_peopleRepository);
            bool deleted = await handler.Handle(new DeletePersonRequest() { Id = 0 }, new CancellationToken());

            Assert.False(deleted);
        }


    }

}
