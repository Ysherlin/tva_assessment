using FluentAssertions;
using Moq;
using tva_assessment.Application.DTOs;
using tva_assessment.Application.Interfaces;
using tva_assessment.Application.Services;
using tva_assessment.Domain.Entities;

namespace tva_assessment.Test.Application.Services
{
    [TestFixture]
    public class AccountServiceTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock = null!;
        private Mock<IPersonRepository> _personRepositoryMock = null!;
        private AccountService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _accountRepositoryMock = new Mock<IAccountRepository>(MockBehavior.Strict);
            _personRepositoryMock = new Mock<IPersonRepository>(MockBehavior.Strict);
            _sut = new AccountService(_accountRepositoryMock.Object, _personRepositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _accountRepositoryMock.VerifyAll();
            _personRepositoryMock.VerifyAll();
        }

        [Test]
        public async Task CreateAsync_WhenPersonDoesNotExist_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = CreateAccountDto();

            _personRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.PersonCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Person?)null);

            // Act
            var act = async () => await _sut.CreateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("The person does not exist.");
        }

        [Test]
        public async Task CreateAsync_WhenAccountNumberAlreadyExists_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = CreateAccountDto();

            _personRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.PersonCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreatePerson(dto.PersonCode));

            _accountRepositoryMock
                .Setup(r => r.GetByAccountNumberAsync(dto.AccountNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateAccount(code: 10, personCode: dto.PersonCode, accountNumber: dto.AccountNumber));

            // Act
            var act = async () => await _sut.CreateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("An account with the same account number already exists.");
        }

        [Test]
        public async Task CreateAsync_WhenValid_ShouldCreateAccountWithZeroBalanceAndOpenStatus()
        {
            // Arrange
            var dto = CreateAccountDto();

            _personRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.PersonCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreatePerson(dto.PersonCode));

            _accountRepositoryMock
                .Setup(r => r.GetByAccountNumberAsync(dto.AccountNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            Account? savedAccount = null;

            _accountRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((a, _) =>
                {
                    a.Code = 42;
                    savedAccount = a;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.CreateAsync(dto, CancellationToken.None);

            // Assert – DTO values
            result.Should().NotBeNull();
            result.Code.Should().Be(42);
            result.PersonCode.Should().Be(dto.PersonCode);
            result.AccountNumber.Should().Be(dto.AccountNumber);
            result.OutstandingBalance.Should().Be(0m);
            result.IsClosed.Should().BeFalse();

            // Assert – entity state
            savedAccount.Should().NotBeNull();
            savedAccount!.OutstandingBalance.Should().Be(0m);
            savedAccount.Status.Should().NotBeNull();
            savedAccount.Status!.IsClosed.Should().BeFalse();
        }

        [Test]
        public async Task UpdateAsync_WhenAccountDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var dto = new AccountDto
            {
                Code = 1,
                PersonCode = 1,
                AccountNumber = "ACC-001"
            };

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.Code, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            // Act
            var result = await _sut.UpdateAsync(dto, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task UpdateAsync_WhenChangingPersonToNonExisting_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var existingAccount = CreateAccount(code: 1, personCode: 1, accountNumber: "ACC-001");

            var dto = new AccountDto
            {
                Code = 1,
                PersonCode = 2,
                AccountNumber = "ACC-001"
            };

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.Code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            _personRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.PersonCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Person?)null);

            // Act
            var act = async () => await _sut.UpdateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("The person does not exist.");
        }

        [Test]
        public async Task UpdateAsync_WhenChangingToExistingAccountNumber_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var existingAccount = CreateAccount(code: 1, personCode: 1, accountNumber: "OLD-ACC");

            var dto = new AccountDto
            {
                Code = 1,
                PersonCode = 1,
                AccountNumber = "NEW-ACC"
            };

            var conflictingAccount = CreateAccount(code: 2, personCode: 3, accountNumber: "NEW-ACC");

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.Code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            _accountRepositoryMock
                .Setup(r => r.GetByAccountNumberAsync(dto.AccountNumber, It.IsAny<CancellationToken>()))
                .ReturnsAsync(conflictingAccount);

            // Act
            var act = async () => await _sut.UpdateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("An account with the same account number already exists.");
        }

        [Test]
        public async Task CloseAsync_WhenAccountDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            const int code = 1;

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            // Act
            var result = await _sut.CloseAsync(code, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task CloseAsync_WhenBalanceIsNotZero_ShouldThrowInvalidOperationException()
        {
            // Arrange
            const int code = 1;

            var account = CreateAccount(
                code: code,
                personCode: 1,
                accountNumber: "ACC-001",
                outstandingBalance: 100m,
                isClosed: false);

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            var act = async () => await _sut.CloseAsync(code, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("The account cannot be closed because the balance is not zero.");
        }

        [Test]
        public async Task CloseAsync_WhenAlreadyClosed_ShouldThrowInvalidOperationException()
        {
            // Arrange
            const int code = 1;

            var account = CreateAccount(
                code: code,
                personCode: 1,
                accountNumber: "ACC-001",
                outstandingBalance: 0m,
                isClosed: true);

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            var act = async () => await _sut.CloseAsync(code, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("The account is already closed.");
        }

        [Test]
        public async Task CloseAsync_WhenValid_ShouldSetStatusClosedAndReturnDto()
        {
            // Arrange
            const int code = 1;

            var account = CreateAccount(
                code: code,
                personCode: 1,
                accountNumber: "ACC-001",
                outstandingBalance: 0m,
                isClosed: false);

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            Account? updatedAccount = null;

            _accountRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((a, _) => updatedAccount = a)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.CloseAsync(code, CancellationToken.None);

            // Assert – DTO values
            result.Should().NotBeNull();
            result!.Code.Should().Be(code);
            result.PersonCode.Should().Be(account.PersonCode);
            result.AccountNumber.Should().Be(account.AccountNumber);
            result.OutstandingBalance.Should().Be(account.OutstandingBalance);
            result.IsClosed.Should().BeTrue();

            // Assert – entity state
            updatedAccount.Should().NotBeNull();
            updatedAccount!.Status.Should().NotBeNull();
            updatedAccount.Status!.IsClosed.Should().BeTrue();
        }

        [Test]
        public async Task ReopenAsync_WhenAccountDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            const int code = 1;

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            // Act
            var result = await _sut.ReopenAsync(code, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task ReopenAsync_WhenAccountIsNotClosed_ShouldThrowInvalidOperationException()
        {
            // Arrange
            const int code = 1;

            var account = CreateAccount(
                code: code,
                personCode: 1,
                accountNumber: "ACC-001",
                outstandingBalance: 0m,
                isClosed: false);

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            var act = async () => await _sut.ReopenAsync(code, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("The account is not closed.");
        }

        [Test]
        public async Task ReopenAsync_WhenValid_ShouldSetStatusOpenAndReturnDto()
        {
            // Arrange
            const int code = 1;

            var account = CreateAccount(
                code: code,
                personCode: 1,
                accountNumber: "ACC-001",
                outstandingBalance: 0m,
                isClosed: true);

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            Account? updatedAccount = null;

            _accountRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((a, _) => updatedAccount = a)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.ReopenAsync(code, CancellationToken.None);

            // Assert – DTO values
            result.Should().NotBeNull();
            result!.Code.Should().Be(code);
            result.PersonCode.Should().Be(account.PersonCode);
            result.AccountNumber.Should().Be(account.AccountNumber);
            result.OutstandingBalance.Should().Be(account.OutstandingBalance);
            result.IsClosed.Should().BeFalse();

            // Assert – entity state
            updatedAccount.Should().NotBeNull();
            updatedAccount!.Status.Should().NotBeNull();
            updatedAccount.Status!.IsClosed.Should().BeFalse();
        }

        private static AccountDto CreateAccountDto(
            int personCode = 1,
            string accountNumber = "ACC-001")
        {
            return new AccountDto
            {
                PersonCode = personCode,
                AccountNumber = accountNumber
            };
        }

        private static Account CreateAccount(
            int code,
            int personCode,
            string accountNumber,
            decimal outstandingBalance = 0m,
            bool isClosed = false)
        {
            return new Account
            {
                Code = code,
                PersonCode = personCode,
                AccountNumber = accountNumber,
                OutstandingBalance = outstandingBalance,
                Status = new AccountStatus
                {
                    AccountCode = code,
                    IsClosed = isClosed
                }
            };
        }

        private static Person CreatePerson(int code, string idNumber = "123")
        {
            return new Person
            {
                Code = code,
                IdNumber = idNumber
            };
        }
    }
}
