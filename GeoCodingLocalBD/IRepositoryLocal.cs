using GeoCodingLocalBD.Data;
using System.Collections.Generic;

namespace GeoCodingLocalBD
{
    public interface IRepositoryLocal
    {
        List<EntityAddress> GetListAddress();
        List<double> GetPolygonById(int id);
    }
}