#pragma checksum "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a163e07dbd127505a2f59190b642120d6888eeee"
// <auto-generated/>
#pragma warning disable 1591
namespace ALISS.Shared
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
    public partial class NavMenu : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
#nullable restore
#line 4 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
 if (menuList == null)
{

#line default
#line hidden
#nullable disable
            __builder.AddMarkupContent(0, "<p><em>Loading...</em></p>");
#nullable restore
#line 7 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
}
else
{

#line default
#line hidden
#nullable disable
            __builder.OpenElement(1, "div");
            __builder.OpenElement(2, "div");
            __builder.AddAttribute(3, "class", "row");
            __builder.OpenElement(4, "div");
            __builder.AddAttribute(5, "class", "col");
            __builder.OpenComponent<Microsoft.AspNetCore.Components.Routing.NavLink>(6);
            __builder.AddAttribute(7, "Match", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.Routing.NavLinkMatch>(
#nullable restore
#line 13 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                NavLinkMatch.All

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(8, "onclick", Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, 
#nullable restore
#line 13 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                            (() => RedirectToUserManagement(mainLayout.loginUser.Username))

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(9, "style", "cursor: pointer;");
            __builder.AddAttribute(10, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Radzen.Blazor.RadzenGravatar>(11);
                __builder2.AddAttribute(12, "Email", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 14 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                            mainLayout.loginUser.Username

#line default
#line hidden
#nullable disable
                ));
                __builder2.CloseComponent();
            }
            ));
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(13, "\r\n        ");
            __builder.OpenElement(14, "div");
            __builder.AddAttribute(15, "class", "row");
            __builder.OpenElement(16, "div");
            __builder.AddAttribute(17, "class", "col");
            __builder.OpenElement(18, "label");
            __builder.AddAttribute(19, "style", "color:#0A6839;");
            __builder.AddContent(20, "Hello, ");
            __builder.AddContent(21, 
#nullable restore
#line 20 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                      mainLayout.loginUser.Fullname

#line default
#line hidden
#nullable disable
            );
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(22, "\r\n        ");
            __builder.OpenElement(23, "div");
            __builder.AddAttribute(24, "class", "row");
            __builder.OpenElement(25, "div");
            __builder.AddAttribute(26, "class", "col");
            __builder.OpenElement(27, "label");
            __builder.AddAttribute(28, "style", "color:#0A6839;");
            __builder.AddContent(29, "Role :  ");
            __builder.AddContent(30, 
#nullable restore
#line 25 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                       mainLayout.loginUser.FullRole

#line default
#line hidden
#nullable disable
            );
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.AddMarkupContent(31, "\r\n        <br>\r\n\r\n        ");
            __builder.OpenElement(32, "div");
            __builder.AddAttribute(33, "class", "row");
            __builder.OpenElement(34, "div");
            __builder.AddAttribute(35, "class", "col");
            __builder.OpenComponent<Radzen.Blazor.RadzenPanelMenu>(36);
            __builder.AddAttribute(37, "Style", "width:250px");
            __builder.AddAttribute(38, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.OpenComponent<Radzen.Blazor.RadzenPanelMenuItem>(39);
                __builder2.AddAttribute(40, "Text", "Home");
                __builder2.AddAttribute(41, "Path", "Home");
                __builder2.AddAttribute(42, "Icon", "home");
                __builder2.CloseComponent();
#nullable restore
#line 34 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                     foreach (var _menu in menuList.Where(x => x.mnu_parent_code == "ROOT").OrderBy(x => x.mnu_order).ToList())
                    {
                        if (menuList.Any(x => x.mnu_parent_code == _menu.rop_mnu_code))
                        {

#line default
#line hidden
#nullable disable
                __builder2.OpenComponent<Radzen.Blazor.RadzenPanelMenuItem>(43);
                __builder2.AddAttribute(44, "Text", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 38 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                         $"{_menu.mnu_name}"

#line default
#line hidden
#nullable disable
                ));
                __builder2.AddAttribute(45, "Icon", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 38 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                                      _menu.mnu_icon

#line default
#line hidden
#nullable disable
                ));
                __builder2.AddAttribute(46, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder3) => {
#nullable restore
#line 39 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                 foreach (var _submenu in menuList.Where(x => x.mnu_parent_code == _menu.rop_mnu_code).OrderBy(x => x.mnu_order_sub).ToList())
                                {
                                    if (menuList.Any(x => x.mnu_parent_code == _submenu.rop_mnu_code))
                                    {

#line default
#line hidden
#nullable disable
                    __builder3.OpenComponent<Radzen.Blazor.RadzenPanelMenuItem>(47);
                    __builder3.AddAttribute(48, "Text", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 43 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                     $"{_submenu.mnu_name}"

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(49, "Icon", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 43 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                                                     _submenu.mnu_icon

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(50, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder4) => {
#nullable restore
#line 44 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                             foreach (var _subsubmenu in menuList.Where(x => x.mnu_parent_code == _submenu.rop_mnu_code).OrderBy(x => x.mnu_order_sub).ToList())
                                            {

#line default
#line hidden
#nullable disable
                        __builder4.OpenComponent<Radzen.Blazor.RadzenPanelMenuItem>(51);
                        __builder4.AddAttribute(52, "Text", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 46 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                             $"{_subsubmenu.mnu_name}"

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(53, "Path", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 46 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                                                                _subsubmenu.mnu_path

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.AddAttribute(54, "Icon", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 46 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                                                                                             _subsubmenu.mnu_icon

#line default
#line hidden
#nullable disable
                        ));
                        __builder4.CloseComponent();
#nullable restore
#line 47 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                            }

#line default
#line hidden
#nullable disable
                    }
                    ));
                    __builder3.CloseComponent();
#nullable restore
#line 49 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                    }
                                    else
                                    {

#line default
#line hidden
#nullable disable
                    __builder3.OpenComponent<Radzen.Blazor.RadzenPanelMenuItem>(55);
                    __builder3.AddAttribute(56, "Text", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 52 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                     $"{_submenu.mnu_name}"

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(57, "Path", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 52 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                                                     _submenu.mnu_path

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.AddAttribute(58, "Icon", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 52 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                                                                               _submenu.mnu_icon

#line default
#line hidden
#nullable disable
                    ));
                    __builder3.CloseComponent();
#nullable restore
#line 53 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                    }

                                }

#line default
#line hidden
#nullable disable
                }
                ));
                __builder2.CloseComponent();
#nullable restore
#line 57 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                        }
                        else
                        {

#line default
#line hidden
#nullable disable
                __builder2.OpenComponent<Radzen.Blazor.RadzenPanelMenuItem>(59);
                __builder2.AddAttribute(60, "Text", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 60 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                         $"{_menu.mnu_name}"

#line default
#line hidden
#nullable disable
                ));
                __builder2.AddAttribute(61, "Path", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 60 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                                      _menu.mnu_path

#line default
#line hidden
#nullable disable
                ));
                __builder2.AddAttribute(62, "Icon", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.String>(
#nullable restore
#line 60 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                                                                                                             _menu.mnu_icon

#line default
#line hidden
#nullable disable
                ));
                __builder2.CloseComponent();
#nullable restore
#line 61 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
                        }
                    }

#line default
#line hidden
#nullable disable
                __builder2.OpenComponent<Radzen.Blazor.RadzenPanelMenuItem>(63);
                __builder2.AddAttribute(64, "Text", "Log out");
                __builder2.AddAttribute(65, "Path", "Logout");
                __builder2.AddAttribute(66, "Icon", "logout");
                __builder2.CloseComponent();
            }
            ));
            __builder.CloseComponent();
            __builder.CloseElement();
            __builder.CloseElement();
            __builder.CloseElement();
#nullable restore
#line 69 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
}

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
#nullable restore
#line 72 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Shared\NavMenu.razor"
       
    [CascadingParameter] MainLayout mainLayout { get; set; }
    private List<LoginUserRolePermissionDTO> menuList;

    protected override async Task OnInitializedAsync()
    {
        menuList = mainLayout.loginUser.LoginUserRolePermissionList.Where(x => x.rop_rol_code == mainLayout.loginUser.rol_code && x.rop_view == true && x.mnu_active == true).ToList();
    }

    private bool collapseSubMenu = true;
    private string currentSubMenu = "";

    private void ToggleSubMenu(string mnu_code)
    {
        if (string.IsNullOrEmpty(currentSubMenu)) currentSubMenu = mnu_code;
        else if (currentSubMenu == mnu_code) currentSubMenu = "";

        StateHasChanged();
    }

    private void DisplaySubMenu(LoginUserRolePermissionDTO _mnu)
    {
        if (_mnu.mnu_status == "A")
        {
            var menu = menuList.FirstOrDefault(x => x.rop_mnu_code == _mnu.rop_mnu_code);
            menu.mnu_status = "S";
        }
        else
        {
            var menu = menuList.FirstOrDefault(x => x.rop_mnu_code == _mnu.rop_mnu_code);
            menu.mnu_status = "A";
        }

        StateHasChanged();
    }

    private string GetToggleSign(LoginUserRolePermissionDTO _mnu)
    {
        if (_mnu.mnu_status == "A")
        {
            return "expand-more";
        }
        else
        {
            return "expand-less";
        }
    }

    private void RedirectToUserManagement(string username)
    {
        navigationManager.NavigateTo("UserManagement/UserLogin/DataDetail/" + username);
    }


#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager navigationManager { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IJSRuntime jsRuntime { get; set; }
    }
}
#pragma warning restore 1591
