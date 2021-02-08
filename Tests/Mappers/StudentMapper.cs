using System.Collections.Generic;
using Tests.Models;

namespace Tests.Mappers
{
    /// <summary>
    /// Just a showcase of a mapping funcion
    /// Can be anything you want
    /// Personally I use AutoMapper
    /// </summary>
    static class StudentMapper
    {
        static StudentViewModel MapToViewModel(Student student)
        {
            return new StudentViewModel
            {
                Id = student.Id,
                Age = student.Age,
                FirstName = student.FirstName,
                LastName = student.LastName,
                FullName = $"{student.FirstName} {student.LastName}",
                Gender = student.Gender
            };
        }

        public static List<StudentViewModel> MapListToViewModel(List<Student> students)
        {
            var studentViewModelList = new List<StudentViewModel>();

            students.ForEach(student =>
            {
                var mappedStudent = MapToViewModel(student);
                studentViewModelList.Add(mappedStudent);
            });

            return studentViewModelList;
        }
    }
}
