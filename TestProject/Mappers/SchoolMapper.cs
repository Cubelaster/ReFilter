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
                Name = school.Name,
                Address = school.Address,
                Country = school.Country != null ? $"{school.Country.Alpha2Code} {school.Country.Name}" : null,
                Contacts = school.Contacts,
                Students = StudentMapper.MapListToViewModel(school.Students),
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
