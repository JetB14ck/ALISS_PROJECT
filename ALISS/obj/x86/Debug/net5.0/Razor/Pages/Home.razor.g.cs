#pragma checksum "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7ff12dde05db4e527d256d31f9655da6a082f093"
// <auto-generated/>
#pragma warning disable 1591
namespace ALISS.Pages
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
    [Microsoft.AspNetCore.Components.RouteAttribute("/Home")]
    public partial class Home : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.AddMarkupContent(0, "<label style=\"font: Bold 22px/22px Arial; letter-spacing: 0px; color: #0A6839; opacity: 1;\">Notifications</label>\r\n<br>\r\n");
            __builder.AddMarkupContent(1, @"<div style=""background: #FFF9E0 0% 0% no-repeat padding-box; border: 1px solid #FED630; border-radius: 4px;""><label style=""text-align: left;font: Regular 18px/22px Arial;letter-spacing: 0px;color: #C6A003;"">ช่วงเวลาอัพโหลดไฟล์ข้อมูล ทุกๆ6เดือน</label></div>
<br>");
#nullable restore
#line 14 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
 if (pageLoading)
{

#line default
#line hidden
#nullable disable
            __builder.AddMarkupContent(2, "<p><em>Loading...</em></p>");
            __builder.OpenComponent<ALISS.Shared.Loading>(3);
            __builder.AddAttribute(4, "ShowModel", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<System.Boolean>(
#nullable restore
#line 18 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
                        true

#line default
#line hidden
#nullable disable
            ));
            __builder.CloseComponent();
#nullable restore
#line 19 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
}
else
{

#line default
#line hidden
#nullable disable
            __builder.OpenElement(5, "div");
            __builder.AddAttribute(6, "style", "background: #FFFFFF 0% 0% no-repeat padding-box;border: 1px solid #EBEBEB;border-radius: 4px;");
            __builder.AddMarkupContent(7, "<label style=\"font: Regular 16px/22px Arial;letter-spacing: 0px;color: #999999;\">Sun 13/02/62 13:54</label><br>\r\n        ");
            __builder.AddMarkupContent(8, "<label style=\"font: Regular 18px/22px Arial;letter-spacing: 0px;\">It is a long established fact that a reader will be distracted by the readable content . Mapping Data Management > files test.xls</label>\r\n        <br>\r\n        <br>");
#nullable restore
#line 27 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
         foreach (var item in noticeMessageList)
        {
            var noticeDate = (item.noti_createdate != null ? item.noti_createdate.Value.ToString("ddd dd/MM/yyyy HH:mm") : "");

#line default
#line hidden
#nullable disable
            __builder.OpenElement(9, "label");
            __builder.AddAttribute(10, "style", "font: Regular 16px/22px Arial;letter-spacing: 0px;color: #999999;");
            __builder.AddContent(11, 
#nullable restore
#line 30 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
                                                                                              noticeDate

#line default
#line hidden
#nullable disable
            );
            __builder.CloseElement();
            __builder.AddMarkupContent(12, "<br>\r\n            ");
            __builder.OpenElement(13, "label");
            __builder.AddAttribute(14, "style", "font: Regular 18px/22px Arial;letter-spacing: 0px;");
            __builder.AddContent(15, 
#nullable restore
#line 31 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
                                                                               item.noti_message

#line default
#line hidden
#nullable disable
            );
            __builder.AddContent(16, " > ");
            __builder.CloseElement();
            __builder.OpenComponent<Microsoft.AspNetCore.Components.Routing.NavLink>(17);
            __builder.AddAttribute(18, "href", 
#nullable restore
#line 31 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
                                                                                                                                item.mnu_path

#line default
#line hidden
#nullable disable
            );
            __builder.AddAttribute(19, "Match", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.Routing.NavLinkMatch>(
#nullable restore
#line 31 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
                                                                                                                                                      NavLinkMatch.All

#line default
#line hidden
#nullable disable
            ));
            __builder.AddAttribute(20, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((__builder2) => {
                __builder2.AddContent(21, 
#nullable restore
#line 31 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
                                                                                                                                                                         item.mnu_name

#line default
#line hidden
#nullable disable
                );
            }
            ));
            __builder.CloseComponent();
            __builder.AddMarkupContent(22, "<label>&nbsp;</label>");
            __builder.OpenComponent<Radzen.Blazor.RadzenButton>(23);
            __builder.AddAttribute(24, "Icon", "close");
            __builder.AddAttribute(25, "class", "btnSave");
            __builder.AddAttribute(26, "Click", Microsoft.AspNetCore.Components.CompilerServices.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback<Microsoft.AspNetCore.Components.Web.MouseEventArgs>>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create<Microsoft.AspNetCore.Components.Web.MouseEventArgs>(this, 
#nullable restore
#line 31 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
                                                                                                                                                                                                                                                                       (() => DeleteNoticeMessage(item))

#line default
#line hidden
#nullable disable
            )));
            __builder.CloseComponent();
            __builder.AddMarkupContent(27, "\r\n            <br>\r\n            <br>");
#nullable restore
#line 34 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
        }

#line default
#line hidden
#nullable disable
            __builder.CloseElement();
#nullable restore
#line 37 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
}

#line default
#line hidden
#nullable disable
        }
        #pragma warning restore 1998
#nullable restore
#line 41 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\Home.razor"
      
    [CascadingParameter] MainLayout mainLayout { get; set; }

    private List<TRNoticeMessageDTO> noticeMessageList;

    private bool pageLoading { get { return (noticeMessageList == null); } }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await mainLayout.GetLoginUser();

            if (string.IsNullOrEmpty(mainLayout.loginUser.Username) == true) navigationManager.NavigateTo("/NoPermissionPage");

            //noticeMessageList = await homeService.GetListByModelAsync(new TRNoticeMessageDTO() { noti_username = mainLayout.loginUser.Username });
            noticeMessageList = await homeService.GetListByModelByRoleAsync(new TRNoticeMessageDTO() { noti_username = mainLayout.loginUser.Username, noti_rol_code =  mainLayout.loginUser.rol_code, arh_code = mainLayout.loginUser.arh_code});

            StateHasChanged();
        }
    }

    private void DeleteNoticeMessage(TRNoticeMessageDTO noti)
    {
        noticeMessageList.Remove(noti);
        StateHasChanged();
    }


#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private HomeService homeService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager navigationManager { get; set; }
    }
}
#pragma warning restore 1591
