using Bogus;
using Dima.API.Data;
using Dima.API.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Reports;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Stripe.V2;
using System.Linq;

namespace Dima.APITestes.Tests.Handlers
{
    [Trait("Category","Handlers")]
    public class ReportHandlerTestes
    {
        private readonly Faker _faker = new("pt_BR");

        private readonly AppDbContext _context;

        private readonly ReportHandler _handler;

        public ReportHandlerTestes()
        {
            DbContextOptionsBuilder<AppDbContext> options = new();
            options.UseSqlite("Filename=:memory:");
            _context = new(options.Options);

            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _handler = new(_context);

           }

        [Fact]
        public async Task GetExpensesByCategoryAsync_DadoUmaListaDeDespesasNoBanco_EntaoDeveRetornarUmaRespostaDeSucessoComEssasDespesas()
        {
            _context.Database.ExecuteSqlRaw("CREATE VIEW \"vwGetExpensesByCategory\" AS " +
                "SELECT " +
                    "t.\"UserId\"," +
                    "c.\"Title\" as \"Category\", " +
                    "strftime('%Y', t.\"PaidOrReceivedAt\") AS \"Year\"," +
                    "SUM(t.\"Amount\") as \"Expenses\"" +
                "FROM \"Transaction\" AS t " +
                "JOIN \"Category\" AS c ON c.\"Id\" = t.\"CategoryId\" " +
                "WHERE t.\"PaidOrReceivedAt\" >= date('now', '-11 months')" +
                " AND t.\"PaidOrReceivedAt\" < date('now', '+1 month')" +
                " AND t.\"Type\" = 2 " +
                "GROUP BY t.\"UserId\", c.\"Title\", strftime('%Y', t.\"PaidOrReceivedAt\")");

            string firstCategoryTitle = _faker.Vehicle.Model();
            string secondCategoryTitle = _faker.Vehicle.Model();
            string thirdCategoryTitle = _faker.Vehicle.Model();
            string firstUserId = _faker.Person.FirstName;
            string secondUserId = _faker.Person.LastName;

            Category firstCategory = new()
            {
                Title = firstCategoryTitle,
                UserId = firstUserId
            };
            Category secondCategory = new()
            {
                Title = secondCategoryTitle,
                UserId = secondUserId
            };

            _context.Categories.Add(firstCategory);
            _context.Categories.Add(secondCategory);
            _context.SaveChanges();

            string transactionTitle = _faker.Vehicle.Model();
            DateTime transactionPaidOrReceivedAt = _faker.Date.Recent();
            decimal transactionAmount = _faker.Random.Decimal(-1000,-1);

            Transaction transaction = new() 
            {
                Title = transactionTitle,
                PaidOrReceivedAt = transactionPaidOrReceivedAt,
                Amount = transactionAmount,
                CategoryId = firstCategory.Id,
                CreatedAt = DateTime.UtcNow,
                Category = firstCategory,
                UserId = firstUserId
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            GetExpensesByCategoryRequest request = new() 
            {
                UserId = firstUserId
            };

            var result = await _handler.GetExpensesByCategoryAsync(request);
            _context.ChangeTracker.Clear();

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeEmpty();
            result.Data!.All(x => x.UserId == firstUserId).Should().BeTrue();

        }

        [Fact]
        public async Task GetFinancialSummaryAsync_DadoUmaListaDeTransacoesDeUmUsuarioIncorreto_EntaoDeveRetornarUmaRespostaComPropriedadeDataComoNulo()
        {
            string firstCategoryTitle = _faker.Vehicle.Model();
            string secondCategoryTitle = _faker.Vehicle.Model();
            string firstUserId = _faker.Person.FirstName;
            string secondUserId = _faker.Person.LastName;

            Category firstCategory = new()
            {
                Title = firstCategoryTitle,
                UserId = firstUserId
            };
            Category secondCategory = new()
            {
                Title = secondCategoryTitle,
                UserId = secondUserId
            };

            _context.Categories.Add(firstCategory);
            _context.Categories.Add(secondCategory);
            _context.SaveChanges();

            string transactionTitle = _faker.Vehicle.Model();
            DateTime transactionPaidOrReceivedAt = _faker.Date.Recent();
            decimal transactionAmount = _faker.Random.Decimal(-1000, -1);

            Transaction transaction = new()
            {
                Title = transactionTitle,
                PaidOrReceivedAt = transactionPaidOrReceivedAt,
                Amount = transactionAmount,
                CategoryId = firstCategory.Id,
                CreatedAt = DateTime.UtcNow,
                Category = firstCategory,
                UserId = firstUserId
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            string incorrectlyUserId = _faker.Person.FullName;
            GetFinancialSummaryRequest request = new() 
            {
                UserId = incorrectlyUserId
            };

            var result = await _handler.GetFinancialSummaryAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task GetFinancialSummaryAsync_DadoUmaTransacaoComUserIdCorreta_EntaoDeveRetornarUmaRespostaDeSucessoComEssaTransacao()
        {
            string firstCategoryTitle = _faker.Vehicle.Model();
            string secondCategoryTitle = _faker.Vehicle.Model();
            string firstUserId = _faker.Person.FirstName;
            string secondUserId = _faker.Person.LastName;

            Category firstCategory = new()
            {
                Title = firstCategoryTitle,
                UserId = firstUserId
            };
            Category secondCategory = new()
            {
                Title = secondCategoryTitle,
                UserId = secondUserId
            };

            _context.Categories.Add(firstCategory);
            _context.Categories.Add(secondCategory);
            _context.SaveChanges();

            string transactionTitle = _faker.Vehicle.Model();
            DateTime transactionPaidOrReceivedAt = _faker.Date.Recent();
            decimal transactionAmount = _faker.Random.Decimal(-1000, -1);

            Transaction transaction = new()
            {
                Title = transactionTitle,
                PaidOrReceivedAt = transactionPaidOrReceivedAt,
                Amount = transactionAmount,
                CategoryId = firstCategory.Id,
                CreatedAt = DateTime.UtcNow,
                Category = firstCategory,
                UserId = firstUserId
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            string incorrectlyUserId = _faker.Person.FullName;
            GetFinancialSummaryRequest request = new()
            {
                UserId = firstUserId
            };

            var result = await _handler.GetFinancialSummaryAsync(request);

            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetIncomesAndExpensesAsync_DadoUmUserIdIncorreto_EntaoDeveRetornarUmaRespostaComDataSendoNull()
        {
            _context.Database.ExecuteSqlRaw("CREATE VIEW \"vwGetIncomesAndExpenses\" AS " +
                "SELECT " +
                    "t.\"UserId\"," +
                    "strftime('%M', t.\"PaidOrReceivedAt\") AS \"Month\"," +
                    "strftime('%Y', t.\"PaidOrReceivedAt\") AS \"Year\"," +
                    "SUM (CASE WHEN t.\"Type\" = 1 THEN t.\"Amount\" ELSE 0 END) AS \"Incomes\"," +
                    "SUM (CASE WHEN t.\"Type\" = 2 THEN t.\"Amount\" ELSE 0 END) AS \"Expenses\"" + 
                 "FROM \"Transaction\" AS t " +
                 "JOIN \"Category\" AS c ON c.\"Id\" = t.\"CategoryId\" " +
                 "WHERE t.\"PaidOrReceivedAt\" >= date('now', '-11 months') AND t.\"PaidOrReceivedAt\" < date('now', '+1 month') " +
                 "GROUP BY t.\"UserId\", strftime('%Y', t.\"PaidOrReceivedAt\"), strftime('%M', t.\"PaidOrReceivedAt\")"
                   );

            string firstCategoryTitle = _faker.Vehicle.Model();
            string firstUserId = _faker.Person.FirstName;

            Category firstCategory = new()
            {
                Title = firstCategoryTitle,
                UserId = firstUserId
            };

            _context.Categories.Add(firstCategory);
            _context.SaveChanges();

            string firstTransactionTitle = _faker.Vehicle.Model();
            DateTime firstTransactionPaidOrReceivedAt = _faker.Date.Recent();
            decimal incomes = _faker.Random.Decimal(1, 1000);

            Transaction firstTransaction = new()
            {
                Title = firstTransactionTitle,
                PaidOrReceivedAt = firstTransactionPaidOrReceivedAt,
                Amount = incomes,
                CategoryId = firstCategory.Id,
                CreatedAt = DateTime.UtcNow,
                Category = firstCategory,
                UserId = firstUserId
            };

            string secondTransactionTitle = _faker.Vehicle.Model();
            DateTime secondTransactionPaidOrReceivedAt = _faker.Date.Recent();
            decimal expenses = _faker.Random.Decimal(-1000, -1);

            Transaction secondTransaction = new()
            {
                Title = secondTransactionTitle,
                PaidOrReceivedAt = secondTransactionPaidOrReceivedAt,
                Amount = expenses,
                CategoryId = firstCategory.Id,
                Category = firstCategory,
                UserId = firstUserId

            };

            _context.Transactions.Add(firstTransaction);
            _context.SaveChanges();

            string incorrectlyUserId = _faker.Person.LastName;
            GetIncomesAndExpensesRequest request = new()
            {
                UserId = incorrectlyUserId
            };

            var result = await _handler.GetIncomesAndExpensesAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetIncomesAndExpenses_DadoUmaListaDeTransacoesComUserIdCorreto_EntaoDeveRetornarUmaRespostaDeSucessoComALista()
        {
            _context.Database.ExecuteSqlRaw("CREATE VIEW \"vwGetIncomesAndExpenses\" AS " +
                "SELECT " +
                    "t.\"UserId\"," +
                    "strftime('%M', t.\"PaidOrReceivedAt\") AS \"Month\"," +
                    "strftime('%Y', t.\"PaidOrReceivedAt\") AS \"Year\"," +
                    "SUM (CASE WHEN t.\"Type\" = 1 THEN t.\"Amount\" ELSE 0 END) AS \"Incomes\"," +
                    "SUM (CASE WHEN t.\"Type\" = 2 THEN t.\"Amount\" ELSE 0 END) AS \"Expenses\"" +
                 "FROM \"Transaction\" AS t " +
                 "JOIN \"Category\" AS c ON c.\"Id\" = t.\"CategoryId\" " +
                 "WHERE t.\"PaidOrReceivedAt\" >= date('now', '-11 months') AND t.\"PaidOrReceivedAt\" < date('now', '+1 month') " +
                 "GROUP BY t.\"UserId\", strftime('%Y', t.\"PaidOrReceivedAt\"), strftime('%M', t.\"PaidOrReceivedAt\")"
                   );

            string firstCategoryTitle = _faker.Vehicle.Model();
            string firstUserId = _faker.Person.FirstName;

            Category firstCategory = new()
            {
                Title = firstCategoryTitle,
                UserId = firstUserId
            };

            _context.Categories.Add(firstCategory);
            _context.SaveChanges();

            string firstTransactionTitle = _faker.Vehicle.Model();
            DateTime firstTransactionPaidOrReceivedAt = _faker.Date.Recent();
            decimal incomes = _faker.Random.Decimal(1, 1000);

            Transaction firstTransaction = new()
            {
                Title = firstTransactionTitle,
                PaidOrReceivedAt = firstTransactionPaidOrReceivedAt,
                Amount = incomes,
                CategoryId = firstCategory.Id,
                CreatedAt = DateTime.UtcNow,
                Category = firstCategory,
                UserId = firstUserId
            };

            string secondTransactionTitle = _faker.Vehicle.Model();
            DateTime secondTransactionPaidOrReceivedAt = _faker.Date.Recent();
            decimal expenses = _faker.Random.Decimal(-1000, -1);

            Transaction secondTransaction = new()
            {
                Title = secondTransactionTitle,
                PaidOrReceivedAt = secondTransactionPaidOrReceivedAt,
                Amount = expenses,
                CategoryId = firstCategory.Id,
                Category = firstCategory,
                UserId = firstUserId

            };

            _context.Transactions.Add(firstTransaction);
            _context.SaveChanges();

            string incorrectlyUserId = _faker.Person.LastName;
            GetIncomesAndExpensesRequest request = new()
            {
                UserId = firstUserId
            };

            var result = await _handler.GetIncomesAndExpensesAsync(request);

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data!.All(x => x.UserId == firstUserId).Should().BeTrue();
            result.Data.Should().NotBeEmpty();

        }

        [Fact]
        public async Task GetIncomesByCategoryAsync_DadoUmUserIdIncorreto_EntaoDeveRetornarUmaMensagemDeSucessoComUmaListaVazia()
        {
            _context.Database.ExecuteSqlRaw("CREATE VIEW \"vwGetExpensesByCategory\" AS " +
               "SELECT " +
                   "t.\"UserId\"," +
                   "c.\"Title\" as \"Category\", " +
                   "strftime('%Y', t.\"PaidOrReceivedAt\") AS \"Year\"," +
                   "SUM(t.\"Amount\") as \"Expenses\"" +
               "FROM \"Transaction\" AS t " +
               "JOIN \"Category\" AS c ON c.\"Id\" = t.\"CategoryId\" " +
               "WHERE t.\"PaidOrReceivedAt\" >= date('now', '-11 months')" +
               " AND t.\"PaidOrReceivedAt\" < date('now', '+1 month')" +
               " AND t.\"Type\" = 2 " +
               "GROUP BY t.\"UserId\", c.\"Title\", strftime('%Y', t.\"PaidOrReceivedAt\")");

            string firstCategoryTitle = _faker.Vehicle.Model();
            string secondCategoryTitle = _faker.Vehicle.Model();
            string firstUserId = _faker.Person.FirstName;
            string secondUserId = _faker.Person.LastName;

            Category firstCategory = new()
            {
                Title = firstCategoryTitle,
                UserId = firstUserId
            };

            _context.Categories.Add(firstCategory);
            _context.SaveChanges();

            string transactionTitle = _faker.Vehicle.Model();
            DateTime transactionPaidOrReceivedAt = _faker.Date.Recent();
            decimal transactionAmount = _faker.Random.Decimal(-1000, -1);

            Transaction transaction = new()
            {
                Title = transactionTitle,
                PaidOrReceivedAt = transactionPaidOrReceivedAt,
                Amount = transactionAmount,
                CategoryId = firstCategory.Id,
                CreatedAt = DateTime.UtcNow,
                Category = firstCategory,
                UserId = firstUserId
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            string incorrectlyUserId = _faker.Person.UserName;
            GetExpensesByCategoryRequest request = new()
            {
                UserId = incorrectlyUserId
            };

            var result = await _handler.GetExpensesByCategoryAsync(request);
            _context.ChangeTracker.Clear();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task GetIncomesByCategoryAsync_DadoUmUserIdCorreto_EntaoDeveRetornarUmaMensagemDeSucessoComUmaListaDeIncomes()
        {
            _context.Database.ExecuteSqlRaw("CREATE VIEW \"vwGetExpensesByCategory\" AS " +
               "SELECT " +
                   "t.\"UserId\"," +
                   "c.\"Title\" as \"Category\", " +
                   "strftime('%Y', t.\"PaidOrReceivedAt\") AS \"Year\"," +
                   "SUM(t.\"Amount\") as \"Expenses\"" +
               "FROM \"Transaction\" AS t " +
               "JOIN \"Category\" AS c ON c.\"Id\" = t.\"CategoryId\" " +
               "WHERE t.\"PaidOrReceivedAt\" >= date('now', '-11 months')" +
               " AND t.\"PaidOrReceivedAt\" < date('now', '+1 month')" +
               " AND t.\"Type\" = 2 " +
               "GROUP BY t.\"UserId\", c.\"Title\", strftime('%Y', t.\"PaidOrReceivedAt\")");

            string firstCategoryTitle = _faker.Vehicle.Model();
            string secondCategoryTitle = _faker.Vehicle.Model();
            string firstUserId = _faker.Person.FirstName;
            string secondUserId = _faker.Person.LastName;

            Category firstCategory = new()
            {
                Title = firstCategoryTitle,
                UserId = firstUserId
            };

            _context.Categories.Add(firstCategory);
            _context.SaveChanges();

            string transactionTitle = _faker.Vehicle.Model();
            DateTime transactionPaidOrReceivedAt = _faker.Date.Recent();
            decimal transactionAmount = _faker.Random.Decimal(-1000, -1);

            Transaction transaction = new()
            {
                Title = transactionTitle,
                PaidOrReceivedAt = transactionPaidOrReceivedAt,
                Amount = transactionAmount,
                CategoryId = firstCategory.Id,
                CreatedAt = DateTime.UtcNow,
                Category = firstCategory,
                UserId = firstUserId
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            GetExpensesByCategoryRequest request = new()
            {
                UserId = firstUserId
            };

            var result = await _handler.GetExpensesByCategoryAsync(request);
            _context.ChangeTracker.Clear();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeEmpty();
            result.Data!.All(x => x.UserId == firstUserId).Should().BeTrue();
        }
    }
}
