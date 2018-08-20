// ts2fable 0.6.1
module rec Fable.Import.CosmosDb

open System
open Fable.Core
open Fable.Import.JS

[<Import("*", from="@azure/cosmos")>]
let Exports: IExports = jsNative

type IExports =
    // [<Import("CosmosClient", "@azure/cosmos")>]
    abstract CosmosClient: CosmosClientStatic

type DatabaseDefinition =
    abstract id: string with get, set

type DatabaseResponse =
    inherit CosmosResponse<DatabaseDefinition, Database>
    /// A reference to the {@link Database} that the returned {@link DatabaseDefinition} corresponds to.
    abstract database: Database with get, set

type [<AllowNullLiteral>] CosmosClientOptions =
    /// The service endpoint to use to create the client.
    abstract endpoint: string with get, set
    /// An object that is used for authenticating requests and must contains one of the options
    abstract auth: AuthOptions with get, set
    /// An instance of {@link ConnectionPolicy} class.
    /// This parameter is optional and the default connectionPolicy will be used if omitted.
    abstract connectionPolicy: ConnectionPolicy option with get, set
    /// An optional parameter that represents the consistency level.
    /// It can take any value from {@link ConsistencyLevel}.
    abstract consistencyLevel: obj option with get, set
    abstract defaultHeaders: IHeaders option with get, set
    abstract agent: Agent option with get, set
    abstract queryCompatibilityMode: QueryCompatibilityMode option with get, set

/// Provides a client-side logical representation of the Azure Cosmos DB database account.
/// This client is used to configure and execute requests in the Azure Cosmos DB database service.
type [<AllowNullLiteral>] CosmosClient =
    abstract options: obj with get, set
    /// Used for creating new databases, or querying/reading all databases.
    ///
    /// Use `.database(id)` to read, replace, or delete a specific, existing database by id.
    abstract databases: Databases
    /// Used for querying & reading all offers.
    ///
    /// Use `.offer(id)` to read, or replace existing offers.
    abstract offers: Offers
    /// Creates a new {@link CosmosClient} object. See {@link CosmosClientOptions} for more details on what options you can use.
    abstract clientContext: obj with get, set
    /// Get information about the current {@link DatabaseAccount} (including which regions are supported, etc.)
    abstract getDatabaseAccount: ?options: RequestOptions -> Promise<CosmosResponse<DatabaseAccount, CosmosClient>>
    /// <summary>Used for reading, updating, or deleting a existing database by id or accessing containers belonging to that database.
    ///
    /// This does not make a network call. Use `.read` to get info about the database after getting the {@link Database} object.</summary>
    /// <param name="id">The id of the database.</param>
    abstract database: id: string -> Database
    /// <summary>Used for reading, or updating a existing offer by id.</summary>
    /// <param name="id">The id of the offer.</param>
    abstract offer: id: string -> Offer

/// Provides a client-side logical representation of the Azure Cosmos DB database account.
/// This client is used to configure and execute requests in the Azure Cosmos DB database service.
type [<AllowNullLiteral>] CosmosClientStatic =
    [<Emit "new $0($1...)">] abstract Create: options: CosmosClientOptions -> CosmosClient

type [<AllowNullLiteral>] IRequestInfo =
    [<Emit "$0[$1]{{=$2}}">] abstract Item: index: string -> obj option with get, set
    abstract verb: string with get, set
    abstract path: string with get, set
    abstract resourceId: string with get, set
    abstract resourceType: string with get, set
    abstract headers: IHeaders with get, set

type [<AllowNullLiteral>] ITokenProvider =
    abstract getToken: (IRequestInfo -> (Error -> string -> unit) -> Promise<string>) with get, set

type [<AllowNullLiteral>] AuthOptions =
    /// The authorization master key to use to create the client.
    abstract masterKey: string option with get, set
    /// An object that contains resources tokens.
    /// Keys for the object are resource Ids and values are the resource tokens.
    abstract resourceTokens: obj option with get, set
    abstract tokenProvider: obj option with get, set
    /// An array of {@link Permission} objects.
    abstract permissionFeed: ResizeArray<PermissionDefinition> option with get, set

type [<AllowNullLiteral>] AuthHandler =
    abstract getAuthorizationTokenUsingMasterKey: obj with get, set
    abstract getAuthorizationTokenUsingResourceTokens: obj with get, set
    abstract getAuthorizationTokenUsingTokenProvider: obj with get, set

type [<AllowNullLiteral>] AuthHandlerStatic =
    [<Emit "new $0($1...)">] abstract Create: unit -> AuthHandler
    abstract getAuthorizationHeader: authOptions: AuthOptions * verb: string * path: string * resourceId: string * resourceType: string * headers: IHeaders -> Promise<string>

/// Operations for creating new databases, and reading/querying all databases
type [<AllowNullLiteral>] Databases =
    abstract client: obj
    abstract clientContext: obj
    /// <summary>Queries all databases.</summary>
    /// <param name="query">Query configuration for the operation. See {</param>
    /// <param name="options">Use to set options like response page size, continuation tokens, etc.</param>
    abstract query: query: U2<string, SqlQuerySpec> * ?options: FeedOptions -> QueryIterator<DatabaseDefinition>
    /// <summary>Send a request for creating a database.
    ///
    /// A database manages users, permissions and a set of containers.
    /// Each Azure Cosmos DB Database Account is able to support multiple independent named databases,
    /// with the database being the logical container for data.
    ///
    /// Each Database consists of one or more containers, each of which in turn contain one or more
    /// documents. Since databases are an an administrative resource, the Service Master Key will be
    /// required in order to access and successfully complete any action using the User APIs.</summary>
    /// <param name="body">The {</param>
    /// <param name="options">Use to set options like response page size, continuation tokens, etc.</param>
    abstract create: body: DatabaseDefinition * ?options: RequestOptions -> Promise<DatabaseResponse>
    /// <summary>Check if a database exists, and if it doesn't, create it.
    /// This will make a read operation based on the id in the `body`, then if it is not found, a create operation.
    ///
    /// A database manages users, permissions and a set of containers.
    /// Each Azure Cosmos DB Database Account is able to support multiple independent named databases,
    /// with the database being the logical container for data.
    ///
    /// Each Database consists of one or more containers, each of which in turn contain one or more
    /// documents. Since databases are an an administrative resource, the Service Master Key will be
    /// required in order to access and successfully complete any action using the User APIs.</summary>
    /// <param name="body">The {</param>
    /// <param name="options"></param>
    abstract createIfNotExists: body: DatabaseDefinition * ?options: RequestOptions -> Promise<DatabaseResponse>
    /// <summary>Reads all databases.</summary>
    /// <param name="options">Use to set options like response page size, continuation tokens, etc.</param>
    abstract readAll: ?options: FeedOptions -> QueryIterator<DatabaseDefinition>

/// Operations for creating new databases, and reading/querying all databases
type [<AllowNullLiteral>] DatabasesStatic =
    [<Emit "new $0($1...)">] abstract Create: client: CosmosClient * clientContext: ClientContext -> Databases

/// Operations for reading or deleting an existing database.
type [<AllowNullLiteral>] Database =
    abstract client: CosmosClient
    abstract id: string
    abstract clientContext: obj with get, set
    /// Used for creating new containers, or querying/reading all containers.
    ///
    /// Use `.container(id)` to read, replace, or delete a specific, existing {@link Database} by id.
    abstract containers: Containers
    /// Used for creating new users, or querying/reading all users.
    ///
    /// Use `.user(id)` to read, replace, or delete a specific, existing {@link User} by id.
    abstract users: Users
    /// Returns a reference URL to the resource. Used for linking in Permissions.
    abstract url: string
    /// Used to read, replace, or delete a specific, existing {@link Database} by id.
    ///
    /// Use `.containers` creating new containers, or querying/reading all containers.
    abstract container: id: string -> Container
    /// Used to read, replace, or delete a specific, existing {@link User} by id.
    ///
    /// Use `.users` for creating new users, or querying/reading all users.
    abstract user: id: string -> User
    /// Read the definition of the given Database.
    abstract read: ?options: RequestOptions -> Promise<DatabaseResponse>
    /// Delete the given Database.
    abstract delete: ?options: RequestOptions -> Promise<DatabaseResponse>

/// Operations for reading or deleting an existing database.
type [<AllowNullLiteral>] DatabaseStatic =
    /// Returns a new {@link Database} instance.
    ///
    /// Note: the intention is to get this object from {@link CosmosClient} via `client.databsae(id)`, not to instaniate it yourself.
    [<Emit "new $0($1...)">] abstract Create: client: CosmosClient * id: string * clientContext: ClientContext -> Database

/// Operations for creating new containers, and reading/querying all containers
type [<AllowNullLiteral>] Containers =
    abstract database: Database
    abstract clientContext: obj
    /// <summary>Queries all containers.</summary>
    /// <param name="query">Query configuration for the operation. See {</param>
    /// <param name="options">Use to set options like response page size, continuation tokens, etc.</param>
    abstract query: query: SqlQuerySpec * ?options: FeedOptions -> QueryIterator<ContainerDefinition>
    /// <summary>Creates a container.
    ///
    /// A container is a named logical container for items.
    ///
    /// A database may contain zero or more named containers and each container consists of
    /// zero or more JSON items.
    ///
    /// Being schema-free, the items in a container do not need to share the same structure or fields.
    ///
    ///
    /// Since containers are application resources, they can be authorized using either the
    /// master key or resource keys.</summary>
    /// <param name="body">Represents the body of the container.</param>
    /// <param name="options">Use to set options like response page size, continuation tokens, etc.</param>
    abstract create: body: ContainerDefinition * ?options: RequestOptions -> Promise<ContainerResponse>
    /// <summary>Checks if a Container exists, and, if it doesn't, creates it.
    /// This will make a read operation based on the id in the `body`, then if it is not found, a create operation.
    /// You should confirm that the output matches the body you passed in for non-default properties (i.e. indexing policy/etc.)
    ///
    /// A container is a named logical container for items.
    ///
    /// A database may contain zero or more named containers and each container consists of
    /// zero or more JSON items.
    ///
    /// Being schema-free, the items in a container do not need to share the same structure or fields.
    ///
    ///
    /// Since containers are application resources, they can be authorized using either the
    /// master key or resource keys.</summary>
    /// <param name="body">Represents the body of the container.</param>
    /// <param name="options">Use to set options like response page size, continuation tokens, etc.</param>
    abstract createIfNotExists: body: ContainerDefinition * ?options: RequestOptions -> Promise<ContainerResponse>
    /// <summary>Read all containers.</summary>
    /// <param name="options">Use to set options like response page size, continuation tokens, etc.</param>
    abstract readAll: ?options: FeedOptions -> QueryIterator<ContainerDefinition>

/// Operations for creating new containers, and reading/querying all containers
type [<AllowNullLiteral>] ContainersStatic =
    [<Emit "new $0($1...)">] abstract Create: database: Database * clientContext: ClientContext -> Containers

type [<AllowNullLiteral>] ContainerDefinition =
    /// The id of the container.
    abstract id: string with get, set
    /// TODO
    abstract partitionKey: PartitionKeyDefinition option with get, set
    /// The indexing policy associated with the container.
    abstract indexingPolicy: IndexingPolicy option with get, set
    /// The default time to live in seconds for items in a container.
    abstract defaultTtl: float option with get, set

type [<AllowNullLiteral>] ContainerResponse =
    inherit CosmosResponse<ContainerDefinition, Container>
    /// A reference to the {@link Container} that the returned {@link ContainerDefinition} corresponds to.
    abstract container: Container with get, set

type [<AllowNullLiteral>] ItemResponse<'T> =
    inherit CosmosResponse<'T, Item>
    /// Reference to the {@link Item} the response corresponds to.
    abstract item: Item with get, set

type [<AllowNullLiteral>] CosmosResponse<'T, 'U> =
    abstract body: 'T option with get, set
    abstract headers: IHeaders with get, set
    abstract ref: 'U option with get, set

/// Operations for reading, replacing, or deleting a specific, existing container by id.
type [<AllowNullLiteral>] Container =
    abstract database: Database
    abstract id: string
    abstract clientContext: obj
    /// Operations for creating new items, and reading/querying all items
    ///
    /// For reading, replacing, or deleting an existing item, use `.item(id)`.
    abstract items: Items
    /// Operations for creating new stored procedures, and reading/querying all stored procedures.
    ///
    /// For reading, replacing, or deleting an existing stored procedure, use `.storedProcedure(id)`.
    abstract storedProcedures: StoredProcedures
    /// Operations for creating new triggers, and reading/querying all triggers.
    ///
    /// For reading, replacing, or deleting an existing trigger, use `.trigger(id)`.
    abstract triggers: Triggers
    /// Operations for creating new user defined functions, and reading/querying all user defined functions.
    ///
    /// For reading, replacing, or deleting an existing user defined function, use `.userDefinedFunction(id)`.
    abstract userDefinedFunctions: UserDefinedFunctions
    /// Returns a reference URL to the resource. Used for linking in Permissions.
    abstract url: string
    /// <summary>Used to read, replace, or delete a specific, existing {@link Item} by id.
    ///
    /// Use `.items` for creating new items, or querying/reading all items.</summary>
    /// <param name="id">The id of the {</param>
    /// <param name="partitionKey">The partition key of the {</param>
    abstract item: id: string * ?partitionKey: string -> Item
    /// <summary>Used to read, replace, or delete a specific, existing {@link UserDefinedFunction} by id.
    ///
    /// Use `.userDefinedFunctions` for creating new user defined functions, or querying/reading all user defined functions.</summary>
    /// <param name="id">The id of the {</param>
    abstract userDefinedFunction: id: string -> UserDefinedFunction
    /// <summary>Used to read, replace, or delete a specific, existing {@link Conflict} by id.
    ///
    /// Use `.conflicts` for creating new conflicts, or querying/reading all conflicts.</summary>
    /// <param name="id">The id of the {</param>
    abstract conflict: id: string -> Conflict
    /// <summary>Used to read, replace, or delete a specific, existing {@link StoredProcedure} by id.
    ///
    /// Use `.storedProcedures` for creating new stored procedures, or querying/reading all stored procedures.</summary>
    /// <param name="id">The id of the {</param>
    abstract storedProcedure: id: string -> StoredProcedure
    /// <summary>Used to read, replace, or delete a specific, existing {@link Trigger} by id.
    ///
    /// Use `.triggers` for creating new triggers, or querying/reading all triggers.</summary>
    /// <param name="id">The id of the {</param>
    abstract trigger: id: string -> Trigger
    /// Read the container's definition
    abstract read: ?options: RequestOptions -> Promise<ContainerResponse>
    /// Replace the container's definition
    abstract replace: body: ContainerDefinition * ?options: RequestOptions -> Promise<ContainerResponse>
    /// Delete the container
    abstract delete: ?options: RequestOptions -> Promise<ContainerResponse>
    /// Gets the partition key definition first by looking into the cache otherwise by reading the collection.
    abstract getPartitionKeyDefinition: unit -> Promise<CosmosResponse<PartitionKeyDefinition, Container>>
    abstract extractPartitionKey: document: obj option * partitionKeyDefinition: PartitionKeyDefinition -> ResizeArray<PartitionKey>

/// Operations for reading, replacing, or deleting a specific, existing container by id.
type [<AllowNullLiteral>] ContainerStatic =
    /// <summary>Returns a container instance. Note: You should get this from `database.container(id)`, rather than creating your own object.</summary>
    /// <param name="database">The parent {</param>
    /// <param name="id">The id of the given container.</param>
    [<Emit "new $0($1...)">] abstract Create: database: Database * id: string * clientContext: ClientContext -> Container

/// Used to perform operations on a specific item.
type [<AllowNullLiteral>] Item =
    abstract container: Container
    abstract id: string
    abstract primaryKey: string
    abstract clientContext: obj
    /// Returns a reference URL to the resource. Used for linking in Permissions.
    abstract url: string
    /// <summary>Read the item's definition.
    ///
    /// Any provided type, T, is not necessarily enforced by the SDK.
    /// You may get more or less properties and it's up to your logic to enforce it.
    /// If the type, T, is a class, it won't pass `typeof` comparisons, because it won't have a match prototype.
    /// It's recommended to only use interfaces.
    ///
    /// There is no set schema for JSON items. They may contain any number of custom properties.</summary>
    /// <param name="options">Additional options for the request, such as the partition key.
    /// Note, if you provide a partition key on the options object, it will override the primary key on `this.primaryKey`.</param>
    abstract read: ?options: RequestOptions -> Promise<ItemResponse<'T>>
    /// <summary>Replace the item's definition.
    ///
    /// Any provided type, T, is not necessarily enforced by the SDK.
    /// You may get more or less properties and it's up to your logic to enforce it.
    ///
    /// There is no set schema for JSON items. They may contain any number of custom properties.</summary>
    /// <param name="body">The definition to replace the existing {</param>
    /// <param name="options">Additional options for the request, such as the partition key.</param>
    abstract replace: body: 'T * ?options: RequestOptions -> Promise<ItemResponse<'T>>
    /// <summary>Delete the item.
    ///
    /// Any provided type, T, is not necessarily enforced by the SDK.
    /// You may get more or less properties and it's up to your logic to enforce it.</summary>
    /// <param name="options">Additional options for the request, such as the partition key.</param>
    abstract delete: ?options: RequestOptions -> Promise<ItemResponse<'T>>

/// Used to perform operations on a specific item.
type [<AllowNullLiteral>] ItemStatic =
    /// <param name="container">The parent {</param>
    /// <param name="id">The id of the given {</param>
    /// <param name="primaryKey">The primary key of the given {</param>
    [<Emit "new $0($1...)">] abstract Create: container: Container * id: string * primaryKey: string * clientContext: ClientContext -> Item

/// Operations for creating new items, and reading/querying all items
type [<AllowNullLiteral>] Items =
    abstract container: Container
    abstract clientContext: obj
    /// <summary>Queries all items.</summary>
    /// <param name="query">Query configuration for the operation. See {</param>
    /// <param name="options">Used for modifying the request (for instance, specifying the partition key).</param>
    abstract query: query: U2<string, SqlQuerySpec> * ?options: FeedOptions -> QueryIterator<'T>
    /// <summary>Read all items.
    ///
    /// Any provided type, T, is not necessarily enforced by the SDK.
    /// You may get more or less properties and it's up to your logic to enforce it.
    ///
    /// There is no set schema for JSON items. They may contain any number of custom properties.</summary>
    /// <param name="options">Used for modifying the request (for instance, specifying the partition key).</param>
    abstract readAll: ?options: FeedOptions -> QueryIterator<'T>
    /// <summary>Create a item.
    ///
    /// Any provided type, T, is not necessarily enforced by the SDK.
    /// You may get more or less properties and it's up to your logic to enforce it.
    ///
    /// There is no set schema for JSON items. They may contain any number of custom properties.</summary>
    /// <param name="body">Represents the body of the item. Can contain any number of user defined properties.</param>
    /// <param name="options">Used for modifying the request (for instance, specifying the partition key).</param>
    abstract create: body: 'T * ?options: RequestOptions -> Promise<ItemResponse<'T>>
    /// <summary>Upsert an item.
    ///
    /// Any provided type, T, is not necessarily enforced by the SDK.
    /// You may get more or less properties and it's up to your logic to enforce it.
    ///
    /// There is no set schema for JSON items. They may contain any number of custom properties.</summary>
    /// <param name="body">Represents the body of the item. Can contain any number of user defined properties.</param>
    /// <param name="options">Used for modifying the request (for instance, specifying the partition key).</param>
    abstract upsert: body: 'T * ?options: RequestOptions -> Promise<ItemResponse<'T>>

/// Operations for creating new items, and reading/querying all items
type [<AllowNullLiteral>] ItemsStatic =
    /// <summary>Create an instance of {@link Items} linked to the parent {@link Container}.</summary>
    /// <param name="container">The parent container.</param>
    [<Emit "new $0($1...)">] abstract Create: container: Container * clientContext: ClientContext -> Items

type QueryIteratorResponse<'T> =
    abstract headers: 'T option with get, set
    abstract result: 'T option with get, set

/// Represents a QueryIterator Object, an implmenetation of feed or query response that enables
/// traversal and iterating over the response
/// in the Azure Cosmos DB database service.
type [<AllowNullLiteral>] QueryIterator<'T> =
    abstract clientContext: obj with get, set
    abstract query: obj with get, set
    abstract options: obj with get, set
    abstract fetchFunctions: obj with get, set
    abstract resourceLink: obj option with get, set
    abstract toArrayTempResources: obj with get, set
    abstract toArrayLastResHeaders: obj with get, set
    abstract queryExecutionContext: obj with get, set
    /// <summary>Calls a specified callback for each item returned from the query.
    /// Runs serially; each callback blocks the next.</summary>
    /// <param name="callback">Specified callback.
    /// First param is the result,
    /// second param (optional) is the current headers object state,
    /// third param (optional) is current index.
    /// No more callbacks will be called if one of them results false.</param>
    abstract forEach: callback: ('T -> IHeaders -> float -> U2<bool, unit>) -> Promise<unit>
    /// Execute a provided function on the next element in the QueryIterator.
    abstract nextItem: unit -> Promise<QueryIteratorResponse<'T>>
    /// Retrieve the current element on the QueryIterator.
    abstract current: unit -> Promise<QueryIteratorResponse<'T>>
    abstract hasMoreResults: unit -> bool
    /// Retrieve all the elements of the feed and pass them as an array to a function
    abstract toArray: unit -> Promise<QueryIteratorResponse<'T[]>>
    /// Retrieve the next batch of the feed and pass them as an array to a function
    abstract executeNext: unit -> Promise<QueryIteratorResponse<'T>>
    /// Reset the QueryIterator to the beginning and clear all the resources inside it
    abstract reset: unit -> unit
    abstract _toArrayImplementation: obj with get, set
    abstract _createQueryExecutionContext: obj with get, set

/// Represents a QueryIterator Object, an implmenetation of feed or query response that enables
/// traversal and iterating over the response
/// in the Azure Cosmos DB database service.
type [<AllowNullLiteral>] QueryIteratorStatic =
    [<Emit "new $0($1...)">] abstract Create: clientContext: ClientContext * query: U2<SqlQuerySpec, string> * options: FeedOptions * fetchFunctions: U2<FetchFunctionCallback, ResizeArray<FetchFunctionCallback>> * ?resourceLink: U2<string, ResizeArray<string>> -> QueryIterator<'T>

type IHeaders = obj
type RequestOptions = interface end // _.RequestOptions
type Offer = interface end // __client_Offer.Offer
type Offers = interface end // __client_Offer.Offers
type DatabaseAccount = interface end // __documents.DatabaseAccount
type Agent = interface end // Http.Agent
type ConnectionPolicy = interface end // __documents.ConnectionPolicy
type ConsistencyLevel = interface end // __documents.ConsistencyLevel
type QueryCompatibilityMode = interface end // __documents.QueryCompatibilityMode
type PermissionDefinition = interface end // __client.PermissionDefinition
type ClientContext = interface end // ______ClientContext.ClientContext
type SqlQuerySpec = interface end // ______queryExecutionContext.SqlQuerySpec
type FeedOptions = interface end // ______request.FeedOptions
type User = interface end //___User.User
type Users = interface end //___User.Users
type QueryIterator = interface end // ______queryIterator.QueryIterator
type IndexingPolicy = interface end //______documents.IndexingPolicy
type PartitionKeyDefinition = interface end //______documents.PartitionKeyDefinition
type PartitionKey = interface end // _____.PartitionKey
type CosmosResponse = interface end // ______request.CosmosResponse
type Conflict = interface end // ___Conflict.Conflict
type StoredProcedure = interface end // ___StoredProcedure.StoredProcedure
type StoredProcedures = interface end // ___StoredProcedure.StoredProcedures
type Trigger = interface end // ___Trigger.Trigger
type Triggers = interface end // ___Trigger.Triggers
type UserDefinedFunction = interface end // ___UserDefinedFunction.UserDefinedFunction
type UserDefinedFunctions = interface end // ___UserDefinedFunction.UserDefinedFunctions
type ItemResponse = interface end // __ItemResponse.ItemResponse
type FetchFunctionCallback = interface end //__queryExecutionContext.FetchFunctionCallback
