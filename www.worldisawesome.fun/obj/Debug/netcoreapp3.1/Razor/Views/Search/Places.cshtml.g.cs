#pragma checksum "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "d640ff2bda292c072b38aff0f8e7229f40e4f3b7c73f3e046297fdef297a985f"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Search_Places), @"mvc.1.0.view", @"/Views/Search/Places.cshtml")]
namespace AspNetCore
{
    #line default
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Threading.Tasks;
    using global::Microsoft.AspNetCore.Mvc;
    using global::Microsoft.AspNetCore.Mvc.Rendering;
    using global::Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 2 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
     using www.worldisawesome.fun;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"Sha256", @"d640ff2bda292c072b38aff0f8e7229f40e4f3b7c73f3e046297fdef297a985f", @"/Views/Search/Places.cshtml")]
    #nullable restore
    public class Views_Search_Places : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/lib/font_nunito/Nunito.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("rel", new global::Microsoft.AspNetCore.Html.HtmlString("stylesheet"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/css/style_material.min.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("href", new global::Microsoft.AspNetCore.Html.HtmlString("~/css/style.min.css"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/script_vars.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", new global::Microsoft.AspNetCore.Html.HtmlString("text/javascript"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/script_apicall.min.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_7 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("src", new global::Microsoft.AspNetCore.Html.HtmlString("~/js/script_search_places.js"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
                                  

    if (ViewBag.IsUserSet)
    {
        ViewBag.PictureStream_Preview = Context.Request.Scheme + "://" + Context.Request.Host + Url.Action("GetUserPictureStream", "View", new { pictureId = ViewBag.User.ProfilePhotoFileId, isPreview = true });
    }

#line default
#line hidden
#nullable disable

            WriteLiteral("\r\n<!DOCTYPE html>\r\n<html lang=\"en\">\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("head", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d640ff2bda292c072b38aff0f8e7229f40e4f3b7c73f3e046297fdef297a985f6588", async() => {
                WriteLiteral("\r\n");
#nullable restore
#line 14 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
       await Html.RenderPartialAsync("_HeadBase"); 

#line default
#line hidden
#nullable disable

                WriteLiteral(@"    <title>Search places | World is Awesome .fun</title>

    <meta name=""description"" content=""Search places in World is Awesome .fun, WiA"" />
    <meta name=""keywords""
          content=""World is Awesome, World is Awesome .fun, World, Awesome, fun, wia, wia.fun, wia fun, experience, Diary, Travel, Trip, traveller, photo, video, search, places, place, city, country"">


    <!-- external -->
    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "d640ff2bda292c072b38aff0f8e7229f40e4f3b7c73f3e046297fdef297a985f7538", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n\r\n    <!-- internal -->\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "d640ff2bda292c072b38aff0f8e7229f40e4f3b7c73f3e046297fdef297a985f8770", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("link", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagOnly, "d640ff2bda292c072b38aff0f8e7229f40e4f3b7c73f3e046297fdef297a985f9973", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d640ff2bda292c072b38aff0f8e7229f40e4f3b7c73f3e046297fdef297a985f11180", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_4);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d640ff2bda292c072b38aff0f8e7229f40e4f3b7c73f3e046297fdef297a985f12391", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_6);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("script", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d640ff2bda292c072b38aff0f8e7229f40e4f3b7c73f3e046297fdef297a985f13602", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.UrlResolutionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_UrlResolutionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_7);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.HeadTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("body", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "d640ff2bda292c072b38aff0f8e7229f40e4f3b7c73f3e046297fdef297a985f15521", async() => {
                WriteLiteral("\r\n");
#nullable restore
#line 35 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
       await Html.RenderPartialAsync("_Base"); 

#line default
#line hidden
#nullable disable

                WriteLiteral("\r\n\r\n");
#nullable restore
#line 38 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
       await Html.RenderPartialAsync("_Header"); 

#line default
#line hidden
#nullable disable

                WriteLiteral("\r\n\r\n    <div class=\"article-container\" id=\"places-search-list-popup\">\r\n\r\n        <div class=\"article\">\r\n");
#nullable restore
#line 44 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
             if (ViewBag.IsUserSet)
            {

#line default
#line hidden
#nullable disable

                WriteLiteral("                <div class=\"profile-selected-container\">\r\n                    <a");
                BeginWriteAttribute("href", " href=\"", 1703, "\"", 1744, 2);
                WriteAttributeValue("", 1710, "/View/User?UserId=", 1710, 18, true);
                WriteAttributeValue("", 1728, 
#nullable restore
#line 47 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
                                                ViewBag.User.Id

#line default
#line hidden
#nullable disable
                , 1728, 16, false);
                EndWriteAttribute();
                WriteLiteral(" title=\"Profile picture\" id=\"userselected-button\" class=\"profile-selected\">\r\n                        <img");
                BeginWriteAttribute("src", " src=\"", 1850, "\"", 1886, 1);
                WriteAttributeValue("", 1856, 
#nullable restore
#line 48 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
                                   ViewBag.PictureStream_Preview

#line default
#line hidden
#nullable disable
                , 1856, 30, false);
                EndWriteAttribute();
                WriteLiteral(" onerror=\"this.src=\'/images/icons/user_avatar_default.svg\'\" class=\"icon\"\r\n                             alt=\"profile picture\" />\r\n                        <h3 class=\"text\">");
                Write(
#nullable restore
#line 50 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
                                          ViewBag.User.Nickname

#line default
#line hidden
#nullable disable
                );
                WriteLiteral("</h3>\r\n                    </a>\r\n                </div>\r\n");
#nullable restore
#line 53 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
            }

#line default
#line hidden
#nullable disable

                WriteLiteral("\r\n            <div class=\"article-header d-flex align-items\">\r\n                <img src=\"/images/icons/place_search.svg\" class=\"article-icon\" alt=\"search places\" />\r\n                <h2 class=\"article-title flex-auto\">\r\n                    ");
                Write(
#nullable restore
#line 58 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
                      ViewBag.IsUserSet ? "Places" : "Global Places"

#line default
#line hidden
#nullable disable
                );
                WriteLiteral(@"
                </h2>
            </div>
            <div class=""article-body"">
                <div class=""search-list-container"">
                    <div class=""input-container search-container"" id=""places-search-container"">
                        <label for=""places-search"" class=""label"">Search</label>
                        <div class=""d-flex align-items"">
                            <input type=""search"" placeholder=""Bogotá, Bogota D.C., Colombia"" id=""places-search""
                                   class=""input"" />

                            <div id=""morningnight-button"" class=""small icon ml-05"" title=""Change daytime"" onclick=""changeMorningNight()""></div>
");
#nullable restore
#line 70 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
                             if (ViewBag.IsUserSet)
                            {

#line default
#line hidden
#nullable disable

                WriteLiteral("                                <div id=\"changetype-button\" class=\"small icon ml-05\" title=\"Change view\" onclick=\"changeView()\"></div>\r\n");
#nullable restore
#line 73 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
                            }

#line default
#line hidden
#nullable disable

                WriteLiteral(@"                        </div>
                    </div>
                    <div class=""t-left"">
                        <b id=""place-count""></b>
                    </div>
                    <div class=""list-container"" id=""places-list-container""></div>
                </div>
            </div>
        </div>
    </div>


");
#nullable restore
#line 86 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
       await Html.RenderPartialAsync("_Footer"); 

#line default
#line hidden
#nullable disable

            }
            );
            __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.Razor.TagHelpers.BodyTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            BeginAddHtmlAttributeValues(__tagHelperExecutionContext, "class", 3, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            AddHtmlAttributeValue("", 1276, "loading", 1276, 7, true);
            AddHtmlAttributeValue(" ", 1283, 
#nullable restore
#line 34 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
                       ViewBag.IsUserLogged ? "logged" : ""

#line default
#line hidden
#nullable disable
            , 1284, 39, false);
            AddHtmlAttributeValue(" ", 1323, 
#nullable restore
#line 34 "D:\GitHub\world-is-awesome-fun\www.worldisawesome.fun\Views\Search\Places.cshtml"
                                                               ViewBag.IsMine ? "ismine" : ""

#line default
#line hidden
#nullable disable
            , 1324, 33, false);
            EndAddHtmlAttributeValues(__tagHelperExecutionContext);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n</html>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
