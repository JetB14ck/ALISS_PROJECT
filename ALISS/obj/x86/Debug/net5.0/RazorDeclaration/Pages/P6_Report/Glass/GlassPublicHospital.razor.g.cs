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
#line 3 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicHospital.razor"
using ALISS.Data.D6_Report.Glass;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicHospital.razor"
using ALISS.GLASS.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicHospital.razor"
using ALISS.Master.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicHospital.razor"
using Radzen;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicHospital.razor"
using Radzen.Blazor;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicHospital.razor"
using System.Globalization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicHospital.razor"
using System.IO;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/Glass/GlassPublicHospital")]
    public partial class GlassPublicHospital : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 151 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P6_Report\Glass\GlassPublicHospital.razor"
       

    [CascadingParameter] MainLayout mainLayout { get; set; }
    private const string MENU_CODE = "MNU_0712";
    private ConfigData configData = new ConfigData();
    private string classLabel = "col-4";
    private string classInput = "col-8";
    private List<HospitalLabDataDTO> lab_ddl_List;
    private RadzenGridCustom<GlassAnalyzeListDTO> inboxGridHospital = new RadzenGridCustom<GlassAnalyzeListDTO>();

    private int iYear = DateTime.Today.Year - 1;
    private List<GlassAnalyzeListDTO> gridHospitalDatas;
    GlassSearchDTO searchGlass = new GlassSearchDTO();
    GlassAnalyzeListDTO selectedRow = new GlassAnalyzeListDTO();
    private bool blnLoadComplete = true;
    NotificationMessage notiMsg = new NotificationMessage();

    private enum eDownloadType
    {
        Excel = 1,
        PDF = 2
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await mainLayout.GetLoginUser();

            if (mainLayout.loginUser.CheckPagePermission(MENU_CODE) == false) navigationManager.NavigateTo("/NoPermissionPage");

            configData.ConfigDTOList = await configDataService.Get_TBConfig_DataList_Async(new TBConfigDTO() { tbc_mnu_code = MENU_CODE });

            var searchData = new HospitalLabDataDTO()
            {
                arh_code = searchGlass.arh_code = mainLayout.loginUser.arh_code,
                prv_code = searchGlass.prv_code = mainLayout.loginUser.prv_code,
                hos_code = searchGlass.hos_code = mainLayout.loginUser.hos_code,
            };
            searchGlass.start_year = iYear;
            searchGlass.end_year = iYear;

            lab_ddl_List = await ddlDataService.GetAllLabListByModelAsync(searchData);
            gridHospitalDatas = await ReportService.GetGlassHospitalAnalyzeModelAsync(searchGlass);
            //await LoadData();
        }
    }

    private async Task LoadData()
    {
        gridHospitalDatas = await ReportService.GetGlassHospitalAnalyzeModelAsync(searchGlass);
        StateHasChanged();
    }

    private async Task SearchData()
    {
        searchGlass.start_year = iYear;
        searchGlass.end_year = searchGlass.start_year;

        gridHospitalDatas = null;
        StateHasChanged();

        gridHospitalDatas = await ReportService.GetGlassHospitalAnalyzeModelAsync(searchGlass);
        if (inboxGridHospital.radzenGrid != null) inboxGridHospital.radzenGrid.GoToPage(0);
        StateHasChanged();     
    }

    private async Task ClearData()
    {
        searchGlass.arh_code = mainLayout.loginUser.arh_code;
        searchGlass.prv_code = mainLayout.loginUser.prv_code;
        searchGlass.hos_code = mainLayout.loginUser.hos_code;
        searchGlass.start_year = DateTime.Today.Year - 1;

        await SearchData();
    }

    private async Task DownloadFile(GlassAnalyzeListDTO Selecteditem, eDownloadType eReportType)
    {
        if (!string.IsNullOrEmpty(Selecteditem.gls_analyze_file_name) && !string.IsNullOrEmpty(Selecteditem.gls_analyze_file_path))
        {
            blnLoadComplete = false;
            var statuscode = await ReportService.RequestAnalyseFileAsync(Selecteditem);
            if (statuscode == "OK")
            {
                string ServerFileName = string.Format("{0}\\{1}", Selecteditem.gls_analyze_file_path.Remove(0, 1), Selecteditem.gls_analyze_file_name);
                string contentType = (eReportType == eDownloadType.PDF) ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string OutputFileName = Selecteditem.gls_analyze_file_name;

                await OIFileSave.DownloadFile(ServerFileName, OutputFileName, contentType);
            }
            else if (statuscode == "ERR_PATH")
            {
                await JSRuntime.InvokeAsync<object>("ShowAlert", "ไม่พบ Config PATH กรุณาติดต่อผู้ดูแลระบบ");
            }
            else
            {
                await JSRuntime.InvokeAsync<object>("ShowAlert", "File not Found");
            }
            blnLoadComplete = true;

        }
        else
        {
            //ToDo: Path incorrect or null
            await JSRuntime.InvokeAsync<object>("ShowAlert", "Path Incorrect");
        }

    }

    //private async Task DownloadHospitalFile(GlassHospitalAnalyzeDTO Selecteditem, eDownloadType eReportType)
    //{
    //    if (!string.IsNullOrEmpty(Selecteditem.gls_analyze_file_name) && !string.IsNullOrEmpty(Selecteditem.gls_analyze_file_path))
    //    {
    //        blnLoadComplete = false;
    //        var statuscode = await ReportService.RequestHospitalAnalyseFileAsync(Selecteditem);
    //        if (statuscode == "OK")
    //        {
    //            string ServerFileName = string.Format("{0}\\{1}", Selecteditem.gls_analyze_file_path.Remove(0, 1), Selecteditem.gls_analyze_file_name);
    //            string contentType = (eReportType == eDownloadType.PDF) ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    //            string OutputFileName = Selecteditem.gls_analyze_file_name;

    //            await OIFileSave.DownloadFile(ServerFileName, OutputFileName, contentType);
    //        }
    //        else if (statuscode == "ERR_PATH")
    //        {
    //            await JSRuntime.InvokeAsync<object>("ShowAlert", "ไม่พบ Config PATH กรุณาติดต่อผู้ดูแลระบบ");
    //        }
    //        else
    //        {
    //            await JSRuntime.InvokeAsync<object>("ShowAlert", "File not Found");
    //        }
    //        blnLoadComplete = true;

    //    }
    //    else
    //    {
    //        //ToDo: Path incorrect or null
    //        await JSRuntime.InvokeAsync<object>("ShowAlert", "Path Incorrect");
    //    }

    //}

    private async Task DownloadPDFFile(GlassAnalyzeListDTO Selecteditem)
    {
        if (!string.IsNullOrEmpty(Selecteditem.gls_analyze_file_name) && !string.IsNullOrEmpty(Selecteditem.gls_analyze_file_path))
        {
            blnLoadComplete = false;
            string statuscode = await ReportService.DownloadPDFFileAsync(Selecteditem);
            if (statuscode == "OK")
            {
                string ServerFileName = string.Format("{0}\\{1}", Selecteditem.gls_analyze_file_path.Remove(0, 1), Selecteditem.gls_analyze_file_name.Replace(".xlsx", ".pdf"));
                var contentType = "application/pdf";
                var extension = Path.GetExtension(Selecteditem.gls_analyze_file_name);
                var OutputFileName = Selecteditem.gls_analyze_file_name.Replace(extension, ".pdf");

                await OIFileSave.DownloadFile(ServerFileName, OutputFileName, contentType);
            }
            else if (statuscode == "ERR_PATH")
            {
                await JSRuntime.InvokeAsync<object>("ShowAlert", "ไม่พบ Config PATH กรุณาติดต่อผู้ดูแลระบบ");
            }
            else
            {
                await JSRuntime.InvokeAsync<object>("ShowAlert", "No Data to Export");
            }
            blnLoadComplete = true;
        }
        else
        {
            //ToDo: Path incorrect or null
            await JSRuntime.InvokeAsync<object>("ShowAlert", "Path Incorrect");
        }
    }

    private void DDL_Change(string ddl_name, object value)
    {
        if (ddl_name == "Arh" && value == null)
        {
            searchGlass.prv_code = null;
            searchGlass.hos_code = null;
        }
        else if (ddl_name == "Prv" && value == null)
        {
            searchGlass.hos_code = null;
        }
        else if (ddl_name == "Prv" && value != null)
        {
            var prv_select = lab_ddl_List.FirstOrDefault(x => x.prv_code == value.ToString());

            searchGlass.arh_code = prv_select.arh_code;
        }
        else if (ddl_name == "Hos" && value == null)
        {
            //searchModel.mps_lab = null;
        }
        else if (ddl_name == "Hos" && value != null)
        {
            var hos_select = lab_ddl_List.FirstOrDefault(x => x.hos_code == value.ToString());

            searchGlass.prv_code = hos_select.prv_code;
            searchGlass.arh_code = hos_select.arh_code;
        }

        StateHasChanged();
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IFileSave OIFileSave { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private DropDownListDataService ddlDataService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager navigationManager { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private ConfigDataService configDataService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NotificationService notificationService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private DialogService dialogService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private GlassService ReportService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IJSRuntime JSRuntime { get; set; }
    }
}
#pragma warning restore 1591
