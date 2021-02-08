using System.Collections.Generic;
using TestProject.Models;

namespace TestProject.Mappers
{
    static class SchoolMapper
    {
        static SchoolViewModel MapToViewModel(School school)
        {
            return new SchoolViewModel
            {
                Id = school.Id,
                Address = school.Address,
                Name = school.Name,
                Contacts = school.Contacts,
                Students = StudentMapper.MapListToViewModel(school.Students)
            };
        }

        public static List<SchoolViewModel> MapListToViewModel(List<School> schools)
        {
            var schoolViewModelList = new List<SchoolViewModel>();

            schools.ForEach(school =>
            {
                var mappedSchool = MapToViewModel(school);
                schoolViewModelList.Add(mappedSchool);
            });

            return schoolViewModelList;
        }
    }
}
