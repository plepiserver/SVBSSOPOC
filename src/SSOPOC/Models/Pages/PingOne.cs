using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace SSOPOC.Models.Pages
{
    [ContentType(DisplayName = "PingOne",
        GroupName = Global.GroupNames.Specialized,
        GUID = "fd31ec84-382d-4abc-a03a-df01f0d33e2b",
        Description = "")]
    public class PingOne : StandardPage
    {
        /*
                [CultureSpecific]
                [Display(
                    Name = "Main body",
                    Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
                public virtual XhtmlString MainBody { get; set; }
         */
    }
}