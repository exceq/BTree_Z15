using System;
using System.Collections.Generic;
using System.Text;

namespace BTree_lib{
    public class Student
    {
        //[Идентификатор студента] [Фамилия] [Имя] [Отчество] [Название факультета] [Номер курса]
        public Student(int id, string ln, string fn, string sn, string fac, int course)
        {
            this.id = id;
            LastName = ln;
            FirstName = fn;
            SecondName = sn;
            Faculty = fac;
            CourseNumber = course;
        }
        public int id { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public string SecondName { get; private set; }
        public string Faculty { get; private set; }
        public int CourseNumber { get; private set; }
    }
}
