using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerCraft
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns a string containing the human readable version of this time span, i.e 4 days, 3 hours, 26 minutes and 13.4 seconds.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
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
