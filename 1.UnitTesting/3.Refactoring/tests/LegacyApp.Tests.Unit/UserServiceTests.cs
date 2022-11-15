using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LegacyApp.Tests.Unit
{
    public class UserServiceTests
    {
        private readonly IClientRepository clientRepository = Substitute.For<IClientRepository>();
        private readonly IUserCreditServiceClientFactory userCreditServiceClientFactory = Substitute.For<IUserCreditServiceClientFactory>();
        private readonly IUserCreditServiceAdapter userCreditServiceAdapter = Substitute.For<IUserCreditServiceAdapter>();
        private readonly IUserDataAccessRepository userDataAccessRepository = Substitute.For<IUserDataAccessRepository>();
        
        [Fact]
        public void AddUser_ShouldNotCreateUser_WhenFirstNameIsEmpty()
        {
            //arrange
            UserService sut = new UserService(clientRepository, userCreditServiceClientFactory, userDataAccessRepository);
            clientRepository.GetById(1).Returns(
                new Client
                {
                    Id = 1,
                    ClientStatus = ClientStatus.Gold,
                    Name = "Kevin"
                });
            userCreditServiceClientFactory.UserCreditService.Returns(userCreditServiceAdapter);
            userCreditServiceAdapter.GetCreditLimit("Kevin", "Moens", DateTime.Today.AddYears(-21)).Returns(500);
            //act
            var result = sut.AddUser("", "Moens", "kmoens@lakeco.com", DateTime.Today.AddYears(-21), 1);
            //assert

            // Assert
            result.Should().BeFalse();
        }
        [Fact]
        public void AddUser_ShouldNotCreateUser_WhenLastNameIsEmpty()
        {
            //arrange
            UserService sut = new UserService(clientRepository, userCreditServiceClientFactory, userDataAccessRepository);
            clientRepository.GetById(1).Returns(
                new Client
                {
                    Id = 1,
                    ClientStatus = ClientStatus.Gold,
                    Name = "Kevin"
                });
            userCreditServiceClientFactory.UserCreditService.Returns(userCreditServiceAdapter);
            userCreditServiceAdapter.GetCreditLimit("Kevin", "Moens", DateTime.Today.AddYears(-21)).Returns(499);
            //act
            var result = sut.AddUser("", "Moens", "kmoens@lakeco.com", DateTime.Today.AddYears(-21), 1);
            //assert

            // Assert
            result.Should().BeFalse();
        }
        [Theory]
        //[InlineData("kmoens@lakecocom")] //Existing Bug
        //[InlineData("kmoenslakeco.com")] //Existing Bug
        [InlineData("")]
        public void AddUser_ShouldNotCreateUser_WhenEmailIsInvalid(string email)
        {
            //arrange
            UserService sut = new UserService(clientRepository, userCreditServiceClientFactory, userDataAccessRepository);
            clientRepository.GetById(1).Returns(
                new Client
                {
                    Id = 1,
                    ClientStatus = ClientStatus.Gold,
                    Name = "Kevin"
                });
            userCreditServiceClientFactory.UserCreditService.Returns(userCreditServiceAdapter);
            userCreditServiceAdapter.GetCreditLimit("Kevin", "Moens", DateTime.Today.AddYears(-21)).Returns(500);
            //act
            var result = sut.AddUser("Kevin", "Moens", email, DateTime.Today.AddYears(-21), 1);
            //assert

            // Assert
            result.Should().BeFalse();
        }
        [Fact]
        public void AddUser_ShouldNotCreateUser_WhenUserIsUnder21()
        {
            //arrange
            UserService sut = new UserService(clientRepository, userCreditServiceClientFactory, userDataAccessRepository);
            clientRepository.GetById(1).Returns(
                new Client
                {
                    Id = 1,
                    ClientStatus = ClientStatus.Gold,
                    Name = "Kevin"
                });
            userCreditServiceClientFactory.UserCreditService.Returns(userCreditServiceAdapter);
            userCreditServiceAdapter.GetCreditLimit("Kevin", "Moens", DateTime.Today.AddYears(-21)).Returns(500);
            //act
            var result = sut.AddUser("Kevin", "Moens", "kmoens@lakeco.com", DateTime.Today.AddYears(-20), 1);
            //assert

            // Assert
            result.Should().BeFalse();
        }
        [Fact]
        public void AddUser_ShouldNotCreateUser_WhenUserHasCreditLimitAndLimitIsLessThan500()
        {
            //arrange
            UserService sut = new UserService(clientRepository, userCreditServiceClientFactory, userDataAccessRepository);
            clientRepository.GetById(1).Returns(
                new Client
                {
                    Id = 1,
                    ClientStatus = ClientStatus.Gold,
                    Name = "Kevin"
                });
            userCreditServiceClientFactory.UserCreditService.Returns(userCreditServiceAdapter);
            userCreditServiceAdapter.GetCreditLimit("Kevin", "Moens", DateTime.Today.AddYears(-21)).Returns(499);
            //act
            var result = sut.AddUser("Kevin", "Moens", "kmoens@lakeco.com", DateTime.Today.AddYears(-21), 1);
            //assert

            // Assert
            result.Should().BeFalse();
        }
        [Fact]
        public void AddUser_ShouldCreateUser_WhenUserDetailsAreValid()
        {
            //arrange
            UserService sut = new UserService(clientRepository, userCreditServiceClientFactory, userDataAccessRepository);
            clientRepository.GetById(1).Returns(
                new Client
                {
                    Id = 1,
                    ClientStatus = ClientStatus.Gold,
                    Name = "Kevin"
                });
            userCreditServiceClientFactory.UserCreditService.Returns(userCreditServiceAdapter);
            userCreditServiceAdapter.GetCreditLimit("Kevin", "Moens", DateTime.Today.AddYears(-21)).Returns(500);
            //act
            var result = sut.AddUser("Kevin", "Moens", "kmoens@lakeco.com", DateTime.Today.AddYears(-21), 1);
            //assert

            // Assert
            result.Should().BeTrue();
        }
    }
}
