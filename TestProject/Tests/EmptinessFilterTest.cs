using LinqKit;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ReFilter.Enums;
using ReFilter.Models;
using ReFilter.ReFilterActions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestProject.Models;
using TestProject.RequiredImplementations;
using TestProject.TestServices;

namespace TestProject.Tests
{
    [TestFixture]
    class EmptinessFilterTest
    {
        // 1: everything set | 2: everything null | 3: empty string / empty list | 4: mixed
        private static List<School> Schools => new()
        {
            new School { Id = 1, Name = "A", Address = "Addr 1", Building = new Building { Name = "B1" }, Contacts = new List<string> { "x" } },
            new School { Id = 2, Name = null, Address = "Addr 2", Building = null, Contacts = null },
            new School { Id = 3, Name = "", Address = "Addr 3", Building = new Building { Name = "B3" }, Contacts = new List<string>() },
            new School { Id = 4, Name = "D", Address = "Addr 4", Building = null, Contacts = new List<string> { "y", "z" } }
        };

        private static BasePagedRequest Request(params PropertyFilterConfig[] pfcs) => new()
        {
            PageIndex = 0,
            PageSize = 10,
            PropertyFilterConfigs = pfcs.ToList()
        };

        private static PropertyFilterConfig Pfc(string propertyName, OperatorComparer operatorComparer) => new()
        {
            PropertyName = propertyName,
            OperatorComparer = operatorComparer
        };

        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(Request(Pfc("Name", OperatorComparer.IsEmpty))).Returns(new[] { 2, 3 }).SetName("Emptiness: String IsEmpty matches null and empty");
                yield return new TestCaseData(Request(Pfc("Name", OperatorComparer.IsNotEmpty))).Returns(new[] { 1, 4 }).SetName("Emptiness: String IsNotEmpty");
                yield return new TestCaseData(Request(Pfc("Building", OperatorComparer.IsEmpty))).Returns(new[] { 2, 4 }).SetName("Emptiness: Object IsEmpty matches null");
                yield return new TestCaseData(Request(Pfc("Building", OperatorComparer.IsNotEmpty))).Returns(new[] { 1, 3 }).SetName("Emptiness: Object IsNotEmpty");
                yield return new TestCaseData(Request(Pfc("Contacts", OperatorComparer.IsEmpty))).Returns(new[] { 2, 3 }).SetName("Emptiness: Collection IsEmpty matches null and no elements");
                yield return new TestCaseData(Request(Pfc("Contacts", OperatorComparer.IsNotEmpty))).Returns(new[] { 1, 4 }).SetName("Emptiness: Collection IsNotEmpty");
                yield return new TestCaseData(Request(Pfc("Age", OperatorComparer.IsEmpty))).Returns(new int[0]).SetName("Emptiness: Non-nullable value type IsEmpty never matches");
                yield return new TestCaseData(Request(Pfc("Age", OperatorComparer.IsNotEmpty))).Returns(new[] { 1, 2, 3, 4 }).SetName("Emptiness: Non-nullable value type IsNotEmpty always matches");

                yield return new TestCaseData(Request(
                    Pfc("Name", OperatorComparer.IsNotEmpty),
                    Pfc("Building", OperatorComparer.IsEmpty)))
                    .Returns(new[] { 4 }).SetName("Emptiness: Combined across properties (And)");

                yield return new TestCaseData(Request(
                    Pfc("Name", OperatorComparer.IsEmpty),
                    new PropertyFilterConfig { PropertyName = "Name", OperatorComparer = OperatorComparer.Equals, Value = "A", PredicateOperator = PredicateOperator.Or }))
                    .Returns(new[] { 1, 2, 3 }).SetName("Emptiness: Or with valued PFC on same property");

                yield return new TestCaseData(Request(
                    Pfc("Name", OperatorComparer.IsNotEmpty),
                    new PropertyFilterConfig { PropertyName = "Name", SortDirection = SortDirection.DESC }))
                    .Returns(new[] { 4, 1 }).SetName("Emptiness: Sort-only PFC on same property is not treated as a filter");

                var withWhere = Request(Pfc("Building", OperatorComparer.IsEmpty));
                withWhere.Where = JObject.Parse("{Address: \"Addr 4\"}");
                yield return new TestCaseData(withWhere).Returns(new[] { 4 }).SetName("Emptiness: Combined with Where");

                var valueIgnored = Request(new PropertyFilterConfig { PropertyName = "Name", OperatorComparer = OperatorComparer.IsEmpty, Value = "ignored" });
                var nested = Request();
                nested.PredicateOperator = PredicateOperator.Or;
                nested.PagedRequests = new List<PagedRequest>
                {
                    new() { PropertyFilterConfigs = new List<PropertyFilterConfig> { Pfc("Name", OperatorComparer.IsEmpty) } },
                    new() { PropertyFilterConfigs = new List<PropertyFilterConfig> { Pfc("Building", OperatorComparer.IsEmpty) } }
                };
                yield return new TestCaseData(nested).Returns(new[] { 2, 3, 4 }).SetName("Emptiness: Nested PagedRequests (Or)");

                yield return new TestCaseData(valueIgnored).Returns(new[] { 2, 3 }).SetName("Emptiness: PFC Value is ignored");
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        [Parallelizable(ParallelScope.All)]
        public async Task<int[]> EmptinessFilter(BasePagedRequest request)
        {
            var unitUnderTest = new SchoolService(Schools);
            var result = await unitUnderTest.GetPaged(request);

            return result.Results.Select(e => e.Id).ToArray();
        }

        [Test]
        public void EmptinessFilterExpressionOverload()
        {
            var reFilterActions = new ReFilterActions(new ReFilterConfigBuilder(), new ReSortConfigBuilder());
            var request = new PagedRequest
            {
                PropertyFilterConfigs = new List<PropertyFilterConfig> { Pfc("Contacts", OperatorComparer.IsEmpty) }
            };

            var schools = Schools.AsQueryable();
            var predicate = reFilterActions.FilterObject(request, schools);
            var ids = schools.AsExpandable().Where(predicate).Select(e => e.Id).ToArray();

            Assert.That(ids, Is.EquivalentTo(new[] { 2, 3 }));
        }
    }
}
