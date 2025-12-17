namespace Sauvio.Business.Dto
{
    public class TransactionDTO
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }

        public string Description { get; set; }

        public string SourceOrCategory { get; set; }

        public string Type { get; set; }
    }
}
