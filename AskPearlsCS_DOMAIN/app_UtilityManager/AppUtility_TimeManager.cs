using System.Globalization;

namespace AskPearlsCS_DOMAIN.app_UtilityManager
{
    public class AppUtility_TimeManager
    {

     public static string Time_GetCurrentTimeInMilliSecond ( )
        {
            try
            {
                //long lng = System.Convert.ToInt64 ( ( DateTime.Now - new DateTime ( 1970 , 1 , 1 ) ).TotalMilliseconds );

                //return lng.ToString ( );
                return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds ( ).ToString ( );
            }
            catch ( Exception ex )
            {
                return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds ( ).ToString ( );
            }
        }


        public static DateTime Time_Get_DateTime_ByMilliSecond ( string DateTimeInMilliSec )
        {
            try
            {
                var iExpireDate = ( new DateTime ( 1970 , 1 , 1 ) ).AddMilliseconds ( double.Parse ( DateTimeInMilliSec ) );


                // Dim lng As Long = CLng((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds)

                return iExpireDate; // lng.ToString
            }
            catch ( Exception ex )
            {
                return DateTime.Now;
            }
        }



        public static string Time_Get_MilliSecond_ByDateTime ( string strDay , string strMonth , string strYear , string strHour , string strMinute , string strSecond )
        {
            try
            {

                // Dim moment As System.DateTime = New System.DateTime(1999, 1, 13, 3, 57, 32, 11)
                System.DateTime moment = new System.DateTime ( System.Convert.ToInt32 ( strYear ) , System.Convert.ToInt32 ( strMonth ) , System.Convert.ToInt32 ( strDay ) , System.Convert.ToInt32 ( strHour ) , System.Convert.ToInt32 ( strMinute ) , System.Convert.ToInt32 ( strSecond ) , 11 );
                int year = moment.Year;
                int month = moment.Month;
                int day = moment.Day;
                int hour = moment.Hour;
                int minute = moment.Minute;
                int second = moment.Second;

                int millisecond = moment.Millisecond;

                return millisecond.ToString ( );
            }
            catch ( Exception ex )
            {
                return null;
            }
        }




        public static string Time_Get_DateTime_String_ByMilliSecond ( string DateTimeInMilliSec )
        {
            try
            {
                var iExpireDate = ( new DateTime ( 1970 , 1 , 1 ) ).AddMilliseconds ( double.Parse ( DateTimeInMilliSec ) );


                // Dim lng As Long = CLng((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds)

                return string.Format ( "{0:dd MMMM, yyyy - dddd}" , iExpireDate ); // lng.ToString
            }
            catch ( Exception ex )
            {
                return null;
            }
        }




        public static string Time_Get_DateTime_String_ByNOW ( )
        {
            try
            {
                DateTime iExpireDate = DateTime.Now; // (New DateTime(1970, 1, 1)).AddMilliseconds(Double.Parse(DateTimeInMilliSec))


                // Dim lng As Long = CLng((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds)

                return string.Format ( "{0:dd MMMM, yyyy - dddd}" , iExpireDate ); // lng.ToString
            }
            catch ( Exception ex )
            {
                return null;
            }
        }


        public static DateTime Time_Get_DateTime_ByValue ( string? iDay , string? iMonth , string? iYear )
        {
            try
            {
                DateTime iExpireDate = DateTime.Now; // (New DateTime(1970, 1, 1)).AddMilliseconds(Double.Parse(DateTimeInMilliSec))


                // Dim lng As Long = CLng((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds)

              //  return "#" + iYear.ToString() + "-" + iMonth.ToString() + "-" + iDay.ToString() + "#"; // String.Format("{0:dd MMMM, yyyy - dddd}", iExpireDate) 'lng.ToString
                  return iExpireDate;
            
            }
            catch ( Exception ex )
            {
                return DateTime.Now;
            }
        }



        public static string Time_Get_DateTime_String_ByDateTime ( DateTime iExpireDate )
        {
            try
            {


                // Dim As Date = DateTime.Now '(New DateTime(1970, 1, 1)).AddMilliseconds(Double.Parse(DateTimeInMilliSec))


                // Dim lng As Long = CLng((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds)

                return string.Format ( "{0:dd MMMM, yyyy - dddd}" , iExpireDate ); // lng.ToString
            }
            catch ( Exception ex )
            {
                return null;
            }
        }


        public static string Time_Get_CurrentWeekOfYear ( )
        {
            try
            {
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                DateTime date1 = DateTime.Now; // #1/1/2011#
                Calendar cal = dfi.Calendar;

                // Console.WriteLine("{0:d}: Week {1} ({2})", date1,
                // cal.GetWeekOfYear(date1, dfi.CalendarWeekRule,
                // dfi.FirstDayOfWeek),
                // cal.ToString().Substring(cal.ToString().LastIndexOf(".") + 1))



                // lblResponse.Text = "CalendarWeekRule == " & cal.GetWeekOfYear(date1, dfi.CalendarWeekRule,
                // dfi.FirstDayOfWeek)

                return cal.GetWeekOfYear ( date1 , dfi.CalendarWeekRule , dfi.FirstDayOfWeek ).ToString ( );
            }
            catch ( Exception ex )
            {
                return null;
            }
        }
    }


}
