// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace ALISS.Pages.P1_Upload
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
#nullable restore
#line 3 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P1_Upload\LabFileUploadDetail.razor"
using ALISS.Data.D1_Upload;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P1_Upload\LabFileUploadDetail.razor"
using ALISS.LabFileUpload.DTO;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/LabFileUploadDetail/{id}")]
    public partial class LabFileUploadDetail : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 206 "D:\G-able\Projects\ALISS\Project\Current\ALISS_PROJECT\ALISS\Pages\P1_Upload\LabFileUploadDetail.razor"
 
    [CascadingParameter] MainLayout mainLayout { get; set; }

    [Parameter]
    public string id { get; set; }

    private string classLabel = "col-4";
    private string classInput = "col-8";
    private string strTitle = "";

    private ConfigData configData = new ConfigData();
    LabFileUploadDataDTO labFileUploadData = new LabFileUploadDataDTO();
    List<LabFileSummaryHeaderListDTO> labFileSummaryHeader = new List<LabFileSummaryHeaderListDTO>();
    List<LabFileSummaryDetailListDTO> labFileSummaryDetail = new List<LabFileSummaryDetailListDTO>();
    List<LabFileErrorHeaderListDTO> labFileErrorHeader = new List<LabFileErrorHeaderListDTO>();
    List<LabFileErrorDetailListDTO> labFileErrorDetail = new List<LabFileErrorDetailListDTO>();
    List<LabFileLabAlertSummaryListDTO> labFileLabAlertSummary = new List<LabFileLabAlertSummaryListDTO>();

    RadzenGridCustom<LabFileSummaryHeaderListDTO> labFileSummaryGrid = new RadzenGridCustom<LabFileSummaryHeaderListDTO>();
    RadzenGridCustom<LabFileErrorHeaderListDTO> labFileErrorGrid = new RadzenGridCustom<LabFileErrorHeaderListDTO>();
    RadzenGridCustom<LabFileLabAlertSummaryListDTO> labFileLabAlertGrid = new RadzenGridCustom<LabFileLabAlertSummaryListDTO>();

    private bool showModal = true;
    private bool pageLoading { get { return (labFileUploadData == null); } }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await mainLayout.GetLoginUser();

            if (mainLayout.loginUser.CheckPagePermission("MNU_0402") == false) navigationManager.NavigateTo("/NoPermissionPage");

            configData.ConfigDTOList = await configDataService.Get_TBConfig_DataList_Async(new TBConfigDTO() { tbc_mnu_code = "MNU_0402" });

            labFileUploadData = await labFileUploadservice.GetLabFileUploadDataAsync(id);


            if (labFileUploadData.lfu_Program == "MLAB")
            {
                strTitle = string.Format("Upload Summary - {0}({1})", labFileUploadData.lfu_FileName, labFileUploadData.lfu_FileType);
            }
            else
            {
                strTitle = string.Format("Upload Summary - {0}({1})", labFileUploadData.lfu_FileName, labFileUploadData.lfu_Program);
            }

            labFileSummaryHeader = await labFileUploadservice.GetLabFileSummaryHeaderListAsync(id);
            labFileErrorHeader = await labFileUploadservice.GetLabFileErrorHeaderListAsync(id);
            //labFileErrorDetail = await labFileUploadservice.GetLabFileErrorDetailListAsync(id);
            labFileLabAlertSummary = await labFileUploadservice.GetLabFileLabAlertSummaryListAsync(id);

            //Add Get Lab Alert service
            //Require Grid UI



            if (labFileSummaryHeader.Count != 0)
            {
                //var lstlabSumDetail = await labFileUploadservice.GetLabFileSummaryDetailListBylfuIdAsync(id);
                //foreach(LabFileSummaryHeaderListDTO item in labFileSummaryHeader)
                //{
                //    var obj = lstlabSumDetail.Where(w => w.fsh_id == item.fsh_id).ToList();

                //   //item.LabFileSummaryDetailLists.Add(new LabFileSummaryDetailListDTO());

                //    obj.ForEach(item.LabFileSummaryDetailLists.Add);
                //}
                foreach (LabFileSummaryHeaderListDTO item in labFileSummaryHeader)
                {
                    item.LabFileSummaryDetailLists = await labFileUploadservice.GetLabFileSummaryDetailListAsync(item.fsh_id.ToString());
                }
            }

            showModal = false;
            StateHasChanged();
        }
    }


    void OpenLabFileUploadIndex()
    {
        navigationManager.NavigateTo("LabFileUpload");
    }


    private void SearchInboxlabFileSummaryGrid()
    {
        if (labFileSummaryGrid.radzenGrid != null) labFileSummaryGrid.radzenGrid.GoToPage(0);
        StateHasChanged();
    }


    private void SearchInboxlabFileErrorGrid()
    {
        if (labFileErrorGrid.radzenGrid != null) labFileErrorGrid.radzenGrid.GoToPage(0);
        StateHasChanged();
    }

    private void SearchInboxlabFileLabAlertGrid()
    {
        if (labFileLabAlertGrid.radzenGrid != null) labFileLabAlertGrid.radzenGrid.GoToPage(0);
        StateHasChanged();
    }


    async Task OpenMappingDetail(string id)
    {
        if (labFileUploadData.lfu_Program == "MLAB")
        {
            navigationManager.NavigateTo("MappingTemplate/" + id);
        }
        else
        {
            navigationManager.NavigateTo("MappingDetail/" + id);
        }

    }

    private async Task GenerateExcelSummary()
    {
        showModal = true;

        labFileErrorDetail = await labFileUploadservice.GetLabFileErrorDetailListAsync(id);
        labFileUploadservice.GenerateExportSummary(iJSRuntime, labFileSummaryHeader, labFileErrorDetail, labFileLabAlertSummary, labFileUploadData.lfu_FileName);

        showModal = false;
    }

    private async Task ShowConfirmDialog(string type)
    {
        if (type.Equals("Approve"))
        {
            var result = await iJSRuntime.InvokeAsync<bool>("ShowConfirm", "Confirm approve data.");
            if (result)
            {
                labFileUploadData.lfu_approveduser = mainLayout.loginUser.Username;
                labFileUploadData.lfu_approveddate = DateTime.Now;
                //labFileUploadData.lfu_updateuser = mainLayout.loginUser.Username;
                await SaveLabFileUpload();
            }
        }
        else if (type.Equals("Reprocess"))
        {
            var result = await iJSRuntime.InvokeAsync<bool>("ShowConfirm", "Confirm re-process data.");
            if (result)
            {
                labFileUploadData.lfu_status = 'R';
                labFileUploadData.lfu_updateuser = mainLayout.loginUser.Username;
                await SaveLabFileUpload();
            }
        }
        else if (type.Equals("Cancel"))
        {
            var result = await iJSRuntime.InvokeAsync<bool>("ShowConfirm", "Confirm cancel data.");
            if (result)
            {
                labFileUploadData.lfu_status = 'D';
                labFileUploadData.lfu_updateuser = mainLayout.loginUser.Username;
                await SaveLabFileUpload();
            }
        }

    }


    private async Task SaveLabFileUpload()
    {
        showModal = true;

        labFileUploadData = await labFileUploadservice.SaveLabFileUploadDataAsync(labFileUploadData);

        labFileUploadData = await labFileUploadservice.GetLabFileUploadDataAsync(id);
        labFileSummaryHeader = await labFileUploadservice.GetLabFileSummaryHeaderListAsync(id);
        labFileErrorHeader = await labFileUploadservice.GetLabFileErrorHeaderListAsync(id);
        labFileErrorDetail = await labFileUploadservice.GetLabFileErrorDetailListAsync(id);
        labFileLabAlertSummary = await labFileUploadservice.GetLabFileLabAlertSummaryListAsync(id);

        if (labFileSummaryHeader.Count != 0)
        {
            foreach (LabFileSummaryHeaderListDTO item in labFileSummaryHeader)
            {
                item.LabFileSummaryDetailLists = await labFileUploadservice.GetLabFileSummaryDetailListAsync(item.fsh_id.ToString());
            }
        }


        showModal = false;
        StateHasChanged();
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IJSRuntime iJSRuntime { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private LabFileUploadService labFileUploadservice { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private ConfigDataService configDataService { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager navigationManager { get; set; }
    }
}
#pragma warning restore 1591
