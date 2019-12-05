// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCodingLocalBD.Data;
using System.Collections.Generic;

namespace GeoCodingLocalBD
{
    public interface IRepositoryLocal
    {
        List<EntityAddress> GetListAddress();
        List<double> GetPolygonById(int id);
        bool CheckDB();
    }
}