using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoCoding.BDService.Data
{
    public class EntityCoordinate
    {
        public int OrponId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Qcode { get; set; }

        public EntityCoordinate(int orponId, string latintude, string longitude, int qcode)
        {
            OrponId = orponId;
            Latitude = double.Parse(latintude.Replace(".", ","));
            Longitude = double.Parse(longitude.Replace(".", ","));
            Qcode = qcode;
        }
    }
}