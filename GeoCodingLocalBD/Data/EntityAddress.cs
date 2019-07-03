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