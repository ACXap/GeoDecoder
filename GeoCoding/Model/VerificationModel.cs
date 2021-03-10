// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        public VerificationModel(string connectionSettings)
        {
            _verification = new Verification(connectionSettings);
        }


        #region PrivateField
        private readonly IVerificationService _verification;

        private CancellationTokenSource cts;
        #endregion PrivateField

        #region PublicProperties
        #endregion PublicProperties

        #region PrivateMethod
        #endregion PrivateMethod

        #region PublicMethod
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

            var list = data.Partition(300);

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var a = Parallel.ForEach(list, po, (item) =>
                    {
                        var geo = item.Select(x =>
                        {
                            x.Status = StatusType.Processed;
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
                                    o.Status = StatusType.OK;
                                    o.Error = e.Message;
                                    o.GlobalIdAfterCompare = 2;
                                    o.Qcode = 2;
                                    o.IsChanges = true;
                                }
                            }
                            else
                            {
                                foreach (var i in geo)
                                {
                                    var o = item.ElementAt(index++);
                                    o.Status = StatusType.OK;
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

        public Exception CheckGeo(List<EntityForCompare> data)
        {
            Exception error = null;

            cts = new CancellationTokenSource();
            var t = cts.Token;

            ParallelOptions po = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 3,
                CancellationToken = t
            };

            var list = data.Partition(300);

            try
            {
                var a = Parallel.ForEach(list, po, (item) =>
                {
                    var geo = item.Select(x =>
                    {
                        x.Status = StatusType.Processed;
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
                                o.Status = StatusType.Error;
                                o.Error = e.Message;
                            }
                        }
                        else
                        {
                            foreach (var i in geo)
                            {
                                var o = item.ElementAt(index++);
                                o.Status = StatusType.OK;
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

            return error;
        }
        #endregion PublicMethod

        public void Check(Action<Exception> callback, IEnumerable<EntityGeoCod> data)
        {
            var list = new List<EntityForCompare>(data.Where(x => x.MainGeoCod != null && x.MainGeoCod.Qcode == 1).Select(x =>
            {
                return new EntityForCompare() { GeoCode = x };
            }).ToList());

            CompareAsync((e) =>
            {
                var d = list.Where(x => x.IsChanges);
                foreach (var item in d)
                {
                    if (item.Qcode != 0)
                    {
                        item.GeoCode.MainGeoCod.Qcode = item.Qcode;
                        if (item.Qcode != 1)
                        {
                            item.GeoCode.MainGeoCod.Kind = KindType.Other;
                            item.GeoCode.MainGeoCod.Precision = PrecisionType.Other;
                        }
                    }
                }
                callback(e);
            }, list);
        }
    }
}