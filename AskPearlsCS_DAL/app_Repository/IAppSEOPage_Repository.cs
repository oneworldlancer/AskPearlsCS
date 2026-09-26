using AskPearlsCS_DAL.DBModels;

using AskPearlsCS_DOMAIN.app_ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskPearlsCS_DAL.app_Repository
{
    public interface IAppSEOPage_Repository
    {
        public Task<PageViewModel> AppService_GET_SEOPage_ByJSONPageCode ( string PageFilePath , string PageSlugCode );


        //public Task<bool> AppRepository_CHECK_IsExist_UserGuest_ByIPAddress ( string objUserIPAddress );
        //public Task<bool> AppRepository_POST_NEW_UserGuest_ByTraceTokenID ( OneworldlancerPortalUserGuestList objUserGuestModel );
        //public Task<List<OneworldlancerPortalUserGuestList>> AppRepository_GET_LIST_UserContactUsRequest ( );

        //public List<EntityTodoModel> AllTodos ( );
    }
}
