# ReFilter

## A Package supporting Filtering, Sorting and Pagination

This package is designed to facilitate both basic and advanced filtering, sorting, and pagination for queryable entities, including lists, arrays, and IQueryable  
It's meant to be used with EntityFramework and CodeFirst approach, although it will work with all other approaches as well.  
Filtering and sorting support simple property based scenarios or advanced override scenarios.  
Search feature uses attributes to determine which properties to do a search against and builds a ready to use OR predicate.  

To use it, you should inject it inside Startup.cs like so:

```cs
    services.AddReFilter(typeof(ReFilterConfigBuilder));
```

This tells your DI to include the ReFilter bootstraper.  
Only `AddReFilter` is required. If you don't have any custom filters, you don't need to use anything else.  
If you do, then you add them as any other service inside your DI.  
For minimum implementation of required stuff check [Filtering and Sorting Examples](#filtering-and-sorting-examples)

And now you're ready to use ReFilter.

## Basic Example

```cs
    public async Task<PagedResult<User>> GetPaged(BasePagedRequest request)
    {
        var testQueryable = testList.AsQueryable(); // Any kind of User queryable

        var pagedRequest = request.GetPagedRequest(returnResults: true); // Transformation

        var result = await testReFilterActions.GetPaged(testQueryable, pagedRequest);
        // Calling ReFilter GetPaged action

        return result;
    }
```

This would return a paginated list of users for the request you supply it.  
`BasePagedRequest` is the base request toward ReFilter and the main messanger object.  
Main parts are Where, PropertyFilterConfigs and SearchQuery. The rest gets populated inside ReFilter.

```cs
        /// <summary>
        /// Object meant for mapping into query conditions.
        /// Only requirenment is that property names match destination
        /// </summary>
        public JObject Where { get; set; }

        /// <summary>
        /// Defines rules for sorting and filtering
        /// Can be left empty and in such way, the default values are used.
        /// Default values are no sort and Equals comparer
        /// </summary>
        public List<PropertyFilterConfig> PropertyFilterConfigs { get; set; }

        /// <summary>
        /// String SearchQuery meant for searching ANY of the tagged property
        /// </summary>
        public string SearchQuery { get; set; }

        /// <summary>
        /// If you need to filter by multiple incompatible filters, this is the easiest way to do it <para />
        /// Depending on <see cref="PredicateOperator"/> set in parent BasePagedRequest, child requests are added either as AND or OR clauses <para />
        /// Predicate is being built the same way every time so you are able to chain multiple complex filters
        /// </summary>
        public List<PagedRequest> PagedRequests { get; set; }
```

## Features

### Pagination

Pagination is a basic feature so lets start with it.
It's all based on the root `BasePagedRequest` and any following `PagedRequests` are ignored when Pagination is concearned.  
Basic params are `PageIndex` and `PageSize`.  
Index starts with 0.
It's enought to use the basic request from example and call any version of `GetPaged` from IReFilterActions.

```cs
    new BasePagedRequest
    {
        PageIndex = 0,
        PageSize = 10
    }
```

### Search

Search makes it possible to search over chosen string properties and fetch results. It's case insensitive and uses `OperatorComparer.Contains` mechanism using every property as OR clause.  
Setting up Search is quite easy and only requires setting an attribute over a property on _database_ model:

```cs
    [ReFilterProperty]
    public string Address { get; set; }
```

This tells ReFilter that the property is available as a search parameter and it uses it in the query.  
To trigger Search you need to pass the `SearchQuery` value to ReFilter query and call any action from IReFilterActions.

#### Considerations and Limitations

Since V.1.1.0 it is possible to use Search values inside child entites and child collections.  
ReFilter recognizes any object or array type marked with "UsedForSearchQuery" as another branch for going through Search value.
String search is expensive and going through a tree of entites searching for a string is very expensive.
Search itself is combined as an OR clause but is combined with every other feature as an AND clause.  
Pending: Custom search provider in form of an `Expression<Func<T, bool>>` => this would make search use custom implementation when desired (not only `OperatorComparer.Contains`).

### Filtering

Filtering is achieved by a combination of `Where` property and `PropertyFilterConfigs`.  
If `PropertyFilterConfigs` is omitted then default filtering parameters are used: `OperatorComparer.Contains` for string and `OperatorComparer.Equals` for everything else.  
If `Where` is omitted then the filtering is based on `PropertyFilterConfigs`.  
Where is any arbitrary object but the keys used for it have to match the model which you want to filter.  
That means if you have a list of `User` objects, you want your `Where` to be a replica of nullable `User` object. The most correct way to think about it is `Partial<User>` from TypeScript.  
Special case is `RangeFilter` which filters out by provided range and falls back to basic property filtering. It essentially unpacks into `PropertyFilterConfigs`.  
All the filter options from `OperatorComparer` use valid built in `ExpressionType` values such as: Contains, StartsWith, EndsWith, Equals, GreaterThan, LessThan, etc. .

Since V.2.0.0 filtering received a major refactor and supports pretty much everything you can think of.  
The most important change was the introduction of `OR` option for filtering. Previously, with the exception of Search, all the filtering was done using `AND` clause. That did not reflect the real world needs and had to be upgraded.  
Because of that a new property was introduced: `PredicateOperator` on `BasePagedRequest` and `PropertyFilterConfigs`; with options of `AND` or `OR`.  
This means that you can use `BasePagedRequest` to wrap filtering by properties or special filters in a selected clause but properties themselves can have multiple AND/OR filters on them.  
Not only that but you can send multiple `BasePagedRequest` objects with different filters and choose to build either clause.  
The `Where` object is no longer necessary but it's still the simplest way to use ReFilter and a major syntactic sugar. I plan to always support this feature.  
While it can still be used the same way as previously, the base of filtering bacame `PropertyFilterConfigs` because it carries the information about the `OperatorComparer`, the `Value` used to filter/compare against and the `PredicateOperator` to be used. 

There are real life examples inside test project and I plan to add even more meaningful examples.  
Example of a model to filter over and matching IReFilterRequest used as Where from test project:

```cs
    class School
    {
        public int Id { get; set; }
        public int IdRange { get; set; }
        [ReFilterProperty]
        public string Name { get; set; }
        [ReFilterProperty]
        public string Address { get; set; }

        public Country Country { get; set; }

        public List<string> Contacts { get; set; }
        public List<Student> Students { get; set; }

        public double Age { get; set; }
        public DateTime FoundingDate { get; set; }
        public DateOnly ValidOn { get; set; }

        public bool IsActive { get; set; }
    }

    class SchoolFilterRequest : IReFilterRequest
    {
        public int? Id { get; set; }
        public RangeFilter<int> IdRange { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        [ReFilterProperty(HasSpecialSort = true)]
        public CountryFilterRequest Country { get; set; }

        public List<string> Contacts { get; set; }
        
        [ReFilterProperty(HasSpecialFilter = true)]
        public List<string> StudentNames { get; set; }

        public RangeFilter<double> Age { get; set; }

        public RangeFilter<DateTime> FoundingDate { get; set; }
        public RangeFilter<DateOnly> ValidOn { get; set; }

        public bool? IsActive { get; set; }
    }
```

When generating filters property by property, we need to know which property to filter by.  
When sending an object that has some properties set, we know to use those properties as filters. But in the case of sending a default School object, the Id would always have a value, even if default one. A default value is a valid value so we can't ignore it and it can never be null and therefore it can't be skipped when filtering.  
That's why the FilterRequest version has every property as nullable. As such, any value it gets is used for filtering and there can be no doubt about intentions.  
This is not a rule, but it's the most common case. If you need a property to always be defined you are free to set it up that way. If you don't need a nullable object for a filter, you don't need to set it up explicitly inside `ReFilterConfigBuilder`, meaning your `GetMatchingType` would fallback to default type, in this case School.  
Additionally, since V.2.0.0 the `Where` in `BasePagedRequest` is no longer necessary and `PropertyFilterConfigs` can be used instead to same effect.

### Sorting

Sorting is a bit different from Filtering in a way that it only requires `PropertyFilterConfigs` set up correctly.  
To set them up correctly, the PFC needs to have a `SortDirection` set and `PropertyName` needs to match the case of the property name.  
Sorting also supports for multiple sorts at the same time.

### Filtering and Sorting Examples

ReFilter needs to be setup in a central place inside your project.
For filtering that's your implementation of `IReFilterConfigBuilder` and for sorting it's your implementation of `IReSortConfigBuilder`.  
Both serve the same purpose: matching nullable objects to database models and are nothing more than "controllers" for redirecting "requests".  
Both have 2 methods: `GetMatchingType` and `GetMatching[Filter/Sort]Builder`.  
They are intentionally separated since Sort and Filter don't need to have the same model.  
Also, both implementations are <b>required</b> for ReFilter to work.  
The minimum implementation (only filter is shown but filter and sort are mirrored) is as shown:

```cs
    class ReFilterConfigBuilder : IReFilterConfigBuilder
    {
        public Type GetMatchingType<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                default:
                    return typeof(T);
            }
        }

        public IReFilterBuilder<T> GetMatchingFilterBuilder<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                default:
                    return null;
            }
        }
    }
```

Then, when you have the basics setup, you can use the following:  
```cs
    services.AddReFilter(typeof(ReFilterConfigBuilder), typeof(ReSortConfigBuilder));
```

Other than the basic sorting and filtering scenarios, there are also advanced ones using custom implementations provided by you.  
The advanced custom scenarions are implemented via `IRe[Filter/Sort]Builder`s. Also, these are the most complicated ones and provide all the options.

```cs
    public interface IReFilterBuilder<T> where T : class, new()
    {
        /// <summary>
        /// Gets all the custom filters and matches them to properties.
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        IEnumerable<IReFilter<T>> GetFilters(IReFilterRequest filterRequest);
        /// <summary>
        /// Entry point of filter builder. Builds default query for that entity.
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        IQueryable<T> BuildEntityQuery(IReFilterRequest filterRequest);
        /// <summary>
        /// First uses GetFilters and then applies them to the provided query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        IQueryable<T> BuildFilteredQuery(IQueryable<T> query, IReFilterRequest filterRequest);
        /// <summary>
        /// Gets the list of Ids for the provided filter parameters in order to use it as an "IN ()" clause.
        /// </summary>
        /// <param name="filterRequest"></param>
        /// <returns></returns>
        List<int> GetForeignKeys(IReFilterRequest filterRequest);
    }
```

A real life example of a UserFilterBuilder will be shown below.  
We'll start with models and move upwards.  
Entity Framework Code First is used as database provider for this solution. The models themselves don't need anything extra in this case (only Search attribute is used directly on EF models).  
What it does require is the _FilterRequest_ class that has a blueprint of what goes into the custom _FilterBuilder_.  
This is the `UserFilterRequest`. Notice that it inherits `IReFilterRequest` and also defines special `ReFilterProperty` attributes over some properties.  
Those tell ReFilter how to handle them when they arrive via `Where` property.  
Once you're done with the configuration, you need to supply it with logic, in this case the `UserFilterBuilder` which takes care of the general implementation for filtering the _User_.  
For every property marked as `HasSpecialFilter` you will need a dedicated filter which will be applied to the query. That's achieved via `BuildFilteredQuery`and specific implementation of target filter showcased in `RoleFilter`.  
Finally, everything is connected inside `ReFilterConfigBuilder`.

```cs
    public class User : IdentityUser<Guid>
    {
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DatabaseEntityStatus Status { get; set; }

        public Person UserDetails { get; set; }

        public List<UserRoles> UserRoles { get; set; }
        public List<UserClaim> UserClaims { get; set; }

        public List<UserRenewToken> UserRenewTokens { get; set; }

        public List<CompanyUser> CompanyUsers { get; set; }
    }
```

```cs
    public class Person : DatabaseEntity<int>
    {
        [Required]
        [StringLength(255)]
        public string FirstName { get; set; }
        [StringLength(255)]
        public string MiddleName { get; set; }
        [Required]
        [StringLength(255)]
        public string LastName { get; set; }

        [StringLength(40)]
        public string Mobile { get; set; }
        [StringLength(40)]
        public string Phone { get; set; }
        [StringLength(40)]
        public string Email { get; set; }

        public Guid? UserId { get; set; }
        public User User { get; set; }
    }
```

```cs
    public class UserFilterRequest : IReFilterRequest
    {
        public string Email { get; set; }
        public string UserName { get; set; }

        [ReFilterProperty(HasSpecialFilter = true)]
        public int? CompanyId { get; set; }
        [ReFilterProperty(HasSpecialFilter = true)]
        public Guid? RoleId { get; set; }

        [ReFilterProperty(HasSpecialFilter = true, HasSpecialSort = true)]
        public string FirstName { get; set; }
        [ReFilterProperty(HasSpecialFilter = true, HasSpecialSort = true)]
        public string MiddleName { get; set; }
        [ReFilterProperty(HasSpecialFilter = true, HasSpecialSort = true)]
        public string LastName { get; set; }

        [ReFilterProperty(HasSpecialFilter = true, HasSpecialSort = true)]
        public string Mobile { get; set; }
        [ReFilterProperty(HasSpecialFilter = true, HasSpecialSort = true)]
        public string Phone { get; set; }

        public bool? EmailConfirmed { get; set; }

        [ReFilterProperty(HasSpecialFilter = true, HasSpecialSort = true)]
        public bool? IsActive { get; set; }

        [ReFilterProperty(HasSpecialFilter = true)]
        public bool? IsSuperAdmin { get; set; }
    }
```

```cs
    internal class UserFilterBuilder : IReFilterBuilder<User>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ApplicationSettings appSettings;

        // Since it's provided via DI, you can use any DI mechanism here
        public UserFilterBuilder(IOptions<ApplicationSettings> appSettings, IUnitOfWork unitOfWork)
        {
            this.appSettings = appSettings.Value;
            this.unitOfWork = unitOfWork;
        }

        public IQueryable<User> BuildEntityQuery(IReFilterRequest filterRequest)
        {
            var query = unitOfWork.GetDbSet<User>().AsQueryable();

            query = BuildFilteredQuery(query, filterRequest);

            return query;
        }

        public IQueryable<User> BuildFilteredQuery(IQueryable<User> query, IReFilterRequest filterRequest)
        {
            var filters = GetFilters(filterRequest).ToList();

            filters.ForEach(filter =>
            {
                query = filter.FilterQuery(query);
            });

            return query;
        }

        public IEnumerable<IReFilter<User>> GetFilters(IReFilterRequest filterRequest)
        {
            List<IReFilter<User>> filters = new List<IReFilter<User>>();

            if (filterRequest == null)
            {
                return filters;
            }

            var realFilter = (UserFilterRequest)filterRequest;
            // Custom filter implementation for these properties
            if (!string.IsNullOrWhiteSpace(realFilter.FirstName))
            {
                filters.Add(new FirstNameFilter(realFilter.FirstName));
            }

            if (!string.IsNullOrWhiteSpace(realFilter.LastName))
            {
                filters.Add(new LastNameFilter(realFilter.LastName));
            }

            if (!string.IsNullOrWhiteSpace(realFilter.MiddleName))
            {
                filters.Add(new MiddleNameFilter(realFilter.MiddleName));
            }

            if (!string.IsNullOrWhiteSpace(realFilter.Mobile))
            {
                filters.Add(new MobileFilter(realFilter.Mobile));
            }

            if (!string.IsNullOrWhiteSpace(realFilter.Phone))
            {
                filters.Add(new PhoneFilter(realFilter.Phone));
            }

            if (realFilter.RoleId.HasValue)
            {
                filters.Add(new RoleFilter(realFilter.RoleId.Value));
            }

            if (realFilter.IsActive.HasValue)
            {
                filters.Add(new IsActiveFilter(realFilter.IsActive.Value));
            }

            if (realFilter.CompanyId.HasValue)
            {
                filters.Add(new CompanyFilter(realFilter.CompanyId.Value));
            }

            if (realFilter.IsSuperAdmin.HasValue)
            {
                filters.Add(new SuperAdminFilter(realFilter.IsSuperAdmin.Value, appSettings.SuperAdminRole));
            }

            return filters;
        }

        public List<int> GetForeignKeys(IReFilterRequest filterRequest)
        {
            var query = unitOfWork.GetDbSet<Customer>().Where(u => u.Status == DatabaseEntityStatus.Active);

            query = BuildFilteredQuery(query, filterRequest);

            return query.Select(e => e.Id)
               .Distinct()
               .ToList();
        }
    }
```

```cs
    internal class RoleFilter : IReFilter<User>
    {
        private readonly Guid roleId;

        public RoleFilter(Guid roleId)
        {
            this.roleId = roleId;
        }

        public IQueryable<User> FilterQuery(IQueryable<User> query)
        {
            return query.Where(e => e.UserRoles.Any(ur => ur.RoleId == roleId));
        }
    }
```

```cs
    class ReFilterConfigBuilder : IReFilterConfigBuilder
    {
        private readonly UserFilterBuilder userFilterBuilder;

        public ReFilterConfigBuilder(UserFilterBuilder userFilterBuilder)
        {
            this.userFilterBuilder = userFilterBuilder;
        }

        public Type GetMatchingType<T>() where T : class, new()
        {
            return EntityTypeMatcher.GetEntityTypeConfig<T>().FilterRequestType ?? typeof(T);
        }

        public IReFilterBuilder<T> GetMatchingFilterBuilder<T>() where T : class, new()
        {
            return typeof(T) switch
            {
                Type user when user == typeof(User) => (IReFilterBuilder<T>)userFilterBuilder,
                _ => null,
            };
        }
    }
```

As a request you would use something like this:

```ts
{
    'Where': {
        'Email': 'user@email.com',
        'UserName': 'username',
        'FirstName': 'user',
        'IsActive': true
    }, // Supplies all the filter params to filter by
    'PropertyFilterConfigs': [
        {
            'PropertyName': 'UserName',
            'OperatorComparer': 1,
            'SortDirection': 1
        } // This way the request is configured to filter AND sort by UserName and apply Desc sort order and use 'StartsWith' built in filter mode
    ],
    'SearchQuery': 'username' // This one wouldn't be applied since no property on User is marked as ReFilterProperty.UsedForSearchQuery = true
}
```

Finally, <a id="real-life-projection-example"></a>real life example of how it all looks inside the service:

```cs
    // PagedResult is always the ReFilter result but you can return anything from your method
    public async Task<ActionResponse<PagedResult<UserGridViewModel>>> GetPaged(BasePagedRequest request)
        {
            try
            {
                // Create any basic query you want to build upon
                // NoTracking for speedy reading
                var query = unitOfWork.GetDbSet<User>().AsQueryable().AsNoTracking();

                // Transform PagedRequest to it's final form, supply it with mapping logic for end result (in that way you don't need to handle result transformation manually)
                // Also, I encourage you to use ProjectTo by AutoMapper <3 becase it builds the select clause for just the stuff you need in your ViewModel
                // If you compare it to Test project this really hides a lot of details under the hood and requires some advanced knowledge of AutoMapper <3 features
                // However, it also provides the best QoL stuff
                var pagedRequest = request.GetPagedRequest((IQueryable<User> x) => mapper.ProjectTo<UserGridViewModel>(x).ToList());
                var pagedResult = await reFilterActions.GetPaged(query, pagedRequest);

                return ActionResponse.Success(pagedResult);
            }
            catch (Exception ex)
            {
                var message = stringLocalizer.GetString(Resources.FetchError);
                logger.LogError(message, ex, request);
                return ActionResponse<PagedResult<UserGridViewModel>>.Error(Message: stringLocalizer.GetString(Resources.FetchError));
            }
        }
```

The only difference concearning sorting is the fact that it's configured using `PropertyFilterConfig`. So, any config that is provided and has the `SortDirection` set will be used to sort the query.  
Again, it's necessary to configure `UserSortBuilder` in order for everyting to apply correctly.

```cs
    internal class UserSortBuilder : IReSortBuilder<User>
    {
        private readonly IUnitOfWork unitOfWork;

        public UserSortBuilder(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IQueryable<User> BuildEntityQuery()
        {
            return unitOfWork.GetDbSet<User>();
        }

        public IOrderedQueryable<User> BuildSortedQuery(IQueryable<User> query, PropertyFilterConfig propertyFilterConfig, bool isFirst = false)
        {
            var sorters = GetSorters(propertyFilterConfig);

            if (sorters == null || sorters.Count == 0)
            {
                return (IOrderedQueryable<User>)query;
            }

            IOrderedQueryable<User> orderedQuery = (IOrderedQueryable<User>)query;

            for (var i = 0; i < sorters.Count; i++)
            {
                orderedQuery = sorters[i].SortQuery(orderedQuery,
                    propertyFilterConfig.SortDirection.Value,
                    isFirst: (i == 0 && isFirst));
            }

            return orderedQuery;
        }

        public List<IReSort<User>> GetSorters(PropertyFilterConfig propertyFilterConfig)
        {
            List<IReSort<User>> sorters = new();

            if (propertyFilterConfig != null)
            {
                if (propertyFilterConfig.PropertyName == nameof(UserFilterRequest.FirstName))
                {
                    sorters.Add(new FirstNameSorter());
                }

                if (propertyFilterConfig.PropertyName == nameof(UserFilterRequest.MiddleName))
                {
                    sorters.Add(new MiddleNameSorter());
                }

                if (propertyFilterConfig.PropertyName == nameof(UserFilterRequest.LastName))
                {
                    sorters.Add(new LastNameSorter());
                }

                if (propertyFilterConfig.PropertyName == nameof(UserFilterRequest.Mobile))
                {
                    sorters.Add(new MobileSorter());
                }

                if (propertyFilterConfig.PropertyName == nameof(UserFilterRequest.Phone))
                {
                    sorters.Add(new PhoneSorter());
                }

                if (propertyFilterConfig.PropertyName == nameof(UserFilterRequest.IsActive))
                {
                    sorters.Add(new IsActiveSorter());
                }
            }

            return sorters;
        }
    }
```

```cs
    internal class MiddleNameSorter : IReSort<User>
    {
        public IOrderedQueryable<User> SortQuery(IQueryable<User> query, SortDirection sortDirection = SortDirection.ASC, bool isFirst = true)
        {
            if (isFirst)
            {
                query = sortDirection == SortDirection.ASC ?
                    query.OrderBy(e => e.UserDetails.MiddleName) :
                    query.OrderByDescending(e => e.UserDetails.MiddleName);
            }
            else
            {
                query = sortDirection == SortDirection.ASC ?
                    ((IOrderedQueryable<User>)query).ThenBy(e => e.UserDetails.MiddleName)
                    : ((IOrderedQueryable<User>)query).ThenByDescending(e => e.UserDetails.MiddleName);
            }

            return (IOrderedQueryable<User>)query;
        }
    }
```

### Projections

Another feature ReFilter "has" is implementing automatic projections.  
This is important because the query built over the database tables would only select fields needed for materialization, unlike when you would do `.ToList()`.  
Use of this feature is highly encouraged and made having [AutoMapper's projection feature](https://docs.automapper.org/en/stable/Queryable-Extensions.html) in mind.  
You can see examples of manual projection inside test project but [real life scenario can be viewed here](#real-life-projection-example).

## Notes

Most of the mechanisms used are public and can be reused in your code. 

## Pending Features

- Special SearchQuery => Custom search provider in form of an `Expression<Func<T, bool>>` => this would make search use custom implementation when desired (not only `OperatorComparer.Contains`).
- SearchQuery in combination with overridable OperationComparers
- recursive IReFilterRequest filtering over child or parent objects => currently achiavable via special filters but the goal is to filter related objects and link the filter to foreign keys
- use Testcontainers for setup of real database and safer testing
- create performance tests

Additionally, check the Docs folder for examples of FE requests.  
If needed, I can give you the AgGrid implementation as well.

Any kind of help or suggestions are welcome.

## Thank you and I hope you enjoy using ReFilter
