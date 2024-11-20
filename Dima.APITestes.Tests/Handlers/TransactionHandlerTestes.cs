using Bogus;
using Dima.API.Data;
using Dima.API.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Dima.APITestes.Tests.Handlers
{
    [Trait("Category", "Handlers")]
    public class TransactionHandlerTestes
    {
        private readonly Faker _faker = new("pt_BR");

        private readonly AppDbContext _context;

        private readonly TransactionHandler _handler;

        public TransactionHandlerTestes()
        {
            DbContextOptionsBuilder<AppDbContext> options = new();
            options.UseSqlite("Filename=:memory:");
            _context = new(options.Options);

            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _handler = new(_context);
        }

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_DadoUmaCriacaoDeTransacaoDeDespesa_EntaoDeveRetornarUmaRespostaDeSucessoComTransactionAmountCorreto()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Manufacturer();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };
            
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            long categoryId = category.Id;
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            CreateTransactionRequest request = new() 
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                Amount = transactionAmount,
                Type = Core.Enums.ETransactionType.Withdraw,
                PaidOrReceivedAt = paidOrReceivedAt
            };

            decimal expectedTransactionAmount = transactionAmount;
            if (transactionAmount >= 0)
                expectedTransactionAmount = -transactionAmount;

            var result = await _handler.CreateAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Title.Should().Be(transactionTitle);
            result.Data.Amount.Should().Be(expectedTransactionAmount);

        }

        [Fact]
        public async Task CreateAsync_DadoUmaCriacaoDeTransacaoDeDeposito_EntaoDeveRetornarUmaRespostaDeSucessoComTransactionAmountCorreto()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Manufacturer();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            long categoryId = category.Id;
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            CreateTransactionRequest request = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                Amount = transactionAmount,
                Type = Core.Enums.ETransactionType.Deposit,
                PaidOrReceivedAt = paidOrReceivedAt
            };

            decimal expectedTransactionAmount = transactionAmount;
            if (transactionAmount <= 0)
                expectedTransactionAmount = -transactionAmount;

            var result = await _handler.CreateAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Title.Should().Be(transactionTitle);
            result.Data.Amount.Should().Be(expectedTransactionAmount);
        }

        [Fact]
        public async Task CreateAsync_DadoUmaCriacaoDeTransacao_EntaoDeveSalvarNoBancoCorretamente()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Manufacturer();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            long categoryId = category.Id;
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            CreateTransactionRequest request = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                Amount = transactionAmount,
                Type = Core.Enums.ETransactionType.Deposit,
                PaidOrReceivedAt = paidOrReceivedAt
            };

            decimal expectedTransactionAmount = transactionAmount;
            if (transactionAmount <= 0)
                expectedTransactionAmount = -transactionAmount;

            var result = await _handler.CreateAsync(request);

            Transaction? createdTransaction = _context.Transactions.SingleOrDefault(t => t.Id == result.Data.Id);


            createdTransaction.Should().NotBeNull();
            createdTransaction!.Title.Should().Be(transactionTitle);
            createdTransaction.Amount.Should().Be(expectedTransactionAmount);
        }


        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_DadoUmIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Manufacturer();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            long categoryId = category.Id;
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                PaidOrReceivedAt = paidOrReceivedAt,
                Amount = transactionAmount,
                Type = Core.Enums.ETransactionType.Deposit
            };

            _context.Add(transaction);
            await _context.SaveChangesAsync();

            long incorrectlyId = _faker.Random.Long(-10, -1);

            DeleteTransactionRequest request = new()
            {
                Id = incorrectlyId,
                UserId = userId
            };

            var result = await _handler.DeleteAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível identificar essa transação");
        }

        [Fact]
        public async Task DeleteAsync_DadoUserIdIncorretoNoRequest_EntaoDeveRetornarRespostaDeFalha()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Manufacturer();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            long categoryId = category.Id;
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                PaidOrReceivedAt = paidOrReceivedAt,
                Amount = transactionAmount,
                Type = Core.Enums.ETransactionType.Deposit
            };

            _context.Add(transaction);
            await _context.SaveChangesAsync();

            long correctlyId = transaction.Id;
            string incorrectlyUserId = _faker.Person.LastName;

            DeleteTransactionRequest request = new()
            {
                UserId = incorrectlyUserId,
                Id = correctlyId
            };

            var result = await _handler.DeleteAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível identificar essa transação");
        }

        [Fact]
        public async Task DeleteAsync_DadoUmIdEUserIdCorretoDeUmaTransacao_EntaoDeveDeletarCorretamenteNoBanco()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Manufacturer();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            long categoryId = category.Id;
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                PaidOrReceivedAt = paidOrReceivedAt,
                Amount = transactionAmount,
                Type = Core.Enums.ETransactionType.Deposit
            };

            _context.Add(transaction);
            await _context.SaveChangesAsync();

            DeleteTransactionRequest request = new()
            {
                UserId = transaction.UserId,
                Id = transaction.Id
            };

            var result = await _handler.DeleteAsync(request);

            Transaction? deletedTransaction = _context.Transactions.SingleOrDefault(t => t.Id == transaction.Id);

            deletedTransaction.Should().BeNull();
           
        }

        [Fact]
        public async Task DeleteAsync_DadoUmIdEUserIdCorretoDeUmaTransacao_EntaoDeveRetornarUmaRespostaDeSucesso()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Manufacturer();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            long categoryId = category.Id;
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                PaidOrReceivedAt = paidOrReceivedAt,
                Amount = transactionAmount,
                Type = Core.Enums.ETransactionType.Deposit
            };

            _context.Add(transaction);
            await _context.SaveChangesAsync();

            DeleteTransactionRequest request = new()
            {
                UserId = transaction.UserId,
                Id = transaction.Id
            };

            var result = await _handler.DeleteAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeNull();
        }

        #endregion

        #region GetByIdAsync 

        [Fact]
        public async Task GetByIdAsync_DadoUmIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Manufacturer();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            long categoryId = category.Id;
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                PaidOrReceivedAt = paidOrReceivedAt,
                Amount = transactionAmount,
                Type = Core.Enums.ETransactionType.Deposit
            };

            _context.Add(transaction);
            await _context.SaveChangesAsync();

            long incorrectlyId = _faker.Random.Long(-10, -1);

            GetByIdTransactionRequest request = new()
            {
                UserId = transaction.UserId,
                Id = incorrectlyId
            };

            var result = await _handler.GetByIdAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível encontrar essa transação");
        }

        [Fact]
        public async Task GetByIdAsync_DadoUserIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Manufacturer();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            long categoryId = category.Id;
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                PaidOrReceivedAt = paidOrReceivedAt,
                Amount = transactionAmount,
                Type = Core.Enums.ETransactionType.Deposit
            };

            _context.Add(transaction);
            await _context.SaveChangesAsync();

            string incorrectlyUserId = _faker.Person.LastName;

            GetByIdTransactionRequest request = new()
            {
                UserId = incorrectlyUserId,
                Id = transaction.Id
            };

            var result = await _handler.GetByIdAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível encontrar essa transação");
        }

        [Fact]
        public async Task GetByIdAsync_DadoUmIdEUserIdCorretosDeUmaTransacao_EntaoDeveRetornarUmaRespostaDeSucessoComATransacao()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Manufacturer();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            long categoryId = category.Id;
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                PaidOrReceivedAt = paidOrReceivedAt,
                Amount = transactionAmount,
                Type = Core.Enums.ETransactionType.Deposit
            };

            _context.Add(transaction);
            await _context.SaveChangesAsync();

            GetByIdTransactionRequest request = new()
            {
                UserId = transaction.UserId,
                Id = transaction.Id
            };

            var result = await _handler.GetByIdAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Title.Should().Be(transactionTitle);
            result.Data.Amount.Should().Be(transactionAmount);
            result.Data.Id.Should().Be(transaction.Id);
        }

        #endregion

        #region GetByPeriodAsync

        [Fact]
        public async Task GetByPeriodAsync_DadoUmPaidOrReceivedAtMenorQueStartDate_EntaoDeveRetornarUmaRespostaComTransactionNulo()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Model();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Manufacturer();
            decimal transactionAmount = _faker.Random.Decimal();
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            long categoryId = category.Id;
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                Amount = transactionAmount,
                PaidOrReceivedAt = paidOrReceivedAt
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            DateTime startDate = paidOrReceivedAt.AddDays(+1);
            DateTime endDate = paidOrReceivedAt.AddDays(+1);
            GetByPeriodTransactionRequest request = new()
            {
                UserId = transaction.UserId,
                StartDate = startDate,
                EndDate = endDate

            };

            var result = await _handler.GetByPeriodAsync(request);

            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();

        }

        [Fact]
        public async Task GetByPeriodAsync_DadoUmPaidOrReceivedAtMaiorQueEndDate_EntaoDeveRetornarUmaRespostaComTransactionNulo()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Model();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Manufacturer();
            decimal transactionAmount = _faker.Random.Decimal();
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            long categoryId = category.Id;
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                Amount = transactionAmount,
                PaidOrReceivedAt = paidOrReceivedAt
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            DateTime startDate = paidOrReceivedAt.AddDays(-1);
            DateTime endDate = paidOrReceivedAt.AddDays(-1);
            GetByPeriodTransactionRequest request = new()
            {
                UserId = transaction.UserId,
                StartDate = startDate,
                EndDate = endDate

            };

            var result = await _handler.GetByPeriodAsync(request);

            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByPeriodAsync_DadoUmUserIdIncorreto_EntaoDeveRetornarUmaRespostaComTransactionNulo()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Model();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Manufacturer();
            decimal transactionAmount = _faker.Random.Decimal();
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            long categoryId = category.Id;
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                Amount = transactionAmount,
                PaidOrReceivedAt = paidOrReceivedAt
            };
            
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            string incorrectlyUserId = _faker.Person.LastName;

            DateTime startDate = paidOrReceivedAt.AddDays(-1);
            DateTime endDate = paidOrReceivedAt.AddDays(+1);
            GetByPeriodTransactionRequest request = new()
            {
                UserId = incorrectlyUserId,
                StartDate = startDate,
                EndDate = endDate

            };

            var result = await _handler.GetByPeriodAsync(request);

            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByPeriodAsync_DadoUmaTransacaoComStartDateEEndDateNulo_EntaoDeveRetornarUmaRespostaComTransactionNulo()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Model();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Manufacturer();
            decimal transactionAmount = _faker.Random.Decimal();
            DateTime paidOrReceivedAt = _faker.Date.Recent();
            long categoryId = category.Id;
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                CategoryId = categoryId,
                Amount = transactionAmount,
                PaidOrReceivedAt = paidOrReceivedAt
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            GetByPeriodTransactionRequest request = new()
            {
                UserId = userId
            };

            var result = await _handler.GetByPeriodAsync(request);

            result.Should().NotBeNull();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByPeriodAsync_DadoUmPeriodoValido_EntaoDeveRetornarTodasAsTransacoesDentroDessePeriodo()
        {
            string userId = _faker.Person.FirstName;

            string categoryTitle = _faker.Vehicle.Model();
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string firstTransactionTitle = _faker.Vehicle.Manufacturer();
            decimal firstTransactionAmount = _faker.Random.Decimal();
            DateTime firstTransactioPaidOrReceivedAt = _faker.Date.Recent();
            Transaction firstTransaction = new()
            {
                Title = firstTransactionTitle,
                UserId = userId,
                CategoryId = category.Id,
                Amount = firstTransactionAmount,
                PaidOrReceivedAt = firstTransactioPaidOrReceivedAt
            };

            string secondTransactionTitle = _faker.Vehicle.Fuel();
            decimal secondTransactionAmount = _faker.Random.Decimal();
            DateTime secondTransactionPaidOrReceivedAt = _faker.Date.Recent();
            Transaction secondTransaction = new()
            {
                Title = secondTransactionTitle,
                UserId = userId,
                CategoryId = category.Id,
                Amount = secondTransactionAmount,
                PaidOrReceivedAt = secondTransactionPaidOrReceivedAt,
                
            };
            
            _context.Transactions.Add(firstTransaction);
            _context.Transactions.Add(secondTransaction);
            await _context.SaveChangesAsync();

            DateTime startDate = firstTransactioPaidOrReceivedAt.AddDays(-1);
            DateTime endDate = firstTransactioPaidOrReceivedAt.AddDays(1);

            GetByPeriodTransactionRequest request = new()
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            var result = await _handler.GetByPeriodAsync(request);

            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.Should().NotBeEmpty();
            result.Data!.SingleOrDefault(x => x.Id == firstTransaction.Id);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_DadoUmIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string userId = _faker.Person.FullName;

            string categoryTitle = _faker.Person.FirstName;
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            DateTime transactionPaidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                Amount = transactionAmount,
                CategoryId = category.Id,
                PaidOrReceivedAt = transactionPaidOrReceivedAt
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            long incorrectlyId = _faker.Random.Long(-10, -1);
            UpdateTransactionRequest request = new()
            {
                Id = incorrectlyId,
                UserId = userId
            };

            var result = await _handler.UpdateAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível identificar essa transação");
        }

        [Fact]
        public async Task UpdateAsync_DadoUmUserIdIncorretoNoRequest_EntaoDeveRetornarUmaRespostaDeFalha()
        {
            string userId = _faker.Person.FullName;

            string categoryTitle = _faker.Person.FirstName;
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            DateTime transactionPaidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                Amount = transactionAmount,
                CategoryId = category.Id,
                PaidOrReceivedAt = transactionPaidOrReceivedAt
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            string incorrectlyUserId = _faker.Person.LastName;
            UpdateTransactionRequest request = new()
            {
                Id = transaction.Id,
                UserId = incorrectlyUserId
            };

            var result = await _handler.UpdateAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Não foi possível identificar essa transação");
        }

        [Fact]
        public async Task UpdateAsync_DadoUmRequestComPropriedadesNulas_EntaoDeveGerarUmaRespostaDeFalha()
        {
            string userId = _faker.Person.FullName;

            string categoryTitle = _faker.Person.FirstName;
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            DateTime transactionPaidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                Amount = transactionAmount,
                CategoryId = category.Id,
                PaidOrReceivedAt = transactionPaidOrReceivedAt
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            UpdateTransactionRequest request = new()
            {
                Id = transaction.Id,
                UserId = userId
            };

            var result = await _handler.UpdateAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.Message.Should().Be("Houve um erro ao atualizar a transação");
        }

        [Fact]
        public async Task UpdateAsync_DadoUmRequestComPropriedadesAtualizadas_EntaoDeveAtualizarATransacaoNoBanco()
        {
            string userId = _faker.Person.FullName;

            string categoryTitle = _faker.Person.FirstName;
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            DateTime transactionPaidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                Amount = transactionAmount,
                CategoryId = category.Id,
                PaidOrReceivedAt = transactionPaidOrReceivedAt
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            string updatedTransactionTitle = _faker.Random.AlphaNumeric(10);
            decimal updatedTransactionAmount = _faker.Random.Decimal();
            DateTime updatedTransactionPaidOrReceivedAt = _faker.Date.Recent();
            UpdateTransactionRequest request = new()
            {
                Id = transaction.Id,
                UserId = userId,
                Amount = updatedTransactionAmount,
                CategoryId = category.Id,
                PaidOrReceivedAt = updatedTransactionPaidOrReceivedAt,
                Type = transaction.Type,
                Title = updatedTransactionTitle
            };

            var result = await _handler.UpdateAsync(request);
            _context.ChangeTracker.Clear();
            Transaction? updatedTransaction = _context.Transactions.SingleOrDefault(x => x.Id == transaction.Id);
            

            updatedTransaction.Should().NotBeNull();
            updatedTransaction!.Title.Should().Be(updatedTransaction.Title);
            updatedTransaction.Amount.Should().Be(updatedTransactionAmount);
        }

        [Fact]
        public async Task UpdateAsync_DadoUmRequestComPropriedadesAtualizadas_EntaoDeveRetornarUmaRespostaDeSucesso()
        {
            string userId = _faker.Person.FullName;

            string categoryTitle = _faker.Person.FirstName;
            Category category = new()
            {
                Title = categoryTitle,
                UserId = userId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            string transactionTitle = _faker.Vehicle.Model();
            decimal transactionAmount = _faker.Random.Decimal();
            DateTime transactionPaidOrReceivedAt = _faker.Date.Recent();
            Transaction transaction = new()
            {
                Title = transactionTitle,
                UserId = userId,
                Amount = transactionAmount,
                CategoryId = category.Id,
                PaidOrReceivedAt = transactionPaidOrReceivedAt
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
             
            string updatedTransactionTitle = _faker.Random.AlphaNumeric(10);
            decimal updatedTransactionAmount = _faker.Random.Decimal();
            DateTime updatedTransactionPaidOrReceivedAt = _faker.Date.Recent();
            UpdateTransactionRequest request = new()
            {
                Id = transaction.Id,
                UserId = userId,
                Amount = updatedTransactionAmount,
                CategoryId = category.Id,
                PaidOrReceivedAt = updatedTransactionPaidOrReceivedAt,
                Type = transaction.Type,
                Title = updatedTransactionTitle
            };

            var result = await _handler.UpdateAsync(request);
            _context.ChangeTracker.Clear();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Id.Should().Be(transaction.Id);
            result.Data.Title.Should().Be(updatedTransactionTitle);
            result.Data.Amount.Should().Be(updatedTransactionAmount);
        }

        #endregion

    }
}
