/*
==========================================================================
This file is part of Briefing Room for DCS World, a mission
generator for DCS World, by @akaAgar
(https://github.com/akaAgar/briefing-room-for-dcs)

Briefing Room for DCS World is free software: you can redistribute it
and/or modify it under the terms of the GNU General Public License
as published by the Free Software Foundation, either version 3 of
the License, or (at your option) any later version.

Briefing Room for DCS World is distributed in the hope that it will
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Briefing Room for DCS World.
If not, see https://www.gnu.org/licenses/
==========================================================================
*/

using System;

namespace BriefingRoom4DCS.Template
{
    public static class StartDateTimeSettings
    {
        public const int DisabledDatePart = 0;
        public const int DisabledTimePart = -1;

        public static int GetFirstYear(Decade decade) => (int)decade;

        public static int GetLastYear(Decade decade) => GetFirstYear(decade) + 9;

        public static bool IsYearInDecade(int year, Decade decade) =>
            year >= GetFirstYear(decade) && year <= GetLastYear(decade);

        public static int GetDaysInMonth(int year, int month)
        {
            if (month < 1 || month > 12) return 31;
            if (year < 1) return month == 2 ? 29 : DateTime.DaysInMonth(2000, month);

            return DateTime.DaysInMonth(year, month);
        }

        public static bool TryGetDate(int year, int month, int day, Decade decade, out DateTime date)
        {
            date = default;

            if (!IsYearInDecade(year, decade)) return false;
            if (month < 1 || month > 12) return false;
            if (day < 1 || day > GetDaysInMonth(year, month)) return false;

            date = new DateTime(year, month, day);
            return true;
        }

        public static bool TryGetTime(int hour, int minute, out TimeSpan time)
        {
            time = default;

            if (hour < 0 || hour > 23) return false;
            if (minute < 0 || minute > 59) return false;

            time = new TimeSpan(hour, minute, 0);
            return true;
        }
    }
}
