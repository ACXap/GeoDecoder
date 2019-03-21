namespace GeoCoding.VerificationService
{
    public class EntityResultCompare : EntityForCompare
    {
        public bool IsMatches { get; set; }
        public int IdFound { get; set; }
        public string Error { get; set; }
    }
}