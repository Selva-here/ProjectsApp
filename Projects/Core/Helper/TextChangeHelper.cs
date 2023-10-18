using Projects.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZohoProjects;

namespace Projects.Core.Helper
{
    public static class TextChangeHelper
    {

        public static int GetDuration(DateTime startDate, DateTime dueDate)
        {
            if (startDate == null || dueDate == null)
                return 0;

            return (dueDate - startDate).Days;
        }
        public static string GetDurationString(DateTime startDate, DateTime dueDate)
        {
            if (startDate == null || dueDate == null)
                return " - ";

            return (dueDate - startDate).Days.ToString();
        }
        public static string GetDaysLeftString(DateTime startDate, DateTime dueDate)
        {
            if (startDate == null || dueDate == null)
                return "";
            int daysLeft = (dueDate - startDate).Days;
            if (daysLeft >= 0)
            {
                return daysLeft.ToString() + " Days Left";
            }
            else
            {
                return daysLeft.ToString() + " Days Overdue";
            }
        }
        public static string GetShortDateString(DateTime dateTime)
        {
            if (dateTime == null || dateTime == DateTime.MinValue)
            {
                return "-";
            }
            return dateTime.ToString("MMM dd");
        }
        public static string GetLongDateString(DateTime dateTime)
        {
            if (dateTime == null || dateTime == DateTime.MinValue)
            {
                return "-";
            }
            return dateTime.ToString("dd MMM yy");
        }
        public static string GetPercentageString(int percentage)
        {
            return percentage.ToString() + " %";
        }
       
       
    }
}
