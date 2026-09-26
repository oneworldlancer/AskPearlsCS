using System.Collections;

namespace AskPearlsCS_DOMAIN.app_UtilityManager
{
    public class AppUtility_ListTokenIDManager
    {
        public static ArrayList ListTokenID_Generate_NewList ( string MiliSecCount )
        {
            try
            {
                ArrayList _arr_MilliSecondTokenList = new ArrayList ( );

                string _iMilliSecondToken; // = AppUtility_TimeManager.Time_GetCurrentTimeInMilliSecond

                while ( _arr_MilliSecondTokenList.Count != System.Convert.ToInt32 ( MiliSecCount ) )
                {
                    _iMilliSecondToken = AppUtility_TimeManager.Time_GetCurrentTimeInMilliSecond();

                    if ( !_arr_MilliSecondTokenList.Contains ( _iMilliSecondToken ) )
                        _arr_MilliSecondTokenList.Add ( _iMilliSecondToken );
                }

                return _arr_MilliSecondTokenList;
            }

            catch ( Exception ex )
            {
                return null;
            }
        }

    }
}
