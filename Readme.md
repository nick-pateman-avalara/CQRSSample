# CQRS Sample Solution

## Overview

This sample application was created, simply to demonstrate several patterns and design concepts that can be used in order to produce easily testable and workable Apis.

### - CQRS

"Segregate operations that read data from operations that update data by using separate interfaces. This can maximize performance, scalability, and security. Supports the evolution of the system over time through higher flexibility, and prevents update commands from causing merge conflicts at the domain level." - Microsoft

Using this design pattern easily allows us to create highly testable, modular code that can be easily swapped out and replaced.

In this sample, we are partially implementing the CQRS patter n using the MediatR Nuget package.

https://github.com/jbogard/MediatR

##### Classes

* CQRSAPI.Extensions.MediatorExtensions

###### Requests

* CQRSAPI.Features.People.Requests.CreatePersonRequest
* CQRSAPI.Features.People.Requests.DeletePersonRequest
* CQRSAPI.Features.People.Requests.GetAllPeopleRequest
* CQRSAPI.Features.People.Requests.GetPersonRequest
* CQRSAPI.Features.People.Requests.UpdatePersonRequest

> Note : The CQRSAPI.Features.PeopleV2 namespace contains the same set of classes but for the PeopleV2 feature. 

###### Request Handlers

* CQRSAPI.Features.People.RequestHandlers.CreatePersonRequestHandler
* CQRSAPI.Features.People.RequestHandlers.DeletePersonRequestHandler
* CQRSAPI.Features.People.RequestHandlers.GetAllPeopleRequestHandler
* CQRSAPI.Features.People.RequestHandlers.GetPersonRequestHandler
* CQRSAPI.Features.People.RequestHandlers.UpdatePersonRequestHandler

> Note : The CQRSAPI.Features.PeopleV2 namespace contains the same set of classes but for the PeopleV2 feature.

###### Responses

> Note: Responses are a way of wrapping the result of a CQRSs Request in a way that makes it easier for the consuming code to assertain the result of the operation, including any error messages.

* CQRSAPI.Features.People.Responses.CreatePersonResponse
* CQRSAPI.Features.People.Responses.DeletePersonResponse
* CQRSAPI.Features.People.Responses.GetAllPeopleResponse
* CQRSAPI.Features.People.Responses.GetPersonResponse
* CQRSAPI.Features.People.Responses.UpdatePersonResponse

> Note : The CQRSAPI.Features.PeopleV2 namespace contains the same set of classes but for the PeopleV2 feature.

### - Feature Folders

Using the 'Feature Folders' source directory structure, allos us to separate logically connected code into features so that we can easily create.  If you take a look at the directory stucture, you will notice a 'Features' folder,

```
  CQRSAPI\Features\
```

In this folder there are 2 features, People, and also PeopleV2 which is a direct copy of the People feature.  These features can be enabled / disabled via the appsettings.json configuration file.

```
{
  ...
  "Features": {
    "People": true,
    "PeopleV2": false
  }
  ...
}
```

Each feature folder contains all necessary logic for handling the feature and as such, can be changed independent of each other.

> Note : In this sample they are V2 is practically the same as V1, with the exception of a few tweaks here and there, such as not allowing the user to create more than one user with the same first and last name.  Consider it as an incremental update to the older endpoint.

### - Dapper

Dapper is a lightweight ORM that is being used in this context as a replacement for Entity Framework.

##### Classes

* CQRSAPI.Data.IRepository
* CQRSAPI.Features.People.Data.CqrsApiPeopleSqlRepository
* CQRSAPI.Features.PeopleV2.Data.CqrsApiPeopleSqlRepository

> Note : Dapper does not handle database initialisation and schema updates.

### - SQL Database Project

The CQRSAPIDB project is a 'SQL Database Project'. This project handles all of the Database schema, i.e tables, stored procedures etc.

It also allows for quick initialisation of test databases and complete source control of all related code.

> Note : Right-click the project and select 'Publish' to create the database on your test server.  You should also save the generated publish profile in the 'PublishProfiles' folder for future reference.

### - NServiceBus

NServiceBus provides us with a highly customisable message queuing library with a swappable Transport layer. Typically speaking, when significant events occur, a message with information of the event should be published, this enables other services to asynchronously respond to these events.

> Note : In this sample, I have implemented RabbitMQ, which can be easily setup for testing using Docket and Kinematic. 

##### Classes

* CQRSAPI.Messages.IMessageTransport
* CQRSAPI.Messages.MessageBase
* CQRSAPI.Messages.RabbitMQMessageTransport
* CQRSAPI.Features.People.Messages.PersonEventMessage
* CQRSAPI.Features.PeopleV2.Messages.PersonEventMessage

### - xUnit Tests

We are using xUnit in this sample due to its improved functionality for testing async methods.  Both features (People, PeopleV2) are tested.

### Swagger

Swagger has also been enabled in the sample. You can view the Api documentation via the url

```
/swagger/index.html
```

