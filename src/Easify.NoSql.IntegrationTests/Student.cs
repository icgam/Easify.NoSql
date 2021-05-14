// This software is part of the Easify.Ef Library
// Copyright (C) 2018 Intermediate Capital Group
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 


using System;

namespace Easify.NoSql.IntegrationTests
{
    public class Student
    {
        public Student()
        {
            Id = Guid.NewGuid().ToString();
            Name = "Student" + Guid.NewGuid();
            GroupId = 1;
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; } = DateTime.Today.Add(TimeSpan.FromDays(new Random().Next(1000) * -1));
        public int Age { get; set; } = (new Random().Next(100));
        public int GroupId { get; set; }
        public StudentStage StudentStage { get; set; }
    }

    public enum StudentStage
    {
        Primary,
        HighSchool
    }
}