using Sauvio.Business.Dto;
using Sauvio.Business.Services.Finance;
using SauvioData.Interfaces;
using SauvioData.Models.Transaction;

namespace Sauvio.Business.Services.Finance
{
    public class FinanceService : IFinanceService
    {
        private readonly IFinanceData _data;

        public FinanceService(IFinanceData data)
        {
            _data = data;
        }

        public async Task<(bool Success, string Message)> AddIncome(TransactionDTO dto)
        {
            if (dto.Amount <= 0)
                return (false, "Amount must be positive");

            var user = await _data.GetUserById(dto.UserId);
            if (user == null)
                return (false, "User not found");

            await _data.AddTransaction(new Transaction
            {
                UserId = dto.UserId,
                Amount = dto.Amount,
                Type = "income",
                Description = dto.Description,
                SourceOrCategory = dto.SourceOrCategory
            });

            await _data.UpdateUserBalance(
                dto.UserId,
                dto.Amount,
                dto.Amount,
                0
            );

            return (true, "Income added successfully");
        }

        public async Task<(bool Success, string Message)> AddExpense(TransactionDTO dto)
        {
            if (dto.Amount <= 0)
                return (false, "Amount must be positive");

            var user = await _data.GetUserById(dto.UserId);
            if (user == null)
                return (false, "User not found");

            if (user.Balance < dto.Amount)
                return (false, "Insufficient balance");

            await _data.AddTransaction(new Transaction
            {
                UserId = dto.UserId,
                Amount = dto.Amount,
                Type = "expense",
                Description = dto.Description,
                SourceOrCategory = dto.SourceOrCategory
            });

            await _data.UpdateUserBalance(
                dto.UserId,
                -dto.Amount,
                0,
                dto.Amount
            );

            return (true, "Expense added successfully");
        }

        public Task<List<Transaction>> GetExpenses(int userId)
            => _data.GetTransactions(userId, "expense");

        public Task<List<Transaction>> GetIncomes(int userId)
            => _data.GetTransactions(userId, "income");

        public async Task<object> GetBalance(int userId)
        {
            var user = await _data.GetUserById(userId);
            if (user == null) return new { Error = "User not found" };

            return new
            {
                user.Balance,
                user.TotalIncome,
                user.TotalExpense
            };
        }
    }
}
