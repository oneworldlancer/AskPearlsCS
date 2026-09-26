using AskPearlsCS_DAL.DBModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskPearlsCS_DAL.app_Repository
{
    public interface IAppUserNewsletterRequest_Repository
    {
        public Task<bool> AppRepository_CHECK_IsExist_UserNewsletterRequest_ByNewsletterRequestTokenID ( string objNewsletterRequestTokenID );
        public Task<bool> AppRepository_CHECK_IsExist_UserNewsletterRequest_ByUserEmailAddress ( string objUserEmailAddress );
        public Task<bool> AppRepository_POST_NEW_UserNewsletterRequest ( AskpearlsPortalUserNewsletterRequestList objUserProjectQuoteModel );
        public Task<List<AskpearlsPortalUserNewsletterRequestList>> AppRepository_GET_LIST_UserNewsletterRequest ( );
      
        //public List<EntityTodoModel> AllTodos ( );
    }
}
