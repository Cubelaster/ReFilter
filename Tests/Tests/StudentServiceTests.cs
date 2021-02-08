using System.Threading.Tasks;
using ReFilter.Models;
using Tests.TestData;
using Xunit;

namespace Tests.Tests
{
    public class StudentServiceTests
    {
        [Fact]
        public void PassingTest()
        {
            Assert.True(true);
        }

        [Theory]
        [ClassData(typeof(StudentServiceTestData))]
        public async Task FilterData(BasePagedRequest request, int count)
        {

        }
    }
}
