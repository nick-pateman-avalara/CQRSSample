using CQRSAPI.Data;
using CQRSAPI.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using CQRSAPI.People.Data;
using CQRSAPI.People.Extensions;
using CQRSAPI.People.Models;
using CQRSAPI.People.RequestHandlers;
using CQRSAPI.People.Requests;
using CQRSAPI.People.Responses;
using CQRSAPI.Responses;
using Xunit;

namespace CQRSAPI.xUnitTests
{

    public class PeopleCqrsTests
    {

        private Random _rnd;
        private CqrsApiPeopleSqlRepository _peopleSqlRepository;
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
            _peopleSqlRepository = new CqrsApiPeopleSqlRepository(dbConnectionStringBuilder.ToString());
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
                _peopleSqlRepository,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            _created = createdPerson.Value;
            Assert.True(createdPerson.Result == ApiResponse<Person>.ResponseType.Ok);
        }

        [Fact]
        public async Task Given_CreatePersonCqrsRequest_When_FirstNameIsMissing_Then_FailureShouldBeReturnedWithModelErrorList()
        {
            Initialise();

            Person person = new Person()
            {
                FirstName = string.Empty,
                LastName = Guid.NewGuid().ToString(),
                Age = _rnd.Next(1, 101)
            };

            CreatePersonRequestHandler handler = new CreatePersonRequestHandler(
                _peopleSqlRepository,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            List<ModelError> errors = createdPerson.Errors;
            Assert.True(createdPerson.Result == ApiResponse<Person>.ResponseType.BadRequest);
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
                _peopleSqlRepository,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            Assert.True(createdPerson.Result == ApiResponse<Person>.ResponseType.BadRequest);
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
                _peopleSqlRepository,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            Assert.True(createdPerson.Result == ApiResponse<Person>.ResponseType.BadRequest);
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
                _peopleSqlRepository,
                new PersonValidator());
            CreatePersonResponse createdPerson = await handler.Handle(new CreatePersonRequest() { Person = person }, new CancellationToken());

            Assert.True(createdPerson.Result == ApiResponse<Person>.ResponseType.BadRequest);
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

            GetPersonRequestHandler handler = new GetPersonRequestHandler(_peopleSqlRepository);
            GetPersonResponse reponse = await handler.Handle(new GetPersonRequest() { Id = _created.Id }, new CancellationToken());

            Assert.True(reponse.Result == ApiResponse<Person>.ResponseType.Ok);
            Assert.True(reponse.Value.IsEqual(_created));
        }

        [Fact]
        public async Task Given_GetPersonCqrsRequest_When_ProvidedWithAnInvalidPersonId_Then_FailureShouldBeReturned()
        {
            Initialise();

            GetPersonRequestHandler handler = new GetPersonRequestHandler(_peopleSqlRepository);
            GetPersonResponse reponse = await handler.Handle(new GetPersonRequest() { Id = 0 }, new CancellationToken());

            Assert.True(reponse.Result == ApiResponse<Person>.ResponseType.NotFound);
        }

        [Fact]
        public async Task Given_GetAllPeopleCqrsRequest_When_RecordsAreInTheDatabase_AndPaginationParametersAreValid_Then_SuccessShouldBeReturnedWithAllPeopleRecords()
        {
            Initialise();

            //Make sure we have at least 1 record in the DB
            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            GetAllPeopleRequestHandler handler = new GetAllPeopleRequestHandler(_peopleSqlRepository);
            GetAllPeopleResponse response = await handler.Handle(new GetAllPeopleRequest() { PageNumber = 1, PageSize = 50 }, new CancellationToken());

            Assert.True(response.Result == ApiResponse<List<Person>>.ResponseType.Ok);
        }

        [Fact]
        public async Task Given_GetAllPeopleCqrsRequest_When_RecordsAreInTheDatabase_AndPageNumberIsInvalid_Then_SuccessShouldBeReturnedWithAllPeopleRecords()
        {
            Initialise();

            //Make sure we have at least 1 record in the DB
            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            GetAllPeopleRequestHandler handler = new GetAllPeopleRequestHandler(_peopleSqlRepository);
            GetAllPeopleResponse response = await handler.Handle(new GetAllPeopleRequest() { PageNumber = 0, PageSize = 50 }, new CancellationToken());

            Assert.False(response.Result == ApiResponse<List<Person>>.ResponseType.Ok);
            Assert.True(response.Errors != null);
            Assert.True(response.Errors.Count == 1);
            Assert.Contains("PageNumber", response.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Given_GetAllPeopleCqrsRequest_When_RecordsAreInTheDatabase_AndPageSizeIsInvalid_Then_SuccessShouldBeReturnedWithAllPeopleRecords()
        {
            Initialise();

            //Make sure we have at least 1 record in the DB
            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            GetAllPeopleRequestHandler handler = new GetAllPeopleRequestHandler(_peopleSqlRepository);
            GetAllPeopleResponse response = await handler.Handle(new GetAllPeopleRequest() { PageNumber = 1, PageSize = 0 }, new CancellationToken());

            Assert.False(response.Result == ApiResponse<List<Person>>.ResponseType.Ok);
            Assert.True(response.Errors != null);
            Assert.True(response.Errors.Count == 1);
            Assert.Contains("PageSize", response.Errors[0].ErrorMessage);
        }

        [Fact]
        public async Task Given_DeletePersonCqrsRequest_When_ProvidedWithAnExistingPersonId_Then_RecordShouldBeReturnedWithSuccess()
        {
            Initialise();

            await Given_CreatePersonCqrsRequest_When_PersonDataIsValid_Then_RecordShouldBeCreatedAndSuccessReturnedWithRecord();

            Assert.True(
                _created != null,
                "Unit test cannot be run before creating a user.");

            DeletePersonRequestHandler handler = new DeletePersonRequestHandler(_peopleSqlRepository);
            DeletePersonResponse response = await handler.Handle(new DeletePersonRequest() { Id = _created.Id }, new CancellationToken());

            Assert.True(response.Result == ApiResponse<bool>.ResponseType.Ok);
        }

        [Fact]
        public async Task Given_DeletePersonCqrsRequest_When_ProvidedWithAnInvalidPersonId_Then_RecordShouldBeReturnedWithSuccess()
        {
            Initialise();

            DeletePersonRequestHandler handler = new DeletePersonRequestHandler(_peopleSqlRepository);
            DeletePersonResponse response = await handler.Handle(new DeletePersonRequest() { Id = 0 }, new CancellationToken());

            Assert.False(response.Result == ApiResponse<bool>.ResponseType.Ok);
        }


    }

}
