// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace GeoCoding
{
    /// <summary>
    /// Вид топонима
    /// </summary>
    public enum KindType
    {
        /// <summary>
        /// Геокодирование не проводилось
        /// </summary>
        None,
        /// <summary>
        /// Отдельный дом - Россия, Москва, улица Тверская, 7
        /// </summary>
        House,
        /// <summary>
        /// Улица - Россия, Москва, улица Тверская
        /// </summary>
        Street,
        /// <summary>
        /// Станция метро - Россия, Москва, Филевская линия, метро Арбатская
        /// </summary>
        Metro,
        /// <summary>
        /// Район города - Россия, Москва, Северо-Восточный административный округ
        /// </summary>
        District,
        /// <summary>
        /// Населённый пункт: город / поселок / деревня / село и т.п. - Россия, Санкт-Петербург
        /// </summary>
        Locality,
        /// <summary>
        /// Район / области - Россия, Ленинградская область, Выборгский район
        /// </summary>
        Area,
        /// <summary>
        /// Область - Россия, Нижегородская область
        /// </summary>
        Province,
        /// <summary>
        /// Страна - Великобритания
        /// </summary>
        Country,
        /// <summary>
        /// Река / озеро / ручей / водохранилище и т.п. - Россия, река Волга
        /// </summary>
        Hydro,
        /// <summary>
        /// Ж.Д.станция - Россия, Москва, Курский вокзал
        /// </summary>
        Rainway,
        /// <summary>
        /// Линия метро / шоссе / ж.д.линия - Россия, Центральный федеральный округ, Ярославское направление
        /// </summary>
        Route,
        /// <summary>
        /// Лес / парк / сад и т.п. - Россия, Санкт-Петербург, Михайловский сад
        /// </summary>
        Vegetation,
        /// <summary>
        /// Аэропорт - Россия, Московская область, аэропорт Домодедово
        /// </summary>
        Airport,
        /// <summary>
        /// Прочее - Россия, Свердловская область, Екатеринбург, Шабур остров
        /// </summary>
        Other
    }
}