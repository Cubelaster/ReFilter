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
                IsActive = school.IsActive
            };
        }

        static CollegeViewModel MapToViewModel(College school)
        {
            return new CollegeViewModel
            {
                Id = school.Id,
                Name = school.Name,
                Age = school.Age.ToString(),
                Address = school.Address,
                Country = school.Country != null ? $"{school.Country.Alpha2Code} {school.Country.Name}" : null,
                Contacts = school.Contacts,
                Students = StudentMapper.MapListToViewModel(school.Students),
                IsActive = school.IsActive
            };
        }
        static College MapToCollege(School school)
        {
            return new College
            {
                Id = school.Id,
                Name = school.Name,
                Age = school.Age.ToString(),
                Address = school.Address,
                Country = school.Country,
                Contacts = school.Contacts,
                Students = school.Students,
                IsActive = school.IsActive
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

        public static List<CollegeViewModel> MapListToViewModel(List<College> schools)
        {
            var schoolViewModelList = new List<CollegeViewModel>();

            schools.ForEach(school =>
            {
                var mappedSchool = MapToViewModel(school);
                schoolViewModelList.Add(mappedSchool);
            });

            return schoolViewModelList;
        }

        public static List<College> MapListToCollege(List<School> schools)
        {
            var schoolViewModelList = new List<College>();

            schools.ForEach(school =>
            {
                var mappedSchool = MapToCollege(school);
                schoolViewModelList.Add(mappedSchool);
            });

            return schoolViewModelList;
        }
    }
}
