﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GeoCoding.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.3.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanGetDataOnce {
            get {
                return ((bool)(this["CanGetDataOnce"]));
            }
            set {
                this["CanGetDataOnce"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Input")]
        public string FolderInput {
            get {
                return ((string)(this["FolderInput"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Output")]
        public string FolderOutput {
            get {
                return ((string)(this["FolderOutput"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Temp")]
        public string FolderTemp {
            get {
                return ((string)(this["FolderTemp"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsFileInputOnFTP {
            get {
                return ((bool)(this["IsFileInputOnFTP"]));
            }
            set {
                this["IsFileInputOnFTP"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanCopyFileOutputToFtp {
            get {
                return ((bool)(this["CanCopyFileOutputToFtp"]));
            }
            set {
                this["CanCopyFileOutputToFtp"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanBreakFileOutput {
            get {
                return ((bool)(this["CanBreakFileOutput"]));
            }
            set {
                this["CanBreakFileOutput"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4999")]
        public int MaxSizePart {
            get {
                return ((int)(this["MaxSizePart"]));
            }
            set {
                this["MaxSizePart"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanGeoCodGetAll {
            get {
                return ((bool)(this["CanGeoCodGetAll"]));
            }
            set {
                this["CanGeoCodGetAll"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanGeoCodGetError {
            get {
                return ((bool)(this["CanGeoCodGetError"]));
            }
            set {
                this["CanGeoCodGetError"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanGeoCodGetNotGeo {
            get {
                return ((bool)(this["CanGeoCodGetNotGeo"]));
            }
            set {
                this["CanGeoCodGetNotGeo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanSaveDataAsTemp {
            get {
                return ((bool)(this["CanSaveDataAsTemp"]));
            }
            set {
                this["CanSaveDataAsTemp"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanSaveDataAsFinished {
            get {
                return ((bool)(this["CanSaveDataAsFinished"]));
            }
            set {
                this["CanSaveDataAsFinished"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FtpServer {
            get {
                return ((string)(this["FtpServer"]));
            }
            set {
                this["FtpServer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("21")]
        public int FtpPort {
            get {
                return ((int)(this["FtpPort"]));
            }
            set {
                this["FtpPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FtpUser {
            get {
                return ((string)(this["FtpUser"]));
            }
            set {
                this["FtpUser"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FtpPassword {
            get {
                return ((string)(this["FtpPassword"]));
            }
            set {
                this["FtpPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FtpFolderInput {
            get {
                return ((string)(this["FtpFolderInput"]));
            }
            set {
                this["FtpFolderInput"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FtpFolderOutput {
            get {
                return ((string)(this["FtpFolderOutput"]));
            }
            set {
                this["FtpFolderOutput"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanOpenFolderAfter {
            get {
                return ((bool)(this["CanOpenFolderAfter"]));
            }
            set {
                this["CanOpenFolderAfter"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanGeoCodAfterGetFile {
            get {
                return ((bool)(this["CanGeoCodAfterGetFile"]));
            }
            set {
                this["CanGeoCodAfterGetFile"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Statistics")]
        public string FolderStatistics {
            get {
                return ((string)(this["FolderStatistics"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Errors")]
        public string FolderErrors {
            get {
                return ((string)(this["FolderErrors"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanSaveStatistics {
            get {
                return ((bool)(this["CanSaveStatistics"]));
            }
            set {
                this["CanSaveStatistics"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Light.Blue")]
        public string ColorTheme {
            get {
                return ((string)(this["ColorTheme"]));
            }
            set {
                this["ColorTheme"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BDServer {
            get {
                return ((string)(this["BDServer"]));
            }
            set {
                this["BDServer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BDName {
            get {
                return ((string)(this["BDName"]));
            }
            set {
                this["BDName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5432")]
        public int BDPort {
            get {
                return ((int)(this["BDPort"]));
            }
            set {
                this["BDPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BDLogin {
            get {
                return ((string)(this["BDLogin"]));
            }
            set {
                this["BDLogin"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BDPassword {
            get {
                return ((string)(this["BDPassword"]));
            }
            set {
                this["BDPassword"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanNotificationSaveSettings {
            get {
                return ((bool)(this["CanNotificationSaveSettings"]));
            }
            set {
                this["CanNotificationSaveSettings"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanNotificationSaveData {
            get {
                return ((bool)(this["CanNotificationSaveData"]));
            }
            set {
                this["CanNotificationSaveData"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanNotificationProcessCancel {
            get {
                return ((bool)(this["CanNotificationProcessCancel"]));
            }
            set {
                this["CanNotificationProcessCancel"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanNotificationDataProcessed {
            get {
                return ((bool)(this["CanNotificationDataProcessed"]));
            }
            set {
                this["CanNotificationDataProcessed"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanNotificationDataEmpty {
            get {
                return ((bool)(this["CanNotificationDataEmpty"]));
            }
            set {
                this["CanNotificationDataEmpty"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanNotificationStatAlreadySave {
            get {
                return ((bool)(this["CanNotificationStatAlreadySave"]));
            }
            set {
                this["CanNotificationStatAlreadySave"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanNotificationOnlyError {
            get {
                return ((bool)(this["CanNotificationOnlyError"]));
            }
            set {
                this["CanNotificationOnlyError"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanStartCompact {
            get {
                return ((bool)(this["CanStartCompact"]));
            }
            set {
                this["CanStartCompact"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanNotificationExit {
            get {
                return ((bool)(this["CanNotificationExit"]));
            }
            set {
                this["CanNotificationExit"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsMultipleRequests {
            get {
                return ((bool)(this["IsMultipleRequests"]));
            }
            set {
                this["IsMultipleRequests"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsMultipleProxy {
            get {
                return ((bool)(this["IsMultipleProxy"]));
            }
            set {
                this["IsMultipleProxy"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsNotProxy {
            get {
                return ((bool)(this["IsNotProxy"]));
            }
            set {
                this["IsNotProxy"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsSystemProxy {
            get {
                return ((bool)(this["IsSystemProxy"]));
            }
            set {
                this["IsSystemProxy"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsManualProxy {
            get {
                return ((bool)(this["IsManualProxy"]));
            }
            set {
                this["IsManualProxy"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsListProxy {
            get {
                return ((bool)(this["IsListProxy"]));
            }
            set {
                this["IsListProxy"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ProxyAddress {
            get {
                return ((string)(this["ProxyAddress"]));
            }
            set {
                this["ProxyAddress"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int ProxyPort {
            get {
                return ((int)(this["ProxyPort"]));
            }
            set {
                this["ProxyPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int CountRequests {
            get {
                return ((int)(this["CountRequests"]));
            }
            set {
                this["CountRequests"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("20")]
        public int CountProxy {
            get {
                return ((int)(this["CountProxy"]));
            }
            set {
                this["CountProxy"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("50")]
        public int MaxCountError {
            get {
                return ((int)(this["MaxCountError"]));
            }
            set {
                this["MaxCountError"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public string MaxCountErrorForProxy {
            get {
                return ((string)(this["MaxCountErrorForProxy"]));
            }
            set {
                this["MaxCountErrorForProxy"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string VerificationServer {
            get {
                return ((string)(this["VerificationServer"]));
            }
            set {
                this["VerificationServer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanUseVerificationModule {
            get {
                return ((bool)(this["CanUseVerificationModule"]));
            }
            set {
                this["CanUseVerificationModule"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanUseBdModule {
            get {
                return ((bool)(this["CanUseBdModule"]));
            }
            set {
                this["CanUseBdModule"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanUseFtpModule {
            get {
                return ((bool)(this["CanUseFtpModule"]));
            }
            set {
                this["CanUseFtpModule"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string VerificationServerFactor {
            get {
                return ((string)(this["VerificationServerFactor"]));
            }
            set {
                this["VerificationServerFactor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool CanUsePolygon {
            get {
                return ((bool)(this["CanUsePolygon"]));
            }
            set {
                this["CanUsePolygon"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CanUseANSI {
            get {
                return ((bool)(this["CanUseANSI"]));
            }
            set {
                this["CanUseANSI"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool BackgroundGeo {
            get {
                return ((bool)(this["BackgroundGeo"]));
            }
            set {
                this["BackgroundGeo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ListDayWeekMode {
            get {
                return ((string)(this["ListDayWeekMode"]));
            }
            set {
                this["ListDayWeekMode"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string ScpriptBackgroundGeo {
            get {
                return ((string)(this["ScpriptBackgroundGeo"]));
            }
            set {
                this["ScpriptBackgroundGeo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool UseScriptBackGeo {
            get {
                return ((bool)(this["UseScriptBackGeo"]));
            }
            set {
                this["UseScriptBackGeo"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BDAccessoryServer {
            get {
                return ((string)(this["BDAccessoryServer"]));
            }
            set {
                this["BDAccessoryServer"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BDAccessoryName {
            get {
                return ((string)(this["BDAccessoryName"]));
            }
            set {
                this["BDAccessoryName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int BDAccessoryPort {
            get {
                return ((int)(this["BDAccessoryPort"]));
            }
            set {
                this["BDAccessoryPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BDAccessoryLogin {
            get {
                return ((string)(this["BDAccessoryLogin"]));
            }
            set {
                this["BDAccessoryLogin"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BDAccessoryPassword {
            get {
                return ((string)(this["BDAccessoryPassword"]));
            }
            set {
                this["BDAccessoryPassword"] = value;
            }
        }
    }
}
