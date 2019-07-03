using GeoCodingLocalBD;
using GeoCodingLocalBD.Data;
using System.Collections.Generic;

namespace GeoCoding
{
    public class PolygonModel
    {
        IRepositoryLocal _repository = new BDLocal();

        public List<EntityAddress> GetAddress()
        {
            return _repository.GetListAddress();
        }

        public List<double> GetPolygon(int id)
        {
            return _repository.GetPolygonById(id);
        }
    }
}