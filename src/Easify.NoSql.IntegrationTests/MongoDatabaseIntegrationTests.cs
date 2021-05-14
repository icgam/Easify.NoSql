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


using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Easify.NoSql.IntegrationTests
{
    public class MongoDatabaseIntegrationTests : IClassFixture<MongoDatabaseFixture>
    {
        private readonly MongoDatabaseFixture _fixture;
        private readonly IDocumentRepository<Student> _repository;

        public MongoDatabaseIntegrationTests(MongoDatabaseFixture fixture)
        {
            _fixture = fixture;

            var factory = _fixture.ServiceProvider.GetService(typeof(IDocumentRepositoryFactory)) as IDocumentRepositoryFactory;
            _repository = factory?.GetRepository<Student>("SampleDbTests");
        }

        // [Theory]
        // [InlineData(1, 1, 1, 0)]
        // [InlineData(3, 2, 3, 0)]
        public async Task Should_InsertAndLoad_ReturnsTheRightSetOfDocuments_WhenInsertedOneByOne(int count, int groupId, int expectedGroupOne, int expectedGroupTwo)
        {
            try
            {
                // Given
                var students = Enumerable.Range(1, count).Select(m => new Student() { GroupId = groupId}).ToList();

                // When
                foreach (var student in students)
                {
                    await _repository.InsertAsync(student);
                }

                var groupOneResults = await _repository.GetListAsync(m => m.GroupId == groupId, s => s.OrderBy(m => m.DateOfBirth).ThenByDescending(m => m.Age));
                var groupTwoResults = await _repository.GetListAsync(m => m.GroupId == groupId * -1, s => s.OrderBy(m => m.DateOfBirth).ThenByDescending(m => m.Age));

                // Then
                groupOneResults.Should().HaveCount(expectedGroupOne);
                groupTwoResults.Should().HaveCount(expectedGroupTwo);

            }
            finally
            {
                await _repository.DeleteManyAsync(m => m.GroupId == groupId);
            }
        }

        // [Theory]
        // [InlineData(1, 3, 1, 0)]
        // [InlineData(3, 4, 3, 0)]
        public async Task Should_InsertAndLoad_ReturnsTheRightSetOfDocumentss_WhenInsertedInBatch(int count, int groupId, int expectedGroupOne, int expectedGroupTwo)
        {
            try
            {
                // Given
                var students = Enumerable.Range(1, count).Select(m => new Student() { GroupId = groupId}).ToList();

                // When
                await _repository.InsertManyAsync(students.ToArray());

                var groupOneResults = await _repository.GetListAsync(m => m.GroupId == groupId, s => s.OrderBy(m => m.DateOfBirth).ThenByDescending(m => m.Age));
                var groupTwoResults = await _repository.GetListAsync(m => m.GroupId == groupId * -1, s => s.OrderBy(m => m.DateOfBirth).ThenByDescending(m => m.Age));

                // Then
                groupOneResults.Should().HaveCount(expectedGroupOne);
                groupTwoResults.Should().HaveCount(expectedGroupTwo);

            }
            finally
            {
                await _repository.DeleteManyAsync(m => m.GroupId == groupId);
            }
        }

        // [Theory]
        // [InlineData(4, 5, 6, 5, 0)]
        public async Task Should_UpdateAndInsertNewAndLoad_ReturnsTheRightSetOfChanges(int count, int groupId, int targetGroupId, int expectedGroupOne, int expectedGroupTwo)
        {
            try
            {
                // Given
                var students = Enumerable.Range(1, count).Select(m => new Student() { GroupId = groupId}).ToList();
                await _repository.InsertManyAsync(students.ToArray());

                // When
                students.ForEach(s => s.GroupId = targetGroupId);
                students.Add(new Student() {GroupId = targetGroupId });
                foreach (var student in students)
                {
                    await _repository.UpdateAsync(m => m.Id == student.Id, student);
                }
                

                var groupOneResults = await _repository.GetListAsync(m => m.GroupId == targetGroupId, s => s.OrderBy(m => m.DateOfBirth).ThenByDescending(m => m.Age));
                var groupTwoResults = await _repository.GetListAsync(m => m.GroupId == groupId, s => s.OrderBy(m => m.DateOfBirth).ThenByDescending(m => m.Age));

                // Then
                groupOneResults.Should().HaveCount(expectedGroupOne);
                groupTwoResults.Should().HaveCount(expectedGroupTwo);

            }
            finally
            {
                await _repository.DeleteManyAsync(m => m.GroupId == targetGroupId);
            }
        }
    }
}