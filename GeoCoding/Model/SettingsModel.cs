//using GeoCoding.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace GeoCoding.Model
//{
//    public class SettingsModel
//    {
//        /// <summary>
//        /// Метод для получения настроек приложения
//        /// </summary>
//        /// <param name="callback">Функция обратного вызова, с параметрами: ошибка, настройки файлов, настройки геокодирования, настройки фтп-сервера</param>
//        public void GetSettings(Action<Exception, FilesSettings, GeoCodSettings, FTPSettings, BDSettings, NotificationSettings, NetSettings, VerificationSettings, GeneralSettings> callback)
//        {
//            Exception error = null;
//            var p = Properties.Settings.Default;
//            var curDir = Environment.CurrentDirectory;

//            FilesSettings f = new FilesSettings()
//            {
//                CanBreakFileOutput = p.CanBreakFileOutput,
//                CanCopyFileOutputToFtp = p.CanCopyFileOutputToFtp,
//                FolderInput = $"{curDir}\\{p.FolderInput}",
//                FolderOutput = $"{curDir}\\{p.FolderOutput}",
//                FolderTemp = $"{curDir}\\{p.FolderTemp}",
//                FolderStatistics = $"{curDir}\\{p.FolderStatistics}",
//                FolderErrors = $"{curDir}\\{p.FolderErrors}",
//                IsFileInputOnFTP = p.IsFileInputOnFTP,
//                MaxSizePart = p.MaxSizePart,
//                CanGetDataOnce = p.CanGetDataOnce,
//                CanUseANSI = p.CanUseANSI
//            };

//            GeoCodSettings g = new GeoCodSettings()
//            {
//                CanGeoCodGetAll = p.CanGeoCodGetAll,
//                CanGeoCodGetError = p.CanGeoCodGetError,
//                CanGeoCodGetNotGeo = p.CanGeoCodGetNotGeo,
//                CanSaveDataAsFinished = p.CanSaveDataAsFinished,
//                CanSaveDataAsTemp = p.CanSaveDataAsTemp,
//                CanOpenFolderAfter = p.CanOpenFolderAfter,
//                CanGeoCodAfterGetFile = p.CanGeoCodAfterGetFile,
//                CanSaveStatistics = p.CanSaveStatistics,
//                GeoService = p.GeoService,
//                IsMultipleProxy = p.IsMultipleProxy,
//                IsMultipleRequests = p.IsMultipleRequests,
//                CountProxy = p.CountProxy,
//                CountRequests = p.CountRequests,
//                MaxCountError = p.MaxCountError,
//                CanUsePolygon = p.CanUsePolygon
//            };

//            FTPSettings ftp = new FTPSettings()
//            {
//                Port = p.FtpPort,
//                User = p.FtpUser,
//                FolderInput = p.FtpFolderInput,
//                FolderOutput = p.FtpFolderOutput
//            };

//            ProtectedDataDPAPI.DecryptData((d, e) =>
//            {
//                if (e == null)
//                {
//                    ftp.Server = d;
//                }
//            }, p.FtpServer);

//            ProtectedDataDPAPI.DecryptData((d, e) =>
//            {
//                if (e == null)
//                {
//                    ftp.Password = d;
//                }
//            }, p.FtpPassword);

//            BDSettings bds = new BDSettings()
//            {
//                BDName = p.BDName,
//                Port = p.BDPort,
//                Login = p.BDLogin,
//                Password = p.BDPassword
//            };

//            ProtectedDataDPAPI.DecryptData((d, e) =>
//            {
//                if (e == null)
//                {
//                    bds.Server = d;
//                }
//            }, p.BDServer);

//            ProtectedDataDPAPI.DecryptData((d, e) =>
//            {
//                if (e == null)
//                {
//                    bds.Password = d;
//                }
//            }, p.BDPassword);


//            NotificationSettings ns = new NotificationSettings()
//            {
//                CanNotificationDataEmpty = p.CanNotificationDataEmpty,
//                CanNotificationDataProcessed = p.CanNotificationDataProcessed,
//                CanNotificationOnlyError = p.CanNotificationOnlyError,
//                CanNotificationProcessCancel = p.CanNotificationProcessCancel,
//                CanNotificationSaveData = p.CanNotificationSaveData,
//                CanNotificationSaveSettings = p.CanNotificationSaveSettings,
//                CanNotificationStatAlreadySave = p.CanNotificationStatAlreadySave,
//                CanNotificationExit = p.CanNotificationExit
//            };

//            NetSettings nset = new NetSettings()
//            {
//                IsNotProxy = p.IsNotProxy,
//                IsSystemProxy = p.IsSystemProxy,
//                IsManualProxy = p.IsManualProxy,
//                IsListProxy = p.IsListProxy,
//                Proxy = new ProxyEntity()
//                {
//                    Address = p.ProxyAddress,
//                    Port = p.ProxyPort
//                }
//            };

//            GeneralSettings gset = new GeneralSettings()
//            {
//                CanStartCompact = p.CanStartCompact,
//                CanUseBdModule = p.CanUseBdModule,
//                CanUseFtpModule = p.CanUseFtpModule,
//                CanUseVerificationModule = p.CanUseVerificationModule,
//                ColorTheme = p.ColorTheme,
//                BackgroundGeo = p.BackgroundGeo,
//                ScpriptBackgroundGeo = p.ScpriptBackgroundGeo,
//                UseScriptBackGeo = p.UseScriptBackGeo
//            };

//            var listDayWeek = ObjectToStringJson.GetObjectOfstring<List<DayWeek>>(p.ListDayWeekMode);
//            if (listDayWeek == null || !listDayWeek.Any())
//            {
//                listDayWeek = new List<DayWeek>();
//                foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
//                {
//                    listDayWeek.Add(new DayWeek() { Day = d });
//                }
//            }
//            var s = listDayWeek.First(x => x.Day == 0);
//            listDayWeek.Remove(s);
//            listDayWeek.Insert(listDayWeek.Count, s);

//            gset.ListDayWeek = listDayWeek;

//            VerificationSettings vset = new VerificationSettings();

//            ProtectedDataDPAPI.DecryptData((d, e) =>
//            {
//                if (e == null)
//                {
//                    vset.VerificationServer = d;
//                }
//            }, p.VerificationServer);

//            //ProtectedDataDPAPI.DecryptData((d, e) =>
//            //{
//            //    if (e == null)
//            //    {
//            //        vset.VerificationServerFactor = d;
//            //    }
//            //}, p.VerificationServerFactor);

//            callback(error, f, g, ftp, bds, ns, nset, vset, gset);
//        }

//        /// <summary>
//        /// Метод для сохранения настроек
//        /// </summary>
//        /// <param name="callback">Функция обратного вызова, с параметром: ошибка</param>
//        /// <param name="filesSettings">Настройки файлов</param>
//        /// <param name="ftpSettings">Настройки фтп-сервера</param>
//        /// <param name="geoCodSettings">Настройки геокодирования</param>
//        public void SaveSettings(Action<Exception> callback, FilesSettings filesSettings, FTPSettings ftpSettings, GeoCodSettings geoCodSettings, BDSettings bdSettings, NotificationSettings ns, NetSettings netSettings, VerificationSettings vset, GeneralSettings gset)
//        {
//            Exception error = null;
//            var p = Properties.Settings.Default;

//            if (filesSettings != null)
//            {
//                p.CanBreakFileOutput = filesSettings.CanBreakFileOutput;
//                p.CanCopyFileOutputToFtp = filesSettings.CanCopyFileOutputToFtp;
//                p.IsFileInputOnFTP = filesSettings.IsFileInputOnFTP;
//                p.MaxSizePart = filesSettings.MaxSizePart;
//                p.CanGetDataOnce = filesSettings.CanGetDataOnce;
//                p.CanUseANSI = filesSettings.CanUseANSI;
//            }

//            if (ftpSettings != null)
//            {
//                p.FtpPort = ftpSettings.Port;
//                p.FtpUser = ftpSettings.User;
//                p.FtpFolderInput = ftpSettings.FolderInput;
//                // ФТП-сервер пароль шифруем
//                ProtectedDataDPAPI.EncryptData((d, e) =>
//                {
//                    if (e == null)
//                    {
//                        p.FtpPassword = d;
//                    }
//                }, ftpSettings.Password);
//                // ФТП-сервер шифруем
//                ProtectedDataDPAPI.EncryptData((d, e) =>
//                {
//                    if (e == null)
//                    {
//                        p.FtpServer = d;
//                    }
//                }, ftpSettings.Server);

//            }

//            if (geoCodSettings != null)
//            {
//                p.CanGeoCodGetAll = geoCodSettings.CanGeoCodGetAll;
//                p.CanGeoCodGetError = geoCodSettings.CanGeoCodGetError;
//                p.CanGeoCodGetNotGeo = geoCodSettings.CanGeoCodGetNotGeo;
//                p.CanOpenFolderAfter = geoCodSettings.CanOpenFolderAfter;
//                p.CanSaveDataAsFinished = geoCodSettings.CanSaveDataAsFinished;
//                p.CanSaveDataAsTemp = geoCodSettings.CanSaveDataAsTemp;
//                p.GeoService = geoCodSettings.GeoService;
//                p.IsMultipleProxy = geoCodSettings.IsMultipleProxy;
//                p.IsMultipleRequests = geoCodSettings.IsMultipleRequests;
//                p.CountProxy = geoCodSettings.CountProxy;
//                p.CountRequests = geoCodSettings.CountRequests;
//                p.MaxCountError = geoCodSettings.MaxCountError;
//                p.CanGeoCodAfterGetFile = geoCodSettings.CanGeoCodAfterGetFile;
//                p.CanSaveStatistics = geoCodSettings.CanSaveStatistics;
//                p.CanUsePolygon = geoCodSettings.CanUsePolygon;
//            }

//            if (gset != null)
//            {
//                p.CanStartCompact = gset.CanStartCompact;
//                p.CanUseVerificationModule = gset.CanUseVerificationModule;
//                p.CanUseBdModule = gset.CanUseBdModule;
//                p.CanUseFtpModule = gset.CanUseFtpModule;
//                p.BackgroundGeo = gset.BackgroundGeo;
//                p.UseScriptBackGeo = gset.UseScriptBackGeo;
//                p.ScpriptBackgroundGeo = gset.ScpriptBackgroundGeo;
//                p.ColorTheme = gset.ColorTheme;
//                p.ListDayWeekMode = ObjectToStringJson.GetStringOfObject(gset.ListDayWeek);
//            }

//            if (netSettings != null)
//            {
//                p.IsNotProxy = netSettings.IsNotProxy;
//                p.IsManualProxy = netSettings.IsManualProxy;
//                p.IsSystemProxy = netSettings.IsSystemProxy;
//                p.IsListProxy = netSettings.IsListProxy;
//                p.ProxyPort = netSettings.Proxy.Port;
//                p.ProxyAddress = netSettings.Proxy.Address;
//            }

//            if (bdSettings != null)
//            {
//                p.BDPort = bdSettings.Port;
//                p.BDName = bdSettings.BDName;
//                p.BDLogin = bdSettings.Login;
//                // БД сервер шифруем
//                ProtectedDataDPAPI.EncryptData((d, e) =>
//                {
//                    if (e == null)
//                    {
//                        p.BDServer = d;
//                    }
//                }, bdSettings.Server);
//                // БД пароль шифруем
//                ProtectedDataDPAPI.EncryptData((d, e) =>
//                {
//                    if (e == null)
//                    {
//                        p.BDPassword = d;
//                    }
//                }, bdSettings.Password);
//            }

//            if (vset != null)
//            {
//                // Сервер проверки адресов шифруем
//                ProtectedDataDPAPI.EncryptData((d, e) =>
//                {
//                    if (e == null)
//                    {
//                        p.VerificationServer = d;
//                    }
//                }, vset.VerificationServer);

//                //ProtectedDataDPAPI.EncryptData((d, e) =>
//                //{
//                //    if (e == null)
//                //    {
//                //        p.VerificationServerFactor = d;
//                //    }
//                //}, vset.VerificationServerFactor);
//            }

//            if (ns != null)
//            {
//                p.CanNotificationProcessCancel = ns.CanNotificationProcessCancel;
//                p.CanNotificationDataEmpty = ns.CanNotificationDataEmpty;
//                p.CanNotificationDataProcessed = ns.CanNotificationDataProcessed;
//                p.CanNotificationSaveData = ns.CanNotificationSaveData;
//                p.CanNotificationSaveSettings = ns.CanNotificationSaveSettings;
//                p.CanNotificationStatAlreadySave = ns.CanNotificationStatAlreadySave;
//                p.CanNotificationOnlyError = ns.CanNotificationOnlyError;
//                p.CanNotificationExit = ns.CanNotificationExit;
//            }

//            try
//            {
//                p.Save();
//            }
//            catch (Exception ex)
//            {
//                error = ex;
//            }

//            callback(error);
//        }
//    }
//}