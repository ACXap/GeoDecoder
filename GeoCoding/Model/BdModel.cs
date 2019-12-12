// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using GeoCoding.BDService;
using GeoCoding.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoCoding.Model
{
    public class BdModel
    {
        private const string _errorIsFormatIDWrong = "Формат значения GlobalId неверный";
        private const string _errorIsAddressEmpty = "Значение адреса пусто";

        private readonly IBDService _bdService = new BDPostgresql();

        private ConnectionSettingsDb GetConnection(BDSettings bds)
        {
            return new ConnectionSettingsDb()
            {
                Server = bds.Server,
                BDName = bds.BDName,
                Port = bds.Port,
                Login = bds.Login,
                Password = bds.Password
            };
        }

        /// <summary>
        /// Метод для подключения к базе данных
        /// </summary>
        /// <param name="bds">Настройки подключения к базе данных</param>
        public async Task<EntityResult<bool>> ConnectBDAsync(BDSettings bds)
        {
            return await Task.Factory.StartNew(() =>
            {
                EntityResult<bool> result = _bdService.ConnectBD(new ConnectionSettingsDb()
                {
                    Server = bds.Server,
                    BDName = bds.BDName,
                    Port = bds.Port,
                    Login = bds.Login,
                    Password = bds.Password
                }); ;

                return result;
            });
        }

        /// <summary>
        /// Метод для получения данных из базы данных
        /// </summary>
        /// <param name="bds">Настройки подключения к базе данных</param>
        public async Task<EntityResult<EntityGeoCod>> GetDataUserScriptFromBDAsync(BDSettings bds)
        {
            return await Task.Factory.StartNew(() =>
            {
                EntityResult<EntityGeoCod> result = new EntityResult<EntityGeoCod>();
                EntityResult<EntityAddress> data = null;

                data = _bdService.ExecuteUserQuery(GetConnection(bds), bds.SQLQuery);

                if (data.Error == null)
                {
                    result.Entities = GetEntityGeoCodByAddress(data.Entities);
                    result.Successfully = true;
                }
                else
                {
                    result.Error = data.Error;
                }

                return result;
            });
        }

        /// <summary>
        /// Метод для получения данных из базы данных
        /// </summary>
        /// <param name="bds">Настройки подключения к базе данных</param>
        /// <param name="gset">Настройки общие приложения</param>
        public async Task<EntityResult<EntityGeoCod>> GetDataFromBDAsync(BDSettings bds, GeneralSettings gset, int limitRow)
        {
            return await Task.Factory.StartNew(() =>
            {
                EntityResult<EntityGeoCod> result = new EntityResult<EntityGeoCod>();
                EntityResult<EntityAddress> data = null;

                // Если только новые адреса нужны
                if (gset.UseGetNewAddressBackGeo)
                {
                    data = _bdService.GetNewAddress(GetConnection(bds), limitRow);
                }

                // Если новые и плохие адреса нужны
                if (gset.UseGetNewBadAddressBackGeo)
                {
                    data = new EntityResult<EntityAddress>();

                    if(limitRow <= gset.CountNewAddress)
                    {
                        var newAddress = _bdService.GetNewAddress(GetConnection(bds), limitRow);
                        if (newAddress.Successfully)
                        {
                            data.Entities = newAddress.Entities;
                            data.Successfully = true;
                        }
                        else
                        {
                            data.Error = newAddress.Error;
                        }
                    }
                    else
                    {
                        var countBad = limitRow - gset.CountNewAddress;
                        var newAddress = _bdService.GetNewAddress(GetConnection(bds), gset.CountNewAddress);
                        if (newAddress.Successfully)
                        {
                            var badAddress = _bdService.GetBadAddress(GetConnection(bds), countBad);
                            if (badAddress.Successfully)
                            {
                                data.Entities = newAddress.Entities.Concat(badAddress.Entities);
                                data.Successfully = true;
                            }
                            else
                            {
                                data.Error = badAddress.Error;
                            }
                        }
                        else
                        {
                            data.Error = newAddress.Error;
                        }
                    }
                }

                // Если нужен пользовательский скрипт
                if (gset.UseScriptBackGeo)
                {
                    data = _bdService.ExecuteUserQuery(GetConnection(bds), gset.ScpriptBackgroundGeo);
                }

                if (data.Error == null)
                {
                    result.Entities = GetEntityGeoCodByAddress(data.Entities);
                    result.Successfully = true;
                }
                else
                {
                    result.Error = data.Error;
                }

                return result;
            });
        }

        private IEnumerable<EntityGeoCod> GetEntityGeoCodByAddress(IEnumerable<EntityAddress> data)
        {
            var list = new List<EntityGeoCod>();

            foreach (var item in data)
            {
                var a = new EntityGeoCod();

                if (item.OrponId == 0)
                {
                    SetError(a, _errorIsFormatIDWrong);
                }
                else
                {
                    a.GlobalID = item.OrponId;
                }

                if (string.IsNullOrEmpty(item.Address))
                {
                    SetError(a, _errorIsAddressEmpty);
                }
                else
                {
                    a.Address = item.Address;
                }
                a.FiasGuid = item.FiasGuid;

                list.Add(a);
            }

            return list;
        }

        private void SetError(EntityGeoCod geocod, string mes)
        {
            geocod.Error = mes;
            geocod.Status = StatusType.Error;
        }
    }
}