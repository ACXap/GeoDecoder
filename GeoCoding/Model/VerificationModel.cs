using GeoCoding.VerificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeoCoding
{
    public class VerificationModel
    {
        private readonly IVerificationService _verification;

        private CancellationTokenSource cts;

        public async void CheckServerAsync(Action<Exception> callback)
        {
            Exception error = null;
            await Task.Factory.StartNew(() =>
            {
                _verification.CheckServiceVerification(e =>
               {
                   if (e != null)
                   {
                       error = e;
                   }
               });

                callback(error);
            });
        }

        public void SettingsService(string connectionSettings)
        {
            _verification.SettingsService(e =>
            {

            }, connectionSettings);
        }

        public async void CompareAsync(Action<Exception> callback, IEnumerable<EntityForCompare> data)
        {
            Exception error = null;

            cts = new CancellationTokenSource();
            var t = cts.Token;

            ParallelOptions po = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 3,
                CancellationToken = t
            };

            var list = data.Partition(400);

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var a = Parallel.ForEach(list, po, (item) =>
                    {
                        var geo = item.Select(x =>
                        {
                            x.Status = StatusCompareType.CompareNow;
                            return new VerificationService.EntityForCompare()
                            {
                                Data = x.GeoCode.MainGeoCod.AddressWeb
                            };
                        }).ToList();

                        _verification.GetId(e =>
                        {
                            var index = 0;

                            if (e != null)
                            {
                                error = e;
                                foreach (var i in geo)
                                {
                                    var o = item.ElementAt(index++);
                                    o.Status = StatusCompareType.Error;
                                    o.Error = e.Message;
                                }
                            }
                            else
                            {
                                foreach (var i in geo)
                                {
                                    var o = item.ElementAt(index++);
                                    o.Status = StatusCompareType.OK;
                                    o.GlobalIdAfterCompare = i.Id;

                                    o.Qcode = o.GlobalIdAfterCompare == o.GeoCode.GlobalID ? (byte)1 : (byte)2;

                                    o.IsChanges = o.Qcode != o.GeoCode.MainGeoCod.Qcode;
                                }
                            }
                        }, geo);
                    });
                }
                catch (Exception ex)
                {
                    error = ex;
                }

                callback(error);
            }, t);
        }

        public void StopCompare()
        {
            cts?.Cancel();
        }

        public VerificationModel(string connectionSettings)
        {
            _verification = new Verification(connectionSettings);
        }
    }
}