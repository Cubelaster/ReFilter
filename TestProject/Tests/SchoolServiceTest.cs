using System;
using System.Collections.Generic;
using System.Linq;
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
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, Where = JObject.Parse("{IsActive: true}") }).Returns(50).SetName("Mapped: Filter by IsActive");
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

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Name: 10}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.NotEqual,
                            PropertyName = "Name"
                        }
                    }
                }).Returns(100).SetName("Mapped: Filter by Name(Not Equal)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Name: 10}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.NotContains,
                            PropertyName = "Name"
                        }
                    }
                }).Returns(98).SetName("Mapped: Filter by Name(Not Contains)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Name: 10}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.NotStartsWith,
                            PropertyName = "Name"
                        }
                    }
                }).Returns(100).SetName("Mapped: Filter by Name(Not Starts With 10)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Name: 10}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.NotEndsWith,
                            PropertyName = "Name"
                        }
                    }
                }).Returns(99).SetName("Mapped: Filter by Name(Not Ends With 10)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Id: 10}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThan,
                            PropertyName = "Id"
                        }
                    }
                }).Returns(90).SetName("Mapped: Filter by Id(Greater Than)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Id: 10}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThan,
                            PropertyName = "Id"
                        }
                    }
                }).Returns(9).SetName("Mapped: Filter by Id(Less Than)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{IdRange: { Start: 1, End: 1 }}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.BetweenInclusive,
                            PropertyName = "IdRange"
                        }
                    }
                }).Returns(1).SetName("Mapped: Range Filter by IdRange(BetweenInclusive)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Age: { Start: 200, End: 400 }}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.BetweenExclusive,
                            PropertyName = "Age"
                        }
                    }
                }).Returns(33).SetName("Mapped: Range Filter by Age(Between Exclusive)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Age: { End: 400 }}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.BetweenExclusive,
                            PropertyName = "Age"
                        }
                    }
                }).Returns(67).SetName("Mapped: Range Filter by Age(Between Exclusive No Low)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Age: { Start: 400 }}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThan,
                            PropertyName = "Age"
                        }
                    }
                }).Returns(32).SetName("Mapped: Range Filter by Age(GreaterThen Low Only)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{Age: { Start: 200 }}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThan,
                            PropertyName = "Age"
                        }
                    }
                }).Returns(33).SetName("Mapped: Range Filter by Age(LessThan Low Only)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{FoundingDate: { Start: \"1904-02-02\" }}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.GreaterThan,
                            PropertyName = "FoundingDate"
                        }
                    }
                }).Returns(99).SetName("Mapped: Range Filter by FoundingDate(GreaterThan Low Only)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{FoundingDate: { Start: \"1904-02-02\" }}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.Equals,
                            PropertyName = "FoundingDate"
                        }
                    }
                }).Returns(1).SetName("Mapped: Range Filter by FoundingDate(Equals Low Only)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{FoundingDate: { Start: \"1904-02-03\" }}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThan,
                            PropertyName = "FoundingDate"
                        }
                    }
                }).Returns(1).SetName("Mapped: Range Filter by FoundingDate(LessThan Low Only)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{ValidOn: { Start: \"1904-02-03 00:00:00\" }}"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.LessThan,
                            PropertyName = "ValidOn"
                        }
                    }
                }).Returns(1).SetName("Mapped: Range Filter by ValidOn(LessThan Low Only)");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    Where = JObject.Parse("{ValidOnSingle: \"1916-05-05T00:00:00Z\" }"),
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            OperatorComparer = OperatorComparer.Equals,
                            PropertyName = "ValidOnSingle"
                        }
                    }
                }).Returns(1).SetName("Mapped: Range Filter by ValidOnSingle(Exact)");
            }
        }

        public static IEnumerable<TestCaseData> TestCasesSearch
        {
            get
            {
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, SearchQuery = "10" }).Returns(2).SetName("Search Query");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, SearchQuery = "100" }).Returns(1).SetName("Search Query 100");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, SearchQuery = "Builder 100" }).Returns(1).SetName("Search SubQuery 1");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, SearchQuery = "Builder 10" }).Returns(2).SetName("Search SubQuery 2");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, SearchQuery = "C11" }).Returns(2).SetName("Search SubQuery Certificates 1");
            }
        }

        public static IEnumerable<TestCaseData> TestCasesSort
        {
            get
            {
                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                        {
                            new PropertyFilterConfig
                            {
                                PropertyName = "Name",
                                SortDirection = SortDirection.ASC
                            }
                        }
                })
                .Returns(100)
                .SetName("Sort by Name");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            PropertyName = "Country",
                            SortDirection = SortDirection.DESC
                        }
                    }
                })
                .Returns(100)
                .SetName("Sort by Special Desc");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            PropertyName = "Country",
                            SortDirection = SortDirection.ASC
                        }
                    }
                })
                .Returns(100)
                .SetName("Sort by Special Asc");

                yield return new TestCaseData(new BasePagedRequest
                {
                    PageIndex = 0,
                    PageSize = 10,
                    PropertyFilterConfigs = new List<PropertyFilterConfig>
                    {
                        new PropertyFilterConfig
                        {
                            PropertyName = "Name",
                            SortDirection = SortDirection.DESC
                        },
                        new PropertyFilterConfig
                        {
                            PropertyName = "Address",
                            SortDirection = SortDirection.ASC
                        }
                    }
                })
                .Returns(100)
                .SetName("Sort by Multiple");
            }
        }

        public static IEnumerable<TestCaseData> TestCasesFilter
        {
            get
            {
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, Where = JObject.Parse("{Address: \"School Address 1\"}") }).Returns(1).SetName("Mapped: Filter by Address with no Property Filter Config");
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
            if (request.PropertyFilterConfigs.Any(pfc => pfc.SortDirection == SortDirection.DESC))
            {
                Assert.IsTrue(result.Results.First().Name.Contains("99"));
            }
            else
            {
                Assert.IsTrue(result.Results.First().Name.Contains("1"));
            }

            return result.RowCount;
        }

        [Test]
        [TestCaseSource(nameof(TestCasesFilter))]
        [Parallelizable(ParallelScope.All)]
        public async Task<int> MappingTestsFilter(BasePagedRequest request)
        {
            var unitUnderTest = new SchoolService(SchoolServiceTestData.Schools);
            var result = await unitUnderTest.GetPagedMapped<SchoolViewModel>(request);

            Type type = result.Results.GetType().GetGenericArguments()[0];

            Assert.IsTrue(type == typeof(SchoolViewModel));

            return result.RowCount;
        }

        [Test]
        public async Task MappingTestsStringSearchInherited()
        {
            var request = new BasePagedRequest { PageIndex = 0, PageSize = 10, SearchQuery = "10" };
            var unitUnderTest = new SchoolService(SchoolServiceTestData.Schools);
            var result = await unitUnderTest.GetCollegePagedSearchQuery<CollegeViewModel>(request);

            Type type = result.Results.GetType().GetGenericArguments()[0];

            Assert.IsTrue(type == typeof(CollegeViewModel));
            Assert.IsTrue(result.Results.Count == 4);
        }
    }
}
