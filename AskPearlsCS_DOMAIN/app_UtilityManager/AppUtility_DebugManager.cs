namespace AskPearlsCS_DOMAIN.app_UtilityManager
{
    public class AppUtility_DebugManager
    {

        public static void Debug_Get_MessageText (string? txtMessage )
        {
            try
            {

                Console.WriteLine ( "***BSRVemcoCS***\n\t\t" + txtMessage );
                    }
            catch ( Exception ex )
            {
                return;
            }
        }


    }
}
