// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCodingLocalBD;
using GeoCodingLocalBD.Data;
using System.Collections.Generic;

namespace GeoCoding
{
    public class PolygonModel
    {
        IRepositoryLocal _repository = new BdLocalNew();

        public bool CheckBD()
        {
            return _repository.CheckDB();
        }

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