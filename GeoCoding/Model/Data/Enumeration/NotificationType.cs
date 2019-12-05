// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace GeoCoding
{
    /// <summary>
    /// Перечисления для типов оповещений
    /// </summary>
    public enum NotificationType
    {
        Error,
        Ok,
        Cancel,
        SettingsSave,
        SaveData,
        StatAlreadySave,
        DataProcessed,
        DataEmpty,
        Close
    }
}