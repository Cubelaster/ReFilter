﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
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
                            OperatorComparer = ReFilter.Enums.OperatorComparer.Contains,
                            PropertyName = "Name"
                        },
                        new PropertyFilterConfig
                        {
                            OperatorComparer = ReFilter.Enums.OperatorComparer.Contains,
                            PropertyName = "Address"
                        }
                    }
                }).Returns(2).SetName("Mapped: Filter by Name(Contains) and Address(Contains)");
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
    }
}
