using Microsoft.AspNetCore.Http;

using AskPearlsCS_DOMAIN.app_UtilityManager;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Text.Json;
using System.Xml.Linq;
using System.Dynamic;

namespace AskPearlsCS_DOMAIN.app_DomainManager
{
    public class appDomain_UserEmailManager
    {






        public static async Task<bool> UserEmail_SEND_NEW_UserEmail_ByEmailCode (
            string EmailCode ,
            dynamic dynamic_UserEmailModel )
        {                                    /* bool showPrevious , bool showUpcoming */

            try
            {

                //dynamic dynamic_UserEmailModel = new ExpandoObject ( );

                //AppUtility_JsonManager.PopulateExpando ( dynamic_UserEmailModel , jsonUserEmailModel );




                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

              //    SmtpClient smtp = new SmtpClient ( "smtp.outlook.com" , 587 );
              SmtpClient smtp = new SmtpClient ( "smtp.office365.com" , 587 );

                smtp.UseDefaultCredentials = false;

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                smtp.EnableSsl = true;

                smtp.Credentials = new NetworkCredential (
                    "HelpDesk@OneWorldLancer.com" , "@goSHAY06021979" , "OneWorldLancer.com" );

                smtp.TargetName = "STARTTLS/smtp.office365.com";
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage msg;


                ArrayList _arrTokenList = new ArrayList ( );

                //_arrTokenList = AppUtility_ListTokenIDManager.ListTokenID_Generate_NewList ( _arrEmailList.Length.ToString ( ) );




                //////////string _genPassword = GetRandomAlphaNumericString ( 8 );
                //////////string emailToList = _arrEmailList[ i ];
                ////////////string emailToList = "oneworldlancer@gmail.com,basil.jackson@vemcoconsulting.com" + "," + _arrEmailList[ i ];
                //////////// string emailToList = "oneworldlancer@gmail.com" + "," + _arrEmailList[i];
                ////////////string emailToList = "oneworldlancer@gmail.com";
                ////////////string xRegID = AppUtility_TimeManager.Time_GetCurrentTimeInMilliSecond().ToString();
                //////////string xRegisterURL = "https://bsr.vemcoconsulting.com/Account/Register?regid=" + _arrTokenList[ i ];
                //////////string xPassword = "<b>" + _genPassword + "</b>";//"123456";
                //////////string valPassword = _genPassword; //"123456";
                //////////                                   //string xPassword = "<b>" + _arrTokenList[i] + "</b>";//"123456";
                //////////string xEmailAddress = "<span style='color:blue;'>" + _arrEmailList[ i ] + "</span>";
                //////////string valEmailAddress = _arrEmailList[ i ];
                //////////string xName = _arrUserNameList[ i ];//"Shaymaa Hafez";

                //////////string[] _arrUserFullNameList = xName.Split ( ' ' );
                //////////string fName = "0";
                //////////string lName = "0";

                //////////if (_arrUserFullNameList.Length > 1)
                //////////{
                //////////    fName = _arrUserFullNameList[ 0 ];
                //////////    lName = _arrUserFullNameList[ 1 ];

                //////////}
                //////////else
                //////////{
                //////////    fName = _arrUserFullNameList[ 0 ];
                //////////    lName = _arrUserFullNameList[ 0 ];
                //////////}

                string emailSubject =await UserEmail_GetEmailSubject_MessageText_ByEmailCode (
 EmailCode , dynamic_UserEmailModel );

                string emailBody =await UserEmail_GetEmailBody_MessageText_ByEmailCode (
EmailCode , dynamic_UserEmailModel );


                //                        string emailBody_X1 = @"Dear <b>" + xName + @"</b>,

                //<br/><br/>Welcome you to the BSR Building Portal, your all-in-one solution for efficient and compliant building management under the Building Act 2022.  Our platform has been meticulously crafted to empower you in overseeing multiple buildings effortlessly while ensuring strict adherence to the latest regulatory standards.

                //To get started, simply use the following login credentials:<br/><br/>

                //<ul>
                //<li>Url: " + xRegisterURL + @"</li>
                //<li>Username: " + xEmailAddress + @"</li>
                //<li>Password: " + xPassword + @"</li>
                //</ul>

                //<br/>
                //Here are a few key features that await you upon logging in:
                //<br/><br/>


                //<ul><li>
                //Building Portfolio Overview: Access a comprehensive overview of all your managed buildings, providing real-time insights into their status and compliance levels.
                //</li></ul>


                //<ul><li>
                //User Management: Invite and designate responsibilities to sub-users seamlessly. Collaborate effectively by assigning specific roles and permissions tailored to your team's needs.
                //</li></ul>


                //<ul><li>
                //Compliance Dashboard: Stay ahead of regulatory requirements with our intuitive compliance dashboard. Receive timely alerts and reminders to ensure your buildings meet and exceed the Building Act 2022 standards.
                //</li></ul>


                //<ul><li>
                //Document Repository: Centralise important documents related to your buildings. Easily manage and retrieve essential files to streamline communication and decision-making.
                //</li></ul>


                //<ul><li>
                //Notifications and Updates: Receive instant notifications regarding critical events, upcoming inspections, and compliance deadlines. Stay informed and in control at all times.
                //</li></ul>

                //<br/>
                //We understand that managing multiple buildings can be a complex task, and that's why we designed the BSR Building Portal to simplify the process for you. Your success is our priority, and we are confident that our app will elevate your building management experience.

                //<br/><br/>Should you have any questions or need assistance during the onboarding process, our dedicated support team is ready to help. Feel free to reach out to us at BSRHelpDesk@vemcoconsulting.com or through the in-app support feature.

                //<br/><br/>Thank you for choosing BSR Building Portal – where building management meets simplicity and compliance seamlessly!

                //<br/><br/>Best Regards,

                //<br/><b>BSRHelpDesk</b>";


                msg = new MailMessage (
                  "HelpDesk@OneWorldLancer.com" ,
                   dynamic_UserEmailModel.jsonEmailAddress.item_txt ,
                   emailSubject , emailBody );

                //msg = new MailMessage (
                //  "HelpDesk@OneWorldLancer.com" ,
                //   dynamic_UserEmailModel.jsonEmailAddress.item_txt ,
                //  "BSR invitation ID# " + _arrTokenList[ i ] , emailBody );


                msg.Bcc.Add ( new MailAddress ( "oneworldlancer@gmail.com" , "Shaymaa Hafez Ebrahiem" ) );
                //msg.Bcc.Add ( new MailAddress ( "basil.jackson@vemcoconsulting.com" , "Basil Jackson" ) );


                msg.IsBodyHtml = true;

                smtp.Send ( msg );

                //Clean up.
                msg.Dispose ( );



                return await Task.FromResult ( true );

                //return Json ( new { status = true , Message = "success" } );



            }
            catch (Exception ex)
            {
                AppUtility_DebugManager.Debug_Get_MessageText ( ex.Message.ToString ( ) );
                return await Task.FromResult ( false );
            }




            //return ViewComponent("QueryFormTableDocumentList",
            //    new
            //    {
            //        bldtknid = bldtknid,
            //        iqueryinftknid = iqueryinftknid
            //    });



            //return ViewComponent ( "DocumentList" ,
            //    new { showPrevious = showPrevious , showUpcoming = showUpcoming } );


        }










        public async Task<bool> UserEmail_POST_ContactUsPage_X1 (
            string iFullName ,
            string iEmailAddress ,
            string iMobilenumber ,
            string iMessageText )
        {                                    /* bool showPrevious , bool showUpcoming */

            try
            {


                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //MailMessage msg = new MailMessage(
                //    "BSRHelpDesk@vemcoconsulting.com", "basil.jackson@vemcoconsulting.com",
                //    "Test E-mail 1101", "Hi there ...");
                //
                string emailToList = "oneworldlancer@gmail.com,basil.jackson@vemcoconsulting.com" + "," + iEmailAddress;

                MailMessage msg = new MailMessage (
                    "BSRHelpDesk@vemcoconsulting.com" ,
                    emailToList ,
                    "Contact us - Ticket ID# " + AppUtility_TimeManager.Time_GetCurrentTimeInMilliSecond ( ).ToString ( ) , iMessageText );


                //MailMessage msg = new MailMessage(
                //    "BSRHelpDesk@vemcoconsulting.com",
                //    "shaymaa.hafez@yahoo.fr,oneworldlancer@gmail.com,hoby333@hotmail.com,oneworldlancer@oneworldlancer.onmicrosoft.com",
                //    "Test E-mail 8808", "Hi there 8808 ...");

                //MailMessage msg = new MailMessage(
                //        "BSRHelpDesk@vemcoconsulting.com",
                //        "oneworldlancer@gmail.com,hoby333@yahoo.com,oneworldlancer@yahoo.com",
                //        "Test E-mail 6606", "Hi there ...");

                //msg.Bcc.Add("oneworldlancer@yahoo.com");
                //msg.CC.Add("shaymaa.hafez@yahoo.fr");

                //msg.To.Add("hoby333@yahoo.com,oneworldlancer@yahoo.com");

                SmtpClient smtp = new SmtpClient ( "smtp.office365.com" , 587 );

                smtp.UseDefaultCredentials = false;

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                smtp.EnableSsl = true;

                smtp.Credentials = new NetworkCredential (
                    "bsrhelpdesk@vemcoconsulting.com" , "@goADMIN06021979" , "vemcoconsulting.com" );

                smtp.TargetName = "STARTTLS/smtp.office365.com";


                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                smtp.Send ( msg );

                //Clean up.

                msg.Dispose ( );

                return await Task.FromResult ( true );

            }
            catch (Exception ex)
            {
                AppUtility_DebugManager.Debug_Get_MessageText ( ex.Message.ToString ( ) );

                return await Task.FromResult ( false );
            }




            //return ViewComponent("QueryFormTableDocumentList",
            //    new
            //    {
            //        bldtknid = bldtknid,
            //        iqueryinftknid = iqueryinftknid
            //    });



            //return ViewComponent ( "DocumentList" ,
            //    new { showPrevious = showPrevious , showUpcoming = showUpcoming } );


        }









        public static async Task<string> UserEmail_GetEmailSubject_MessageText_ByEmailCode (
            string EmailCode ,
          dynamic dynamic_UserEmailModel )
        {
            try
            {
                string _iEmailSubject = "0";

                if (EmailCode.Equals ( "contact_us" ))
                {
                    _iEmailSubject = "ONEWORLDLANCER - Contact Us - Ticket ID# " + dynamic_UserEmailModel.iContactUsRequestTokenID;
                }
                else if (EmailCode.Equals ( "project_quote" ))
                {
                    _iEmailSubject = "ONEWORLDLANCER - Project Quote - Ticket ID# " + dynamic_UserEmailModel.iProjectQuoteRequestTokenID;

                }

                return await Task.FromResult ( _iEmailSubject );
            }
            catch (Exception ex)
            {
                AppUtility_DebugManager.Debug_Get_MessageText ( ex.Message.ToString ( ) );

                return await Task.FromResult ( "0" );
            }




        }







        public static async Task<string> UserEmail_GetEmailBody_MessageText_ByEmailCode (
            string EmailCode ,
          dynamic dynamic_UserEmailModel )
        {
            try
            {

                string _iEmailBody = "0";


                if (EmailCode.Equals ( "contact_us" ))
                {
                    _iEmailBody = @"<img src='https://oneworldlancer.com/app_img/logo_circle.jpg' style='width:50px;height:auto;'/><br/><br/>Dear <b>" + dynamic_UserEmailModel.jsonFirstName.item_txt + " " + dynamic_UserEmailModel.jsonLastName.item_txt + @"</b>,

<br/><br/>We are delighted to inform you that your contact us request has been successfully received. Thank you for taking the time to reach out to us. At <b>OneWorldLancer</b>, we prioritize open and transparent communication and are committed to addressing your inquiries and concerns promptly and effectively." +
"<br/><br/><strong>What to Expect Next? </strong>Our team of dedicated professionals is now reviewing your message. We strive to provide timely and accurate responses to all inquiries. You can expect a detailed reply from us within the next <b>48 hours</b>. Should we require any further information to better assist you, we will reach out to you directly." +
"<br/><br/><strong>Our Commitment to Excellence: </strong>At <b>OneWorldLancer</b>, we are passionate about delivering exceptional customer service. Your feedback and inquiries are invaluable to us as we continually seek to enhance our services and support. We are committed to ensuring that your experience with us is positive and exceeds your expectations." +
"<br/><br/><strong>Immediate Assistance: </strong>If your matter is urgent or if you require immediate assistance, please do not hesitate to contact us directly at Whatsapp <b><img src='https://oneworldlancer.com/app_img/img_whatsapp_black.png' style='width:15px;height:15px;' /> +201221977230</b> or reply to this email. Our team is available to provide the support you need and to ensure that your concerns are addressed without delay." +
"<br/><br/><strong>Stay Connected: </strong>For updates, news, and valuable information, we invite you to visit our website at <a href='https://oneworldlancer.com' target='_blank'><b>OneWorldLancer</b></a> or follow us on <a href='https://www.facebook.com/OneWorldLancer' target='_blank'><b>Facebook</b></a>. We are here to keep you informed and engaged with all that <b>OneWorldLancer</b> has to offer." +
"<br/><br/>Once again, thank you for contacting us. We appreciate the opportunity to assist you and look forward to resolving your inquiry to your satisfaction." +
"<br/><br/>Best Regards," +
"<br/><a href='https://oneworldlancer.com' target='_blank'><b>OneWorldLancer</b></a>" +
"<br/><img src='https://oneworldlancer.com/app_img/img_whatsapp_black.png' style='width:15px;height:15px;' /><b> +201221977230</b>" +
"<br/><b>HelpDesk Team</b>" +
"<br/><br/><img src='https://oneworldlancer.com/app_img/logo_default.png' style='width:300px;height:auto;' />";




                    // < br/><br/>Thank you for reaching out to us through our website's contact form. We appreciate your interest in <b>OneWorldLancer</b>." +

                    // We received your contact us message and we're currently reviewing your request and will get back to you as soon as possible, typically within <b>48 hours</b>.<br/><br/>
                    // In the meantime, if your inquiry is urgent, please feel free to call us at <b>+201221977230</b>.<br/><br/>
                    //  We look forward to connecting with you soon. 

                    //<br/><br/>Best Regards,

                    //<br/><b>HelpDesk Team</b>";
                }
                else if (EmailCode.Equals ( "project_quote" ))
                {
                    _iEmailBody = @"<img src='https://oneworldlancer.com/app_img/logo_circle.jpg' style='width:50px;height:auto;'/><br/><br/>Dear <b>" + dynamic_UserEmailModel.jsonFirstName.item_txt + " " + dynamic_UserEmailModel.jsonLastName.item_txt + @"</b>,
 <br/><br/>We are pleased to inform you that your project quote request for <b>" + dynamic_UserEmailModel.jsonProjectTitle.item_txt + "</b> has been successfully submitted. We appreciate you considering <b>OneWorldLancer</b> for your upcoming project." +
"<br/><br/><strong>What's Next? </strong>Our dedicated team of experts is now reviewing the details of your request. We are committed to providing you with a comprehensive and competitive quote that meets your project requirements. This process involves a thorough analysis of the information you've provided to ensure we deliver the best possible solution tailored to your needs." +
"<br/><br/><strong>Timeline: </strong>You can expect to receive your detailed project quote within the next <b>3-5 business days</b>. If there are any additional details or clarifications required, our team will reach out to you directly to ensure everything is perfectly aligned with your vision and objectives." +
"<br/><br/><strong>Our Commitment to You: </strong> At <b>OneWorldLancer</b>, we pride ourselves on delivering exceptional service and support. We understand the importance of your project and are dedicated to exceeding your expectations every step of the way. From initial consultation to project completion, we strive to provide a seamless and rewarding experience." +
"<br/><br/><strong>Stay Connected: </strong>For updates, news, and valuable information, we invite you to visit our website at <a href='https://oneworldlancer.com' target='_blank'><b>OneWorldLancer</b></a> or follow us on <a href='https://www.facebook.com/OneWorldLancer' target='_blank'><b>Facebook</b></a>. We are here to keep you informed and engaged with all that <b>OneWorldLancer</b> has to offer." +
"<br/><br/><strong>Contact Us: </strong>Should you have any questions, need further assistance, or wish to discuss any aspect of your project in more detail, please do not hesitate to contact us at Whatsapp <b><img src='https://oneworldlancer.com/app_img/img_whatsapp_black.png' style='width:15px;height:15px;' /> +201221977230</b> or reply to this email. Our team is here to assist you and ensure all your inquiries are addressed promptly and thoroughly." +
"<br/><br/>Thank you once again for choosing <b>OneWorldLancer</b>. We look forward to the opportunity to work with you and bring your project to life." +
"<br/><br/>Best Regards," +
"<br/><a href='https://oneworldlancer.com' target='_blank'><b>OneWorldLancer</b></a>" +
"<br/><img src='https://oneworldlancer.com/app_img/img_whatsapp_black.png' style='width:15px;height:15px;' /><b> +201221977230</b>" +
"<br/><b>HelpDesk Team</b>" +
"<br/><br/><img src='https://oneworldlancer.com/app_img/logo_default.png' style='width:300px;height:auto;' />";













                    //                    _iEmailBody = @"Dear <b>" + dynamic_UserEmailModel.jsonFirstName.item_txt + " " + dynamic_UserEmailModel.jsonLastName.item_txt + @"</b>,

                    //<br/><br/>Thank you for your interest in <b>OneWorldLancer</b> and for submitting your project quote request for <b>" + dynamic_UserEmailModel.jsonProjectTitle.item_txt + "</b>. We appreciate you considering us for your project.<br/><br/>" +
                    //"We've received your request and are currently reviewing the details you provided. We will prepare a personalized quote based on your specific needs and requirements outlined in the form.<br/><br/>" +
                    //"You can expect to receive your detailed quote within <b>3-5 business days</b>. This quote will include a breakdown of costs, timelines, and any other relevant information.<br/><br/>" +
                    //"In the meantime, if you have any urgent questions or would like to discuss your project further, please don't hesitate to contact us at <b>+201221977230</b> or reply to this email.<br/><br/>" +
                    // "We look forward to the opportunity to work with you on this project." +
                    //"<br/><br/>Best Regards," +
                    //"<br/><b>HelpDesk Team</b>";

                }
                else if (EmailCode.Equals ( "contact_us_2" ))
                {
                    _iEmailBody = @"Dear <b>" + dynamic_UserEmailModel.jsonFirstName.item_txt + " " + dynamic_UserEmailModel.jsonLastName.item_txt + @"</b>,

<br/><br/>Thank you for reaching out to us through our website's contact form. We appreciate your interest in <b>OneWorldLancer</b>.<br/><br/>
 We received your contact us message and we're currently reviewing your request and will get back to you as soon as possible, typically within <b>48 hours</b>.<br/><br/>
 In the meantime, if your inquiry is urgent, please feel free to call us at <b>+201221977230</b>.<br/><br/>
  We look forward to connecting with you soon. 
 
<br/><br/>Best Regards,

<br/><b>HelpDesk Team</b>";
                }
                else if (EmailCode.Equals ( "project_quote_2" ))
                {
                    _iEmailBody = @"Dear <b>" + dynamic_UserEmailModel.jsonFirstName.item_txt + " " + dynamic_UserEmailModel.jsonLastName.item_txt + @"</b>,

<br/><br/>Thank you for your interest in <b>OneWorldLancer</b> and for submitting your project quote request for <b>" + dynamic_UserEmailModel.jsonProjectTitle.item_txt + "</b>@. We appreciate you considering us for your project.<br/><br/>" +
"We've received your request and are currently reviewing the details you provided. We will prepare a personalized quote based on your specific needs and requirements outlined in the form.<br/><br/>" +
"You can expect to receive your detailed quote within <b>3-5 business days</b>. This quote will include a breakdown of costs, timelines, and any other relevant information.<br/><br/>" +
"In the meantime, if you have any urgent questions or would like to discuss your project further, please don't hesitate to contact us at <b>+201221977230</b> or reply to this email.<br/><br/>" +
 "We look forward to the opportunity to work with you on this project." +
"<br/><br/>Best Regards," +
"<br/><b>HelpDesk Team</b>";

                }


                return await Task.FromResult ( _iEmailBody );
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
