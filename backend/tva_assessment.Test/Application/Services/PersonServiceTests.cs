using FluentAssertions;
using Moq;
using tva_assessment.Application.DTOs;
using tva_assessment.Application.Interfaces;
using tva_assessment.Application.Services;
using tva_assessment.Domain.Entities;

namespace tva_assessment.Test.Application.Services
{
    [TestFixture]
    public class PersonServiceTests
    {
        private Mock<IPersonRepository> _personRepositoryMock = null!;
        private PersonService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _sut = new PersonService(_personRepositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _personRepositoryMock.VerifyAll();
        }

        [Test]
        public async Task CreateAsync_WhenIdNumberAlreadyExists_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = CreatePersonDto();

            _personRepositoryMock
                .Setup(r => r.ExistsByIdNumberAsync(dto.IdNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var act = async () => await _sut.CreateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("A person with the same ID number already exists.");
        }

        [Test]
        public async Task CreateAsync_WhenValid_ShouldAddPersonAndReturnDtoWithCode()
        {
            // Arrange
            var dto = CreatePersonDto();

            _personRepositoryMock
                .Setup(r => r.ExistsByIdNumberAsync(dto.IdNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _personRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()))
                .Callback<Person, CancellationToken>((p, _) =>
                {
                    p.Code = 42;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.CreateAsync(dto, CancellationToken.None);

            // Assert – DTO values
            result.Should().NotBeNull();
            result.Code.Should().Be(42);
            result.IdNumber.Should().Be(dto.IdNumber);
            result.Name.Should().Be(dto.Name);
            result.Surname.Should().Be(dto.Surname);
        }

        [Test]
        public async Task UpdateAsync_WhenPersonDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var dto = new PersonDto
            {
                Code = 1,
                IdNumber = "123",
                Name = "Jane",
                Surname = "Smith"
            };

            _personRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.Code, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Person?)null);

            // Act
            var result = await _sut.UpdateAsync(dto, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task UpdateAsync_WhenChangingToExistingIdNumber_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var existingPerson = CreatePerson(
                code: 1,
                idNumber: "OLD-ID",
                name: "Jane",
                surname: "Smith");

            var dto = new PersonDto
            {
                Code = 1,
                IdNumber = "NEW-ID",
                Name = "Jane Updated",
                Surname = "Smith Updated"
            };

            _personRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.Code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPerson);

            _personRepositoryMock
                .Setup(r => r.ExistsByIdNumberAsync(dto.IdNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var act = async () => await _sut.UpdateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("A person with the same ID number already exists.");
        }

        [Test]
        public async Task DeleteAsync_WhenPersonDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var code = 123;

            _personRepositoryMock
                .Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Person?)null);

            // Act
            var result = await _sut.DeleteAsync(code, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public async Task DeleteAsync_WhenPersonHasOpenAccounts_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var code = 1;

            var person = CreatePerson(
                code: code,
                idNumber: "123",
                name: "John",
                surname: "Doe",
                accounts: new[]
                {
                    new Account
                    {
                        Code = 10,
                        AccountNumber = "ACC-001",
                        Status = new AccountStatus { IsClosed = false }
                    }
                });

            _personRepositoryMock
                .Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);

            // Act
            var act = async () => await _sut.DeleteAsync(code, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("The person cannot be deleted because one or more accounts are still open.");
        }

        [Test]
        public async Task DeleteAsync_WhenPersonHasNoAccounts_ShouldDeleteAndReturnTrue()
        {
            // Arrange
            var code = 1;

            var person = CreatePerson(
                code: code,
                idNumber: "123",
                name: "John",
                surname: "Doe",
                accounts: Enumerable.Empty<Account>());

            _personRepositoryMock
                .Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);

            _personRepositoryMock
                .Setup(r => r.DeleteAsync(person, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.DeleteAsync(code, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task SearchAsync_WhenPageNumberIsLessThanOne_ShouldThrowInvalidOperationException()
        {
            // Arrange
            const int pageNumber = 0;
            const int pageSize = 5;

            // Act
            var act = async () =>
                await _sut.SearchAsync(
                    idNumber: null,
                    surname: null,
                    accountNumber: null,
                    pageNumber: pageNumber,
                    pageSize: pageSize,
                    cancellationToken: CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Page number must be at least 1.");
        }

        [Test]
        public async Task SearchAsync_WhenPageSizeExceedsMax_ShouldClampToTenAndCalculatePaging()
        {
            // Arrange
            const int pageNumber = 1;
            const int requestedPageSize = 50;
            const int totalCount = 25;

            _personRepositoryMock
                .Setup(r => r.CountAsync(null, null, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(totalCount);

            _personRepositoryMock
                .Setup(r => r.SearchAsync(null, null, null, 0, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Person>());

            // Act
            var result = await _sut.SearchAsync(
                idNumber: null,
                surname: null,
                accountNumber: null,
                pageNumber: pageNumber,
                pageSize: requestedPageSize,
                cancellationToken: CancellationToken.None);

            // Assert
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(25);
            result.TotalPages.Should().Be(3);
        }

        private static PersonDto CreatePersonDto(
            string idNumber = "1234567890",
            string name = "John",
            string surname = "Doe")
        {
            return new PersonDto
            {
                IdNumber = idNumber,
                Name = name,
                Surname = surname
            };
        }

        private static Person CreatePerson(
            int code,
            string idNumber,
            string name,
            string surname,
            IEnumerable<Account>? accounts = null)
        {
            return new Person
            {
                Code = code,
                IdNumber = idNumber,
                Name = name,
                Surname = surname,
                Accounts = accounts?.ToList() ?? new List<Account>()
            };
        }
    }
}
