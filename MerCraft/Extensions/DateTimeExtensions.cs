using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerCraft
{
    /// <summary>
    /// Allows Date/Time to become human readable
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Turns a TimeSpan into a readable string
        /// </summary>
        /// <param name="timeSpan">Timespan input</param>
        /// <param name="showMilliseconds">Show milliseconds?</param>
        /// <param name="showTicks">Show ticks?</param>
        /// <returns>Readable string</returns>
        public static string ToHumanReadable(this TimeSpan timeSpan, bool showMilliseconds = false, bool showTicks = false)
        {
            StringBuilder sb = new StringBuilder();

            if (timeSpan.Days > 0)
            {
                sb.Append(timeSpan.Days);
                if (timeSpan.Days == 1)
                    sb.Append(" day");
                else
                    sb.Append(" days");
            }

            if (timeSpan.Hours > 0)
            {
                if (sb.Length > 1)
                    sb.Append(", ");

                if (timeSpan.Minutes == 0 && timeSpan.Days > 0)
                    sb.Append("and ");
                sb.Append(timeSpan.Hours);

                if (timeSpan.Hours == 1)
                    sb.Append(" hour");
                else
                    sb.Append(" hours");
            }

            if (timeSpan.Minutes > 0)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                if (timeSpan.Seconds == 0 && timeSpan.Hours > 0)
                    sb.Append("and ");
                sb.Append(timeSpan.Minutes);

                if (timeSpan.Minutes == 1)
                    sb.Append(" minute");
                else
                    sb.Append(" minutes");
            }

            if (timeSpan.Seconds > 0)
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                if (timeSpan.Minutes > 0)
                    sb.Append("and ");
                sb.Append(timeSpan.Seconds);

                if (showMilliseconds && timeSpan.Milliseconds > 0)
                {
                    sb.Append(".");
                    sb.Append(timeSpan.Milliseconds);
                }

                if (timeSpan.Seconds == 1 && timeSpan.Milliseconds == 0)
                    sb.Append(" second");
                else
                    sb.Append(" seconds");
            }

            if (showMilliseconds && timeSpan.Milliseconds > 0 && timeSpan.Seconds == 0)
            {
                sb.Append(timeSpan.Milliseconds);
                if (timeSpan.Milliseconds == 1)
                    sb.Append(" millisecond");
                else
                    sb.Append(" milliseconds");

                if (showTicks)
                {
                    sb.Append(" (");
                    sb.Append(timeSpan.Ticks);
                    sb.Append(" ticks)");
                }
            }

            return sb.ToString();
        }
    }
}
