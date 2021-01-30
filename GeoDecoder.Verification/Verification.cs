// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
            MaxReceivedMessageSize = 1500000,
            SendTimeout = new TimeSpan(0, 8, 0),
            ReceiveTimeout = new TimeSpan(0, 8, 0),
            OpenTimeout = new TimeSpan(0, 8, 0),
            CloseTimeout = new TimeSpan(0, 8, 0),
        };
        private EndpointAddress _address;

        public void CheckServiceVerification(Action<Exception> callback)
        {
            Exception error = null;

            try
            {
                using (var client = new VerificationWebService.wsSearchAddrElByFullNamePortType2Client(_binding, _address))
                {
                    var address = new VerificationWebService.AddressElementNameDataAddressElementFullNameGroup[]
                        {
                            new VerificationWebService.AddressElementNameDataAddressElementFullNameGroup(){FullAddress = "Российская Федерация, Адыгея Респ., Майкоп г., Привокзальная ул., дом 5"}
                        };
                    var r = client.SearchAddressElementByFullName(new VerificationWebService.AddressElementNameData() { AddressElementFullNameList = address });

                    if(r.AddressElementResponseList2.First().GlobalID!= "34539553")
                    {
                        throw new Exception("Test not correct");
                    }
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
                using (var client = new VerificationWebService.wsSearchAddrElByFullNamePortType2Client(_binding, _address))
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
                        var list = r.AddressElementResponseList2;
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
                    else
                    {
                        new Exception("SOAP filed. Null client. Error url");
                    }

                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            callback(error);
        }

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