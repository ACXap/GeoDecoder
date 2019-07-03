namespace GeoCodingLocalBD.Data.Model
{
    public class Address
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public string Polygons { get; set; }
        public string Bbox { get; set; }
        public int OrponId { get; set; }
        public int AdminLevel { get; set; }
    }
}