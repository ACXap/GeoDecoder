// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace GeoCodingLocalBD.Data
{
    public class EntityAddress
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public int OrponId { get; set; }
        public int AdminLevel { get; set; }
        public int ParentId { get; set; }
    }
}