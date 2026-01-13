namespace SauvioData.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
        public decimal Balance { get; set; } = 0;
        public decimal TotalIncome { get; set; } = 0;
        public decimal TotalExpense { get; set; } = 0;
    }
}
