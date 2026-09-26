using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AskPearlsCS_DOMAIN.app_ViewModels
{

    public class PageViewModel
    {
        public string Title { get; set; } = "0";
        public string Title_Code { get; set; } = "0";

        public string Title_Slug_Code { get; set; } = "0";
        public string Title_Slug_text { get; set; } = "0";

        public string Title_Path_Code { get; set; } = "0";
        public string Title_Path_Text { get; set; } = "0";

        public string Title_Path_Slug_Code { get; set; } = "0";
        public string Title_Path_Slug_Text { get; set; } = "0";

        public string Title_SEOView { get; set; } = "0";
        public string Title_PageView { get; set; } = "0";

        public string Page_Category_Code { get; set; } = "0";
        public string Page_Category_Text { get; set; } = "0";
     
        public string Page_Slug { get; set; } = "0";
        public string Page_Title { get; set; } = "0";
        public string Page_Name { get; set; } = "0";
      
        public string Page_Parent_Code { get; set; } = "0";
        public string Page_Parent_Text { get; set; } = "0";

        public HTMLContentModel PageHTMLContentModel { get; set; } = new ();


        [JsonPropertyName ( "metaseotagmodel" )]
        public MetaSEOTagModel PageMetaSEOTagModel { get; set; } = new MetaSEOTagModel ( );
     
        [JsonPropertyName ( "htmlcontentmodel" )]
        public List<HTMLContentModel> arrPageHTMLContentList { get; set; } = new  ( );



        //public string HTMLContent { get; set; } = "0";
        //public HeaderModel Header { get; set; } = new HeaderModel ( );
        //public List<SectionModel> Sections { get; set; } = new ( );
        //public List<SectionModel> Sections { get; set; }    = new() ;
    }

    public class HeaderModel
    {
        public string Headline { get; set; } = "0";
        public string SubHeadline { get; set; } = "0";
    }

    public class SectionModel
    {
        public string Title { get; set; } = "0";
        public string Body { get; set; } = "0";
    }



    public class PageListCollection
    {
        public List<PageViewModel> arrPagesList { get; set; } = new ( );
    }



    public class MetaSEOTagModel
    {
        public string Meta_SEO_Tag_keywords { get; set; } = "0";
        public string Meta_SEO_Tag_Description { get; set; } = "0";
        public string Meta_SEO_Tag_Robots { get; set; } = "0";
        public string Meta_SEO_Tag_Canonical { get; set; } = "0";
        public string Meta_SEO_Tag_Graph_title { get; set; } = "0";
        public string Meta_SEO_Tag_Graph_Description { get; set; } = "0";
        public string Meta_SEO_Tag_Graph_Image_URL { get; set; } = "0";
        public string Meta_SEO_Tag_Graph_page_URL { get; set; } = "0";
        public string Meta_SEO_Tag_Graph_type { get; set; } = "0";
        public string Meta_SEO_Tag_Twitter_title { get; set; } = "0";
        public string Meta_SEO_Tag_Twitter_Description { get; set; } = "0";
        public string Meta_SEO_Tag_Twitter_Image_URL { get; set; } = "0";
        public string Meta_SEO_Tag_Twitter_card_Image_URL { get; set; } = "0";

        //public string Body { get; set; }   = "0";
        //public string Title { get; set; }  = "0";
        //public string Body { get; set; }   = "0";
        //public string Title { get; set; }  = "0";
        //public string Body { get; set; }   = "0";
        //public string Title { get; set; }  = "0";
        //public string Body { get; set; }   = "0";
        //public string Title { get; set; }  = "0";
        //public string Body { get; set; }   = "0";
        //public string Title { get; set; }  = "0";
        //public string Body { get; set; }   = "0";
        //public string Title { get; set; }  = "0";
        //public string Body { get; set; }   = "0";
        //public string Title { get; set; }  = "0";
        //public string Body { get; set; }   = "0";
        //public string Title { get; set; }  = "0";
        //public string Body { get; set; }   = "0";
    }








    public class HTMLContentModel
    {
        public string Headline { get; set; } = "0";
        public string SubHeadline { get; set; } = "0";
        public string HeroImageURL { get; set; } = "0";
        public string Title { get; set; } = "0";
        public string BodyContent { get; set; } = "0";
    }



    //public class PageViewModel
    //{
    //    public string Title { get; set; } = "0";
    //    public string Slug { get; set; }    = "0";
    //    public string Content { get; set; }   = "0";
    //    public string Keywords { get; set; }    = "0";
    //}
}
