using System.Globalization;

namespace urlshortner.Extensions
{
    public static class DateFormatterExtensions
    {
        public static string ToDateTimeString(this DateTime inputDateTime)
        {
            // Convert to local time
            var localDateTime = inputDateTime.ToLocalTime();

            const string outputFormat = "dd'th' MMMM yyyy hh:mm tt";

            // Format the DateTime object using the desired output format
            var formattedDateTime = localDateTime.ToString(outputFormat, CultureInfo.InvariantCulture);

            return formattedDateTime; // 12th July 2024 03:45 PM
        }
    }
}