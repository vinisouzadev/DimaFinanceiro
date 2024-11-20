using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;

namespace Dima.Core.Handlers
{
    public interface IReportHandler
    {
        Task<Response<List<ExpensesByCategory>?>> GetExpensesByCategoryAsync(GetExpensesByCategoryRequest request);

        Task<Response<List<IncomesByCategory>?>> GetIncomesByCategoryAsync(GetIncomesByCategoryRequest request);

        Task<Response<List<IncomesAndExpenses>?>> GetIncomesAndExpensesAsync(GetIncomesAndExpensesRequest request);

        Task<Response<FinancialSummary>> GetFinancialSummaryAsync(GetFinancialSummaryRequest request);
    }
}
