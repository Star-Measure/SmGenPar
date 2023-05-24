using JetBrains.Annotations;
using SMIO.Commands;
using SMIO.Data;

namespace SmGenPar.Logic.Types;

[PublicAPI]
public static class SelfReadingExtensions
{
    public static BcdDateTime FromDateTime(DateTime dateTime)
    {
        var time = new BcdTime
        {
            Minute = Bcd.ToBcd(dateTime.Minute),
            Hour   = Bcd.ToBcd(dateTime.Hour)
        };

        var date = new BcdDate
        {
            Day   = Bcd.ToBcd(dateTime.Day),
            Month = Bcd.ToBcd(dateTime.Month),
            Year  = Bcd.ToBcd(dateTime.Year % 100)
        };

        return new(time, date);
    }
    public static ConjuntoSelfRead ToConjuntosSelfRead(this IEnumerable<DateTime> dateTimes)
    {
        var bcdDateTimes = dateTimes.Select(FromDateTime).ToArray();
        return ConjuntoSelfRead.CastFromDataTimes(bcdDateTimes);
    }
}