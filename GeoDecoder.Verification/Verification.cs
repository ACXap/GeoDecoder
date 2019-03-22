using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace GeoCoding.VerificationService
{
    public class Verification : IVerificationService
    {
        private readonly BasicHttpBinding _binding = new BasicHttpBinding()
        {
            Name = "SearchAddress",
            MaxReceivedMessageSize = 1000000,
            SendTimeout = new TimeSpan(0, 5, 0),
            ReceiveTimeout = new TimeSpan(0, 5, 0),
            OpenTimeout = new TimeSpan(0, 5, 0),
            CloseTimeout = new TimeSpan(0, 5, 0),
        };
        private EndpointAddress _address;

        public void CheckServiceVerification(Action<Exception> callback)
        {
            Exception error = null;

            try
            {
                using (var client = new VerificationWebService.wsSearchAddrElByFullNamePortTypeClient(_binding, _address))
                {
                    client.Open();
                    //client.Close();
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }

        public void GetId(Action<Exception> callback, IEnumerable<EntityForCompare> data)
        {
            Exception error = null;

            try
            {
                using (var client = new VerificationWebService.wsSearchAddrElByFullNamePortTypeClient(_binding, _address))
                {
                    var address = data.Select(x =>
                    {
                        return new VerificationWebService.AddressElementNameDataAddressElementFullNameGroup()
                        {
                            FullAddress = x.Data
                        };
                    }).ToArray();

                    var r = client.SearchAddressElementByFullName(new VerificationWebService.AddressElementNameData()
                    {
                        AddressElementFullNameList = address
                    });

                    if (r != null)
                    {
                        var list = r.AddressElementResponseList;
                        if (list != null && list.Length > 0)
                        {
                            int rowIndex = 0;

                            foreach (var item in list)
                            {
                                var i = data.ElementAt(rowIndex++);
                                int.TryParse(item.GlobalID, out int id);
                                i.Id = id;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }


        //public void GetLightMatch(Action<bool, Exception> callback, EntityForCompare data)
        //{
        //    Exception error = null;
        //    bool result = false;

        //    using (var client = new VerificationWebService.wsSearchAddrElByFullNamePortTypeClient(_binding, _address))
        //    {
        //        var address = new VerificationWebService.AddressElementNameDataAddressElementFullNameGroup[]
        //            {
        //                new VerificationWebService.AddressElementNameDataAddressElementFullNameGroup(){FullAddress = data.Data}
        //            };

        //        var r = client.SearchAddressElementByFullName(new VerificationWebService.AddressElementNameData() { AddressElementFullNameList = address });
        //        if (r != null)
        //        {
        //            var list = r.AddressElementResponseList;
        //            if (list != null && list.Length > 0)
        //            {
        //                if (list[0].GlobalID == data.Id.ToString())
        //                {
        //                    result = true;
        //                }
        //            }
        //        }

        //        client.Close();
        //    }

        //    callback(result, error);
        //}

        //public void GetMatch(Action<EntityResultCompare, Exception> callback, EntityForCompare data)
        //{
        //    Exception error = null;
        //    EntityResultCompare result = new EntityResultCompare()
        //    {
        //        Id = data.Id,
        //        Data = data.Data,
        //    };

        //    try
        //    {
        //        using (var client = new VerificationWebService.wsSearchAddrElByFullNamePortTypeClient(_binding, _address))
        //        {
        //            var address = new VerificationWebService.AddressElementNameDataAddressElementFullNameGroup[]
        //                {
        //                new VerificationWebService.AddressElementNameDataAddressElementFullNameGroup(){FullAddress = data.Data}
        //                };

        //            var r = client.SearchAddressElementByFullName(new VerificationWebService.AddressElementNameData() { AddressElementFullNameList = address });
        //            if (r != null)
        //            {
        //                var list = r.AddressElementResponseList;
        //                if (list != null && list.Length > 0)
        //                {
        //                    if (list[0].GlobalID == data.Id.ToString())
        //                    {
        //                        result.IsMatches = true;
        //                    }
        //                    else
        //                    {
        //                        int.TryParse(list[0].GlobalID, out int id);
        //                        result.IdFound = id;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex;
        //    }

        //    callback(result, error);
        //}

        //public void GetMatches(Action<IEnumerable<EntityResultCompare>, Exception> callback, IEnumerable<EntityForCompare> data)
        //{
        //    Exception error = null;
        //    List<EntityResultCompare> result = new List<EntityResultCompare>(data.Count());

        //    try
        //    {
        //        using (var client = new VerificationWebService.wsSearchAddrElByFullNamePortTypeClient(_binding, _address))
        //        {
        //            var address = data.Select(x =>
        //            {
        //                return new VerificationWebService.AddressElementNameDataAddressElementFullNameGroup()
        //                {
        //                    FullAddress = x.Data
        //                };
        //            }).ToArray();

        //            var r = client.SearchAddressElementByFullName(new VerificationWebService.AddressElementNameData()
        //            {
        //                AddressElementFullNameList = address
        //            });

        //            if (r != null)
        //            {
        //                int rowIndex = 0;
        //                EntityResultCompare res = null;

        //                foreach (var item in r.AddressElementResponseList)
        //                {

        //                    var d = data.ElementAt(rowIndex);
        //                    res = new EntityResultCompare()
        //                    {
        //                        Data = d.Data,
        //                        Id = d.Id
        //                    };

        //                    if (d.Id.ToString() == item.GlobalID)
        //                    {
        //                        res.IsMatches = true;
        //                    }
        //                    else
        //                    {
        //                        int.TryParse(item.GlobalID, out int id);
        //                        res.IdFound = id;
        //                    }
        //                    result.Add(res);
        //                    rowIndex++;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        error = ex;
        //    }

        //    callback(result, error);
        //}

        public void SettingsService(Action<Exception> callback, string connectSettings)
        {
            Exception error = null;

            try
            {
                _address = new EndpointAddress(connectSettings);
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }

        public Verification(string connectionSettings)
        {
            if (!string.IsNullOrEmpty(connectionSettings))
            {
                _address = new EndpointAddress(connectionSettings);
            }
        }
    }
}