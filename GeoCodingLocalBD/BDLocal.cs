using Dapper;
using GeoCodingLocalBD.Data;
using GeoCodingLocalBD.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace GeoCodingLocalBD
{
    public class BDLocal : IRepositoryLocal
    {
        private const string _fileName = "db.db";
        private string _connectionString = $"Data Source={_fileName};Version=3;";

        public bool CheckDB()
        {
            return File.Exists(_fileName);
        }

        public List<EntityAddress> GetListAddress()
        {
            List<EntityAddress> _data = null;
            var com = "SELECT Id, Name, OrponId, AdminLevel, ParentId FROM Address";
            try
            {
                var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                _data = connection.Query<Address>(com).Select(x =>
                {
                    return new EntityAddress()
                    {
                        Id = x.Id,
                        Address = x.Name,
                        OrponId = x.OrponId,
                        AdminLevel = x.AdminLevel,
                        ParentId =x.ParentId
                    };
                }).OrderBy(x=>x.Address).ToList();

                connection.Close();
            }
            catch (Exception ex)
            {

            }

            return _data;
        }

        public List<double> GetPolygonById(int id)
        {
            string _data = string.Empty;

            var com = "SELECT Bbox FROM Address Where Id=@Id and OrponId>0";
            var param = new DynamicParameters();
            param.Add("@Id", id);
            try
            {
                var connection = new SQLiteConnection(_connectionString);
                connection.Open();

                _data = connection.Query<string>(com, param).FirstOrDefault();

                connection.Close();
            }
            catch (Exception ex)
            {

            }

            return StringTolist(_data);
        }

        private List<double> StringTolist(string data)
        {
            var str = data.Substring(1, data.Length - 2);

            return str.Split(',').Select(x=>
            {
                return double.Parse(x.Replace('.',','));
            }).ToList();
        }
    }
}