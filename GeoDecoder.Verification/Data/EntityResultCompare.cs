// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace GeoCoding.VerificationService
{
    public class EntityResultCompare : EntityForCompare
    {
        public bool IsMatches { get; set; }
        public int IdFound { get; set; }
        public string Error { get; set; }
    }
}