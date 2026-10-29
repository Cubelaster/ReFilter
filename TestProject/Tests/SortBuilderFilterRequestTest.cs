using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ReFilter.Enums;
using ReFilter.Models;
using ReFilter.Models.Filtering.Contracts;
using ReFilter.ReFilterActions;
using ReFilter.ReFilterConfigBuilder;
using ReFilter.ReSortBuilder;
using TestProject.Models;
using TestProject.Models.FilterRequests;
using TestProject.RequiredImplementations;
using TestProject.SortBuilders;
using TestProject.TestData;

namespace TestProject.Tests
{
    [TestFixture]
    class SortBuilderFilterRequestTest
    {
        // Wraps SchoolSortBuilder and records the filterRequest each call receives
        private class CapturingSortBuilder : IReSortBuilder<School>
        {
            private readonly SchoolSortBuilder inner = new();

            public List<IReFilterRequest> ReceivedFilterRequests { get; } = new();

            public IOrderedQueryable<School> BuildSortedQuery(IQueryable<School> query, PropertyFilterConfig propertyFilterConfig,
                List<PropertyFilterConfig> propertyFilterConfigs, IReFilterRequest filterRequest, bool isFirst = false)
            {
                ReceivedFilterRequests.Add(filterRequest);
                return inner.BuildSortedQuery(query, propertyFilterConfig, propertyFilterConfigs, filterRequest, isFirst);
            }
        }

        private class CapturingSortConfigBuilder : IReSortConfigBuilder
        {
            private readonly ReSortConfigBuilder inner = new();

            public CapturingSortBuilder SortBuilder { get; } = new();

            public IReSortBuilder<T> GetMatchingSortBuilder<T>() where T : class, new()
                => typeof(T) == typeof(School) ? (IReSortBuilder<T>)SortBuilder : inner.GetMatchingSortBuilder<T>();

            public Type GetMatchingType<T>() where T : class, new() => inner.GetMatchingType<T>();
        }

        private static PropertyFilterConfig CountrySort => new()
        {
            PropertyName = nameof(SchoolFilterRequest.Country),
            SortDirection = SortDirection.ASC
        };

        private static List<int> ExpectedCountrySortIds(IEnumerable<School> schools) => schools
            .OrderBy(e => e.Country == null ? null : e.Country.Alpha2Code)
            .ThenBy(e => e.Address)
            .Select(e => e.Id)
            .ToList();

        [Test]
        public async Task SpecialSort_ReceivesFilterRequest_FromWhere()
        {
            var sortConfigBuilder = new CapturingSortConfigBuilder();
            var reFilterActions = new ReFilterActions(new ReFilterConfigBuilder(), sortConfigBuilder);

            var request = new PagedRequest
            {
                PageIndex = 0,
                PageSize = 100,
                ReturnResults = true,
                Where = JObject.Parse("{Address: \"School Address 1\"}"),
                PropertyFilterConfigs = new List<PropertyFilterConfig> { CountrySort }
            };

            var result = await reFilterActions.GetPaged(SchoolServiceTestData.Schools.AsQueryable(), request);

            var received = sortConfigBuilder.SortBuilder.ReceivedFilterRequests;
            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0], Is.InstanceOf<SchoolFilterRequest>());
            Assert.That(((SchoolFilterRequest)received[0]).Address, Is.EqualTo("School Address 1"));

            Assert.That(result.Results, Is.Not.Empty);
            Assert.That(result.Results.Select(e => e.Id), Is.EqualTo(ExpectedCountrySortIds(result.Results)));
        }

        [Test]
        public async Task SpecialSort_ReceivesFilterRequest_FromPfcValue()
        {
            var sortConfigBuilder = new CapturingSortConfigBuilder();
            var reFilterActions = new ReFilterActions(new ReFilterConfigBuilder(), sortConfigBuilder);

            var request = new PagedRequest
            {
                PageIndex = 0,
                PageSize = 100,
                ReturnResults = true,
                PropertyFilterConfigs = new List<PropertyFilterConfig>
                {
                    new() { PropertyName = nameof(SchoolFilterRequest.Address), OperatorComparer = OperatorComparer.Equals, Value = "School Address 1" },
                    CountrySort
                }
            };

            await reFilterActions.GetPaged(SchoolServiceTestData.Schools.AsQueryable(), request);

            var received = sortConfigBuilder.SortBuilder.ReceivedFilterRequests;
            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0], Is.InstanceOf<SchoolFilterRequest>());
            Assert.That(((SchoolFilterRequest)received[0]).Address, Is.EqualTo("School Address 1"));
        }

        [Test]
        public async Task SpecialSort_ReceivesNullFilterRequest_WithoutWhereOrPfcValues()
        {
            var sortConfigBuilder = new CapturingSortConfigBuilder();
            var reFilterActions = new ReFilterActions(new ReFilterConfigBuilder(), sortConfigBuilder);

            var request = new PagedRequest
            {
                PageIndex = 0,
                PageSize = 100,
                ReturnResults = true,
                PropertyFilterConfigs = new List<PropertyFilterConfig> { CountrySort }
            };

            var result = await reFilterActions.GetPaged(SchoolServiceTestData.Schools.AsQueryable(), request);

            var received = sortConfigBuilder.SortBuilder.ReceivedFilterRequests;
            Assert.That(received, Has.Count.EqualTo(1));
            Assert.That(received[0], Is.Null);
            Assert.That(result.Results.Select(e => e.Id), Is.EqualTo(ExpectedCountrySortIds(SchoolServiceTestData.Schools)));
        }
    }
}
