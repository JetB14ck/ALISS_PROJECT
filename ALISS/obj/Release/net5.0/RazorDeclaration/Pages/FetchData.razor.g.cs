// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

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
#nullable restore
#line 3 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\FetchData.razor"
using ALISS.Data;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/fetchdata")]
    public partial class FetchData : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 39 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\FetchData.razor"
       
    private WeatherForecast[] forecasts;

    protected override async Task OnInitializedAsync()
    {
        forecasts = await ForecastService.GetForecastAsync(DateTime.Now);
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private WeatherForecastService ForecastService { get; set; }
    }
}
#pragma warning restore 1591
