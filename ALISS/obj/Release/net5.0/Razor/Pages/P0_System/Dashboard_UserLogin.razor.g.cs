#pragma checksum "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "290c9071f477e2c62ff78461a59b25ed6cc955b0"
// <auto-generated/>
#pragma warning disable 1591
namespace ALISS.Pages.P0_System
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
    [Microsoft.AspNetCore.Components.RouteAttribute("/Dashboard_UserLogin")]
    public partial class Dashboard_UserLogin : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.AddMarkupContent(0, "<div><div class=\"divHead\">\r\n        Dashboard UserLogin\r\n    </div></div>");
#nullable restore
#line 11 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
 if (pageLoading)
{

#line default
#line hidden
#nullable disable
            __builder.AddMarkupContent(1, "<p><em>Loading...</em></p>");
            __builder.OpenComponent<ALISS.Shared.Loading>(2);
            __builder.AddAttribute(3, "ShowModel", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 15 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                        true

#line default
#line hidden
#nullable disable
            ));
            __builder.CloseComponent();
#nullable restore
#line 16 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
}
else
{


#line default
#line hidden
#nullable disable
            __builder.OpenElement(4, "div");
            __builder.OpenElement(5, "div");
            __builder.AddAttribute(6, "style", "width:90%;margin:0px auto;");
            __builder.OpenElement(7, "div");
            __builder.AddAttribute(8, "class", "container");
            __builder.OpenElement(9, "div");
            __builder.AddAttribute(10, "class", "row justify-content-end");
            __builder.AddMarkupContent(11, "<div class=\"col-auto\"><label>Rows : </label></div>\r\n                    ");
            __builder.OpenElement(12, "div");
            __builder.AddAttribute(13, "class", "col-auto");
            __builder.OpenComponent<Radzen.Blazor.RadzenDropDown<int>>(14);
            __builder.AddAttribute(15, "AllowFiltering", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 28 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                                                      true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(16, "Data", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.IEnumerable>(
#nullable restore
#line 28 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                                                                  inboxGrid.PageSizeOption

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(17, "Change", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<System.Object>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<System.Object>(this, 
#nullable restore
#line 28 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                                                                                                    SearchInboxData

#line default
#line hidden
#nullable disable
            )));
            __builder.AddAttribute(18, "Style", "width:50px");
            __builder.AddAttribute(19, "Value", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Object>(
#nullable restore
#line 28 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                     inboxGrid.PageSize

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(20, "ValueChanged", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<int>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<int>(this, Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.CreateInferredEventCallback(this, __value => inboxGrid.PageSize = __value, inboxGrid.PageSize))));
            __builder.AddAttribute(21, "ValueExpression", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Linq.Expressions.Expression<System.Func<int>>>(() => inboxGrid.PageSize));
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(22, "\r\n            ");
            __builder.OpenComponent<Radzen.Blazor.RadzenGrid<LoginUser>>(23);
            __builder.AddAttribute(24, "Data", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Collections.Generic.IEnumerable<LoginUser>>(
#nullable restore
#line 32 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                             loginUserData.LoginUserList

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(25, "PageSize", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Int32>(
#nullable restore
#line 32 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                                                                    inboxGrid.PageSize

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(26, "ColumnWidth", "200px");
            __builder.AddAttribute(27, "AllowPaging", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 32 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                                                                                                                         true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(28, "AllowFiltering", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 32 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                                                                                                                                               true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(29, "FilterCaseSensitivity", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Radzen.FilterCaseSensitivity>(
#nullable restore
#line 32 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                                                                                                                                                                            FilterCaseSensitivity.CaseInsensitive

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(30, "AllowSorting", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 32 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                                                                                                                                                                                                                                 true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(31, "AllowColumnResize", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 32 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                                                                                                                                                                                                                                                          true

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(32, "Columns", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<LoginUser>>(33);
                __builder2.AddAttribute(34, "Property", "mnu_id");
                __builder2.AddAttribute(35, "Title", "");
                __builder2.AddAttribute(36, "Width", "50px");
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(37, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<LoginUser>>(38);
                __builder2.AddAttribute(39, "Property", "mnu_code");
                __builder2.AddAttribute(40, "Title", "mnu_code");
                __builder2.AddAttribute(41, "FooterTemplate", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
                    __builder3.OpenElement(42, "label");
                    __builder3.AddAttribute(43, "title", 
#nullable restore
#line 37 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                           inboxGrid.FooterLabelString

#line default
#line hidden
#nullable disable
                    );
                    __builder3.AddContent(44, 
#nullable restore
#line 37 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                                         inboxGrid.FooterLabelString

#line default
#line hidden
#nullable disable
                    );
                    __builder3.CloseElement();
                }
                ));
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(45, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<LoginUser>>(46);
                __builder2.AddAttribute(47, "Property", "mnu_name");
                __builder2.AddAttribute(48, "Title", "mnu_name");
                __builder2.CloseComponent();
                __builder2.AddMarkupContent(49, "\r\n                    ");
                __builder2.OpenComponent<Radzen.Blazor.RadzenGridColumn<LoginUser>>(50);
                __builder2.AddAttribute(51, "Property", "mnu_path");
                __builder2.AddAttribute(52, "Title", "mnu_path");
                __builder2.CloseComponent();
            }
            ));
            __builder.AddComponentReferenceCapture(53, (__value) => {
#nullable restore
#line 32 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
                                                inboxGrid.radzenGrid = (Radzen.Blazor.RadzenGrid<LoginUser>)__value;

#line default
#line hidden
#nullable disable
            }
            );
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.AddMarkupContent(54, "\r\n\r\n        <br>");
            __builder.CloseElement();
#nullable restore
#line 49 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
}

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
#nullable restore
#line 51 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P0_System\Dashboard_UserLogin.razor"
       
    [CascadingParameter] MainLayout mainLayout { get; set; }

    private bool pageLoading = false;

    private RadzenGridCustom<LoginUser> inboxGrid = new RadzenGridCustom<LoginUser>();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {

            StateHasChanged();
        }
    }

    private void SearchInboxData()
    {
        if (inboxGrid.radzenGrid != null) inboxGrid.radzenGrid.GoToPage(0);

        StateHasChanged();
    }


#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private LoginUserDataList loginUserData { get; set; }
    }
}
#pragma warning restore 1591
