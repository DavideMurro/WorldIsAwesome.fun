#pragma checksum "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "dc91ea3dc18d14e3df1a54adcd07cb6901ad7a24"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared__Header), @"mvc.1.0.view", @"/Views/Shared/_Header.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"dc91ea3dc18d14e3df1a54adcd07cb6901ad7a24", @"/Views/Shared/_Header.cshtml")]
    public class Views_Shared__Header : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
  
    ViewBag.IsUserSet = ViewBag.IsUserSet != null ? ViewBag.IsUserSet : false;
    ViewBag.IsMine = ViewBag.IsMine != null ? ViewBag.IsMine : false;
    ViewBag.IsHome = ViewBag.IsHome != null ? ViewBag.IsHome : false;
    ViewBag.IsSearchable = ViewBag.IsSearchable != null ? ViewBag.IsSearchable : false;
    ViewBag.IsParallax = ViewBag.IsParallax != null ? ViewBag.IsParallax : false;
    ViewBag.IsLogin = ViewBag.IsLogin != null ? ViewBag.IsLogin : false;

    if (ViewBag.IsUserLogged)
    {
        ViewBag.UserLogged_PictureStream_Preview = Context.Request.Scheme + "://" + Context.Request.Host + Url.Action("GetUserPictureStream", "View", new { pictureId = ViewBag.UserLogged.ProfilePhotoFileId, isPreview = true });
    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"

<header id=""global-header-container"">
    <div id=""home-button"" class=""header-button"" onclick=""openPopup('home-popup')"">
        <img src=""/images/logo_icon.svg"" class=""icon"" alt=""world is awesome .fun"" title=""World is Awesome .fun"" />
        <!--<span class=""text"">Menu</span>-->
    </div>

    <h4 id=""header-title"">");
#nullable restore
#line 22 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
                     Write(ViewBag.HeaderTitle);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h4>\r\n\r\n");
#nullable restore
#line 24 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
     if (ViewBag.IsSearchable)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        <div id=""search-button"" class=""header-button"" onclick=""toggleExperiencePlacesClick()"">
            <!--<img src=""/images/icons/search.svg"" class=""icon"" alt=""search"" title=""Search"" align=""center"" />-->
            <div class=""icon"" title=""Search""></div>
            <span class=""text"">Search</span>
        </div>
");
#nullable restore
#line 31 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    <div class=\"flex-auto\"></div>\r\n\r\n");
#nullable restore
#line 35 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
     if (!ViewBag.IsUserLogged && !ViewBag.IsLogin)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <a href=\"/Login\" title=\"Login\" id=\"login-button\" class=\"header-button\">\r\n            <span class=\"text ml-0 mr-05\">Login</span>\r\n            <img src=\"/images/icons/login_avatar.svg\" class=\"icon\" alt=\"login\" title=\"Login\" />\r\n        </a>\r\n");
#nullable restore
#line 41 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
    }
    else if (ViewBag.IsUserLogged)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <a");
            BeginWriteAttribute("href", " href=\"", 1889, "\"", 1936, 2);
            WriteAttributeValue("", 1896, "/View/User?UserId=", 1896, 18, true);
#nullable restore
#line 44 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 1914, ViewBag.UserLogged.Id, 1914, 22, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" title=\"My Profile\" id=\"user-logged-button\" class=\"header-button ismine\">\r\n            <div class=\"text ml-0 mr-05\">");
#nullable restore
#line 45 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
                                    Write(ViewBag.UserLogged.Nickname);

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\r\n            <img");
            BeginWriteAttribute("src", " src=\"", 2105, "\"", 2152, 1);
#nullable restore
#line 46 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 2111, ViewBag.UserLogged_PictureStream_Preview, 2111, 41, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("\r\n                 onerror=\"this.src=\'/images/icons/user_avatar_default.svg\'\" class=\"icon\" alt=\"profile\"\r\n                 title=\"Profile\" />\r\n        </a>\r\n");
#nullable restore
#line 50 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("</header>\r\n\r\n<div class=\"popup-container\" id=\"home-popup\" onclose=\"closePopup(\'home-popup\')\">\r\n    <div class=\"popup small\">\r\n        <div class=\"popup-header d-flex align-items\">\r\n");
#nullable restore
#line 56 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
             if (ViewBag.IsHome)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <div class=\"popup-title flex-auto logo-title\">\r\n                    <h1>World is Awesome</h1>\r\n                    <h2>Pin your world</h2>\r\n                </div>\r\n");
#nullable restore
#line 62 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
            }
            else
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <h2 class=\"popup-title flex-auto\">Menu</h2>\r\n");
#nullable restore
#line 66 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            <img src=\"/images/icons/close.svg\" title=\"Close\" alt=\"close\" class=\"icon\"\r\n                 onclick=\"closePopup(\'home-popup\')\" />\r\n        </div>\r\n        <div class=\"popup-body\">\r\n");
#nullable restore
#line 72 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
             if (ViewBag.IsParallax)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <button onclick=""startHomeView()"" title=""Go top"" class=""button user-option-item home-view-hide"">
                    <img src=""/images/icons/arrowup.svg"" title=""Go top"" alt=""go top"" class=""icon first"" />
                    Go top
                </button>
                <!--<div class=""divider-horizontal home-view-hide""></div>-->
");
#nullable restore
#line 79 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 81 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
             if (!ViewBag.IsHome)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <a href=""/"" title=""Home"" class=""button user-option-item"">
                    <img src=""/images/logo_icon.svg"" title=""Home"" alt=""home"" class=""icon first"" />
                    Home page
                </a>
                <div class=""divider-horizontal""></div>
");
#nullable restore
#line 88 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n");
#nullable restore
#line 91 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
             if (!ViewBag.IsUserLogged && !ViewBag.IsUserSet)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <a href=""/Login?Redirect=/InsertExperience"" title=""New experience""
                   class=""button user-option-item large"" id=""newexperience-button-notlogged"">
                    <img src=""/images/icons/experiencenew.svg"" title=""New experience"" alt=""new experience""
                         class=""icon first"" />
                    Insert a new Experience now!
                </a>
                <div class=""divider-horizontal""></div>
");
#nullable restore
#line 100 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 102 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
             if (ViewBag.IsUserSet && !ViewBag.IsMine)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <a");
            BeginWriteAttribute("href", " href=\"", 4471, "\"", 4512, 2);
            WriteAttributeValue("", 4478, "/View/User?UserId=", 4478, 18, true);
#nullable restore
#line 104 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 4496, ViewBag.User.Id, 4496, 16, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" title=\"Profile\" class=\"button user-option-item\" id=\"profile-button\">\r\n                    <img src=\"/images/icons/user_avatar_default.svg\" title=\"Profile\" alt=\"profile\" class=\"icon first\" />\r\n                    <span class=\'text\'><b>");
#nullable restore
#line 106 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
                                     Write(ViewBag.User.Nickname);

#line default
#line hidden
#nullable disable
            WriteLiteral("</b>\'s Profile </span>\r\n                </a>\r\n                <a");
            BeginWriteAttribute("href", " href=\"", 4834, "\"", 4879, 2);
            WriteAttributeValue("", 4841, "/Search/Places?UserId=", 4841, 22, true);
#nullable restore
#line 108 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 4863, ViewBag.User.Id, 4863, 16, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" title=""Profile Places"" class=""button user-option-item""
                   id=""searchprofileplaces-button"">
                    <img src=""/images/icons/place.svg"" title=""Profile Places"" alt=""profile places"" class=""icon first"" />
                    <span class='text'><b>");
#nullable restore
#line 111 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
                                     Write(ViewBag.User.Nickname);

#line default
#line hidden
#nullable disable
            WriteLiteral("</b>\'s Places </span>\r\n                </a>\r\n                <a");
            BeginWriteAttribute("href", " href=\"", 5239, "\"", 5289, 2);
            WriteAttributeValue("", 5246, "/Search/Experiences?UserId=", 5246, 27, true);
#nullable restore
#line 113 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 5273, ViewBag.User.Id, 5273, 16, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" title=""Profile Experiences"" class=""button user-option-item""
                   id=""searchprofileexperiences-button"">
                    <img src=""/images/icons/experiencemorning.svg"" title=""profile Experiences"" alt=""profile experiences""
                         class=""icon first"" />
                    <span class='text'><b>");
#nullable restore
#line 117 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
                                     Write(ViewBag.User.Nickname);

#line default
#line hidden
#nullable disable
            WriteLiteral("</b>\'s Experiences </span>\r\n                </a>\r\n                <a");
            BeginWriteAttribute("href", " href=\"", 5712, "\"", 5772, 2);
            WriteAttributeValue("", 5719, "/Search/People?ViewTypeEnum=4&UserId=", 5719, 37, true);
#nullable restore
#line 119 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 5756, ViewBag.User.Id, 5756, 16, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" title=""Profile Friends"" class=""button user-option-item""
                   id=""searchprofilefriends-button"">
                    <img src=""/images/icons/friend.svg"" title=""profile Friends"" alt=""profile friends""
                         class=""icon first"" />
                    <span class='text'><b>");
#nullable restore
#line 123 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
                                     Write(ViewBag.User.Nickname);

#line default
#line hidden
#nullable disable
            WriteLiteral("</b>\'s Friends </span>\r\n                </a>\r\n                <div class=\"divider-horizontal\" id=\"profile-separator\"></div>\r\n");
#nullable restore
#line 126 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
            }

#line default
#line hidden
#nullable disable
#nullable restore
#line 127 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
             if (ViewBag.IsUserLogged)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <a href=""/InsertExperience"" title=""New experience"" class=""button user-option-item""
                   id=""newexperience-button"">
                    <img src=""/images/icons/experiencenew.svg"" title=""New experience"" alt=""new experience""
                         class=""icon first"" />
                    New Experience
                </a>
                <a");
            BeginWriteAttribute("href", " href=\"", 6674, "\"", 6721, 2);
            WriteAttributeValue("", 6681, "/View/User?UserId=", 6681, 18, true);
#nullable restore
#line 135 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 6699, ViewBag.UserLogged.Id, 6699, 22, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" title=""My Profile"" class=""button user-option-item"" id=""myprofile-button"">
                    <img src=""/images/icons/user_avatar_default.svg"" title=""My Profile"" alt=""my profile""
                         class=""icon first"" />
                    <span class=""text""><b>My</b> Profile</span>
                </a>
                <a");
            BeginWriteAttribute("href", " href=\"", 7057, "\"", 7095, 2);
            WriteAttributeValue("", 7064, "/?UserId=", 7064, 9, true);
#nullable restore
#line 140 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 7073, ViewBag.UserLogged.Id, 7073, 22, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" title=""My World Profile"" class=""button user-option-item""
                   id=""myworldprofile-button"">
                    <img src=""/images/logo_icon.svg"" title=""My World Profile"" alt=""my world profile""
                         class=""icon first"" />
                    <span class=""text""><b>My</b> World Profile</span>
                </a>
                <a");
            BeginWriteAttribute("href", " href=\"", 7464, "\"", 7515, 2);
            WriteAttributeValue("", 7471, "/Search/Places?UserId=", 7471, 22, true);
#nullable restore
#line 146 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 7493, ViewBag.UserLogged.Id, 7493, 22, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" title=""My Places"" class=""button user-option-item"" id=""searchmyplaces-button"">
                    <img src=""/images/icons/my_place.svg"" title=""My Places"" alt=""my places"" class=""icon first"" />
                    <span class=""text""><b>My</b> Places</span>
                </a>
                <a");
            BeginWriteAttribute("href", " href=\"", 7815, "\"", 7871, 2);
            WriteAttributeValue("", 7822, "/Search/Experiences?UserId=", 7822, 27, true);
#nullable restore
#line 150 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 7849, ViewBag.UserLogged.Id, 7849, 22, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" title=""My Experiences"" class=""button user-option-item""
                   id=""searchmyexperiences-button"">
                    <img src=""/images/icons/experiencemorning.svg"" title=""My Experiences"" alt=""my experiences""
                         class=""icon first"" />
                    <span class=""text""><b>My</b> Experiences &amp; Drafts</span>
                </a>
                <a");
            BeginWriteAttribute("href", " href=\"", 8264, "\"", 8330, 2);
            WriteAttributeValue("", 8271, "/Search/People?ViewTypeEnum=4&UserId=", 8271, 37, true);
#nullable restore
#line 156 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
WriteAttributeValue("", 8308, ViewBag.UserLogged.Id, 8308, 22, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" title=""My Friends &amp; Request"" class=""button user-option-item""
                   id=""searchmyfriends-button"">
                    <img src=""/images/icons/friend.svg"" title=""My Friends &amp; Request"" alt=""my friends and request""
                         class=""icon first"" />
                    <span class=""text""><b>My</b> Friends &amp; Request</span>
                </a>
                <div class=""divider-horizontal""></div>
");
#nullable restore
#line 163 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            <a href=""/Search/People"" title=""Search People"" class=""button user-option-item"">
                <img src=""/images/icons/people_search.svg"" title=""Search People"" alt=""search people""
                     class=""icon first"" />
                <span class='text'>Search <b>Global</b> People</span>
            </a>
            <a href=""/Search/Places"" title=""Search Places"" class=""button user-option-item"">
                <img src=""/images/icons/place_search.svg"" title=""Search Places"" alt=""search places""
                     class=""icon first"" />
                <span class='text'>Search <b>Global</b> Places</span>
            </a>
            <a href=""/Search/Experiences"" title=""Search Experiences"" class=""button user-option-item"">
                <img src=""/images/icons/experience_search.svg"" title=""Search Experiences"" alt=""search experiences""
                     class=""icon first"" />
                <span class='text'>Search <b>Global</b> Experiences</span>
            </a>
            ");
            WriteLiteral("<div class=\"divider-horizontal\"></div>\r\n");
#nullable restore
#line 180 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
             if (!ViewBag.IsUserLogged)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                <a href=""/SignUp"" title=""Signup"" class=""button user-option-item"">
                    <img src=""/images/icons/login_avatar.svg"" title=""Signup"" alt=""signup"" class=""icon first"" />
                    Signup
                </a>
                <a href=""/Login"" title=""Login"" class=""button user-option-item"">
                    <img src=""/images/icons/login_avatar.svg"" title=""Login"" alt=""login"" class=""icon first"" />
                    Login
                </a>
");
#nullable restore
#line 190 "C:\Users\Betacom\Documents\GitHub\WorldIsAwesome.fun\www.worldisawesome.fun\Views\Shared\_Header.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"            <a href=""/ContactUs"" title=""Contact us"" class=""button user-option-item"">
                <img src=""/images/icons/mail.svg"" title=""Contact us"" alt=""contact us"" class=""icon first"" />
                Contact us
            </a>
        </div>
    </div>
</div>
");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591