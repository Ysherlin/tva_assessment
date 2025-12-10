using FluentAssertions;
using Moq;
using tva_assessment.Application.DTOs;
using tva_assessment.Application.Interfaces;
using tva_assessment.Application.Services;
using tva_assessment.Domain.Entities;

namespace tva_assessment.Test.Application.Services
{
    [TestFixture]
    public class TransactionServiceTests
    {
        private Mock<ITransactionRepository> _transactionRepositoryMock = null!;
        private Mock<IAccountRepository> _accountRepositoryMock = null!;
        private TransactionService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _transactionRepositoryMock = new Mock<ITransactionRepository>(MockBehavior.Strict);
            _accountRepositoryMock = new Mock<IAccountRepository>(MockBehavior.Strict);
            _sut = new TransactionService(_transactionRepositoryMock.Object, _accountRepositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _transactionRepositoryMock.VerifyAll();
            _accountRepositoryMock.VerifyAll();
        }

        [Test]
        public async Task CreateAsync_WhenAccountDoesNotExist_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = CreateTransactionDto();

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.AccountCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            // Act
            var act = async () => await _sut.CreateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("The account does not exist.");
        }

        [Test]
        public async Task CreateAsync_WhenAccountIsClosed_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = CreateTransactionDto();

            var account = CreateAccount(
                code: dto.AccountCode,
                accountNumber: "TestAccountNumber",
                outstandingBalance: 0m,
                isClosed: true);

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.AccountCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            var act = async () => await _sut.CreateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Transactions cannot be posted to a closed account.");
        }

        [Test]
        public async Task CreateAsync_WhenAmountIsZero_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = CreateTransactionDto(amount: 0m);

            var account = CreateAccount(
                code: dto.AccountCode,
                accountNumber: "TestAccountNumber",
                outstandingBalance: 0m,
                isClosed: false);

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.AccountCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            var act = async () => await _sut.CreateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("The transaction amount cannot be zero.");
        }

        [Test]
        public async Task CreateAsync_WhenTransactionDateInFuture_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var dto = CreateTransactionDto(
                transactionDate: DateTime.UtcNow.AddMinutes(5),
                amount: 100m,
                description: "Future trx");

            var account = CreateAccount(
                code: dto.AccountCode,
                accountNumber: "TestAccountNumber",
                outstandingBalance: 0m,
                isClosed: false);

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.AccountCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            // Act
            var act = async () => await _sut.CreateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("The transaction date cannot be in the future.");
        }

        [Test]
        public async Task CreateAsync_WhenValid_ShouldCreateTransaction_UpdateBalance_AndReturnDto()
        {
            // Arrange
            const int accountCode = 1;

            var dto = CreateTransactionDto(
                accountCode: accountCode,
                transactionDate: DateTime.UtcNow.AddMinutes(-10),
                amount: 150m,
                description: "Valid trx");

            var account = CreateAccount(
                code: accountCode,
                accountNumber: "TestAccountNumber",
                outstandingBalance: 0m,
                isClosed: false);

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(accountCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            var transactions = new List<Transaction>();

            _transactionRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .Callback<Transaction, CancellationToken>((t, _) =>
                {
                    t.Code = 10;
                    transactions.Add(t);
                })
                .Returns(Task.CompletedTask);

            _transactionRepositoryMock
                .Setup(r => r.GetByAccountCodeAsync(accountCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => transactions.ToList());

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(accountCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            Account? updatedAccount = null;

            _accountRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((a, _) => updatedAccount = a)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.CreateAsync(dto, CancellationToken.None);

            // Assert – DTO values
            result.Should().NotBeNull();
            result.Code.Should().Be(10);
            result.AccountCode.Should().Be(accountCode);
            result.TransactionDate.Should().Be(dto.TransactionDate);
            result.Amount.Should().Be(dto.Amount);
            result.Description.Should().Be(dto.Description);
            result.CaptureDate.Should().NotBe(default);

            // Assert – balance recalculation
            updatedAccount.Should().NotBeNull();
            updatedAccount!.OutstandingBalance.Should().Be(150m);
        }

        [Test]
        public async Task UpdateAsync_WhenTransactionDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var dto = CreateTransactionDto(
                code: 1,
                accountCode: 1,
                amount: 50m,
                description: "Update");

            _transactionRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.Code, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Transaction?)null);

            // Act
            var result = await _sut.UpdateAsync(dto, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task UpdateAsync_WhenChangingAccountCode_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var existing = CreateTransaction(
                code: 1,
                accountCode: 1,
                transactionDate: DateTime.UtcNow.AddDays(-1),
                captureDate: DateTime.UtcNow.AddDays(-1),
                amount: 100m,
                description: "Old");

            var dto = CreateTransactionDto(
                code: 1,
                accountCode: 2,
                transactionDate: DateTime.UtcNow,
                amount: 200m,
                description: "New");

            _transactionRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.Code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            // Act
            var act = async () => await _sut.UpdateAsync(dto, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Changing the account of a transaction is not allowed.");
        }

        [Test]
        public async Task UpdateAsync_WhenValid_ShouldUpdateFields_RecalculateBalance_AndReturnDto()
        {
            // Arrange
            const int accountCode = 1;

            var existing = CreateTransaction(
                code: 1,
                accountCode: accountCode,
                transactionDate: DateTime.UtcNow.AddDays(-2),
                captureDate: DateTime.UtcNow.AddDays(-2),
                amount: 50m,
                description: "Old");

            var dto = CreateTransactionDto(
                code: 1,
                accountCode: accountCode,
                transactionDate: DateTime.UtcNow.AddMinutes(-30),
                amount: 200m,
                description: "Updated");

            var transactions = new List<Transaction> { existing };

            _transactionRepositoryMock
                .Setup(r => r.GetByCodeAsync(dto.Code, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing);

            _transactionRepositoryMock
                .Setup(r => r.GetByAccountCodeAsync(accountCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => transactions.ToList());

            _transactionRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>()))
                .Callback<Transaction, CancellationToken>((t, _) =>
                {
                    var tx = transactions.Single(x => x.Code == t.Code);
                    tx.TransactionDate = t.TransactionDate;
                    tx.Amount = t.Amount;
                    tx.Description = t.Description;
                    tx.CaptureDate = t.CaptureDate;
                })
                .Returns(Task.CompletedTask);

            var account = CreateAccount(
                code: accountCode,
                accountNumber: "TestAccountNumber",
                outstandingBalance: 0m,
                isClosed: false);

            _accountRepositoryMock
                .Setup(r => r.GetByCodeAsync(accountCode, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            Account? updatedAccount = null;

            _accountRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()))
                .Callback<Account, CancellationToken>((a, _) => updatedAccount = a)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.UpdateAsync(dto, CancellationToken.None);

            // Assert – DTO values
            result.Should().NotBeNull();
            result!.Code.Should().Be(dto.Code);
            result.AccountCode.Should().Be(dto.AccountCode);
            result.TransactionDate.Should().Be(dto.TransactionDate);
            result.Amount.Should().Be(dto.Amount);
            result.Description.Should().Be(dto.Description);
            result.CaptureDate.Should().NotBe(default);

            // Assert – balance recalculation
            updatedAccount.Should().NotBeNull();
            updatedAccount!.OutstandingBalance.Should().Be(200m);
        }

        private static TransactionDto CreateTransactionDto(
            int code = 0,
            int accountCode = 1,
            DateTime? transactionDate = null,
            decimal amount = 100m,
            string description = "Test")
        {
            return new TransactionDto
            {
                Code = code,
                AccountCode = accountCode,
                TransactionDate = transactionDate ?? DateTime.UtcNow,
                Amount = amount,
                Description = description
            };
        }

        private static Account CreateAccount(
            int code,
            string accountNumber,
            decimal outstandingBalance,
            bool isClosed)
        {
            return new Account
            {
                Code = code,
                AccountNumber = accountNumber,
                OutstandingBalance = outstandingBalance,
                Status = new AccountStatus
                {
                    AccountCode = code,
                    IsClosed = isClosed
                }
            };
        }

        private static Transaction CreateTransaction(
            int code,
            int accountCode,
            DateTime transactionDate,
            DateTime captureDate,
            decimal amount,
            string description)
        {
            return new Transaction
            {
                Code = code,
                AccountCode = accountCode,
                TransactionDate = transactionDate,
                CaptureDate = captureDate,
                Amount = amount,
                Description = description
            };
        }
    }
}
