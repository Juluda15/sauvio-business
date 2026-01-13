using Sauvio.Business.Dto;
using SauvioData.Entities.Transaction;

namespace Sauvio.Business.Services.Finance
{
    public interface IFinanceService
    {
        Task AddIncome(TransactionDTO dto);
        Task AddExpense(TransactionDTO dto);
        Task<List<Transaction>> GetExpenses(int userId);
        Task<List<Transaction>> GetIncomes(int userId);
        Task<(decimal Balance, decimal Income, decimal Expense)> GetBalance(int userId);
        Task DeleteTransaction(int transactionId);
        Task UpdateTransaction(int transactionId, TransactionDTO dto);
    }
}
