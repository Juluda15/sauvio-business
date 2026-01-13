namespace SauvioData.Models.Transaction
{
    public class Transaction
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public string Type { get; set; }  

        public string Description { get; set; } 

        public string SourceOrCategory { get; set; } 

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
