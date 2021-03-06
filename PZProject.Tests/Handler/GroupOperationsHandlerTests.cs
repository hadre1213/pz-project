using Moq;
using NUnit.Framework;
using PZProject.Data.Database.Entities.User;
using PZProject.Data.Repositories.Group;
using PZProject.Data.Repositories.User;
using PZProject.Data.Requests.GroupRequests;
using PZProject.Handlers.Group;
using PZProject.Handlers.Group.Operations.AssignUser;
using PZProject.Handlers.Group.Operations.Create;
using PZProject.Handlers.Group.Operations.Delete;
using PZProject.Handlers.Group.Operations.Edit;
using PZProject.Handlers.Group.Operations.RemoveUser;
using PZProject.Tests.Infrastructure;
using System;

namespace PZProject.Tests.Handler
{
    public class GroupOperationsHandlerTests : TestsInfrastructure
    {
        [Test]
        public void Should_Throw_Exception_For_Invalid_IssuerId([Values(-1, 0, null)] int invalidIssuerId)
        {
            //ARRANGE
            var fixture = new GroupOperationsHandlerTestsFixture()
                .ConfigureSut();

            var request = CreateValidRequest();

            //ACT && ASSERT
            var exception = Assert.Throws<Exception>(() => fixture.Sut.CreateNewGroup(request, invalidIssuerId));
            Assert.That(exception.Message, Is.EqualTo($"Could not find entity of type [{typeof(UserEntity).Name}]"));
        }

        [Test]
        public void Should_Create_Group_For_Valid_Input_Parameters()
        {
            //ARRANGE
            var issuerId = 100;
            var fixture = new GroupOperationsHandlerTestsFixture()
                .SetupUserRepositoryToReturnUserForId(issuerId)
                .ConfigureSut();

            var request = CreateValidRequest();

            //ACT && ASSERT
            Assert.DoesNotThrow(() => fixture.Sut.CreateNewGroup(request, issuerId));
        }

        private CreateGroupRequest CreateValidRequest()
        {
            return new CreateGroupRequest
            {
                GroupName = "groupName",
                GroupDescription = "groupDescription"
            };
        }
    }

    public class GroupOperationsHandlerTestsFixture
    {
        public GroupOperationsHandler Sut { get; set; }
        public int ValidIssuerId { get; set; }

        private readonly Mock<IGroupCreator> _groupCreatorMock = new Mock<IGroupCreator>();
        private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
        private readonly Mock<IGroupRepository> _groupRepositoryMock = new Mock<IGroupRepository>();
        private readonly Mock<IGroupEditHandler> _groupEditHandlerMock = new Mock<IGroupEditHandler>();
        private readonly Mock<IGroupDeleteHandler> _groupDeleteHandlerMock = new Mock<IGroupDeleteHandler>();
        private readonly Mock<IGroupAssignUserHandler> _groupAssignUserHandlerMock = new Mock<IGroupAssignUserHandler>();
        private readonly Mock<IGroupRemoveUserHandler> _groupRemoveUserHandlerMock = new Mock<IGroupRemoveUserHandler>();
        
        public GroupOperationsHandlerTestsFixture ConfigureSut()
        {
            Sut = new GroupOperationsHandler(_groupRepositoryMock.Object,
                _userRepositoryMock.Object,
                _groupCreatorMock.Object,
                _groupDeleteHandlerMock.Object,
                _groupAssignUserHandlerMock.Object,
                _groupRemoveUserHandlerMock.Object,
                _groupEditHandlerMock.Object);

            return this;
        }

        public GroupOperationsHandlerTestsFixture SetupUserRepositoryToReturnUserForId(int userId)
        {
            _userRepositoryMock
                .Setup(r => r.GetUserById(It.IsAny<int>()))
                .Returns(new UserEntity
                {
                    UserId = userId,
                    FirstName = "TestUser",
                    LastName = "LastName"
                });

            return this;
        }
    }
}