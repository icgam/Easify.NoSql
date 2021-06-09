# Easify NoSql

The library contains provides high level repository and extensions for Mongo databases. 

The library exposes two following interfaces for repository:
- IDocumentRepository: which is a general purpose interface for all the document database repository operations.
- IMongoDbRepository: which is exposing more of mongo specific features such as sorting and projection. This should be used in advanced scenarios.


## How to use
The library provides helper extension to add mongodb support to console app or web api.

### API/Console projects
To use the library in any projects you can add support using the following snippet:

```c#
services.AddMongoDbSupport(Configuration);

or

services.AddMongoDbSupportWithTelemetry(Configuration);
```

The configuration for MongoDb needs to be defined in appSettings.json like

```json
{
    "MongoDbOptions": {
      "ConnectionUrl": "Connection to the mongodb database",
      "IsSslEnabled": true, //if the ssl is enabled on service
      "PrimaryDatabaseName": "name of the database"
    }
}
```


and for registration of each Repository there are couple of extension method which helps you.

```c#
// Register document repository for database and collection
services.AddDocumentRepository<SampleObject>("SampleDatabase", "SampleCollection");

// Register document repository for collection. Takes the database name from PrimaryDatabaseName in configuration
services.AddDocumentRepository<SampleObject>("SampleCollection");

// Register document repository for collection using name of the type. Takes the database name from PrimaryDatabaseName in configuration
services.AddDocumentRepository<SampleObject>(); // It takes name of the database from 

// Similar services specific to Mongo
services.AddMongoDbRepository<SampleObject>("SampleDatabase", "SampleCollection");
services.AddMongoDbRepository<SampleObject>("SampleDb");
services.AddMongoDbRepository<SampleObject>(); 
```

Built-in support is available for CRUD operation as one or many operations.

#### interfaces

You can use IDocumentRepository for general document repository (Implementation independent)

```c#
public interface IDocumentRepository<T>
{
    Task InsertAsync(T t);
    Task InsertManyAsync(T[] ts);

    Task<T> GetAsync(Expression<Func<T, bool>> condition);
    Task<T> GetAsync(Specification<T> specification);

    Task<T> GetAsync(Expression<Func<T, bool>> condition, Action<ISort<T>> sortAction,
        CancellationToken cancellationToken);

    Task<T> GetAsync(Expression<Func<T, bool>> condition, Action<ISort<T>> sortAction);

    Task<List<T>> GetListAsync();
    Task<List<T>> GetListAsync(Specification<T> specification);
    Task<List<T>> GetListAsync(Specification<T> specification, CancellationToken cancellationToken);
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition);
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition, Action<ISort<T>> sortAction);
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition, Action<ISort<T>> sortAction,
        CancellationToken cancellationToken);
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken);

    Task<T> UpdateAsync(Expression<Func<T, bool>> condition, T t, bool isUpsert = true);
    Task<T> UpdateAsync(Expression<Func<T, bool>> condition, T t, bool isUpsert,
        CancellationToken cancellationToken);
    Task<T> UpdateAsync(Specification<T> specification, T t, bool isUpsert,
        CancellationToken cancellationToken);
    Task<T> UpdateAsync(Specification<T> specification, T t, bool isUpsert = true);

    Task<long> GetCountAsync(Expression<Func<T, bool>> condition = null);
    Task<long> GetCountAsync(Specification<T> specification);

    Task DeleteOneAsync(Expression<Func<T, bool>> condition);
    Task DeleteOneAsync(Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken);
    Task DeleteOneAsync(Specification<T> specification, CancellationToken cancellationToken);
    Task DeleteOneAsync(Specification<T> specification);

    Task DeleteManyAsync(Expression<Func<T, bool>> condition);
    Task DeleteManyAsync(Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken);
    Task DeleteManyAsync(Specification<T> specification, CancellationToken cancellationToken);
    Task DeleteManyAsync(Specification<T> specification);
}
  
```

Or the interface with extra features for advance query, sorting and projections

```c#
public interface IMongoDbRepository<T> : IDocumentRepository<T>
{
    Task<T> GetAsync(Func<IFindFluent<T, T>, IFindFluent<T, T>> query);
    Task<T> GetAsync(Specification<T> specification, Func<IFindFluent<T, T>, IFindFluent<T, T>> query);
    Task<T> GetAsync(Expression<Func<T, bool>> condition, Func<IFindFluent<T, T>, IFindFluent<T, T>> query);

    Task<T> GetAsync(Expression<Func<T, bool>> condition, Func<IFindFluent<T, T>, IFindFluent<T, T>> query,
        CancellationToken cancellationToken);

    Task<List<T>> GetListAsync(Func<IFindFluent<T, T>, IFindFluent<T, T>> query);
    Task<List<T>> GetListAsync(Func<IFindFluent<T, T>, IFindFluent<T, T>> query,
        CancellationToken cancellationToken);
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> condition,
        Func<IFindFluent<T, T>, IFindFluent<T, T>> query, CancellationToken cancellationToken);
    Task<List<T>> GetListAsync(Specification<T> specification,
        Func<IFindFluent<T, T>, IFindFluent<T, T>> query, CancellationToken cancellationToken);
}
```

### Usage
s
To use the repository, You can inject the repository to your class and all the relevant methods will be available to use. If you have selected WithTelemetry, ensure that the InstrumentationKey for your target application insights resource has been configured either in your calling application config: 

```c#
"ApplicationInsights": {
    /* Run this command to get your Instrumentation key
    az resource show --resource-group HoldingsFeedDevResourceGroup --name HoldingsFeedDevAppInsights --resource-type        "Microsoft.Insights/components" --query properties.InstrumentationKey
    */
    "InstrumentationKey": "########-####-####-####-############"
  }
```
or set as an Environment Variable on the hosting server.

### Best Practices

There are couple of best practices in using no sql database (Specifically in MongoDb):
- Every document need to have an ID. An ID field will be assigned to the documents without ID. The naming in mongodb for the ID field is _id and the object type will be ObjectID. 
To avoid such a behaviors its recommended to choose a user defined id with a type similar to guid or string
- Mongodb creates the database and the collection on the first insert/update if it's currently unavailable. The library should be extended if there is a need for custom creation of database/collection
- Documents are better to have a Version tag so it will be much easier to maintain and migrate in future.

### How to Engage, Contribute, and Give Feedback

Description of the steps or process to be a contributor to the project.

Some of the best ways to contribute are to try things out, file issues, join in design conversations,
and make pull-requests.

* [Be an active contributor](./docs/CONTRIBUTING.md): Check out the contributing page to see the best places to log issues and start discussions.
* [Roadmap](./docs/ROADMAP.md): The schedule and milestone themes for project.

## Reporting bugs and useful features

Security issues and bugs should be reported by creating the relevant features and bugs in issues sections

## Related projects

- Easify: https://github.com/icgam/Easify
- Easify.Ef: https://github.com/icgam/Easify.Ef
- MongoDb: https://github.com/mongodb/mongo-csharp-driver
- Mongo2Go: https://github.com/Mongo2Go/Mongo2Go
  
