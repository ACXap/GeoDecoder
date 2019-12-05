// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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