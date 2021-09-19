using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReFilter.Core.Models;
using ReFilter.ReFilterActions;
using ReFilter.ReFilterTypeMatcher;
using TestProject.Mappers;
using TestProject.Models;
using TestProject.RequiredImplementations;

namespace TestProject.TestServices
{
    class SchoolService
    {
        #region Ctors and Members

        private readonly List<School> testList;
        private readonly ReFilterActions testReFilterActions;

        /// <summary>
        /// Usually you will use DI for initialization
        /// </summary>
        /// <param name="testList"></param>
        public SchoolService(List<School> testList)
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

        public async Task<PagedResult<School>> GetPaged(BasePagedRequest request)
        {
            var testQueryable = testList.AsQueryable();

            var pagedRequest = request.GetPagedRequest(returnResults: true);

            var result = await testReFilterActions.GetPaged(testQueryable, pagedRequest);

            return result;
        }

        public async Task<PagedResult<SchoolViewModel>> GetPagedMapped<U>(BasePagedRequest request)
        {
            var testQueryable = testList.AsQueryable();

            List<SchoolViewModel> mappingFunction(List<School> x) => SchoolMapper.MapListToViewModel(x);
            var pagedRequest = request.GetPagedRequest((Func<List<School>, List<SchoolViewModel>>)mappingFunction);

            var result = await testReFilterActions.GetPaged(testQueryable, pagedRequest);

            return result;
        }

        public async Task<PagedResult<SchoolViewModel>> GetPagedSearchQuery<U>(BasePagedRequest request)
        {
            var testQueryable = testList.AsQueryable();

            List<SchoolViewModel> mappingFunction(List<School> x) => SchoolMapper.MapListToViewModel(x);
            var pagedRequest = request.GetPagedRequest((Func<List<School>, List<SchoolViewModel>>)mappingFunction);

            var result = await testReFilterActions.GetBySearchQuery(testQueryable, pagedRequest);

            return result;
        }
    }
}
