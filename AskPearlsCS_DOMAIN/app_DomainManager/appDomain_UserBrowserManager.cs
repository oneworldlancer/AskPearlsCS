using Microsoft.AspNetCore.Http;

using AskPearlsCS_DOMAIN.app_UtilityManager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AskPearlsCS_DOMAIN.app_DomainManager
{
    public class appDomain_UserBrowserManager
    {

        public static async Task<string> UserBrowser_GetBrowserInfo ( IHttpContextAccessor _iHttpContextAccessor )
        {

            try
            {
                var userAgent = _iHttpContextAccessor.HttpContext?.Request.Headers[ "User-Agent" ].ToString ( );

                if (string.IsNullOrEmpty ( userAgent ))
                {
                    //return Ok ( new { Browser = "Unknown" } ); // Or return a more appropriate error response
                    return "Unknown";


                }

                // Basic parsing (can be improved with a dedicated User-Agent parser library)
                string browser = "Unknown";
                if (userAgent.Contains ( "Chrome" ))
                {
                    browser = "Chrome";
                }
                else if (userAgent.Contains ( "Firefox" ))
                {
                    browser = "Firefox";
                }
                else if (userAgent.Contains ( "Edge" ))
                {
                    browser = "Edge";
                }
                else if (userAgent.Contains ( "Safari" ))
                {
                    browser = "Safari";
                }
                else if (userAgent.Contains ( "MSIE" ) || userAgent.Contains ( "Trident" )) // For Internet Explorer
                {
                    browser = "Internet Explorer";
                }

                // return Ok ( new { Browser = browser } );

                return await Task.FromResult ( browser );
            }
            catch (Exception ex)
            {
                AppUtility_DebugManager.Debug_Get_MessageText ( ex.Message.ToString ( ) );

                return await Task.FromResult ( "0" );
            }




        }


        public static async Task<string> UserBrowser_GetClientIpAddress ( IHttpContextAccessor _iHttpContextAccessor )
        {


            try
            {
                var request = _iHttpContextAccessor.HttpContext?.Request;
                if (request == null) return await Task.FromResult ( "Unknown" );

                // Check for forwarded headers (useful for proxies, load balancers)
                string ip = request.Headers[ "X-Forwarded-For" ].FirstOrDefault ( ); // Common header
                if (!string.IsNullOrEmpty ( ip ))
                {
                    return await Task.FromResult ( ip.Split ( ',' )[ 0 ].Trim ( ) );  // Get the first IP in the list
                }

                ip = request.Headers[ "CF-Connecting-IP" ].FirstOrDefault ( ); // Cloudflare header
                if (!string.IsNullOrEmpty ( ip ))
                {
                    return await Task.FromResult ( ip );
                }

                ip = request.Headers[ "REMOTE_ADDR" ].FirstOrDefault ( ); // Older header, sometimes available
                if (!string.IsNullOrEmpty ( ip ))
                {
                    return await Task.FromResult ( ip );
                }

                // Fallback to the connection's remote IP (less reliable, especially behind proxies)
                if (request.HttpContext?.Connection.RemoteIpAddress != null)
                {
                    return await Task.FromResult ( request.HttpContext.Connection.RemoteIpAddress.ToString ( ) );
                }

                return await Task.FromResult ( "Unknown" );
            }
            catch (Exception ex)
            {
                AppUtility_DebugManager.Debug_Get_MessageText ( ex.Message.ToString ( ) );

                return await Task.FromResult ( "0" );
            }


        }


    }
}
