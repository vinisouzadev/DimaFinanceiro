using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using Dima.Web.Common;
using System.Net.Http.Json;

namespace Dima.Web.Handlers
{
    public class ReportHandler(IHttpClientFactory clientFactory) : IReportHandler
    {
        private readonly HttpClient _client = clientFactory.CreateClient(Configuration.HttpClientName);

        public async Task<Response<List<ExpensesByCategory>?>> GetExpensesByCategoryAsync(GetExpensesByCategoryRequest request)
        {
            var result = await _client.GetFromJsonAsync<Response<List<ExpensesByCategory>?>>("v1/reports/expenses");

            return result.IsSuccess
                ? new Response<List<ExpensesByCategory>?>(result.Data, 200, "Despesas retornadas com sucesso!")
                : new Response<List<ExpensesByCategory>?>(null, 400, "Não foi possível identificar suas despesas por categoria.");
        }

        public async Task<Response<FinancialSummary>> GetFinancialSummaryAsync(GetFinancialSummaryRequest request)
        {
            var result = await _client.GetFromJsonAsync<Response<FinancialSummary>>("v1/reports/financial-summary");

            return result.IsSuccess
                ? new Response<FinancialSummary>(result.Data, 200, "Sumário financeiro retornado com sucesso!")
                : new Response<FinancialSummary>(null, 400, "Não foi possível identificar seu sumário financeiro");
        }

        public async Task<Response<List<IncomesAndExpenses>?>> GetIncomesAndExpensesAsync(GetIncomesAndExpensesRequest request)
        {
            var result = await _client.GetFromJsonAsync<Response<List<IncomesAndExpenses>?>>("v1/reports/incomes-expenses");

            return result.IsSuccess
                ? new Response<List<IncomesAndExpenses>?>(result.Data, 200, "Depósitos e despesas retornados com sucesso!")
                : new Response<List<IncomesAndExpenses>?>(null, 400, "Não foi possível identificar seus depósitos e despesas");
        }

        public async Task<Response<List<IncomesByCategory>?>> GetIncomesByCategoryAsync(GetIncomesByCategoryRequest request)
        {
            var result = await _client.GetFromJsonAsync<Response<List<IncomesByCategory>?>>("v1/reports/incomes");

            return result.IsSuccess
                ? new Response<List<IncomesByCategory>?>(result.Data, 200, "Depósitos retornados com sucesso!")
                : new Response<List<IncomesByCategory>?>(null, 400, "Não foi possível identificar seus depósitos");
        }
    }
}
