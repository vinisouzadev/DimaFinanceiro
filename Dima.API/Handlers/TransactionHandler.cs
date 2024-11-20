using Dima.API.Data;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transactions;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;
using Dima.Core.Enums;

namespace Dima.API.Handlers
{
    public class TransactionHandler : ITransactionHandler
    {
        private readonly AppDbContext _context;

        public TransactionHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
        {
            if (request is { Amount: >= 0, Type: ETransactionType.Withdraw })
                request.Amount *= -1;
            else if (request is { Amount: <= 0, Type: ETransactionType.Deposit })
                request.Amount *= -1;

            try
            {
                request.PaidOrReceivedAt = DateTime.SpecifyKind((DateTime)request.PaidOrReceivedAt, DateTimeKind.Utc);
                var transaction =  new Transaction()
                {
                    Title = request.Title,
                    Amount = request.Amount,
                    UserId = request.UserId,
                    CategoryId = request.CategoryId,
                    Type = request.Type,
                    CreatedAt = DateTime.UtcNow,
                    PaidOrReceivedAt = request.PaidOrReceivedAt
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return new Response<Transaction?>(transaction, 201, "Transação criada com sucesso!");
            }
            catch 
            {
                return new Response<Transaction?>(null, 500, "Falha ao criar uma transação");
            }
        }

        public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
        {
            try
            {
                var transaction = _context.Transactions.FirstOrDefault(r => r.Id == request.Id && r.UserId == request.UserId);

                if (transaction is null)
                    return new Response<Transaction?>(null, 404, "Não foi possível identificar essa transação");

                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();

                return new Response<Transaction?>(null, 204, "Transação deletada com sucesso");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Houve um erro para deletar sua transação");
            }
        }

        public async Task<Response<Transaction?>> GetByIdAsync(GetByIdTransactionRequest request)
        {
            try
            {
                var transaction = await _context.Transactions.FirstOrDefaultAsync(r => r.Id == request.Id && r.UserId == request.UserId);

                return transaction is null
                    ? new Response<Transaction?>(null, 404, "Não foi possível encontrar essa transação")
                    : new Response<Transaction?>(transaction, 200, "Transação encontrada com sucesso");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Houve um erro ao encontrar esta transação");
            }
        }

        public async Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetByPeriodTransactionRequest request)
        {
            try
            {
                if (request.StartDate is null)
                    request.StartDate = DateTime.UtcNow;
                if (request.EndDate is null)
                    request.EndDate = DateTime.UtcNow;
                request.StartDate = DateTime.SpecifyKind((DateTime)request.StartDate, DateTimeKind.Utc);
                request.EndDate = DateTime.SpecifyKind((DateTime)request.EndDate, DateTimeKind.Utc);
                
                var query = _context.Transactions
                    .AsNoTracking()
                    .Where(q => 
                        q.PaidOrReceivedAt >= request.StartDate &&
                        q.PaidOrReceivedAt <= request.EndDate &&
                        q.UserId == request.UserId)
                    .OrderBy(o => o.CreatedAt);

                var transactions = await query
                    .Skip(request.PageSize * (request.PageNumber - 1))
                    .Take(request.PageSize)
                    .ToListAsync();

                int count = await query.CountAsync();

                return new PagedResponse<List<Transaction>?>(transactions,count, request.PageNumber, request.PageSize);
            }
            catch
            {
                return new PagedResponse<List<Transaction>?>(null, 500, "Não foi possível identificar suas transações");
            }
        }

        public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
        {
            try
            {   
                var transaction = _context.Transactions.FirstOrDefault(r => r.Id == request.Id && r.UserId == request.UserId);

                if (transaction is null)
                    return new Response<Transaction?>(null, 404, "Não foi possível identificar essa transação");

                transaction.PaidOrReceivedAt = request.PaidOrReceivedAt;
                transaction.CategoryId = request.CategoryId;
                transaction.Amount = request.Amount;
                transaction.Title = request.Title;
                transaction.Type = request.Type;

                _context.Transactions.Update(transaction);
                await _context.SaveChangesAsync();

                return new Response<Transaction?>(transaction, 204, "Transação atualizada com sucesso");
            
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Houve um erro ao atualizar a transação");
            }
        }
    }
}
