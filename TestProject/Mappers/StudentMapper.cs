using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TestProject.Models;

namespace TestProject.Mappers
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

        public static Expression<Func<Student, StudentViewModel>> MappingExpression()
        {
            Expression<Func<Student, StudentViewModel>> expression = student => new StudentViewModel
            {
                Id = student.Id,
                Age = student.Age,
                FirstName = student.FirstName,
                LastName = student.LastName,
                FullName = $"{student.FirstName} {student.LastName}",
                Gender = student.Gender
            };
            return expression;
        }

        public static List<StudentViewModel> MapIQueryableToViewModel(IQueryable<Student> students)
        {
            return students.Select(MappingExpression()).ToList();
        }
    }
}
