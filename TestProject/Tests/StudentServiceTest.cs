using System;
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
    public class StudentServiceTest
    {
        public static IEnumerable<TestCaseData> TestCasesNoMapping
        {
            get
            {
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10 }).Returns(StudentServiceTestData.Students.Count).SetName("No Filters");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, Where = JObject.Parse("{FirstName: 10}")}).Returns(0).SetName("Syntax correct but default values included so expect 0");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, Where = JObject.Parse("{Id: 10, Gender: 1}") }).Returns(1).SetName("Override default value so filter can work correctly");
            }
        }

        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10 }).Returns(StudentServiceTestData.Students.Count).SetName("Mapped: No Filters");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, Where = JObject.Parse("{FirstName: 10}") }).Returns(0).SetName("Mapped: Syntax correct but default values included so expect 0");
                yield return new TestCaseData(new BasePagedRequest { PageIndex = 0, PageSize = 10, Where = JObject.Parse("{Id: 10, Gender: 1}") }).Returns(1).SetName("Mapped: Override default value so filter can work correctly");
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCasesNoMapping))]
        [Parallelizable(ParallelScope.All)]
        public async Task<int> NoMappingTest(BasePagedRequest request)
        {
            var unitUnderTest = new StudentService(StudentServiceTestData.Students);
            var result = await unitUnderTest.GetPaged(request);

            return result.RowCount;
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        [Parallelizable(ParallelScope.All)]
        public async Task<int> MappingTests(BasePagedRequest request)
        {
            var unitUnderTest = new StudentService(StudentServiceTestData.Students);
            var result = await unitUnderTest.GetPagedMapped<StudentViewModel>(request);

            Type type = result.Results.GetType().GetGenericArguments()[0];

            Assert.IsTrue(type == typeof(StudentViewModel));
            Assert.IsTrue(result.Results.TrueForAll(s => s.FullName == $"{s.FirstName} {s.LastName}"));

            return result.RowCount;
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        [Parallelizable(ParallelScope.All)]
        public async Task<int> MappingTestsProjection(BasePagedRequest request)
        {
            var unitUnderTest = new StudentService(StudentServiceTestData.Students);
            var result = await unitUnderTest.GetPagedMappedProjection<StudentViewModel>(request);

            Type type = result.Results.GetType().GetGenericArguments()[0];

            Assert.IsTrue(type == typeof(StudentViewModel));
            Assert.IsTrue(result.Results.TrueForAll(s => s.FullName == $"{s.FirstName} {s.LastName}"));

            return result.RowCount;
        }
    }
}
