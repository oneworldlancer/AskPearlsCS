

using AskPearlsCS_DAL.DBContext;
using AskPearlsCS_DAL.DBModels;

using AskPearlsCS_DOMAIN.app_UtilityManager;
using AskPearlsCS_DOMAIN.app_ViewModels;

using System.Text.Json;

namespace AskPearlsCS_DAL.app_Repository
{
    public class AppSEOPage_Repository : IAppSEOPage_Repository
    {


        private readonly DBContextModel _dbContext;
     
        public AppSEOPage_Repository (
           DBContextModel dbContext )
        {
           this ._dbContext= dbContext;    
        }

        public async Task<bool> AppRepository_CHECK_IsExist_UserGuest_ByIPAddress ( string objUserIPAddress )
        {

            try
            {
                //        AppUserOwnerModelManager? iOwnerModel =
                //(AppUserOwnerModelManager) JsonConvert.DeserializeObject<AppUserOwnerModelManager> ( HttpContext.Session.GetString ( "iOwnerModel" ) );



                //string iBuildingTokenID = AppUtility_TimeManager.Time_GetCurrentTimeInMilliSecond ( ).ToString ( );


                var _iUserContactUsRequestModel = _dbContext.AskpearlsPortalUserGuestLists
                        .Where ( u => u.GuestUserIp == objUserIPAddress )
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

        public async Task<List<AskpearlsPortalUserGuestList>> AppRepository_GET_LIST_UserContactUsRequest ( )
        {
            throw new NotImplementedException ( );
        }

     
        public async Task<bool> AppRepository_POST_NEW_UserGuest_ByTraceTokenID ( AskpearlsPortalUserGuestList objUserGuestModel )
        {



            try
            {

                await _dbContext.AskpearlsPortalUserGuestLists.AddAsync ( objUserGuestModel );

                await _dbContext.SaveChangesAsync ( );


                //        AppUserOwnerModelManager? iOwnerModel =
                //(AppUserOwnerModelManager) JsonConvert.DeserializeObject<AppUserOwnerModelManager> ( HttpContext.Session.GetString ( "iOwnerModel" ) );



                //string iBuildingTokenID = AppUtility_TimeManager.Time_GetCurrentTimeInMilliSecond ( ).ToString ( );


                //////////var _iUserContactUsRequestModel = _dbContext.OneworldlancerPortalUserContactusRequestLists
                //////////        .Where ( u => u.ContactUsRequestTokenId == objUserContactUsRequestModel.ContactUsRequestTokenId )
                //////////        //.Select (u  )
                //////////        .FirstOrDefault ( ); // This is what actually executes the request and return a response

                //////////if (_iUserContactUsRequestModel == null)
                //////////{
                //////////    await _dbContext.OneworldlancerPortalUserContactusRequestLists.AddAsync ( objUserContactUsRequestModel );

                //////////    await _dbContext.SaveChangesAsync ( );

                //////////    //  return true;
                //////////    //  return JsonResult ( new { message = "Hello, world!" } );
                //////////}


                //var data = new
                //{
                //    Id = objUserContactUsModel.ContactUsRequestTokenId ,
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
                //    Id = objUserContactUsModel.ContactUsRequestTokenId ,
                //    Status = "false" ,
                //    Message = "Error" ,
                //    Description = "Error"
                //};

                //return JsonSerializer.Serialize ( data );


                return false;
            }
 
        }

        public async Task<PageViewModel> AppService_GET_SEOPage_ByJSONPageCode ( string PageFilePath , string PageSlugCode )
        {

            try
            {

                PageViewModel pageViewModel;

                if (string.IsNullOrEmpty ( PageSlugCode ))
                    PageSlugCode = "home";

                // Normalize
                PageSlugCode = PageSlugCode.ToLower ( );

                //var filePath = Path.Combine ( _env.WebRootPath , $"db/pages/app-page-{PageSlugCode}.json" );

                if (!System.IO.File.Exists ( PageFilePath ))
                    throw new FileNotFoundException ( "Page File JSON not found." );



                var json = System.IO.File.ReadAllText ( PageFilePath );

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                pageViewModel = new PageViewModel ( );



                pageViewModel = JsonSerializer.Deserialize<PageViewModel> ( json , options );


                //var json = System.IO.File.ReadAllText ( $"db/pages/app-page-{PageSlugCode}.json" );
                // var json = System.IO.File.ReadAllText ( "db/pages/app-page-about-us.json" );

                // pageViewModel = JsonSerializer.Deserialize<PageViewModel> ( json );



















                //////// Example: load from JSON / DB
                //////var page = PageService.GetPage ( PageSlugCode );

                //////if (page == null)
                //////    return NotFound ( );

                //////return View ( "DynamicPage" , page );


                // var json = System.IO.File.ReadAllText ( $"db/pages/app-page-{PageSlugCode}.json" );
                // var json = System.IO.File.ReadAllText ( "db/pages/app-page-about-us.json" );

                //  pageViewModel = new PageViewModel ( );

                //  pageViewModel = JsonSerializer.Deserialize<PageViewModel> ( json );















                //"Data/pages.json"
                //var json = System.IO.File.ReadAllText ( "db/pages/app-page-about-us.json" );

                //pageViewModel = new PageViewModel ( );

                //pageViewModel = JsonSerializer.Deserialize<PageViewModel> ( json );


                //  _pages = JsonSerializer.Deserialize<List<PageViewModel>> ( json );


                //////////pageViewModel = new PageViewModel
                //////////  {
                //////////      Title = "Home" ,
                //////////      PageSlugCode = "home" ,
                //////////      Content = "Welcome to the Home Page!" ,
                //////////      Keywords = "home, welcome"
                //////////  };
                //  pageViewModel = _pages.FirstOrDefault ( p => p.PageSlugCode == PageSlugCode ) ?? pageViewModel;
                //////////ViewData[ "appPageTitle" ] = pageViewModel.Title;
                //////////ViewData[ "appPageMetaDescription" ] = pageViewModel.Description;
                //////////ViewData[ "appPageKeywords" ] = pageViewModel.Keywords;
               //ViewBag.Content = pageViewModel.Content;   


                // async
                //"~/Views/Page/Index.cshtml" , 
                return pageViewModel ;



            }
            catch (Exception ex)
            {
                AppUtility_DebugManager.Debug_Get_MessageText ( ex.Message.ToString ( ) );

                return null ;
            }

            // throw new NotImplementedException ( );
        }
    }
}
