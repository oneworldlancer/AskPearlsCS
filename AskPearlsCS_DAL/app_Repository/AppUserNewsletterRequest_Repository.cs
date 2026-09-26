

using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using AskPearlsCS_DOMAIN.app_UtilityManager;

using System.Text.Json;

namespace AskPearlsCS_DAL.app_Repository
{
    public class AppUserNewsletterRequest_Repository : IAppUserNewsletterRequest_Repository
    {


        private readonly DBContextModel _dbContext;

        public AppUserNewsletterRequest_Repository ( DBContextModel dbContext )
        {
            this ._dbContext= dbContext;    
        }

        public async Task<bool> AppRepository_CHECK_IsExist_UserNewsletterRequest_ByNewsletterRequestTokenID ( string objNewsletterRequestTokenID )
        {

            try
            {
                //        AppUserOwnerModelManager? iOwnerModel =
                //(AppUserOwnerModelManager) JsonConvert.DeserializeObject<AppUserOwnerModelManager> ( HttpContext.Session.GetString ( "iOwnerModel" ) );



                //string iBuildingTokenID = AppUtility_TimeManager.Time_GetCurrentTimeInMilliSecond ( ).ToString ( );


                var _iUserContactUsRequestModel = _dbContext.AskpearlsPortalUserNewsletterRequestLists
                        .Where ( u => u.NewsletterRequestTokenId == objNewsletterRequestTokenID )
                        //.Select (u  )
                        .FirstOrDefault ( ); // This is what actually executes the request and return a response

                if (_iUserContactUsRequestModel == null)
                {

                    return await Task.FromResult ( false );
                    //  return JsonResult ( new { message = "Hello, world!" } );
                }
                else
                {
                    return await Task.FromResult ( true );
                }
            }
            catch (Exception ex)
            {
                AppUtility_DebugManager.Debug_Get_MessageText ( ex.Message.ToString ( ) );

                return await Task.FromResult ( false );
            }



            //throw new NotImplementedException ( );

        }

      
        public async Task<bool> AppRepository_CHECK_IsExist_UserNewsletterRequest_ByUserEmailAddress ( string objUserEmailAddress )
        {

            try
            {
                //        AppUserOwnerModelManager? iOwnerModel =
                //(AppUserOwnerModelManager) JsonConvert.DeserializeObject<AppUserOwnerModelManager> ( HttpContext.Session.GetString ( "iOwnerModel" ) );



                //string iBuildingTokenID = AppUtility_TimeManager.Time_GetCurrentTimeInMilliSecond ( ).ToString ( );


                var _iUserContactUsRequestModel = _dbContext.AskpearlsPortalUserNewsletterRequestLists
                        .Where ( u => u.NewsletterMessageText.ToLower() == objUserEmailAddress.ToLower() )
                        //.Select (u  )
                        .FirstOrDefault ( ); // This is what actually executes the request and return a response

                if (_iUserContactUsRequestModel == null)
                {

                    return await Task.FromResult ( false );
                    //  return JsonResult ( new { message = "Hello, world!" } );
                }
                else
                {
                    return await Task.FromResult ( true );
                }
            }
            catch (Exception ex)
            {
                AppUtility_DebugManager.Debug_Get_MessageText ( ex.Message.ToString ( ) );

                return await Task.FromResult ( false );
            }



            //throw new NotImplementedException ( );

        }

        public async Task<List<AskpearlsPortalUserNewsletterRequestList>> AppRepository_GET_LIST_UserNewsletterRequest ( )
        {
            throw new NotImplementedException ( );
        }

     
        public async Task<bool> AppRepository_POST_NEW_UserNewsletterRequest (AskpearlsPortalUserNewsletterRequestList objUserNewsletterRequestModel )
        {


            try
            {

                await _dbContext.AskpearlsPortalUserNewsletterRequestLists.AddAsync ( objUserNewsletterRequestModel );

                await _dbContext.SaveChangesAsync ( );


                //        AppUserOwnerModelManager? iOwnerModel =
                //(AppUserOwnerModelManager) JsonConvert.DeserializeObject<AppUserOwnerModelManager> ( HttpContext.Session.GetString ( "iOwnerModel" ) );



                //string iBuildingTokenID = AppUtility_TimeManager.Time_GetCurrentTimeInMilliSecond ( ).ToString ( );


                //////////var _iUserContactUsRequestModel = _dbContext.AskpearlsPortalUserNewsletterRequestLists
                //////////        .Where ( u => u.NewsletterRequestTokenId == objUserNewsletterModel.NewsletterRequestTokenId )
                //////////        //.Select (u  )
                //////////        .FirstOrDefault ( ); // This is what actually executes the request and return a response

                //////////if (_iUserContactUsRequestModel == null)
                //////////{
                //////////    await _dbContext.AskpearlsPortalUserNewsletterRequestLists.AddAsync ( objUserNewsletterModel );

                //////////    await _dbContext.SaveChangesAsync ( );

                //////////    //  return true;
                //////////    //  return JsonResult ( new { message = "Hello, world!" } );
                //////////}


                //var data = new
                //{
                //    Id = objUserNewsletterModel.ContactUsRequestTokenId ,
                //    Status = "true" ,
                //    Message = "Created sucessfully" ,
                //    Description = "Created sucessfully"
                //};

                // return JsonSerializer.Serialize ( data );

                return true;
                // return false;


                //////////////////////////////////////////



                //return ViewComponent ( "BSRVemcoPage_BuildingList" );


                //   return Json ( new { status = true , Message = "success" } );





                //return ViewComponent("QueryFormTableDocumentList",
                //    new
                //    {
                //        bldtknid = bldtknid,
                //        iqueryinftknid = iqueryinftknid
                //    });



                //return ViewComponent ( "DocumentList" ,
                //    new { showPrevious = showPrevious , showUpcoming = showUpcoming } );


            }
            catch (Exception ex)
            {
                AppUtility_DebugManager.Debug_Get_MessageText ( ex.Message.ToString ( ) );
                //var data = new
                //{
                //    Id = objUserNewsletterModel.ContactUsRequestTokenId ,
                //    Status = "false" ,
                //    Message = "Error" ,
                //    Description = "Error"
                //};

                //return JsonSerializer.Serialize ( data );


                return false;
            }


        }
    }
}
