using Dima.API.Data;
using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;
using Dima.Core.Enums;

namespace Dima.API.Handlers
{
    public class ReportHandler(AppDbContext context) : IReportHandler
    {
        private readonly AppDbContext _context = context;

        public async Task<Response<List<ExpensesByCategory>?>> GetExpensesByCategoryAsync(GetExpensesByCategoryRequest request)
        {
            try
            {
                var data = await _context.ExpensesByCategories
                .Where(x => x.UserId == request.UserId)
                .AsNoTracking()
                .OrderByDescending(x => x.Year)
                .ThenBy(x => x.Category)
                .ToListAsync();
                return new Response<List<ExpensesByCategory>?>(data);
            }
            catch
            {
                return new Response<List<ExpensesByCategory>?>(null, 500, "Não foi possível identificar suas despesas.");
            }
            
        }

        public async Task<Response<FinancialSummary>> GetFinancialSummaryAsync(GetFinancialSummaryRequest request)
        {
            try
            {   
                var startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
                var data = await _context.Transactions
                    .AsNoTracking()
                    .Where(x => x.UserId == request.UserId && x.PaidOrReceivedAt >= startDateUtc && x.PaidOrReceivedAt <= DateTime.UtcNow)
                    .GroupBy(x => 1)
                    .Select
                        (x => new FinancialSummary
                                (   
                                    request.UserId,
                                    x.Where(t => t.Type == ETransactionType.Deposit).Sum(x => x.Amount),
                                    x.Where(t => t.Type == ETransactionType.Withdraw).Sum(x => x.Amount)
                                )
                        )
                    .FirstOrDefaultAsync();

                return new Response<FinancialSummary>(data);
            }
            catch
            {
                return new Response<FinancialSummary>(null, 500, "Não foi possível identificar seu sumário financeiro"); 
            }
        }

        public async Task<Response<List<IncomesAndExpenses>?>> GetIncomesAndExpensesAsync(GetIncomesAndExpensesRequest request)
        {
            try
            {
                var data = await _context.IncomesAndExpenses
                    .AsNoTracking()
                    .Where(x => x.UserId == request.UserId)
                    .OrderByDescending(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();
                return new Response<List<IncomesAndExpenses>?>(data);
            }
            catch
            {
                return new Response<List<IncomesAndExpenses>?>(null, 500, "Não foi possível identificar seu relatório de depositos e despesas");
            }
        }

        public async Task<Response<List<IncomesByCategory>?>> GetIncomesByCategoryAsync(GetIncomesByCategoryRequest request)
        {
            try
            {
                var data = await _context.IncomesByCategories
                    .AsNoTracking()
                    .Where(x => x.UserId == request.UserId)
                    .OrderByDescending(x => x.Year)
                    .ThenBy(x => x.Category)
                    .ToListAsync();

                return new Response<List<IncomesByCategory>?>(data);
            }
            catch
            {
                return new Response<List<IncomesByCategory>?>(null, 500, "Não foi possível identificar suas entradas");
            }
        }
    }
}
