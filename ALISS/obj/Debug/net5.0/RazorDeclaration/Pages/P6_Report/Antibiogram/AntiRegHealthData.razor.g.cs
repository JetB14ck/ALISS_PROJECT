// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace ALISS.Pages.P6_Report.Antibiogram
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
#nullable restore
#line 3 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P6_Report\Antibiogram\AntiRegHealthData.razor"
using ALISS.Data.D6_Report.Antibiogram;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P6_Report\Antibiogram\AntiRegHealthData.razor"
using ALISS.ANTIBIOGRAM.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P6_Report\Antibiogram\AntiRegHealthData.razor"
using ALISS.Master.DTO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P6_Report\Antibiogram\AntiRegHealthData.razor"
using Radzen;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P6_Report\Antibiogram\AntiRegHealthData.razor"
using Radzen.Blazor;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/Report/AntiRegHealthData")]
    public partial class AntiRegHealthData : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 142 "D:\G-able\GitHub\ALISS_PROJECT\ALISS\Pages\P6_Report\Antibiogram\AntiRegHealthData.razor"
       

    [CascadingParameter] MainLayout mainLayout { get; set; }
    private const string MENU_CODE = "MNU_0702";
    private List<HospitalLabDataDTO> lab_ddl_List;
    private RadzenGridCustom<AntibiogramDataDTO> inboxGrid = new RadzenGridCustom<AntibiogramDataDTO>();
    private ConfigData configData = new ConfigData();
    private string classLabel = "col-4";
    private string classInput = "col-8";
    private string classMonth = "col-4";

    private List<AntibiogramDataDTO> gridDatas;
    private List<MasterDataDTO> AreaHealthData;
    private string _Action = "";
    AntiAreaHealthSearchDTO searchAnti = new AntiAreaHealthSearchDTO();
    private List<SpecimenDTO> SpecimenMaster = new List<SpecimenDTO>();
    private int iMonthFrom = 1;
    private int iMonthTo = DateTime.Today.Month;
    private int iYearFrom = DateTime.Today.Year;
    private int iYearTo = DateTime.Today.Year;
    private DateTime dtStartDate;
    private DateTime dtEndDate;

    private class MonthItem
    {
        public int MonthValue { get; set; }
        public string MonthName { get; set; }
    }

    MonthItem[] MonthData = new MonthItem[] {
        new MonthItem
        {
            MonthValue = 1,
            MonthName = "January"
        },
        new MonthItem
        {
           MonthValue = 2,
           MonthName = "Febuary"
        },
        new MonthItem
        {
          MonthValue = 3,
          MonthName = "March"
        },
        new MonthItem
        {
           MonthValue = 4,
           MonthName = "April"
        },
        new MonthItem
        {
           MonthValue = 5,
           MonthName = "May"
        },
        new MonthItem
        {
            MonthValue = 6,
            MonthName = "June"
        },
        new MonthItem
        {
           MonthValue = 7,
            MonthName = "July"
        },
        new MonthItem
        {
           MonthValue = 8,
            MonthName = "August"
        },
        new MonthItem
        {
            MonthValue = 9,
            MonthName = "September"
        },
        new MonthItem
        {
           MonthValue = 10,
            MonthName = "October"
        },
        new MonthItem
        {
            MonthValue = 11,
            MonthName = "November"
        },
        new MonthItem
        {
           MonthValue = 12,
           MonthName = "December"
        }
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await mainLayout.GetLoginUser();

            if (mainLayout.loginUser.CheckPagePermission(MENU_CODE) == false) navigationManager.NavigateTo("/NoPermissionPage");

            configData.ConfigDTOList = await configDataService.Get_TBConfig_DataList_Async(new TBConfigDTO() { tbc_mnu_code = MENU_CODE });

            var searchData = new HospitalLabDataDTO()
            {
                arh_code = searchAnti.arh_code = mainLayout.loginUser.arh_code,
            };

            lab_ddl_List = await ddlDataService.GetAllLabListByModelAsync(searchData);
            var objspecimen = await specimenService.GetListByModelActiveAsync(new SpecimenDTO());
            objspecimen.Add(new SpecimenDTO{ spc_code = "A", spc_name = "All Specimen"} );
            SpecimenMaster = objspecimen.OrderBy(w => w.spc_code).ToList();

            gridDatas = await ReportService.GetAntibiogramAreaHealthListModelAsync(searchAnti);
        }
        
    }

    private void DDL_Change(string ddl_name, object value)
    {
        StateHasChanged();
    }
   
    private async Task SearchData()
    {
        dtStartDate = new DateTime(iYearFrom, iMonthFrom, 1);

        int numberOfDays = DateTime.DaysInMonth(iYearTo, iMonthTo);
        dtEndDate = new DateTime(iYearTo, iMonthTo, numberOfDays);

        searchAnti.start_date = dtStartDate;
        searchAnti.end_date = dtEndDate;
        gridDatas = null;
        StateHasChanged();

        gridDatas = await ReportService.GetAntibiogramAreaHealthListModelAsync(searchAnti);
        StateHasChanged();
    }

    private async Task ClearData()
    {
        searchAnti.arh_code = mainLayout.loginUser.arh_code;
        searchAnti.spc_code = null;        
        StateHasChanged();

        await SearchData();
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private SpecimenService specimenService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private DropDownListDataService ddlDataService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager navigationManager { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private ConfigDataService configDataService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private DialogService dialogService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private ReportService ReportService { get; set; }
    }
}
#pragma warning restore 1591
