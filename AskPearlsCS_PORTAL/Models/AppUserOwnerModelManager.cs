using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AskPearlsCS_PORTAL.Models
{
    public class AppUserOwnerModelManager
    {

        public string OwnerUserTokenID { get; set; } = "0";
        public string OwnerMobileNumberTokenID { get; set; } = "0";
      
        public string UserSessionTokenID { get; set; } = "0";


        public string CompanyName { get; set; } = "0";
        public string CompanyTokenID { get; set; } = "0";
        public string UserName { get; set; } = "0";
        public string EmailAddress { get; set; } = "0";
        public string FirstName { get; set; } = "0";
        public string LastName { get; set; } = "0";
        public string AvatarName { get; set; } = "0";
        public string AvatarBackgroundColorCSS { get; set; } = "#000000";
        public string AvatarTextColorCSS { get; set; } = "#ffffff";
        public string FullName { get; set; } = "0";
        public string Password { get; set; } = "0";
        //public string EmailAddress { get; set; } = "0";
        public string RoleTokenID { get; set; } = "0";
        public string RoleCode { get; set; } = "0";
        public string RoleName { get; set; } = "0";
        public string MobileNumberE164 { get; set; } = "0";
        public string ProfileImageServerURL { get; set; } = "0";
        public string SignatureTokenID { get; set; } = "0";
        public string SignatureImageURL { get; set; } = "0";// "_iUMedia/_iUSignature/signature.jpg";//"_iUMedia/_iUSignature/img_" + "1710522715031" + ".png";





        //////[Required]
        //////[DisplayName ( "Confirm Password" )]
        //////[DataType ( DataType.Password )]
        //////[Compare ( "Password" , ErrorMessage = "Password and confirmation do not match." )]
        //////public string PasswordConfirm { get; set; }
    }
}
