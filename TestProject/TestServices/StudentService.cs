using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReFilter.Models;
using ReFilter.ReFilterActions;
using ReFilter.ReFilterTypeMatcher;
using TestProject.Mappers;
using TestProject.Models;
using TestProject.RequiredImplementations;

namespace TestProject.TestServices
{
    /// <summary>
    /// A simple test class showcasing 2 main principles
    /// </summary>
    class StudentService
    {
        #region Ctors and Members

        private readonly List<Student> testList;
        private readonly ReFilterActions testReFilterActions;

        /// <summary>
        /// Usually you will use DI for initialization
        /// </summary>
        /// <param name="testList"></param>
        public StudentService(List<Student> testList)
        {
            this.testList = testList;

            testReFilterActions = InitializeTestFilterActions(new ReFilterConfigBuilder());
        }

        /// <summary>
        /// Just a helper instead of DI
        /// </summary>
        /// <param name="reFilterConfigBuilder"></param>
        /// <returns></returns>
        private ReFilterActions InitializeTestFilterActions(IReFilterConfigBuilder reFilterConfigBuilder)
        {
            return new ReFilterActions(reFilterConfigBuilder);
        }

        #endregion Ctors and Members

        public async Task<PagedResult<Student>> GetPaged(BasePagedRequest request)
        {
            var testQueryable = testList.AsQueryable();

            var pagedRequest = request.GetPagedRequest(returnResultsOnly: true);

            var result = await testReFilterActions.GetPaged(testQueryable, pagedRequest);

            return result;
        }

        public async Task<PagedResult<StudentViewModel>> GetPagedMapped<U>(BasePagedRequest request)
        {
            var testQueryable = testList.AsQueryable();

            List<StudentViewModel> mappingFunction(List<Student> x) => StudentMapper.MapListToViewModel(x);
            var pagedRequest = request.GetPagedRequest((Func<List<Student>, List<StudentViewModel>>)mappingFunction);

            var result = await testReFilterActions.GetPaged(testQueryable, pagedRequest);

            return result;
        }

        public async Task<PagedResult<StudentViewModel>> GetPagedMappedProjection<U>(BasePagedRequest request)
        {
            var testQueryable = testList.AsQueryable();

            List<StudentViewModel> mappingFunction(List<Student> x) => StudentMapper.MapListToViewModel(x);
            var pagedRequest = request.GetPagedRequest((Func<List<Student>, List<StudentViewModel>>)mappingFunction);

            var result = await testReFilterActions.GetPaged(testQueryable, pagedRequest);

            return result;
        }
    }
}
