using Sauvio.Business.Dto;
using SauvioData.Models.Transaction;

namespace Sauvio.Business.Services.Finance
{
    public interface IFinanceService
    {
        Task<(bool Success, string Message)> AddIncome(TransactionDTO dto);
        Task<(bool Success, string Message)> AddExpense(TransactionDTO dto);
        Task<List<Transaction>> GetExpenses(int userId);
        Task<List<Transaction>> GetIncomes(int userId);
        Task<object> GetBalance(int userId);
    }
}
