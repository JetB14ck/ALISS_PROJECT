#pragma checksum "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9676c626fec823e90b93cde0d0689bae6f7bebbc"
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
#line 1 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using System.Text.Json;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using System.Security.Claims;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using Microsoft.AspNetCore.HttpOverrides;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using Blazored;

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using Blazored.Menu;

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using Radzen;

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using Radzen.Blazor;

#line default
#line hidden
#nullable disable
#nullable restore
#line 18 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using BlazorDownloadFile;

#line default
#line hidden
#nullable disable
#nullable restore
#line 20 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS;

#line default
#line hidden
#nullable disable
#nullable restore
#line 21 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 22 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.Account;

#line default
#line hidden
#nullable disable
#nullable restore
#line 23 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.Client;

#line default
#line hidden
#nullable disable
#nullable restore
#line 25 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 27 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.DropDownList.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 28 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.LoginManagement.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 29 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Master.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 30 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Process.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 31 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.AUTH.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 32 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.MasterManagement.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 33 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.UserManagement.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 34 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.HISUpload.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 35 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.ANTIBIOGRAM.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 36 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.ANTIBIOTREND.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 37 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.GLASS.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 39 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D0_Master;

#line default
#line hidden
#nullable disable
#nullable restore
#line 41 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D3_Process;

#line default
#line hidden
#nullable disable
#nullable restore
#line 42 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D4_UserManagement.AUTH;

#line default
#line hidden
#nullable disable
#nullable restore
#line 43 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D4_UserManagement.MasterManagement;

#line default
#line hidden
#nullable disable
#nullable restore
#line 44 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D4_UserManagement.UserManagement;

#line default
#line hidden
#nullable disable
#nullable restore
#line 46 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D5_HISData;

#line default
#line hidden
#nullable disable
#nullable restore
#line 47 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D6_Report.Antibiogram;

#line default
#line hidden
#nullable disable
#nullable restore
#line 48 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D6_Report.Antibiotrend;

#line default
#line hidden
#nullable disable
#nullable restore
#line 49 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\_Imports.razor"
using ALISS.Data.D6_Report.Glass;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/AUTH/Role")]
    public partial class Role : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.AddMarkupContent(0, "<div><div class=\"divHead\">\r\n        Role\r\n    </div></div>");
#nullable restore
#line 14 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
 if (pageLoading)
{

#line default
#line hidden
#nullable disable
            __builder.AddMarkupContent(1, "<p><em>Loading...</em></p>");
            __builder.OpenComponent<ALISS.Shared.Loading>(2);
            __builder.AddAttribute(3, "ShowModel", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 18 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                        true

#line default
#line hidden
#nullable disable
            ));
            __builder.CloseComponent();
#nullable restore
#line 19 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
}
else
{

#line default
#line hidden
#nullable disable
            __builder.OpenElement(4, "div");
            __builder.AddAttribute(5, "class", "container inputArea");
            __builder.OpenElement(6, "div");
            __builder.AddAttribute(7, "class", "row justify-content-between");
            __builder.OpenComponent<ALISS.Shared.InputBox>(8);
            __builder.AddAttribute(9, "ConfigData", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<ALISS.Master.DTO.TBConfigDTO>(
#nullable restore
#line 24 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                    configData.Get_ConfigRow("sch_rol_code")

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(10, "classLabel", "col");
            __builder.AddAttribute(11, "classInput", "col");
            __builder.AddAttribute(12, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Radzen.Blazor.RadzenTextBox>(13);
                __builder2.AddAttribute(14, "Value", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 25 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                             searchModel.sch_rol_code

#line default
#line hidden
#nullable disable
                ));
                __builder2.AddAttribute(15, "ValueChanged", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.String>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.String>(this, Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.CreateInferredEventCallback(this, __value => searchModel.sch_rol_code = __value, searchModel.sch_rol_code))));
                __builder2.AddAttribute(16, "ValueExpression", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Linq.Expressions.Expression<System.Func<System.String>>>(() => searchModel.sch_rol_code));
                __builder2.CloseComponent();
            }
            ));
            __builder.CloseComponent();
            __builder.AddMarkupContent(17, "\r\n            ");
            __builder.OpenElement(18, "div");
            __builder.AddAttribute(19, "class", 
#nullable restore
#line 27 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                         classColumn

#line default
#line hidden
#nullable disable
            );
            __builder.OpenElement(20, "div");
            __builder.AddAttribute(21, "class", "row justify-content-center divGroup buttonArea");
            __builder.OpenElement(22, "div");
            __builder.AddAttribute(23, "class", "col-auto");
            __builder.OpenComponent<Radzen.Blazor.RadzenButton>(24);
            __builder.AddAttribute(25, "class", "btnSearch");
            __builder.AddAttribute(26, "Text", "Search");
            __builder.AddAttribute(27, "Click", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Microsoft.AspNetCore.Components.Web.MouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, 
#nullable restore
#line 30 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                             SearchInboxData

#line default
#line hidden
#nullable disable
            )));
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.AddMarkupContent(28, "\r\n                    ");
            __builder.OpenElement(29, "div");
            __builder.AddAttribute(30, "class", "col-auto");
            __builder.OpenComponent<Radzen.Blazor.RadzenButton>(31);
            __builder.AddAttribute(32, "class", "btnClear");
            __builder.AddAttribute(33, "Text", "Clear");
            __builder.AddAttribute(34, "Click", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Microsoft.AspNetCore.Components.Web.MouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, 
#nullable restore
#line 33 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
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
            __builder.AddMarkupContent(35, "<hr>");
            __builder.OpenElement(36, "div");
            __builder.OpenElement(37, "div");
            __builder.AddAttribute(38, "class", "container");
            __builder.OpenElement(39, "div");
            __builder.AddAttribute(40, "class", "row justify-content-end");
            __builder.OpenElement(41, "div");
            __builder.AddAttribute(42, "class", "col-auto buttonArea");
#nullable restore
#line 46 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                     if (mainLayout.loginUser.PagePermission.rop_create)
                    {

#line default
#line hidden
#nullable disable
            __builder.OpenComponent<Radzen.Blazor.RadzenButton>(43);
            __builder.AddAttribute(44, "Icon", "add");
            __builder.AddAttribute(45, "class", "btnAdd");
            __builder.AddAttribute(46, "Text", "Add Role");
            __builder.AddAttribute(47, "Click", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Microsoft.AspNetCore.Components.Web.MouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, 
#nullable restore
#line 48 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                         () => NavigateToDetailPage("")

#line default
#line hidden
#nullable disable
            )));
            __builder.CloseComponent();
#nullable restore
#line 49 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                    }

#line default
#line hidden
#nullable disable
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(48, "\r\n\r\n        <br>\r\n\r\n        ");
            __builder.OpenElement(49, "div");
            __builder.AddAttribute(50, "style", "width:90%;margin:0px auto;");
            __builder.OpenElement(51, "div");
            __builder.AddAttribute(52, "class", "container");
            __builder.OpenElement(53, "div");
            __builder.AddAttribute(54, "class", "row justify-content-end");
            __builder.AddMarkupContent(55, "<div class=\"col-auto\"><label>Rows : </label></div>\r\n                    ");
            __builder.OpenElement(56, "div");
            __builder.AddAttribute(57, "class", "col-auto");
            __builder.OpenComponent<Radzen.Blazor.RadzenDropDown<int>>(58);
            __builder.AddAttribute(59, "AllowFiltering", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 63 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                                      true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(60, "Data", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.IEnumerable>(
#nullable restore
#line 63 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                                                  inboxGrid.PageSizeOption

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(61, "Change", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.Object>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.Object>(this, 
#nullable restore
#line 63 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                                                                                    SearchInboxData

#line default
#line hidden
#nullable disable
            )));
            __builder.AddAttribute(62, "Style", "width:50px");
            __builder.AddAttribute(63, "Value", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Object>(
#nullable restore
#line 63 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                     inboxGrid.PageSize

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(64, "ValueChanged", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<int>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<int>(this, Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.CreateInferredEventCallback(this, __value => inboxGrid.PageSize = __value, inboxGrid.PageSize))));
            __builder.AddAttribute(65, "ValueExpression", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Linq.Expressions.Expression<System.Func<int>>>(() => inboxGrid.PageSize));
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(66, "\r\n            ");
            __builder.OpenComponent<Radzen.Blazor.RadzenGrid<RoleDTO>>(67);
            __builder.AddAttribute(68, "Data", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.Generic.IEnumerable<RoleDTO>>(
#nullable restore
#line 67 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                           searchResultList

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(69, "PageSize", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Int32>(
#nullable restore
#line 67 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                                       inboxGrid.PageSize

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(70, "ColumnWidth", "200px");
            __builder.AddAttribute(71, "AllowPaging", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 67 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                                                                                            true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(72, "AllowFiltering", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 67 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                                                                                                                  true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(73, "FilterCaseSensitivity", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Radzen.FilterCaseSensitivity>(
#nullable restore
#line 67 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                                                                                                                                               FilterCaseSensitivity.CaseInsensitive

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(74, "AllowSorting", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 67 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                                                                                                                                                                                                    true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(75, "AllowColumnResize", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 67 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                                                                                                                                                                                                                             true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(76, "Columns", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<RoleDTO>>(77);
                __builder2.AddAttribute(78, "Property", "rol_code");
                __builder2.AddAttribute(79, "Title", "");
                __builder2.AddAttribute(80, "Width", "50px");
                __builder2.AddAttribute(81, "Template", (Microsoft.AspNetCore.Components.RenderFragment<RoleDTO>)((data) => (__builder3) => {
#nullable restore
#line 71 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                             if (mainLayout.loginUser.PagePermission.rop_edit)
                            {

#line default
#line hidden
#nullable disable
                    __builder3.OpenComponent<Radzen.Blazor.RadzenButton>(82);
                    __builder3.AddAttribute(83, "Icon", "edit");
                    __builder3.AddAttribute(84, "Style", "background: #0A6839");
                    __builder3.AddAttribute(85, "Click", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Microsoft.AspNetCore.Components.Web.MouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, 
#nullable restore
#line 73 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                               () => NavigateToDetailPage(data.rol_code)

#line default
#line hidden
#nullable disable
                    )));
                    __builder3.CloseComponent();
#nullable restore
#line 74 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                            }

#line default
#line hidden
#nullable disable
                }
                ));
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(86, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<RoleDTO>>(87);
                __builder2.AddAttribute(88, "Property", "rol_code");
                __builder2.AddAttribute(89, "Title", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 77 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                   configData.Get_Label("rol_code")

#line default
#line hidden
#nullable disable
                ));
                __builder2.AddAttribute(90, "FooterTemplate", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenElement(91, "label");
                    __builder3.AddAttribute(92, "title", 
#nullable restore
#line 79 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                           inboxGrid.FooterLabelString

#line default
#line hidden
#nullable disable
                    );
                    __builder3.AddContent(93, 
#nullable restore
#line 79 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                         inboxGrid.FooterLabelString

#line default
#line hidden
#nullable disable
                    );
                    __builder3.CloseElement();
                }
                ));
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(94, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<RoleDTO>>(95);
                __builder2.AddAttribute(96, "Property", "rol_name");
                __builder2.AddAttribute(97, "Title", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 82 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                   configData.Get_Label("rol_name")

#line default
#line hidden
#nullable disable
                ));
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(98, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<RoleDTO>>(99);
                __builder2.AddAttribute(100, "Property", "rol_desc");
                __builder2.AddAttribute(101, "Title", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 83 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                   configData.Get_Label("rol_desc")

#line default
#line hidden
#nullable disable
                ));
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(102, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<RoleDTO>>(103);
                __builder2.AddAttribute(104, "Property", "rol_active");
                __builder2.AddAttribute(105, "Title", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 84 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                                                                     configData.Get_Label("rol_active")

#line default
#line hidden
#nullable disable
                ));
                __builder2.AddAttribute(106, "Template", (Microsoft.AspNetCore.Components.RenderFragment<RoleDTO>)((data) => (__builder3) => {
#nullable restore
#line 86 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                             if (data.rol_active == true)
                            {

#line default
#line hidden
#nullable disable
                    __builder3.AddMarkupContent(107, "<label>Active</label>");
#nullable restore
#line 89 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                            }
                            else
                            {

#line default
#line hidden
#nullable disable
                    __builder3.AddMarkupContent(108, "<label>Inactive</label>");
#nullable restore
#line 93 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                            }

#line default
#line hidden
#nullable disable
                }
                ));
                __builder2.CloseComponent();
            }
            ));
            __builder.AddComponentReferenceCapture(109, (__value) => {
#nullable restore
#line 67 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                                              inboxGrid.radzenGrid = (Radzen.Blazor.RadzenGrid<RoleDTO>)__value;

#line default
#line hidden
#nullable disable
            }
            );
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.AddMarkupContent(110, "\r\n\r\n        <br>");
            __builder.CloseElement();
            __builder.AddMarkupContent(111, "<hr>");
            __builder.OpenComponent<ALISS.Shared.Loading>(112);
            __builder.AddAttribute(113, "ShowModel", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 106 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
                        showLoading

#line default
#line hidden
#nullable disable
            ));
            __builder.CloseComponent();
#nullable restore
#line 107 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
}

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
#nullable restore
#line 109 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P4_UserManagement\AUTH\Role.razor"
       
    [CascadingParameter] MainLayout mainLayout { get; set; }

    private ConfigData configData = new ConfigData();
    private string classColumn = "col-12 col-xl-4";
    private string classLabel = "col-4";
    private string classInput = "col-8";
    private bool showLoading;

    private RadzenGridCustom<RoleDTO> inboxGrid = new RadzenGridCustom<RoleDTO>();
    private RoleSearchDTO searchModel = new RoleSearchDTO();
    private List<RoleDTO> searchResultList;

    private bool pageLoading { get { return (searchResultList == null); } }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await mainLayout.GetLoginUser();

            if (mainLayout.loginUser.CheckPagePermission("MNU_0103") == false) navigationManager.NavigateTo("/NoPermissionPage");

            configData.ConfigDTOList = await configDataService.Get_TBConfig_DataList_Async(new TBConfigDTO() { tbc_mnu_code = "MNU_0103" });
            if (mainLayout.loginUser.rol_code == "ROL_00")
            {
                configData.ConfigDTOList.ForEach(x =>
                {
                    x.tbc_edit = true;
                });
            }

            searchResultList = await roleService.GetListByModelAsync(searchModel);

            StateHasChanged();
        }
    }

    private async Task SearchInboxData()
    {
        showLoading = true;
        StateHasChanged();

        searchModel.sch_rol_code = searchModel.sch_rol_code?.Trim();

        searchResultList = await roleService.GetListByModelAsync(searchModel);
        if (inboxGrid.radzenGrid != null) inboxGrid.radzenGrid.GoToPage(0);

        showLoading = false;
        StateHasChanged();
    }

    private async Task ClearInboxData()
    {
        showLoading = true;
        StateHasChanged();

        searchModel = new RoleSearchDTO();

        await SearchInboxData();
    }

    private void NavigateToDetailPage(string rol_code)
    {
        navigationManager.NavigateTo("/Auth/RoleDetail/" + rol_code);
    }


#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private RoleService roleService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private ConfigDataService configDataService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager navigationManager { get; set; }
    }
}
#pragma warning restore 1591
