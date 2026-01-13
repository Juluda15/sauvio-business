using Sauvio.Business.Dto;
using Sauvio.Business.Exceptions;
using SauvioData.Entities.Transaction;
using SauvioData.Interfaces;

namespace Sauvio.Business.Services.Finance
{
    public class FinanceService : IFinanceService
    {
        private readonly IFinanceData _data;

        public FinanceService(IFinanceData data)
        {
            _data = data;
        }

        public async Task AddIncome(TransactionDTO dto)
        {
            if (dto.Amount <= 0)
                throw new ValidationException("Amount must be positive");

            var user = await _data.GetUserById(dto.UserId)
                ?? throw new NotFoundException("User", dto.UserId);

            await _data.AddTransaction(new Transaction
            {
                UserId = dto.UserId,
                Amount = dto.Amount,
                Type = "income",
                Description = dto.Description,
                SourceOrCategory = dto.SourceOrCategory
            });

            await _data.UpdateUserBalance(dto.UserId, dto.Amount, dto.Amount, 0);
        }

        public async Task AddExpense(TransactionDTO dto)
        {
            if (dto.Amount <= 0)
                throw new ValidationException("Amount must be positive");

            var user = await _data.GetUserById(dto.UserId)
                ?? throw new NotFoundException("User", dto.UserId);

            if (user.Balance < dto.Amount)
                throw new ValidationException("Insufficient balance");

            await _data.AddTransaction(new Transaction
            {
                UserId = dto.UserId,
                Amount = dto.Amount,
                Type = "expense",
                Description = dto.Description,
                SourceOrCategory = dto.SourceOrCategory
            });

            await _data.UpdateUserBalance(dto.UserId, -dto.Amount, 0, dto.Amount);
        }

        public Task<List<Transaction>> GetExpenses(int userId)
            => _data.GetTransactions(userId, "expense");

        public Task<List<Transaction>> GetIncomes(int userId)
            => _data.GetTransactions(userId, "income");

        public async Task<(decimal Balance, decimal Income, decimal Expense)> GetBalance(int userId)
        {
            var user = await _data.GetUserById(userId)
                ?? throw new NotFoundException("User", userId);

            return (user.Balance, user.TotalIncome, user.TotalExpense);
        }

        public async Task UpdateTransaction(int transactionId, TransactionDTO dto)
        {
            var transaction = await _data.GetTransactionById(transactionId)
                ?? throw new NotFoundException("Transaction", transactionId);

            if (dto.Amount <= 0)
                throw new ValidationException("Amount must be positive");

            transaction.Description = dto.Description;
            transaction.Amount = dto.Amount;
            transaction.SourceOrCategory = dto.SourceOrCategory;

            await _data.UpdateTransaction(transaction);

            var user = await _data.GetUserById(transaction.UserId)
                ?? throw new NotFoundException("User", transaction.UserId);

            if (transaction.Type == "income")
            {
                user.TotalIncome = await _data.CalculateTotalIncome(user.Id);
                user.Balance = await _data.CalculateBalance(user.Id);
            }
            else if (transaction.Type == "expense")
            {
                user.TotalExpense = await _data.CalculateTotalExpense(user.Id);
                user.Balance = await _data.CalculateBalance(user.Id);
            }

            await _data.UpdateUserBalance(user.Id, user.Balance, user.TotalIncome, user.TotalExpense);
        }

        public async Task DeleteTransaction(int transactionId)
        {
            var transaction = await _data.GetTransactionById(transactionId)
                ?? throw new NotFoundException("Transaction", transactionId);

            await _data.DeleteTransaction(transactionId);

            var user = await _data.GetUserById(transaction.UserId);
            if (user != null)
            {
                user.TotalIncome = await _data.CalculateTotalIncome(user.Id);
                user.TotalExpense = await _data.CalculateTotalExpense(user.Id);
                user.Balance = await _data.CalculateBalance(user.Id);
                await _data.UpdateUserBalance(user.Id, user.Balance, user.TotalIncome, user.TotalExpense);
            }
        }
    }
}
