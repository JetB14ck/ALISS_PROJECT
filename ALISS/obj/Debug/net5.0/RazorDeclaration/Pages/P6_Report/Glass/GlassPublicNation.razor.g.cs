// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace ALISS.Pages.P6_Report.Glass
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
#line 3 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicNation.razor"
using ALISS.Data.D6_Report.Glass;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicNation.razor"
using ALISS.GLASS.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicNation.razor"
using Radzen;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicNation.razor"
using Radzen.Blazor;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicNation.razor"
using ALISS.Master.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicNation.razor"
using System.Globalization;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/Glass/GlassPublicNation")]
    public partial class GlassPublicNation : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 115 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicNation.razor"
       

    [CascadingParameter] MainLayout mainLayout { get; set; }
    private const string MENU_CODE = "MNU_0708";
    private ConfigData configData = new ConfigData();
    private string classLabel = "col-4";
    private string classInput = "col-8";
    private RadzenGridCustom<GlassFileListDTO> inboxGrid = new RadzenGridCustom<GlassFileListDTO>();

    [Inject]
    private IFileSave OIFileSave { get; set; }

    private int iYearFrom = DateTime.Today.Year - 1;
    private int iYearTo = DateTime.Today.Year;
    private List<GlassFileListDTO> gridDatas;
    GlassSearchDTO searchGlass = new GlassSearchDTO();
    GlassFileListDTO selectedRow = new GlassFileListDTO();

    NotificationMessage notiMsg = new NotificationMessage();
    //IList<GlassFileListDTO> selectedRow;
    private bool blnLoadComplete = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await mainLayout.GetLoginUser();

            if (mainLayout.loginUser.CheckPagePermission(MENU_CODE) == false) navigationManager.NavigateTo("/NoPermissionPage");

            configData.ConfigDTOList = await configDataService.Get_TBConfig_DataList_Async(new TBConfigDTO() { tbc_mnu_code = MENU_CODE });

            searchGlass.start_year = iYearFrom;
            searchGlass.end_year = iYearTo;
            gridDatas = await ReportService.GetGlassPublicFileListModelAsync(searchGlass);

            //dialogService.OnOpen += Open;
            //dialogService.OnClose += Close;
        }

    }

    //void Open(string title, Type type, Dictionary<string, object> parameters, DialogOptions options)
    //{
    //    StateHasChanged();
    //}

    //void Close(dynamic result)
    //{
    //    StateHasChanged();
    //}

    private async Task SearchData()
    {
        searchGlass.start_year = iYearFrom;
        searchGlass.end_year = iYearTo;

        gridDatas = null;
        StateHasChanged();

        gridDatas = await ReportService.GetGlassPublicFileListModelAsync(searchGlass);
        StateHasChanged();
    }

    private async Task ClearData()
    {
        searchGlass.start_year = DateTime.Today.Year - 1;
        searchGlass.end_year = DateTime.Today.Year;
        StateHasChanged();

        await SearchData();
    }

    private async Task DownloadSampleFile(GlassFileListDTO Selecteditem)
    {
        blnLoadComplete = false;
        if (!string.IsNullOrEmpty(Selecteditem.sample_file_name) && !string.IsNullOrEmpty(Selecteditem.sample_file_path))
        {
            string ServerFileName = string.Format("{0}//{1}", Selecteditem.sample_file_path.Remove(0, 1), Selecteditem.sample_file_name);
            string contentType = Selecteditem.sample_file_type;
            string OutputFileName = Selecteditem.sample_file_name;

            await OIFileSave.DownloadFile(ServerFileName, OutputFileName, contentType);
        }
        else
        {
            //ToDo: Path incorrect or null
            await jsRuntime.InvokeAsync<object>("ShowAlert", "File not Found");
        }
        blnLoadComplete = true;
    }

    async void DownloadRISFile(GlassFileListDTO Selecteditem)
    {
        blnLoadComplete = false;
        if (!string.IsNullOrEmpty(Selecteditem.ris_file_name) && !string.IsNullOrEmpty(Selecteditem.ris_file_path))
        {

            string ServerFileName = string.Format("{0}//{1}", Selecteditem.ris_file_path.Remove(0, 1), Selecteditem.ris_file_name);
            string contentType = Selecteditem.ris_file_type;
            string OutputFileName = Selecteditem.ris_file_name;

            await OIFileSave.DownloadFile(ServerFileName, OutputFileName, contentType);
        }
        else
        {
            //ToDo: Path incorrect or null
            await jsRuntime.InvokeAsync<object>("ShowAlert", "File not Found");
        }
        blnLoadComplete = true;
    }

    //async Task ShowNotification(NotificationMessage message)
    //{
    //    notificationService.Notify(message);
    //    await InvokeAsync(() => { StateHasChanged(); });
    //}

    

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager navigationManager { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private ConfigDataService configDataService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NotificationService notificationService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private DialogService dialogService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private GlassService ReportService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IJSRuntime jsRuntime { get; set; }
    }
}
#pragma warning restore 1591