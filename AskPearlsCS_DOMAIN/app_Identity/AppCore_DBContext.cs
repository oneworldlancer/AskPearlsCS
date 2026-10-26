
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AskPearlsCS_DOMAIN.app_Identity
{

    public partial class AppCore_DBContext : IdentityDbContext<AppCore_IdentityUser, AppCore_IdentityRole, string >
    {

        public AppCore_DBContext ( DbContextOptions<AppCore_DBContext> options ) : base ( options )
        {
           //
        }


        protected override void OnModelCreating ( ModelBuilder builder )
        {

            base.OnModelCreating ( builder );

            //////////var keysProperties =
            //////////    builder.Model.GetEntityTypes ( )
            //////////    .Select ( x => x.FindPrimaryKey ( ) )
            //////////    .SelectMany ( x => x.Properties );

            //////////foreach ( var property in keysProperties )
            //////////{
            //////////    property.ValueGenerated = ValueGenerated.OnAdd;
            //////////}
        }

    }
}
