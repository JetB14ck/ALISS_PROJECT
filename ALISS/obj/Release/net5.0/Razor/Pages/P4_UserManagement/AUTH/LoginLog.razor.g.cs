#pragma checksum "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "009d2ec11c6c5ed853778349dbbd6719ca389a8e"
// <auto-generated/>
#pragma warning disable 1591
namespace ALISS.Pages.P4_UserManagement.AUTH
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using System.Text.Json;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using System.Security.Claims;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.AspNetCore.HttpOverrides;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using Blazored;

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using Blazored.Menu;

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using Radzen;

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using Radzen.Blazor;

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using BlazorDownloadFile;

#line default
#line hidden
#nullable disable
#nullable restore
#line 20 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS;

#line default
#line hidden
#nullable disable
#nullable restore
#line 21 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 22 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.Account;

#line default
#line hidden
#nullable disable
#nullable restore
#line 23 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.Client;

#line default
#line hidden
#nullable disable
#nullable restore
#line 25 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 27 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.DropDownList.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 28 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.LoginManagement.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 29 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Master.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 30 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Process.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 31 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.AUTH.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 32 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.MasterManagement.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 33 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.UserManagement.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 34 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.HISUpload.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 35 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.ANTIBIOGRAM.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 36 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.ANTIBIOTREND.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 37 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.GLASS.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 39 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D0_Master;

#line default
#line hidden
#nullable disable
#nullable restore
#line 41 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D3_Process;

#line default
#line hidden
#nullable disable
#nullable restore
#line 42 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D4_UserManagement.AUTH;

#line default
#line hidden
#nullable disable
#nullable restore
#line 43 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D4_UserManagement.MasterManagement;

#line default
#line hidden
#nullable disable
#nullable restore
#line 44 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D4_UserManagement.UserManagement;

#line default
#line hidden
#nullable disable
#nullable restore
#line 46 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D5_HISData;

#line default
#line hidden
#nullable disable
#nullable restore
#line 47 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D6_Report.Antibiogram;

#line default
#line hidden
#nullable disable
#nullable restore
#line 48 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D6_Report.Antibiotrend;

#line default
#line hidden
#nullable disable
#nullable restore
#line 49 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D6_Report.Glass;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/Auth/LoginLog")]
    public partial class LoginLog : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.AddMarkupContent(0, "<div><div class=\"divHead\">\r\n        Login log\r\n    </div></div>");
#nullable restore
#line 14 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
 if (pageLoading)
{

#line default
#line hidden
#nullable disable
            __builder.AddMarkupContent(1, "<p><em>Loading...</em></p>");
            __builder.OpenComponent<ALISS.Shared.Loading>(2);
            __builder.AddAttribute(3, "ShowModel", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 18 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                        true

#line default
#line hidden
#nullable disable
            ));
            __builder.CloseComponent();
#nullable restore
#line 19 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
}
else
{

#line default
#line hidden
#nullable disable
            __builder.OpenElement(4, "div");
            __builder.AddAttribute(5, "class", "container inputArea");
            __builder.OpenElement(6, "div");
            __builder.AddAttribute(7, "class", "row");
            __builder.OpenElement(8, "div");
            __builder.AddAttribute(9, "class", "col-12 col-xl-4");
            __builder.OpenElement(10, "div");
            __builder.AddAttribute(11, "class", "row justify-content-center divGroup");
            __builder.OpenElement(12, "div");
            __builder.AddAttribute(13, "class", 
#nullable restore
#line 26 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                 classLabel

#line default
#line hidden
#nullable disable
            );
            __builder.OpenComponent<ALISS.Shared.LabelBox>(14);
            __builder.AddAttribute(15, "inputLabel", "User name");
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.AddMarkupContent(16, "\r\n                    ");
            __builder.OpenElement(17, "div");
            __builder.AddAttribute(18, "class", 
#nullable restore
#line 29 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                 classInput

#line default
#line hidden
#nullable disable
            );
            __builder.OpenComponent<Radzen.Blazor.RadzenTextBox>(19);
            __builder.AddAttribute(20, "Value", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 30 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                     searchModel.log_usr_id

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(21, "ValueChanged", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.String>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.String>(this, Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.CreateInferredEventCallback(this, __value => searchModel.log_usr_id = __value, searchModel.log_usr_id))));
            __builder.AddAttribute(22, "ValueExpression", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Linq.Expressions.Expression<System.Func<System.String>>>(() => searchModel.log_usr_id));
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(23, "\r\n            ");
            __builder.OpenElement(24, "div");
            __builder.AddAttribute(25, "class", "col-12 col-xl-4");
            __builder.OpenElement(26, "div");
            __builder.AddAttribute(27, "class", "row justify-content-center divGroup");
            __builder.OpenElement(28, "div");
            __builder.AddAttribute(29, "class", 
#nullable restore
#line 36 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                 classLabel

#line default
#line hidden
#nullable disable
            );
            __builder.OpenComponent<ALISS.Shared.LabelBox>(30);
            __builder.AddAttribute(31, "inputLabel", "Login date from");
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.AddMarkupContent(32, "\r\n                    ");
            __builder.OpenElement(33, "div");
            __builder.AddAttribute(34, "class", 
#nullable restore
#line 39 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                 classInput

#line default
#line hidden
#nullable disable
            );
            __Blazor.ALISS.Pages.P4_UserManagement.AUTH.LoginLog.TypeInference.CreateRadzenDatePicker_0(__builder, 35, 36, "dd/MM/yyyy", 37, "width:100%;", 38, 
#nullable restore
#line 40 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                                                DateRenderFrom

#line default
#line hidden
#nullable disable
            , 39, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.DateTime?>(this, 
#nullable restore
#line 40 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                                                                        DateFromChange

#line default
#line hidden
#nullable disable
            ), 40, 
#nullable restore
#line 40 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                       searchModel.log_login_timestamp

#line default
#line hidden
#nullable disable
            , 41, Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.CreateInferredEventCallback(this, __value => searchModel.log_login_timestamp = __value, searchModel.log_login_timestamp)), 42, () => searchModel.log_login_timestamp);
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(43, "\r\n            ");
            __builder.OpenElement(44, "div");
            __builder.AddAttribute(45, "class", "col-12 col-xl-4");
            __builder.OpenElement(46, "div");
            __builder.AddAttribute(47, "class", "row justify-content-center divGroup");
            __builder.OpenElement(48, "div");
            __builder.AddAttribute(49, "class", 
#nullable restore
#line 46 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                 classLabel

#line default
#line hidden
#nullable disable
            );
            __builder.OpenComponent<ALISS.Shared.LabelBox>(50);
            __builder.AddAttribute(51, "inputLabel", "Login date to");
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.AddMarkupContent(52, "\r\n                    ");
            __builder.OpenElement(53, "div");
            __builder.AddAttribute(54, "class", 
#nullable restore
#line 49 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                 classInput

#line default
#line hidden
#nullable disable
            );
            __Blazor.ALISS.Pages.P4_UserManagement.AUTH.LoginLog.TypeInference.CreateRadzenDatePicker_1(__builder, 55, 56, "dd/MM/yyyy", 57, "width:100%;", 58, 
#nullable restore
#line 50 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                                                  DateRenderTo

#line default
#line hidden
#nullable disable
            , 59, Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.DateTime?>(this, 
#nullable restore
#line 50 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                                                                        DateToChange

#line default
#line hidden
#nullable disable
            ), 60, 
#nullable restore
#line 50 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                       searchModel.log_logout_timestamp

#line default
#line hidden
#nullable disable
            , 61, Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.CreateInferredEventCallback(this, __value => searchModel.log_logout_timestamp = __value, searchModel.log_logout_timestamp)), 62, () => searchModel.log_logout_timestamp);
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(63, "\r\n        ");
            __builder.OpenElement(64, "div");
            __builder.AddAttribute(65, "class", "row justify-content-end");
            __builder.OpenElement(66, "div");
            __builder.AddAttribute(67, "class", "col-12 col-xl-4");
            __builder.OpenElement(68, "div");
            __builder.AddAttribute(69, "class", "row justify-content-center divGroup buttonArea");
            __builder.OpenElement(70, "div");
            __builder.AddAttribute(71, "class", "col-auto");
            __builder.OpenComponent<Radzen.Blazor.RadzenButton>(72);
            __builder.AddAttribute(73, "class", "btnSearch");
            __builder.AddAttribute(74, "Text", "Search");
            __builder.AddAttribute(75, "Click", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Microsoft.AspNetCore.Components.Web.MouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, 
#nullable restore
#line 59 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                             SearchInboxData

#line default
#line hidden
#nullable disable
            )));
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.AddMarkupContent(76, "\r\n                    ");
            __builder.OpenElement(77, "div");
            __builder.AddAttribute(78, "class", "col-auto");
            __builder.OpenComponent<Radzen.Blazor.RadzenButton>(79);
            __builder.AddAttribute(80, "class", "btnClear");
            __builder.AddAttribute(81, "Text", "Clear");
            __builder.AddAttribute(82, "Click", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Microsoft.AspNetCore.Components.Web.MouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, 
#nullable restore
#line 62 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                           ClearInboxData

#line default
#line hidden
#nullable disable
            )));
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(83, "<hr>");
            __builder.OpenElement(84, "div");
            __builder.AddMarkupContent(85, "<br>\r\n        ");
            __builder.OpenElement(86, "div");
            __builder.AddAttribute(87, "style", "width:90%;margin:0px auto;");
            __builder.OpenElement(88, "div");
            __builder.AddAttribute(89, "class", "container");
            __builder.OpenElement(90, "div");
            __builder.AddAttribute(91, "class", "row justify-content-end");
            __builder.AddMarkupContent(92, "<div class=\"col-auto\"><label>Rows : </label></div>\r\n                    ");
            __builder.OpenElement(93, "div");
            __builder.AddAttribute(94, "class", "col-auto");
            __builder.OpenComponent<Radzen.Blazor.RadzenDropDown<int>>(95);
            __builder.AddAttribute(96, "AllowFiltering", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 80 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                      true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(97, "Data", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.IEnumerable>(
#nullable restore
#line 80 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                  inboxGrid.PageSizeOption

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(98, "Change", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.Object>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.Object>(this, 
#nullable restore
#line 80 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                                                    SearchInboxData

#line default
#line hidden
#nullable disable
            )));
            __builder.AddAttribute(99, "Style", "width:50px");
            __builder.AddAttribute(100, "Value", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Object>(
#nullable restore
#line 80 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                     inboxGrid.PageSize

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(101, "ValueChanged", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<int>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<int>(this, Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.CreateInferredEventCallback(this, __value => inboxGrid.PageSize = __value, inboxGrid.PageSize))));
            __builder.AddAttribute(102, "ValueExpression", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Linq.Expressions.Expression<System.Func<int>>>(() => inboxGrid.PageSize));
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(103, "\r\n            ");
            __builder.OpenComponent<Radzen.Blazor.RadzenGrid<LogUserLoginDTO>>(104);
            __builder.AddAttribute(105, "Data", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.Generic.IEnumerable<LogUserLoginDTO>>(
#nullable restore
#line 84 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                   searchResultList

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(106, "PageSize", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Int32>(
#nullable restore
#line 84 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                               inboxGrid.PageSize

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(107, "ColumnWidth", "200px");
            __builder.AddAttribute(108, "AllowPaging", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 84 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                                                                    true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(109, "AllowFiltering", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 84 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                                                                                          true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(110, "FilterCaseSensitivity", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Radzen.FilterCaseSensitivity>(
#nullable restore
#line 84 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                                                                                                                       FilterCaseSensitivity.CaseInsensitive

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(111, "AllowSorting", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 84 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                                                                                                                                                                            true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(112, "AllowColumnResize", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 84 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                                                                                                                                                                                                                                                     true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(113, "Columns", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<LogUserLoginDTO>>(114);
                __builder2.AddAttribute(115, "Property", "log_id");
                __builder2.AddAttribute(116, "Width", "50px");
                __builder2.AddAttribute(117, "Title", "ID");
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(118, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<LogUserLoginDTO>>(119);
                __builder2.AddAttribute(120, "Property", "log_usr_id");
                __builder2.AddAttribute(121, "Title", "User name");
                __builder2.AddAttribute(122, "FooterTemplate", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenElement(123, "label");
                    __builder3.AddAttribute(124, "title", 
#nullable restore
#line 89 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                           inboxGrid.FooterLabelString

#line default
#line hidden
#nullable disable
                    );
                    __builder3.AddContent(125, 
#nullable restore
#line 89 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                                         inboxGrid.FooterLabelString

#line default
#line hidden
#nullable disable
                    );
                    __builder3.CloseElement();
                }
                ));
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(126, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<LogUserLoginDTO>>(127);
                __builder2.AddAttribute(128, "Property", "log_login_timestamp");
                __builder2.AddAttribute(129, "Title", "Login date");
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(130, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<LogUserLoginDTO>>(131);
                __builder2.AddAttribute(132, "Property", "log_logout_timestamp");
                __builder2.AddAttribute(133, "Title", "Logout date");
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(134, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<LogUserLoginDTO>>(135);
                __builder2.AddAttribute(136, "Property", "log_status");
                __builder2.AddAttribute(137, "Title", "Status");
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(138, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<LogUserLoginDTO>>(139);
                __builder2.AddAttribute(140, "Property", "log_remark");
                __builder2.AddAttribute(141, "Title", "Remark");
                __builder2.CloseComponent();
            }
            ));
            __builder.AddComponentReferenceCapture(142, (__value) => {
#nullable restore
#line 84 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                                      inboxGrid.radzenGrid = (Radzen.Blazor.RadzenGrid<LogUserLoginDTO>)__value;

#line default
#line hidden
#nullable disable
            }
            );
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.AddMarkupContent(143, "\r\n        <br>");
            __builder.CloseElement();
            __builder.AddMarkupContent(144, "<hr>");
            __builder.OpenComponent<ALISS.Shared.Loading>(145);
            __builder.AddAttribute(146, "ShowModel", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 105 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                        showLoading

#line default
#line hidden
#nullable disable
            ));
            __builder.CloseComponent();
            __builder.OpenComponent<ALISS.Shared.ColumnConfigTable>(147);
            __builder.AddAttribute(148, "SearchResultList", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.Generic.List<ALISS.AUTH.DTO.ColumnConfigDTO>>(
#nullable restore
#line 107 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
                                         searchColumnConfigResultList

#line default
#line hidden
#nullable disable
            ));
            __builder.CloseComponent();
#nullable restore
#line 108 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
}

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
#nullable restore
#line 110 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\LoginLog.razor"
       
    [CascadingParameter] MainLayout mainLayout { get; set; }

    private string currentMenuCode = "MNU_0104";
    private string classLabel = "col-4";
    private string classInput = "col-8";
    private bool showLoading;

    private RadzenGridCustom<LogUserLoginDTO> inboxGrid = new RadzenGridCustom<LogUserLoginDTO>();
    private LogUserLoginDTO searchModel = new LogUserLoginDTO();
    private List<LogUserLoginDTO> searchResultList;

    private List<ColumnConfigDTO> searchColumnConfigResultList;

    private bool pageLoading { get { return (searchResultList == null); } }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await mainLayout.GetLoginUser();

            if (mainLayout.loginUser.CheckPagePermission(currentMenuCode) == false) navigationManager.NavigateTo("/NoPermissionPage");

            searchResultList = await loginLogService.GetListByModelAsync(searchModel);

            searchColumnConfigResultList = await columnConfigService.GetListByModelAsync(new ColumnConfigSearchDTO() { sch_mnu_code = currentMenuCode });

            StateHasChanged();
        }
    }

    private async Task SearchInboxData()
    {
        showLoading = true;
        StateHasChanged();

        searchResultList = await loginLogService.GetListByModelAsync(searchModel);
        if (inboxGrid.radzenGrid != null) inboxGrid.radzenGrid.GoToPage(0);

        showLoading = false;
        StateHasChanged();
    }

    private async Task ClearInboxData()
    {
        showLoading = true;
        StateHasChanged();

        searchModel = new LogUserLoginDTO();

        await SearchInboxData();
    }

    void DateRenderFrom(DateRenderEventArgs args)
    {
        args.Disabled = ((searchModel.log_logout_timestamp != null && args.Date > searchModel.log_logout_timestamp) || (args.Date > DateTime.Now));
    }

    void DateRenderTo(DateRenderEventArgs args)
    {
        args.Disabled = ((searchModel.log_login_timestamp != null && args.Date < searchModel.log_login_timestamp) || (args.Date > DateTime.Now));
    }

    void DateFromChange(DateTime? args)
    {
        if ((args != null) && ((searchModel.log_logout_timestamp != null && args > searchModel.log_logout_timestamp) || (args > DateTime.Now)))
        {
            searchModel.log_login_timestamp = null;
        }
    }

    void DateToChange(DateTime? args)
    {
        if ((args != null) && ((searchModel.log_login_timestamp != null && args < searchModel.log_login_timestamp) || (args > DateTime.Now)))
        {
            searchModel.log_logout_timestamp = null;
        }
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private LoginLogService loginLogService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private ColumnConfigService columnConfigService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager navigationManager { get; set; }
    }
}
namespace __Blazor.ALISS.Pages.P4_UserManagement.AUTH.LoginLog
{
    #line hidden
    internal static class TypeInference
    {
        public static void CreateRadzenDatePicker_0<TValue>(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder, int seq, int __seq0, global::System.String __arg0, int __seq1, global::System.String __arg1, int __seq2, global::System.Action<global::Radzen.DateRenderEventArgs> __arg2, int __seq3, global::Microsoft.AspNetCore.Components.EventCallback<global::System.DateTime?> __arg3, int __seq4, global::System.Object __arg4, int __seq5, global::Microsoft.AspNetCore.Components.EventCallback<TValue> __arg5, int __seq6, global::System.Linq.Expressions.Expression<global::System.Func<TValue>> __arg6)
        {
        __builder.OpenComponent<global::Radzen.Blazor.RadzenDatePicker<TValue>>(seq);
        __builder.AddAttribute(__seq0, "DateFormat", __arg0);
        __builder.AddAttribute(__seq1, "Style", __arg1);
        __builder.AddAttribute(__seq2, "DateRender", __arg2);
        __builder.AddAttribute(__seq3, "Change", __arg3);
        __builder.AddAttribute(__seq4, "Value", __arg4);
        __builder.AddAttribute(__seq5, "ValueChanged", __arg5);
        __builder.AddAttribute(__seq6, "ValueExpression", __arg6);
        __builder.CloseComponent();
        }
        public static void CreateRadzenDatePicker_1<TValue>(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder, int seq, int __seq0, global::System.String __arg0, int __seq1, global::System.String __arg1, int __seq2, global::System.Action<global::Radzen.DateRenderEventArgs> __arg2, int __seq3, global::Microsoft.AspNetCore.Components.EventCallback<global::System.DateTime?> __arg3, int __seq4, global::System.Object __arg4, int __seq5, global::Microsoft.AspNetCore.Components.EventCallback<TValue> __arg5, int __seq6, global::System.Linq.Expressions.Expression<global::System.Func<TValue>> __arg6)
        {
        __builder.OpenComponent<global::Radzen.Blazor.RadzenDatePicker<TValue>>(seq);
        __builder.AddAttribute(__seq0, "DateFormat", __arg0);
        __builder.AddAttribute(__seq1, "Style", __arg1);
        __builder.AddAttribute(__seq2, "DateRender", __arg2);
        __builder.AddAttribute(__seq3, "Change", __arg3);
        __builder.AddAttribute(__seq4, "Value", __arg4);
        __builder.AddAttribute(__seq5, "ValueChanged", __arg5);
        __builder.AddAttribute(__seq6, "ValueExpression", __arg6);
        __builder.CloseComponent();
        }
    }
}
#pragma warning restore 1591
