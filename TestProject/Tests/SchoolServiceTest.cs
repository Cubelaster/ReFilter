﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ReFilter.Enums;
using ReFilter.Models;
using TestProject.Models;
using TestProject.TestData;
using TestProject.TestServices;

namespace TestProject.Tests
{
    [TestFixture]
    class SchoolServiceTest
    {
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10 }).Returns(SchoolServiceTestData.Schools.Count).SetName("Mapped: No Filters");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, Where = JObject.Parse("{Address: \"School Address 1\"}") }).Returns(1).SetName("Mapped: Filter by Address with no Property Filter Config");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, Where = JObject.Parse("{Name: 10}") }).Returns(0).SetName("Mapped: Filter by Name(Equals)");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, Where = JObject.Parse("{Name: 10, Address: 10}") }).Returns(0).SetName("Mapped: Filter by Name(Equals) and Address(Equals)");
                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Name: 10, Address: 10}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.Contains,
                            PropertyName = "Name"
                        },
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.Contains,
                            PropertyName = "Address"
                        }
                    }
                }).Returns(2).SetName("Mapped: Filter by Name(Contains) and Address(Contains)");
            }
        }

        public static IEnumerable<TestCaseData> TestCasesSearch
        {
            get
            {
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, SearchQuery = "10" }).Returns(2).SetName("Search Query");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, SearchQuery = "100" }).Returns(1).SetName("Search Query 100");
            }
        }

        public static IEnumerable<TestCaseData> TestCasesSort
        {
            get
            {
                //yield return new TestCaseData(new BasePagedRequest
                //{
                //    PageIndex = 0,
                //    PageSize = 10,
                //    PropertyFilterConfigs = new List<PropertyFilterConfig>
                //        {
                //            new PropertyFilterConfig
                //            {
                //                PropertyName = "Name",
                //                SortDirection = SortDirection.ASC
                //            }
                //        }
                //})
                //.Returns(10)
                //.SetName("Sort by Name");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            PropertyName = "Address",
                            SortDirection = SortDirection.DESC
                        }
                    }
                })
                .Returns(10)
                .SetName("Sort by Special");
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        [Parallelizable(ParallelScope.All)]
        public async Task<int> MappingTests(BasePagedRequest request)
        {
            var unitUnderTest = new SchoolService(SchoolServiceTestData.Schools);
            var result = await unitUnderTest.GetPagedMapped<SchoolViewModel>(request);

            Type type = result.Results.GetType().GetGenericArguments()[0];

            Assert.IsTrue(type == typeof(SchoolViewModel));
            //Assert.IsTrue(result.Results.TrueForAll(s => s.Name == $"{s.FirstName} {s.LastName}"));

            return result.RowCount;
        }

        [Test]
        public async Task MappingTests_Empty()
        {
            var request = new BasePagedRequest { PageIndex = 0, PageSize = 10, SearchQuery = "10" };
            var unitUnderTest = new SchoolService(new List<School>());
            var result = await unitUnderTest.GetPagedMapped<SchoolViewModel>(request);

            Type type = result.Results.GetType().GetGenericArguments()[0];

            Assert.IsTrue(type == typeof(SchoolViewModel));
            Assert.IsTrue(result.Results.Count == 0);
            //Assert.IsTrue(result.Results.TrueForAll(s => s.Name == $"{s.FirstName} {s.LastName}"));
        }

        [Test]
        [TestCaseSource(nameof(TestCasesSearch))]
        [Parallelizable(ParallelScope.All)]
        public async Task<int> SearchMappingTests(BasePagedRequest request)
        {
            var unitUnderTest = new SchoolService(SchoolServiceTestData.Schools);
            var result = await unitUnderTest.GetPagedSearchQuery<SchoolViewModel>(request);

            Type type = result.Results.GetType().GetGenericArguments()[0];

            Assert.IsTrue(type == typeof(SchoolViewModel));
            //Assert.IsTrue(result.Results.TrueForAll(s => s.Name == $"{s.FirstName} {s.LastName}"));

            return result.RowCount;
        }

        [Test]
        [TestCaseSource(nameof(TestCasesSort))]
        [Parallelizable(ParallelScope.All)]
        public async Task<int> MappingTestsSort(BasePagedRequest request)
        {
            var unitUnderTest = new SchoolService(SchoolServiceTestData.Schools);
            var result = await unitUnderTest.GetPagedMapped<SchoolViewModel>(request);

            Type type = result.Results.GetType().GetGenericArguments()[0];

            Assert.IsTrue(type == typeof(SchoolViewModel));
            //Assert.IsTrue(result.Results.TrueForAll(s => s.Name == $"{s.FirstName} {s.LastName}"));

            return result.RowCount;
        }
    }
}
