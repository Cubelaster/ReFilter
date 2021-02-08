# ReFilter

## A Package meant for supporting Filtering and Pagination

Supports basic property filtering and pagination.  
Also supports selection of filter types: Equals, Contains, etc.  
Additionally, it is possible to implement custom filter builders for advanced filtering.  

To use it, you should inject it inside Startup.cs like so:  
```cs
    services.AddReFilter(typeof(ReFilterConfigBuilder));
    services.AddTransient<CustomerFilterBuilder>();
```

Implementing ReFilterConfigBuilder is required, as you can see.  
The second line is adding a specific filter implementation to DI container.

```cs
    public class ReFilterConfigBuilder : IReFilterConfigBuilder
    {
        private readonly CustomerFilterBuilder customerFilterBuilder;

        public ReFilterConfigBuilder(CustomerFilterBuilder customerFilterBuilder)
        {
            this.customerFilterBuilder = customerFilterBuilder;
        }

        public Type GetMatchingType<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                case Type customerType when customerType == typeof(EfModels.Customer):
                    return typeof(CustomerFilterRequest);
                default:
                    return typeof(T);
            }
        }

        public IReFilterBuilder<T> GetMatchingFilterBuilder<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                case Type customerType when customerType == typeof(EfModels.Customer):
                    return (IReFilterBuilder<T>)customerFilterBuilder;
                default:
                    return null;
            }
        }
    }
```

There are 2 roles to ConfigBuilder:
1. Matching FilterRequests to EF models when filtering
2. Matching FilterBuilders to EF models

Minimum implementation details would be to reproduce default cases.
There are examples inside test cases. They're not pretty but will show the basic idea. 

As to features, Pagination is pretty easy, so lets starti with that first.
It's all based on *BasePagedRequest*. Basic params are PageIndex and PageSize. Index should start with 0.
That's pretty much it.

Most of the other stuff is about filtering.
First stuff you need to know about filtering is that there are a couple of *modes*.  

![TypeScript models](/Docs/ReFilter.ts)

### 1. Default mode
First is the default mode, which is just plain simple Equals, with no custom stuff.
Minimal setup for such is a request like so:
```cs
    new BasePagedRequest 
    {
        PageIndex = 0,
        PageSize = 10,
        Where = new Object 
        {
            Name = "Name"
        }
    }
```

Now, ofcourse, you will want to use such request on FE.
That's why by definition, *Where* is a JObject, meaning you can send any Json from FE.  
Only thing that matters is if it can be Parsed into a object set via ReFilterConfigBuilder.
A small catch: if the FilterObject has NULLABLE properties, it's all good, however  
if they are not then their default value will be used in filtering.  
You can avoid that by setting the EF model property as nullable but that is quite bad practice.  
My personal opinion is that you should split Filter and EF models (ViewModels as well).

### 2. PropertyFilterConfig
Second is an additional config for different type of filtering.  
There is a *PropertyFilterConfigs* inside a *BasePagedRequest* class used for fine tuning the filter.
It basically sets the mode.  
Available modes are inside *OperatorComparer* enum (Contains, Equals, StartsWith...).  
Default is Equals and you can override it by setting PropertyFilterConfig for that property:
```cs
    new BasePagedRequest 
    {
        PageIndex = 0,
        PageSize = 10,
        Where = new Object 
        {
            Name = "Name"
        },
        PropertyFilterConfigs = new List<PropertyFilterConfigs> 
        {
            new PropertyFilterConfig 
            {
                PropertyName = "Name",
                OperatorComparer = Enums.OperatorComparer.Contains
            }
        }
    }
```

### 3. CustomFilterBuilder
This is the most advanced filter mode.
It's basically completely customizable and this package just gives you an idea for doing it.  
The way about it is to implement *IReFilterBuilder<>*.  
The examples in test solution aren't a stellar example but this should give you a better idea:
```cs
    public class CustomerFilterBuilder : IReFilterBuilder<Customer>
    {
        private readonly IUnitOfWork unitOfWork;

        public CustomerFilterBuilder(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IQueryable<Customer> BuildEntityQuery(IReFilterRequest filterRequest)
        {
            var query = unitOfWork.GetGenericRepository<Customer, int>().ReadActive();

            query = BuildFilteredQuery(query, filterRequest);

            return query;
        }

        public IQueryable<Customer> BuildFilteredQuery(IQueryable<Customer> query, IReFilterRequest filterRequest)
        {
            var filters = GetFilters(filterRequest).ToList();

            filters.ForEach(filter =>
            {
                query = filter.FilterQuery(query);
            });

            return query;
        }

        public IEnumerable<IReFilter<Customer>> GetFilters(IReFilterRequest filterRequest)
        {
            List<IReFilter<Customer>> filters = new List<IReFilter<Customer>>();

            if (filterRequest == null)
            {
                return filters;
            }

            var realFilter = (CustomerFilterRequest)filterRequest;

            if (!string.IsNullOrWhiteSpace(realFilter.Name))
            {
                filters.Add(new NameFilter(realFilter.Name));
            }

            return filters;
        }

        public List<int> GetForeignKeys(IReFilterRequest filterRequest)
        {
            var query = unitOfWork.GetGenericRepository<Customer, int>()
                .ReadActive();

            query = BuildFilteredQuery(query, filterRequest);

            return query.Select(e => e.Id)
                .Distinct()
                .ToList();
        }
    }
```

This won't work unless you have a dedicated *FilterRequest* object meant specifically for this EF class.  
The way it springs into action is by attributes set on *FilterRequest* properties:  
```cs
    public class CustomerFilterRequest : IReFilterRequest
    {
        [ReFilterProperty(HasSpecialFilter = true)]
        public string Name { get; set; }
        public string OIB { get; set; }
        public string PovId { get; set; }
        public string MBSuda { get; set; }
        public string MBSubjekta { get; set; }
        public string NKD { get; set; }
    }
```

These 2 combined make for a successful filtering.

Custom filter gives you the ability to implement a complete filtering solution for a EF model in one place.  
It also gives you ability to use such filtering from connected entities via *GetForeignKeys* if such a need arises.  
More examples will be available soon.

### Full real life example
```cs
    public async Task<ActionResponse<PagedResult<CustomerViewModel>>> GetPaged(BasePagedRequest request)
    {
        try
        {
            var query = unitOfWork.GetGenericRepository<Customer, int>().ReadActive();

            List<CustomerViewModel> mappingFunction(List<Customer> x) => mapper.Map<List<CustomerViewModel>>(x);
            var pagedResult = await reFilterActions.GetPaged(query, request.GetPagedRequest((Func<List<Customer>, List<CustomerViewModel>>)mappingFunction));

            return await ActionResponse<PagedResult<CustomerViewModel>>.ReturnSuccess(pagedResult);
        }
        catch (Exception ex)
        {
            logger.LogError(stringLocalizer.GetString(Resources.FetchError), ex, JsonConvert.SerializeObject(request));
            return await ActionResponse<PagedResult<CustomerViewModel>>.ReturnError(stringLocalizer.GetString(Resources.FetchError));
        }
    }
```

Feel free to comment and suggest better stuff.  
Thank you!