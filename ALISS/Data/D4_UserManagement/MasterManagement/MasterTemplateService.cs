using ALISS.Data.Client;
using ALISS.DropDownList.DTO;
using ALISS.Master.DTO;
using ALISS.MasterManagement.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ALISS.Data.D4_UserManagement.MasterManagement
{
    public class MasterTemplateService
    {
        private IConfiguration Configuration { get; }

        private ApiHelper _apiHelper;

        public MasterTemplateService(IConfiguration configuration)
        {
            _apiHelper = new ApiHelper(configuration["ApiClient:ApiUrl"]);
        }

        public async Task<List<MasterTemplateDTO>> GetListAsync()
        {
            List<MasterTemplateDTO> objList = new List<MasterTemplateDTO>();

            objList = await _apiHelper.GetDataListAsync<MasterTemplateDTO>("mastertemplate_api/Get_List");

            return objList;
        }

        public async Task<List<MasterTemplateDTO>> GetListByParamAsync(MasterTemplateSearchDTO searchData)
        {
            List<MasterTemplateDTO> objList = new List<MasterTemplateDTO>();

            var searchJson = JsonSerializer.Serialize(searchData);

            objList = await _apiHelper.GetDataListByParamsAsync<MasterTemplateDTO>("mastertemplate_api/Get_List", searchJson);

            return objList;
        }

        public async Task<List<MasterTemplateDTO>> GetListByModelAsync(MasterTemplateSearchDTO searchData)
        {
            List<MasterTemplateDTO> objList = new List<MasterTemplateDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<MasterTemplateDTO, MasterTemplateSearchDTO>("mastertemplate_api/Get_ListByModel", searchData);

            return objList;
        }

        public async Task<MasterTemplateDTO> GetListByModelActiveAsync(MasterTemplateSearchDTO searchData)
        {
            MasterTemplateDTO objList = new MasterTemplateDTO();

            objList = await _apiHelper.GetDataByModelAsync<MasterTemplateDTO, MasterTemplateSearchDTO>("mastertemplate_api/Get_List_Active_ByModel", searchData);

            return objList;
        }

        public async Task<MasterTemplateDTO> GetDataAsync(string mst_code)
        {
            MasterTemplateDTO menu = new MasterTemplateDTO();

            menu = await _apiHelper.GetDataByIdAsync<MasterTemplateDTO>("mastertemplate_api/Get_Data", mst_code);

            return menu;
        }

        public async Task<MasterTemplateDTO> SaveDataAsync(MasterTemplateDTO model)
        {
            //MenuDTO menu = new MenuDTO()
            //{
            //    mnu_id = "2",
            //    mnu_name = "TEST_name",
            //    mnu_area = "TEST_area",
            //    mnu_controller = "TEST_controller"
            //};

            var menua = await _apiHelper.PostDataAsync<MasterTemplateDTO>("mastertemplate_api/Post_SaveData", model);

            return menua;
        }

        public async Task<MasterTemplateDTO> SaveCopyDataAsync(MasterTemplateDTO model)
        {
            //MenuDTO menu = new MenuDTO()
            //{
            //    mnu_id = "2",
            //    mnu_name = "TEST_name",
            //    mnu_area = "TEST_area",
            //    mnu_controller = "TEST_controller"
            //};

            var menua = await _apiHelper.PostDataAsync<MasterTemplateDTO>("mastertemplate_api/Post_SaveCopyData", model);

            return menua;
        }

        public async Task<List<LogProcessDTO>> GetHistoryAsync(string tran_id)
        {
            List<LogProcessDTO> objList = new List<LogProcessDTO>();

            LogProcessSearchDTO searchData = new LogProcessSearchDTO() { log_mnu_name = "MasterTemplate", log_tran_id = tran_id };

            objList = await _apiHelper.GetDataListByModelAsync<LogProcessDTO, LogProcessSearchDTO>("logprocess_api/Get_List", searchData);

            return objList;
        }

        public Task<GridData[]> GetHistoryAsync()
        {
            var rng = new Random();
            var objReturn = new List<GridData>(){
                new GridData { Column_01 = "(A0000)Super admin", Column_02 = "Active role", Column_03 = "Lorem", Column_04 = "00/00/00 00:00", Column_05 = "" },
                new GridData { Column_01 = "(A0000)Super admin", Column_02 = "Inactive role", Column_03 = "Lorem", Column_04 = "00/00/00 00:00", Column_05 = "" },
            };
            return Task.FromResult(objReturn.ToArray());
        }

        #region WHONet

        public async Task<List<TCWHONET_AntibioticsDTO>> Get_TCWHONET_Antibiotics_List(TCWHONET_AntibioticsDTO searchModel)
        {
            List<TCWHONET_AntibioticsDTO> objList = new List<TCWHONET_AntibioticsDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<TCWHONET_AntibioticsDTO, TCWHONET_AntibioticsDTO>("whonetdata_api/WHONET_Antibiotics_List", searchModel);

            return objList.OrderBy(x => x.who_ant_code).ToList();
        }

        public async Task<List<TCWHONET_AntibioticSpeciesDTO>> Get_TCWHONET_AntibioticSpecies_List(TCWHONET_AntibioticSpeciesDTO searchModel)
        {
            List<TCWHONET_AntibioticSpeciesDTO> objList = new List<TCWHONET_AntibioticSpeciesDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<TCWHONET_AntibioticSpeciesDTO, TCWHONET_AntibioticSpeciesDTO>("whonetdata_api/WHONET_AntibioticSpecies_List", searchModel);

            return objList.OrderBy(x => x.who_ant_spe_ant_code).ToList();
        }

        public async Task<List<TCWHONET_DepartmentsDTO>> Get_TCWHONET_Departments_List(TCWHONET_DepartmentsDTO searchModel)
        {
            List<TCWHONET_DepartmentsDTO> objList = new List<TCWHONET_DepartmentsDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<TCWHONET_DepartmentsDTO, TCWHONET_DepartmentsDTO>("whonetdata_api/WHONET_Departments_List", searchModel);

            return objList.OrderBy(x => x.who_dep_code).ToList();
        }

        public async Task<List<TCWHONET_SpecimenDTO>> Get_TCWHONET_Specimen_List(TCWHONET_SpecimenDTO searchModel)
        {
            List<TCWHONET_SpecimenDTO> objList = new List<TCWHONET_SpecimenDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<TCWHONET_SpecimenDTO, TCWHONET_SpecimenDTO>("whonetdata_api/WHONET_Specimen_List", searchModel);

            return objList.OrderBy(x => x.who_spc_code).ToList();
        }

        public async Task<List<TCWHONETColumnDTO>> Get_TCWHONET_Column_List(TCWHONETColumnDTO searchModel)
        {
            List<TCWHONETColumnDTO> objList = new List<TCWHONETColumnDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<TCWHONETColumnDTO, TCWHONETColumnDTO>("whonetdata_api/WHONET_Column_List", searchModel);

            return objList.OrderBy(x => x.wnc_code).ToList();
        }

        public async Task<List<TCWHONET_OrganismDTO>> Get_TCWHONET_Organism_List(TCWHONET_OrganismDTO searchModel)
        {
            List<TCWHONET_OrganismDTO> objList = new List<TCWHONET_OrganismDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<TCWHONET_OrganismDTO, TCWHONET_OrganismDTO>("whonetdata_api/WHONET_Organism_List", searchModel);

            return objList.OrderBy(x => x.who_org).ToList();
        }

        public async Task<List<TCWHONET_MacroDTO>> Get_TCWHONET_Macro_List(TCWHONET_MacroDTO searchModel)
        {
            List<TCWHONET_MacroDTO> objList = new List<TCWHONET_MacroDTO>();

            objList = await _apiHelper.GetDataListByModelAsync<TCWHONET_MacroDTO, TCWHONET_MacroDTO>("whonetdata_api/WHONET_Macro_List", searchModel);

            return objList.OrderBy(x => x.who_mac_name).ToList();
        }

        public async Task<TCWHONET_AntibioticsDTO> SaveWHONETAntibioticsListDataAsync(List<TCWHONET_AntibioticsDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_AntibioticsDTO>("whonetdata_api/Post_WhonetAntibiotics_SaveListData", models);

            return _object;
        }

        public async Task<TCWHONET_AntibioticsDTO> DeleteWHONETAntibioticsListDataAsyn(List<TCWHONET_AntibioticsDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_AntibioticsDTO>("whonetdata_api/Post_WhonetAntibiotics_DeleteListData", models);

            return _object;
        }

        public async Task<TCWHONET_AntibioticSpeciesDTO> SaveAntibioticSpeciesListDataAsync(List<TCWHONET_AntibioticSpeciesDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_AntibioticSpeciesDTO>("whonetdata_api/Post_AntibioticSpecies_SaveListData", models);

            return _object;
        }

        public async Task<TCWHONET_AntibioticSpeciesDTO> DeleteAntibioticSpeciesListDataAsyn(List<TCWHONET_AntibioticSpeciesDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_AntibioticSpeciesDTO>("whonetdata_api/Post_AntibioticSpecies_DeleteListData", models);

            return _object;
        }

        public async Task<TCWHONET_DepartmentsDTO> SaveDepartmentsListDataAsync(List<TCWHONET_DepartmentsDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_DepartmentsDTO>("whonetdata_api/Post_Departments_SaveListData", models);

            return _object;
        }

        public async Task<TCWHONET_DepartmentsDTO> DeleteDepartmentsListDataAsyn(List<TCWHONET_DepartmentsDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_DepartmentsDTO>("whonetdata_api/Post_Departments_DeleteListData", models);

            return _object;
        }

        public async Task<TCAntibioticDTO> SaveAntibioticListDataAsync(List<TCAntibioticDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCAntibioticDTO>("whonetdata_api/Post_Antibiotic_SaveListData", models);

            return _object;
        }
        public async Task<TCAntibioticDTO> DeleteAntibioticListDataAsyn(List<TCAntibioticDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCAntibioticDTO>("whonetdata_api/Post_Antibiotic_DeleteListData", models);

            return _object;
        }

        public async Task<TCWHONET_SpecimenDTO> SaveSpecimenListDataAsync(List<TCWHONET_SpecimenDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_SpecimenDTO>("whonetdata_api/Post_Specimen_SaveListData", models);

            return _object;
        }
        public async Task<TCWHONET_SpecimenDTO> DeleteSpecimenListDataAsyn(List<TCWHONET_SpecimenDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_SpecimenDTO>("whonetdata_api/Post_Specimen_DeleteListData", models);

            return _object;
        }

        public async Task<TCWHONETColumnDTO> SaveColumnListDataAsync(List<TCWHONETColumnDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONETColumnDTO>("whonetdata_api/Post_Column_SaveListData", models);

            return _object;
        }

        public async Task<TCWHONETColumnDTO> DeleteColumnListDataAsyn(List<TCWHONETColumnDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONETColumnDTO>("whonetdata_api/Post_Column_DeleteListData", models);

            return _object;
        }

        public async Task<TCWHONET_OrganismDTO> SaveOrganismListDataAsync(List<TCWHONET_OrganismDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_OrganismDTO>("whonetdata_api/Post_Organism_SaveListData", models);

            return _object;
        }

        public async Task<TCWHONET_OrganismDTO> DeleteOrganismListDataAsyn(List<TCWHONET_OrganismDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_OrganismDTO>("whonetdata_api/Post_Organism_DeleteListData", models);

            return _object;
        }

        public async Task<TCWHONET_MacroDTO> SaveMacroListDataAsync(List<TCWHONET_MacroDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_MacroDTO>("whonetdata_api/Post_Macro_SaveListData", models);

            return _object;
        }
        public async Task<TCWHONET_MacroDTO> DeleteMacroListDataAsyn(List<TCWHONET_MacroDTO> models)
        {
            var _object = await _apiHelper.PostListofDataAsync<TCWHONET_MacroDTO>("whonetdata_api/Post_Macro_DeleteListData", models);

            return _object;
        }
        #endregion


        public async Task<string> GetPath()
        {
            var ErrorMessage = new List<TCProcessExcelErrorDTO>();

            string path = "";
            List<ParameterDTO> objParamList = new List<ParameterDTO>();
            var searchModel = new ParameterDTO() { prm_code_major = "MASTER_TEMPLATE_PATH" };

            objParamList = await _apiHelper.GetDataListByModelAsync<ParameterDTO, ParameterDTO>("dropdownlist_api/GetParameterList", searchModel);

            if (objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH") != null)
            {
                path = objParamList.FirstOrDefault(x => x.prm_code_minor == "PATH").prm_value;
            }
            else
            {
            }

            return path;
        }
        public async Task<List<TCProcessExcelErrorDTO>> ValidateAndImportFile(string path, MasterTemplateDTO masterTemplate, string mst_code, string createUser,bool bReplace)
        {
            DateTime createDate = DateTime.Now;
            var ErrorMessage = new List<TCProcessExcelErrorDTO>();
            string tableMapping = string.Empty;
            int tabLineIndex = 0;

            List<TCWHONET_AntibioticsDTO> tCWHONET_Antibiotics = new List<TCWHONET_AntibioticsDTO>();
            List<TCWHONET_AntibioticSpeciesDTO> tCWHONET_AntibioticSpecies = new List<TCWHONET_AntibioticSpeciesDTO>();
            List<TCWHONET_DepartmentsDTO> tCWHONET_Departments = new List<TCWHONET_DepartmentsDTO>();
            List<TCWHONETColumnDTO> tCWHONETColumns = new List<TCWHONETColumnDTO>();

            try
            {
                //path = Path.Combine(path);
                #region Read File Data
                //if (Path.GetExtension(fileName) == ".xlsm")
                var dataRows = File.ReadAllLines(path);
                foreach(var (line, index) in dataRows.Select((v, i) => (v, i)))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.IndexOf("[") != -1 && line.IndexOf("]") != -1)
                        {
                            switch (line)
                            {
                                case "[Antibiotics]": tableMapping = "TCWHONET_Antibiotics"; tabLineIndex = index; continue;
                                case "[Antibiotic Species-Specific Breakpoints]": tableMapping = "TCWHONET_AntibioticSpecies"; continue;
                                case "[Data fields]": tableMapping = "TCWHONETColumn"; continue;
                                case "[Departments]": tableMapping = "TCWHONET_Departments"; continue;
                                default: tableMapping = string.Empty; continue;
                            }
                        }

                        #region TCWHONET_Antibiotics
                        if (tableMapping == "TCWHONET_Antibiotics")
                        {
                            var datas = line.Replace("\"", "").Split(",");
                            if (datas.Length > 1)
                            {
                                var subDatas = datas[1].Split("_");
                                tCWHONET_Antibiotics.Add(new TCWHONET_AntibioticsDTO
                                {
                                    who_ant_mst_code = mst_code,
                                    who_ant_source = line,
                                    who_ant_code = datas[0],
                                    who_ant_name = subDatas.Length >= 1 ? subDatas[0] : string.Empty,
                                    who_ant_type = subDatas.Length >= 2 ? subDatas[1] : string.Empty,
                                    who_ant_lab = subDatas.Length >= 3 ? subDatas[2] : string.Empty,
                                    who_ant_size = subDatas.Length >= 4 ? subDatas[3] : string.Empty,
                                    who_ant_S = datas.Length > 2 ? datas[2] : string.Empty,
                                    who_ant_I = datas.Length == 5 ? datas[3] : string.Empty,
                                    who_ant_R = datas.Length == 5 ? datas[4] : datas[3],
                                    who_ant_active = true,
                                    who_ant_createdate = createDate,
                                    who_ant_createuser = createUser
                                });
                            }
                        }
                        #endregion

                        #region TCWHONET_AntibioticSpecies
                        if (tableMapping == "TCWHONET_AntibioticSpecies")
                        {
                            var datas = line.Replace("\"", "").Split(",");
                            if (datas.Length > 1)
                            {
                                tCWHONET_AntibioticSpecies.Add(new TCWHONET_AntibioticSpeciesDTO
                                {
                                    who_mst_code = mst_code,
                                    who_ant_spe_source = line,
                                    who_ant_spe_org_code = datas.Length > 0 ?datas[0]:string.Empty,
                                    who_ant_spe_ant_code = datas.Length > 1 ? datas[1] : string.Empty,
                                    who_ant_spe_I_low = datas.Length > 2 ? datas[2] : string.Empty,
                                    who_ant_spe_I_up = datas.Length > 3 ? datas[3] : string.Empty,
                                    who_ant_spe_spc = datas.Length > 4 ? datas[4] : string.Empty,
                                    who_ant_spe_1 = datas.Length > 5 ? datas[5] : string.Empty,
                                    who_ant_spe_2 = datas.Length > 6 ? datas[6] : string.Empty,
                                    who_ant_spe_active = true,
                                    who_ant_spe_createdate = createDate,
                                    who_ant_spe_createuser = createUser
                                });
                            }
                        }
                        #endregion

                        #region TCWHONETColumn
                        if (tableMapping == "TCWHONETColumn")
                        {
                            var datas = line.Replace("\"", "").Split(",");
                            if (datas.Length > 1)
                            {
                                tCWHONETColumns.Add(new TCWHONETColumnDTO
                                {
                                    wnc_mst_code = mst_code,
                                    wnc_code = datas.Length > 1 ? datas[1] : string.Empty,
                                    wnc_name = datas.Length > 0 ? datas[0] : string.Empty,
                                    wnc_data_type = datas.Length > 3 ? datas[3] : string.Empty,
                                    wnc_active = true,
                                    wnc_mendatory_his = true,
                                    wnc_mendatory_lab = true,
                                    wnc_createdate = createDate,
                                    wnc_createuser = createUser
                                });
                            }
                        }
                        #endregion

                        #region TCWHONET_Departments
                        if (tableMapping == "TCWHONET_Departments")
                        {
                            var datas = line.Replace("\"", "").Split(",");
                            if (datas.Length > 1)
                            {
                                tCWHONET_Departments.Add(new TCWHONET_DepartmentsDTO
                                {
                                    who_mst_code = mst_code,
                                    who_dep_code = datas.Length > 0 ? datas[0] : string.Empty,
                                    who_dep_name = datas.Length > 1 ? datas[1] : string.Empty,
                                    who_dep_active = true,
                                    who_dep_createdate = createDate,
                                    who_dep_createuser = createUser
                                });
                            }
                        }
                        #endregion
                    }
                }

                #endregion

                #region Save Data

                // Delete Old Data
                if (bReplace)
                {
                    var delAntibiotics = await DeleteWHONETAntibioticsListDataAsyn(tCWHONET_Antibiotics);
                    var delAntibioticSpicies = await DeleteAntibioticSpeciesListDataAsyn(tCWHONET_AntibioticSpecies);
                    var delDepartments = await DeleteDepartmentsListDataAsyn(tCWHONET_Departments);
                    var delColumn = await DeleteColumnListDataAsyn(tCWHONETColumns);
                }
                var resultAntibiotics = await SaveWHONETAntibioticsListDataAsync(tCWHONET_Antibiotics);
                var resultAntibioticSpicies = await SaveAntibioticSpeciesListDataAsync(tCWHONET_AntibioticSpecies);
                var resultDepartments = await SaveDepartmentsListDataAsync(tCWHONET_Departments);
                var resultColumn = await SaveColumnListDataAsync(tCWHONETColumns);
                #endregion
            }
            catch (Exception ex)
            {
                ErrorMessage.Add(new TCProcessExcelErrorDTO
                {
                    tcp_status = 'E',
                    tcp_Err_type = 'E',
                    tcp_Err_No = "",
                    tcp_Err_SheetName = "Total",
                    tcp_Err_Message = ex.Message
                });
            }

            return ErrorMessage;
        }

        public async Task<List<TCProcessExcelErrorDTO>> ValidateAndImportAntibioticFile(string path, MasterTemplateDTO masterTemplate, string mst_code, string createUser, bool bReplace)
        {
            DateTime createDate = DateTime.Now;
            var ErrorMessage = new List<TCProcessExcelErrorDTO>();
            string tableMapping = string.Empty;

            List<TCAntibioticDTO> tCAntibiotic = new List<TCAntibioticDTO>();

            try
            {
                //path = Path.Combine(path);
                #region Read File Data            
                var dataRows = File.ReadAllLines(path);
                int WHON5_CODEIndex = 0;
                int WHON4_CODEIndex = 0;
                int WHO_CODEIndex = 0;
                int DIN_CODEIndex = 0;
                int JAC_CODEIndex = 0;
                int USER_CODEIndex = 0;
                int ANTIBIOTICIndex = 0;
                int GUIDELINESIndex = 0;
                int CLSIIndex = 0;
                int EUCASTIndex = 0;
                int SFMIndex = 0;
                int SRGAIndex = 0;
                int BSACIndex = 0;
                int DINIndex = 0;
                int NEOIndex = 0;
                int AFAIndex = 0;
                int ABX_NUMBERIndex = 0;
                int POTENCYIndex = 0;
                int BETALACTAMIndex = 0;
                int CLASSIndex = 0;
                int SUBCLASSIndex = 0;
                int PROF_CLASSIndex = 0;
                int CLSI_ORDERIndex = 0;
                int HUMANIndex = 0;
                int VETERINARYIndex = 0;
                int ANIMAL_GPIndex = 0;
                int WHO_IMPORTIndex = 0;
                int LOINCCOMPIndex = 0;
                int LOINCGENIndex = 0;
                int LOINCDISKIndex = 0;
                int LOINCMICIndex = 0;
                int LOINCETESTIndex = 0;
                int LOINCSLOWIndex = 0;
                int LOINCAFBIndex = 0;
                int LOINCSBTIndex = 0;
                int LOINCMLCIndex = 0;
                int CLSI19_DRIndex = 0;
                int CLSI19_DIIndex = 0;
                int CLSI19_DSIndex = 0;
                int CLSI19_MSIndex = 0;
                int CLSI19_MRIndex = 0;
                int CLSI18_DRIndex = 0;
                int CLSI18_DIIndex = 0;
                int CLSI18_DSIndex = 0;
                int CLSI18_MSIndex = 0;
                int CLSI18_MRIndex = 0;
                int CLSI17_DRIndex = 0;
                int CLSI17_DIIndex = 0;
                int CLSI17_DSIndex = 0;
                int CLSI17_MSIndex = 0;
                int CLSI17_MRIndex = 0;
                int CLSI16_DRIndex = 0;
                int CLSI16_DIIndex = 0;
                int CLSI16_DSIndex = 0;
                int CLSI16_MSIndex = 0;
                int CLSI16_MRIndex = 0;
                int CLSI15_DRIndex = 0;
                int CLSI15_DIIndex = 0;
                int CLSI15_DSIndex = 0;
                int CLSI15_MSIndex = 0;
                int CLSI15_MRIndex = 0;
                int CLSI14_DRIndex = 0;
                int CLSI14_DIIndex = 0;
                int CLSI14_DSIndex = 0;
                int CLSI14_MSIndex = 0;
                int CLSI14_MRIndex = 0;
                int CLSI13_DRIndex = 0;
                int CLSI13_DIIndex = 0;
                int CLSI13_DSIndex = 0;
                int CLSI13_MSIndex = 0;
                int CLSI13_MRIndex = 0;
                int CLSI12_DRIndex = 0;
                int CLSI12_DIIndex = 0;
                int CLSI12_DSIndex = 0;
                int CLSI12_MSIndex = 0;
                int CLSI12_MRIndex = 0;
                int CLSI11_DRIndex = 0;
                int CLSI11_DIIndex = 0;
                int CLSI11_DSIndex = 0;
                int CLSI11_MSIndex = 0;
                int CLSI11_MRIndex = 0;
                int CLSI10_DRIndex = 0;
                int CLSI10_DIIndex = 0;
                int CLSI10_DSIndex = 0;
                int CLSI10_MSIndex = 0;
                int CLSI10_MRIndex = 0;
                int EUCST19_DRIndex = 0;
                int EUCST19_DIIndex = 0;
                int EUCST19_DSIndex = 0;
                int EUCST19_MSIndex = 0;
                int EUCST19_MRIndex = 0;
                int EUCST18_DRIndex = 0;
                int EUCST18_DIIndex = 0;
                int EUCST18_DSIndex = 0;
                int EUCST18_MSIndex = 0;
                int EUCST18_MRIndex = 0;
                int EUCST17_DRIndex = 0;
                int EUCST17_DIIndex = 0;
                int EUCST17_DSIndex = 0;
                int EUCST17_MSIndex = 0;
                int EUCST17_MRIndex = 0;
                int EUCST16_DRIndex = 0;
                int EUCST16_DIIndex = 0;
                int EUCST16_DSIndex = 0;
                int EUCST16_MSIndex = 0;
                int EUCST16_MRIndex = 0;
                int EUCST15_DRIndex = 0;
                int EUCST15_DIIndex = 0;
                int EUCST15_DSIndex = 0;
                int EUCST15_MSIndex = 0;
                int EUCST15_MRIndex = 0;
                int EUCST14_DRIndex = 0;
                int EUCST14_DIIndex = 0;
                int EUCST14_DSIndex = 0;
                int EUCST14_MSIndex = 0;
                int EUCST14_MRIndex = 0;
                int EUCST13_DRIndex = 0;
                int EUCST13_DIIndex = 0;
                int EUCST13_DSIndex = 0;
                int EUCST13_MSIndex = 0;
                int EUCST13_MRIndex = 0;
                int ECV12_DRIndex = 0;
                int ECV12_DIIndex = 0;
                int ECV12_DSIndex = 0;
                int ECV12_MSIndex = 0;
                int ECV12_MRIndex = 0;
                int EUCST11_DRIndex = 0;
                int EUCST11_DIIndex = 0;
                int EUCST11_DSIndex = 0;
                int EUCST11_MSIndex = 0;
                int EUCST11_MRIndex = 0;
                int NEODK16_DRIndex = 0;
                int NEODK16_DIIndex = 0;
                int NEODK16_DSIndex = 0;
                int NEODK16_MSIndex = 0;
                int NEODK16_MRIndex = 0;
                int NEODK13_DRIndex = 0;
                int NEODK13_DIIndex = 0;
                int NEODK13_DSIndex = 0;
                int NEODK13_MSIndex = 0;
                int NEODK13_MRIndex = 0;
                int NEODK98_DRIndex = 0;
                int NEODK98_DIIndex = 0;
                int NEODK98_DSIndex = 0;
                int NEODK98_MSIndex = 0;
                int NEODK98_MRIndex = 0;
                int SFM07_DRIndex = 0;
                int SFM07_DIIndex = 0;
                int SFM07_DSIndex = 0;
                int SFM07_MSIndex = 0;
                int SFM07_MRIndex = 0;
                int DIN04_DRIndex = 0;
                int DIN04_DIIndex = 0;
                int DIN04_DSIndex = 0;
                int DIN04_MSIndex = 0;
                int DIN04_MRIndex = 0;
                int CRG96_DRIndex = 0;
                int CRG96_DIIndex = 0;
                int CRG96_DSIndex = 0;
                int CRG96_MSIndex = 0;
                int CRG96_MRIndex = 0;
                int AFA00_DRIndex = 0;
                int AFA00_DIIndex = 0;
                int AFA00_DSIndex = 0;
                int AFA00_MSIndex = 0;
                int AFA00_MRIndex = 0;
                int MENS00_DRIndex = 0;
                int MENS00_DIIndex = 0;
                int MENS00_DSIndex = 0;
                int MENS00_MSIndex = 0;
                int MENS00_MRIndex = 0;
                int SRGA98_DRIndex = 0;
                int SRGA98_DIIndex = 0;
                int SRGA98_DSIndex = 0;
                int SRGA98_MSIndex = 0;
                int SRGA98_MRIndex = 0;
                int BSAC00_DRIndex = 0;
                int BSAC00_DIIndex = 0;
                int BSAC00_DSIndex = 0;
                int BSAC00_MSIndex = 0;
                int BSAC00_MRIndex = 0;
                int OTHER_DRIndex = 0;
                int OTHER_DIIndex = 0;
                int OTHER_DSIndex = 0;
                int OTHER_MSIndex = 0;
                int OTHER_MRIndex = 0;
                foreach (var (line, index) in dataRows.Select((v, i) => (v, i)))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var datas = line.Split("\t");
                        //Header
                        if (index == 0)
                        {
                            WHON5_CODEIndex = Array.IndexOf(datas, "WHON5_CODE");
                            WHON4_CODEIndex = Array.IndexOf(datas, "WHON4_CODE");
                            WHO_CODEIndex = Array.IndexOf(datas, "WHO_CODE");
                            DIN_CODEIndex = Array.IndexOf(datas, "DIN_CODE");
                            JAC_CODEIndex = Array.IndexOf(datas, "JAC_CODE");
                            USER_CODEIndex = Array.IndexOf(datas, "USER_CODE");
                            ANTIBIOTICIndex = Array.IndexOf(datas, "ANTIBIOTIC");
                            GUIDELINESIndex = Array.IndexOf(datas, "GUIDELINES");
                            CLSIIndex = Array.IndexOf(datas, "CLSI");
                            EUCASTIndex = Array.IndexOf(datas, "EUCAST");
                            SFMIndex = Array.IndexOf(datas, "SFM");
                            SRGAIndex = Array.IndexOf(datas, "SRGA");
                            BSACIndex = Array.IndexOf(datas, "BSAC");
                            DINIndex = Array.IndexOf(datas, "DIN");
                            NEOIndex = Array.IndexOf(datas, "NEO");
                            AFAIndex = Array.IndexOf(datas, "AFA");
                            ABX_NUMBERIndex = Array.IndexOf(datas, "ABX_NUMBER");
                            POTENCYIndex = Array.IndexOf(datas, "POTENCY");
                            BETALACTAMIndex = Array.IndexOf(datas, "BETALACTAM");
                            CLASSIndex = Array.IndexOf(datas, "CLASS");
                            SUBCLASSIndex = Array.IndexOf(datas, "SUBCLASS");
                            PROF_CLASSIndex = Array.IndexOf(datas, "PROF_CLASS");
                            CLSI_ORDERIndex = Array.IndexOf(datas, "CLSI_ORDER");
                            HUMANIndex = Array.IndexOf(datas, "HUMAN");
                            VETERINARYIndex = Array.IndexOf(datas, "VETERINARY");
                            ANIMAL_GPIndex = Array.IndexOf(datas, "ANIMAL_GP");
                            WHO_IMPORTIndex = Array.IndexOf(datas, "WHO_IMPORT");
                            LOINCCOMPIndex = Array.IndexOf(datas, "LOINCCOMP");
                            LOINCGENIndex = Array.IndexOf(datas, "LOINCGEN");
                            LOINCDISKIndex = Array.IndexOf(datas, "LOINCDISK");
                            LOINCMICIndex = Array.IndexOf(datas, "LOINCMIC");
                            LOINCETESTIndex = Array.IndexOf(datas, "LOINCETEST");
                            LOINCSLOWIndex = Array.IndexOf(datas, "LOINCSLOW");
                            LOINCAFBIndex = Array.IndexOf(datas, "LOINCAFB");
                            LOINCSBTIndex = Array.IndexOf(datas, "LOINCSBT");
                            LOINCMLCIndex = Array.IndexOf(datas, "LOINCMLC");
                            CLSI19_DRIndex = Array.IndexOf(datas, "CLSI19_DR");
                            CLSI19_DIIndex = Array.IndexOf(datas, "CLSI19_DI");
                            CLSI19_DSIndex = Array.IndexOf(datas, "CLSI19_DS");
                            CLSI19_MSIndex = Array.IndexOf(datas, "CLSI19_MS");
                            CLSI19_MRIndex = Array.IndexOf(datas, "CLSI19_MR");
                            CLSI18_DRIndex = Array.IndexOf(datas, "CLSI18_DR");
                            CLSI18_DIIndex = Array.IndexOf(datas, "CLSI18_DI");
                            CLSI18_DSIndex = Array.IndexOf(datas, "CLSI18_DS");
                            CLSI18_MSIndex = Array.IndexOf(datas, "CLSI18_MS");
                            CLSI18_MRIndex = Array.IndexOf(datas, "CLSI18_MR");
                            CLSI17_DRIndex = Array.IndexOf(datas, "CLSI17_DR");
                            CLSI17_DIIndex = Array.IndexOf(datas, "CLSI17_DI");
                            CLSI17_DSIndex = Array.IndexOf(datas, "CLSI17_DS");
                            CLSI17_MSIndex = Array.IndexOf(datas, "CLSI17_MS");
                            CLSI17_MRIndex = Array.IndexOf(datas, "CLSI17_MR");
                            CLSI16_DRIndex = Array.IndexOf(datas, "CLSI16_DR");
                            CLSI16_DIIndex = Array.IndexOf(datas, "CLSI16_DI");
                            CLSI16_DSIndex = Array.IndexOf(datas, "CLSI16_DS");
                            CLSI16_MSIndex = Array.IndexOf(datas, "CLSI16_MS");
                            CLSI16_MRIndex = Array.IndexOf(datas, "CLSI16_MR");
                            CLSI15_DRIndex = Array.IndexOf(datas, "CLSI15_DR");
                            CLSI15_DIIndex = Array.IndexOf(datas, "CLSI15_DI");
                            CLSI15_DSIndex = Array.IndexOf(datas, "CLSI15_DS");
                            CLSI15_MSIndex = Array.IndexOf(datas, "CLSI15_MS");
                            CLSI15_MRIndex = Array.IndexOf(datas, "CLSI15_MR");
                            CLSI14_DRIndex = Array.IndexOf(datas, "CLSI14_DR");
                            CLSI14_DIIndex = Array.IndexOf(datas, "CLSI14_DI");
                            CLSI14_DSIndex = Array.IndexOf(datas, "CLSI14_DS");
                            CLSI14_MSIndex = Array.IndexOf(datas, "CLSI14_MS");
                            CLSI14_MRIndex = Array.IndexOf(datas, "CLSI14_MR");
                            CLSI13_DRIndex = Array.IndexOf(datas, "CLSI13_DR");
                            CLSI13_DIIndex = Array.IndexOf(datas, "CLSI13_DI");
                            CLSI13_DSIndex = Array.IndexOf(datas, "CLSI13_DS");
                            CLSI13_MSIndex = Array.IndexOf(datas, "CLSI13_MS");
                            CLSI13_MRIndex = Array.IndexOf(datas, "CLSI13_MR");
                            CLSI12_DRIndex = Array.IndexOf(datas, "CLSI12_DR");
                            CLSI12_DIIndex = Array.IndexOf(datas, "CLSI12_DI");
                            CLSI12_DSIndex = Array.IndexOf(datas, "CLSI12_DS");
                            CLSI12_MSIndex = Array.IndexOf(datas, "CLSI12_MS");
                            CLSI12_MRIndex = Array.IndexOf(datas, "CLSI12_MR");
                            CLSI11_DRIndex = Array.IndexOf(datas, "CLSI11_DR");
                            CLSI11_DIIndex = Array.IndexOf(datas, "CLSI11_DI");
                            CLSI11_DSIndex = Array.IndexOf(datas, "CLSI11_DS");
                            CLSI11_MSIndex = Array.IndexOf(datas, "CLSI11_MS");
                            CLSI11_MRIndex = Array.IndexOf(datas, "CLSI11_MR");
                            CLSI10_DRIndex = Array.IndexOf(datas, "CLSI10_DR");
                            CLSI10_DIIndex = Array.IndexOf(datas, "CLSI10_DI");
                            CLSI10_DSIndex = Array.IndexOf(datas, "CLSI10_DS");
                            CLSI10_MSIndex = Array.IndexOf(datas, "CLSI10_MS");
                            CLSI10_MRIndex = Array.IndexOf(datas, "CLSI10_MR");
                            EUCST19_DRIndex = Array.IndexOf(datas, "EUCST19_DR");
                            EUCST19_DIIndex = Array.IndexOf(datas, "EUCST19_DI");
                            EUCST19_DSIndex = Array.IndexOf(datas, "EUCST19_DS");
                            EUCST19_MSIndex = Array.IndexOf(datas, "EUCST19_MS");
                            EUCST19_MRIndex = Array.IndexOf(datas, "EUCST19_MR");
                            EUCST18_DRIndex = Array.IndexOf(datas, "EUCST18_DR");
                            EUCST18_DIIndex = Array.IndexOf(datas, "EUCST18_DI");
                            EUCST18_DSIndex = Array.IndexOf(datas, "EUCST18_DS");
                            EUCST18_MSIndex = Array.IndexOf(datas, "EUCST18_MS");
                            EUCST18_MRIndex = Array.IndexOf(datas, "EUCST18_MR");
                            EUCST17_DRIndex = Array.IndexOf(datas, "EUCST17_DR");
                            EUCST17_DIIndex = Array.IndexOf(datas, "EUCST17_DI");
                            EUCST17_DSIndex = Array.IndexOf(datas, "EUCST17_DS");
                            EUCST17_MSIndex = Array.IndexOf(datas, "EUCST17_MS");
                            EUCST17_MRIndex = Array.IndexOf(datas, "EUCST17_MR");
                            EUCST16_DRIndex = Array.IndexOf(datas, "EUCST16_DR");
                            EUCST16_DIIndex = Array.IndexOf(datas, "EUCST16_DI");
                            EUCST16_DSIndex = Array.IndexOf(datas, "EUCST16_DS");
                            EUCST16_MSIndex = Array.IndexOf(datas, "EUCST16_MS");
                            EUCST16_MRIndex = Array.IndexOf(datas, "EUCST16_MR");
                            EUCST15_DRIndex = Array.IndexOf(datas, "EUCST15_DR");
                            EUCST15_DIIndex = Array.IndexOf(datas, "EUCST15_DI");
                            EUCST15_DSIndex = Array.IndexOf(datas, "EUCST15_DS");
                            EUCST15_MSIndex = Array.IndexOf(datas, "EUCST15_MS");
                            EUCST15_MRIndex = Array.IndexOf(datas, "EUCST15_MR");
                            EUCST14_DRIndex = Array.IndexOf(datas, "EUCST14_DR");
                            EUCST14_DIIndex = Array.IndexOf(datas, "EUCST14_DI");
                            EUCST14_DSIndex = Array.IndexOf(datas, "EUCST14_DS");
                            EUCST14_MSIndex = Array.IndexOf(datas, "EUCST14_MS");
                            EUCST14_MRIndex = Array.IndexOf(datas, "EUCST14_MR");
                            EUCST13_DRIndex = Array.IndexOf(datas, "EUCST13_DR");
                            EUCST13_DIIndex = Array.IndexOf(datas, "EUCST13_DI");
                            EUCST13_DSIndex = Array.IndexOf(datas, "EUCST13_DS");
                            EUCST13_MSIndex = Array.IndexOf(datas, "EUCST13_MS");
                            EUCST13_MRIndex = Array.IndexOf(datas, "EUCST13_MR");
                            ECV12_DRIndex = Array.IndexOf(datas, "ECV12_DR");
                            ECV12_DIIndex = Array.IndexOf(datas, "ECV12_DI");
                            ECV12_DSIndex = Array.IndexOf(datas, "ECV12_DS");
                            ECV12_MSIndex = Array.IndexOf(datas, "ECV12_MS");
                            ECV12_MRIndex = Array.IndexOf(datas, "ECV12_MR");
                            EUCST11_DRIndex = Array.IndexOf(datas, "EUCST11_DR");
                            EUCST11_DIIndex = Array.IndexOf(datas, "EUCST11_DI");
                            EUCST11_DSIndex = Array.IndexOf(datas, "EUCST11_DS");
                            EUCST11_MSIndex = Array.IndexOf(datas, "EUCST11_MS");
                            EUCST11_MRIndex = Array.IndexOf(datas, "EUCST11_MR");
                            NEODK16_DRIndex = Array.IndexOf(datas, "NEODK16_DR");
                            NEODK16_DIIndex = Array.IndexOf(datas, "NEODK16_DI");
                            NEODK16_DSIndex = Array.IndexOf(datas, "NEODK16_DS");
                            NEODK16_MSIndex = Array.IndexOf(datas, "NEODK16_MS");
                            NEODK16_MRIndex = Array.IndexOf(datas, "NEODK16_MR");
                            NEODK13_DRIndex = Array.IndexOf(datas, "NEODK13_DR");
                            NEODK13_DIIndex = Array.IndexOf(datas, "NEODK13_DI");
                            NEODK13_DSIndex = Array.IndexOf(datas, "NEODK13_DS");
                            NEODK13_MSIndex = Array.IndexOf(datas, "NEODK13_MS");
                            NEODK13_MRIndex = Array.IndexOf(datas, "NEODK13_MR");
                            NEODK98_DRIndex = Array.IndexOf(datas, "NEODK98_DR");
                            NEODK98_DIIndex = Array.IndexOf(datas, "NEODK98_DI");
                            NEODK98_DSIndex = Array.IndexOf(datas, "NEODK98_DS");
                            NEODK98_MSIndex = Array.IndexOf(datas, "NEODK98_MS");
                            NEODK98_MRIndex = Array.IndexOf(datas, "NEODK98_MR");
                            SFM07_DRIndex = Array.IndexOf(datas, "SFM07_DR");
                            SFM07_DIIndex = Array.IndexOf(datas, "SFM07_DI");
                            SFM07_DSIndex = Array.IndexOf(datas, "SFM07_DS");
                            SFM07_MSIndex = Array.IndexOf(datas, "SFM07_MS");
                            SFM07_MRIndex = Array.IndexOf(datas, "SFM07_MR");
                            DIN04_DRIndex = Array.IndexOf(datas, "DIN04_DR");
                            DIN04_DIIndex = Array.IndexOf(datas, "DIN04_DI");
                            DIN04_DSIndex = Array.IndexOf(datas, "DIN04_DS");
                            DIN04_MSIndex = Array.IndexOf(datas, "DIN04_MS");
                            DIN04_MRIndex = Array.IndexOf(datas, "DIN04_MR");
                            CRG96_DRIndex = Array.IndexOf(datas, "CRG96_DR");
                            CRG96_DIIndex = Array.IndexOf(datas, "CRG96_DI");
                            CRG96_DSIndex = Array.IndexOf(datas, "CRG96_DS");
                            CRG96_MSIndex = Array.IndexOf(datas, "CRG96_MS");
                            CRG96_MRIndex = Array.IndexOf(datas, "CRG96_MR");
                            AFA00_DRIndex = Array.IndexOf(datas, "AFA00_DR");
                            AFA00_DIIndex = Array.IndexOf(datas, "AFA00_DI");
                            AFA00_DSIndex = Array.IndexOf(datas, "AFA00_DS");
                            AFA00_MSIndex = Array.IndexOf(datas, "AFA00_MS");
                            AFA00_MRIndex = Array.IndexOf(datas, "AFA00_MR");
                            MENS00_DRIndex = Array.IndexOf(datas, "MENS00_DR");
                            MENS00_DIIndex = Array.IndexOf(datas, "MENS00_DI");
                            MENS00_DSIndex = Array.IndexOf(datas, "MENS00_DS");
                            MENS00_MSIndex = Array.IndexOf(datas, "MENS00_MS");
                            MENS00_MRIndex = Array.IndexOf(datas, "MENS00_MR");
                            SRGA98_DRIndex = Array.IndexOf(datas, "SRGA98_DR");
                            SRGA98_DIIndex = Array.IndexOf(datas, "SRGA98_DI");
                            SRGA98_DSIndex = Array.IndexOf(datas, "SRGA98_DS");
                            SRGA98_MSIndex = Array.IndexOf(datas, "SRGA98_MS");
                            SRGA98_MRIndex = Array.IndexOf(datas, "SRGA98_MR");
                            BSAC00_DRIndex = Array.IndexOf(datas, "BSAC00_DR");
                            BSAC00_DIIndex = Array.IndexOf(datas, "BSAC00_DI");
                            BSAC00_DSIndex = Array.IndexOf(datas, "BSAC00_DS");
                            BSAC00_MSIndex = Array.IndexOf(datas, "BSAC00_MS");
                            BSAC00_MRIndex = Array.IndexOf(datas, "BSAC00_MR");
                            OTHER_DRIndex = Array.IndexOf(datas, "OTHER_DR");
                            OTHER_DIIndex = Array.IndexOf(datas, "OTHER_DI");
                            OTHER_DSIndex = Array.IndexOf(datas, "OTHER_DS");
                            OTHER_MSIndex = Array.IndexOf(datas, "OTHER_MS");
                            OTHER_MRIndex = Array.IndexOf(datas, "OTHER_MR");
                            continue;
                        }

                        //Data
                        //if (numericColumnIndex == -1 || codeColumnIndex == -1 || nameColumnIndex == -1)
                        //    continue;
                        if (datas.Length > 1)
                        {
                            tCAntibiotic.Add(new TCAntibioticDTO
                            {
                                ant_mst_code = mst_code,
                                ant_code = datas[ABX_NUMBERIndex].Trim() + "_" + datas[WHON5_CODEIndex].Trim(),
                                ant_name = (ANTIBIOTICIndex != -1) ? datas[ANTIBIOTICIndex].Trim() : null,
                                WHON5_CODE = (WHON5_CODEIndex != -1) ? datas[WHON5_CODEIndex].Trim() : null,
                                WHON4_CODE = (WHON4_CODEIndex != -1) ? datas[WHON4_CODEIndex].Trim() : null,
                                WHO_CODE = (WHO_CODEIndex != -1) ? datas[WHO_CODEIndex].Trim() : null,
                                DIN_CODE = (DIN_CODEIndex != -1) ? datas[DIN_CODEIndex].Trim() : null,
                                JAC_CODE = (JAC_CODEIndex != -1) ? datas[JAC_CODEIndex].Trim() : null,
                                USER_CODE = (USER_CODEIndex != -1) ? datas[USER_CODEIndex].Trim() : null,
                                ANTIBIOTIC = (ANTIBIOTICIndex != -1) ? datas[ANTIBIOTICIndex].Trim() : null,
                                GUIDELINES = (GUIDELINESIndex != -1) ? datas[GUIDELINESIndex].Trim() : null,
                                CLSI = (CLSIIndex != -1) ? datas[CLSIIndex].Trim() : null,
                                EUCAST = (EUCASTIndex != -1) ? datas[EUCASTIndex].Trim() : null,
                                SFM = (SFMIndex != -1) ? datas[SFMIndex].Trim() : null,
                                SRGA = (SRGAIndex != -1) ? datas[SRGAIndex].Trim() : null,
                                BSAC = (BSACIndex != -1) ? datas[BSACIndex].Trim() : null,
                                DIN = (DINIndex != -1) ? datas[DINIndex].Trim() : null,
                                NEO = (NEOIndex != -1) ? datas[NEOIndex].Trim() : null,
                                AFA = (AFAIndex != -1) ? datas[AFAIndex].Trim() : null,
                                ABX_NUMBER = (ABX_NUMBERIndex != -1) ? datas[ABX_NUMBERIndex].Trim() : null,
                                POTENCY = (POTENCYIndex != -1) ? datas[POTENCYIndex].Trim() : null,
                                BETALACTAM = (BETALACTAMIndex != -1) ? datas[BETALACTAMIndex].Trim() : null,
                                CLASS = (CLASSIndex != -1) ? datas[CLASSIndex].Trim() : null,
                                SUBCLASS = (SUBCLASSIndex != -1) ? datas[SUBCLASSIndex].Trim() : null,
                                PROF_CLASS = (PROF_CLASSIndex != -1) ? datas[PROF_CLASSIndex].Trim() : null,
                                CLSI_ORDER = (CLSI_ORDERIndex != -1) ? datas[CLSI_ORDERIndex].Trim() : null,
                                HUMAN = (HUMANIndex != -1) ? datas[HUMANIndex].Trim() : null,
                                VETERINARY = (VETERINARYIndex != -1) ? datas[VETERINARYIndex].Trim() : null,
                                ANIMAL_GP = (ANIMAL_GPIndex != -1) ? datas[ANIMAL_GPIndex].Trim() : null,
                                WHO_IMPORT = (WHO_IMPORTIndex != -1) ? datas[WHO_IMPORTIndex].Trim() : null,
                                LOINCCOMP = (LOINCCOMPIndex != -1) ? datas[LOINCCOMPIndex].Trim() : null,
                                LOINCGEN = (LOINCGENIndex != -1) ? datas[LOINCGENIndex].Trim() : null,
                                LOINCDISK = (LOINCDISKIndex != -1) ? datas[LOINCDISKIndex].Trim() : null,
                                LOINCMIC = (LOINCMICIndex != -1) ? datas[LOINCMICIndex].Trim() : null,
                                LOINCETEST = (LOINCETESTIndex != -1) ? datas[LOINCETESTIndex].Trim() : null,
                                LOINCSLOW = (LOINCSLOWIndex != -1) ? datas[LOINCSLOWIndex].Trim() : null,
                                LOINCAFB = (LOINCAFBIndex != -1) ? datas[LOINCAFBIndex].Trim() : null,
                                LOINCSBT = (LOINCSBTIndex != -1) ? datas[LOINCSBTIndex].Trim() : null,
                                LOINCMLC = (LOINCMLCIndex != -1) ? datas[LOINCMLCIndex].Trim() : null,
                                CLSI19_DR = (CLSI19_DRIndex != -1) ? datas[CLSI19_DRIndex].Trim() : null,
                                CLSI19_DI = (CLSI19_DIIndex != -1) ? datas[CLSI19_DIIndex].Trim() : null,
                                CLSI19_DS = (CLSI19_DSIndex != -1) ? datas[CLSI19_DSIndex].Trim() : null,
                                CLSI19_MS = (CLSI19_MSIndex != -1) ? datas[CLSI19_MSIndex].Trim() : null,
                                CLSI19_MR = (CLSI19_MRIndex != -1) ? datas[CLSI19_MRIndex].Trim() : null,
                                CLSI18_DR = (CLSI18_DRIndex != -1) ? datas[CLSI18_DRIndex].Trim() : null,
                                CLSI18_DI = (CLSI18_DIIndex != -1) ? datas[CLSI18_DIIndex].Trim() : null,
                                CLSI18_DS = (CLSI18_DSIndex != -1) ? datas[CLSI18_DSIndex].Trim() : null,
                                CLSI18_MS = (CLSI18_MSIndex != -1) ? datas[CLSI18_MSIndex].Trim() : null,
                                CLSI18_MR = (CLSI18_MRIndex != -1) ? datas[CLSI18_MRIndex].Trim() : null,
                                CLSI17_DR = (CLSI17_DRIndex != -1) ? datas[CLSI17_DRIndex].Trim() : null,
                                CLSI17_DI = (CLSI17_DIIndex != -1) ? datas[CLSI17_DIIndex].Trim() : null,
                                CLSI17_DS = (CLSI17_DSIndex != -1) ? datas[CLSI17_DSIndex].Trim() : null,
                                CLSI17_MS = (CLSI17_MSIndex != -1) ? datas[CLSI17_MSIndex].Trim() : null,
                                CLSI17_MR = (CLSI17_MRIndex != -1) ? datas[CLSI17_MRIndex].Trim() : null,
                                CLSI16_DR = (CLSI16_DRIndex != -1) ? datas[CLSI16_DRIndex].Trim() : null,
                                CLSI16_DI = (CLSI16_DIIndex != -1) ? datas[CLSI16_DIIndex].Trim() : null,
                                CLSI16_DS = (CLSI16_DSIndex != -1) ? datas[CLSI16_DSIndex].Trim() : null,
                                CLSI16_MS = (CLSI16_MSIndex != -1) ? datas[CLSI16_MSIndex].Trim() : null,
                                CLSI16_MR = (CLSI16_MRIndex != -1) ? datas[CLSI16_MRIndex].Trim() : null,
                                CLSI15_DR = (CLSI15_DRIndex != -1) ? datas[CLSI15_DRIndex].Trim() : null,
                                CLSI15_DI = (CLSI15_DIIndex != -1) ? datas[CLSI15_DIIndex].Trim() : null,
                                CLSI15_DS = (CLSI15_DSIndex != -1) ? datas[CLSI15_DSIndex].Trim() : null,
                                CLSI15_MS = (CLSI15_MSIndex != -1) ? datas[CLSI15_MSIndex].Trim() : null,
                                CLSI15_MR = (CLSI15_MRIndex != -1) ? datas[CLSI15_MRIndex].Trim() : null,
                                CLSI14_DR = (CLSI14_DRIndex != -1) ? datas[CLSI14_DRIndex].Trim() : null,
                                CLSI14_DI = (CLSI14_DIIndex != -1) ? datas[CLSI14_DIIndex].Trim() : null,
                                CLSI14_DS = (CLSI14_DSIndex != -1) ? datas[CLSI14_DSIndex].Trim() : null,
                                CLSI14_MS = (CLSI14_MSIndex != -1) ? datas[CLSI14_MSIndex].Trim() : null,
                                CLSI14_MR = (CLSI14_MRIndex != -1) ? datas[CLSI14_MRIndex].Trim() : null,
                                CLSI13_DR = (CLSI13_DRIndex != -1) ? datas[CLSI13_DRIndex].Trim() : null,
                                CLSI13_DI = (CLSI13_DIIndex != -1) ? datas[CLSI13_DIIndex].Trim() : null,
                                CLSI13_DS = (CLSI13_DSIndex != -1) ? datas[CLSI13_DSIndex].Trim() : null,
                                CLSI13_MS = (CLSI13_MSIndex != -1) ? datas[CLSI13_MSIndex].Trim() : null,
                                CLSI13_MR = (CLSI13_MRIndex != -1) ? datas[CLSI13_MRIndex].Trim() : null,
                                CLSI12_DR = (CLSI12_DRIndex != -1) ? datas[CLSI12_DRIndex].Trim() : null,
                                CLSI12_DI = (CLSI12_DIIndex != -1) ? datas[CLSI12_DIIndex].Trim() : null,
                                CLSI12_DS = (CLSI12_DSIndex != -1) ? datas[CLSI12_DSIndex].Trim() : null,
                                CLSI12_MS = (CLSI12_MSIndex != -1) ? datas[CLSI12_MSIndex].Trim() : null,
                                CLSI12_MR = (CLSI12_MRIndex != -1) ? datas[CLSI12_MRIndex].Trim() : null,
                                CLSI11_DR = (CLSI11_DRIndex != -1) ? datas[CLSI11_DRIndex].Trim() : null,
                                CLSI11_DI = (CLSI11_DIIndex != -1) ? datas[CLSI11_DIIndex].Trim() : null,
                                CLSI11_DS = (CLSI11_DSIndex != -1) ? datas[CLSI11_DSIndex].Trim() : null,
                                CLSI11_MS = (CLSI11_MSIndex != -1) ? datas[CLSI11_MSIndex].Trim() : null,
                                CLSI11_MR = (CLSI11_MRIndex != -1) ? datas[CLSI11_MRIndex].Trim() : null,
                                CLSI10_DR = (CLSI10_DRIndex != -1) ? datas[CLSI10_DRIndex].Trim() : null,
                                CLSI10_DI = (CLSI10_DIIndex != -1) ? datas[CLSI10_DIIndex].Trim() : null,
                                CLSI10_DS = (CLSI10_DSIndex != -1) ? datas[CLSI10_DSIndex].Trim() : null,
                                CLSI10_MS = (CLSI10_MSIndex != -1) ? datas[CLSI10_MSIndex].Trim() : null,
                                CLSI10_MR = (CLSI10_MRIndex != -1) ? datas[CLSI10_MRIndex].Trim() : null,
                                EUCST19_DR = (EUCST19_DRIndex != -1) ? datas[EUCST19_DRIndex].Trim() : null,
                                EUCST19_DI = (EUCST19_DIIndex != -1) ? datas[EUCST19_DIIndex].Trim() : null,
                                EUCST19_DS = (EUCST19_DSIndex != -1) ? datas[EUCST19_DSIndex].Trim() : null,
                                EUCST19_MS = (EUCST19_MSIndex != -1) ? datas[EUCST19_MSIndex].Trim() : null,
                                EUCST19_MR = (EUCST19_MRIndex != -1) ? datas[EUCST19_MRIndex].Trim() : null,
                                EUCST18_DR = (EUCST18_DRIndex != -1) ? datas[EUCST18_DRIndex].Trim() : null,
                                EUCST18_DI = (EUCST18_DIIndex != -1) ? datas[EUCST18_DIIndex].Trim() : null,
                                EUCST18_DS = (EUCST18_DSIndex != -1) ? datas[EUCST18_DSIndex].Trim() : null,
                                EUCST18_MS = (EUCST18_MSIndex != -1) ? datas[EUCST18_MSIndex].Trim() : null,
                                EUCST18_MR = (EUCST18_MRIndex != -1) ? datas[EUCST18_MRIndex].Trim() : null,
                                EUCST17_DR = (EUCST17_DRIndex != -1) ? datas[EUCST17_DRIndex].Trim() : null,
                                EUCST17_DI = (EUCST17_DIIndex != -1) ? datas[EUCST17_DIIndex].Trim() : null,
                                EUCST17_DS = (EUCST17_DSIndex != -1) ? datas[EUCST17_DSIndex].Trim() : null,
                                EUCST17_MS = (EUCST17_MSIndex != -1) ? datas[EUCST17_MSIndex].Trim() : null,
                                EUCST17_MR = (EUCST17_MRIndex != -1) ? datas[EUCST17_MRIndex].Trim() : null,
                                EUCST16_DR = (EUCST16_DRIndex != -1) ? datas[EUCST16_DRIndex].Trim() : null,
                                EUCST16_DI = (EUCST16_DIIndex != -1) ? datas[EUCST16_DIIndex].Trim() : null,
                                EUCST16_DS = (EUCST16_DSIndex != -1) ? datas[EUCST16_DSIndex].Trim() : null,
                                EUCST16_MS = (EUCST16_MSIndex != -1) ? datas[EUCST16_MSIndex].Trim() : null,
                                EUCST16_MR = (EUCST16_MRIndex != -1) ? datas[EUCST16_MRIndex].Trim() : null,
                                EUCST15_DR = (EUCST15_DRIndex != -1) ? datas[EUCST15_DRIndex].Trim() : null,
                                EUCST15_DI = (EUCST15_DIIndex != -1) ? datas[EUCST15_DIIndex].Trim() : null,
                                EUCST15_DS = (EUCST15_DSIndex != -1) ? datas[EUCST15_DSIndex].Trim() : null,
                                EUCST15_MS = (EUCST15_MSIndex != -1) ? datas[EUCST15_MSIndex].Trim() : null,
                                EUCST15_MR = (EUCST15_MRIndex != -1) ? datas[EUCST15_MRIndex].Trim() : null,
                                EUCST14_DR = (EUCST14_DRIndex != -1) ? datas[EUCST14_DRIndex].Trim() : null,
                                EUCST14_DI = (EUCST14_DIIndex != -1) ? datas[EUCST14_DIIndex].Trim() : null,
                                EUCST14_DS = (EUCST14_DSIndex != -1) ? datas[EUCST14_DSIndex].Trim() : null,
                                EUCST14_MS = (EUCST14_MSIndex != -1) ? datas[EUCST14_MSIndex].Trim() : null,
                                EUCST14_MR = (EUCST14_MRIndex != -1) ? datas[EUCST14_MRIndex].Trim() : null,
                                EUCST13_DR = (EUCST13_DRIndex != -1) ? datas[EUCST13_DRIndex].Trim() : null,
                                EUCST13_DI = (EUCST13_DIIndex != -1) ? datas[EUCST13_DIIndex].Trim() : null,
                                EUCST13_DS = (EUCST13_DSIndex != -1) ? datas[EUCST13_DSIndex].Trim() : null,
                                EUCST13_MS = (EUCST13_MSIndex != -1) ? datas[EUCST13_MSIndex].Trim() : null,
                                EUCST13_MR = (EUCST13_MRIndex != -1) ? datas[EUCST13_MRIndex].Trim() : null,
                                ECV12_DR = (ECV12_DRIndex != -1) ? datas[ECV12_DRIndex].Trim() : null,
                                ECV12_DI = (ECV12_DIIndex != -1) ? datas[ECV12_DIIndex].Trim() : null,
                                ECV12_DS = (ECV12_DSIndex != -1) ? datas[ECV12_DSIndex].Trim() : null,
                                ECV12_MS = (ECV12_MSIndex != -1) ? datas[ECV12_MSIndex].Trim() : null,
                                ECV12_MR = (ECV12_MRIndex != -1) ? datas[ECV12_MRIndex].Trim() : null,
                                EUCST11_DR = (EUCST11_DRIndex != -1) ? datas[EUCST11_DRIndex].Trim() : null,
                                EUCST11_DI = (EUCST11_DIIndex != -1) ? datas[EUCST11_DIIndex].Trim() : null,
                                EUCST11_DS = (EUCST11_DSIndex != -1) ? datas[EUCST11_DSIndex].Trim() : null,
                                EUCST11_MS = (EUCST11_MSIndex != -1) ? datas[EUCST11_MSIndex].Trim() : null,
                                EUCST11_MR = (EUCST11_MRIndex != -1) ? datas[EUCST11_MRIndex].Trim() : null,
                                NEODK16_DR = (NEODK16_DRIndex != -1) ? datas[NEODK16_DRIndex].Trim() : null,
                                NEODK16_DI = (NEODK16_DIIndex != -1) ? datas[NEODK16_DIIndex].Trim() : null,
                                NEODK16_DS = (NEODK16_DSIndex != -1) ? datas[NEODK16_DSIndex].Trim() : null,
                                NEODK16_MS = (NEODK16_MSIndex != -1) ? datas[NEODK16_MSIndex].Trim() : null,
                                NEODK16_MR = (NEODK16_MRIndex != -1) ? datas[NEODK16_MRIndex].Trim() : null,
                                NEODK13_DR = (NEODK13_DRIndex != -1) ? datas[NEODK13_DRIndex].Trim() : null,
                                NEODK13_DI = (NEODK13_DIIndex != -1) ? datas[NEODK13_DIIndex].Trim() : null,
                                NEODK13_DS = (NEODK13_DSIndex != -1) ? datas[NEODK13_DSIndex].Trim() : null,
                                NEODK13_MS = (NEODK13_MSIndex != -1) ? datas[NEODK13_MSIndex].Trim() : null,
                                NEODK13_MR = (NEODK13_MRIndex != -1) ? datas[NEODK13_MRIndex].Trim() : null,
                                NEODK98_DR = (NEODK98_DRIndex != -1) ? datas[NEODK98_DRIndex].Trim() : null,
                                NEODK98_DI = (NEODK98_DIIndex != -1) ? datas[NEODK98_DIIndex].Trim() : null,
                                NEODK98_DS = (NEODK98_DSIndex != -1) ? datas[NEODK98_DSIndex].Trim() : null,
                                NEODK98_MS = (NEODK98_MSIndex != -1) ? datas[NEODK98_MSIndex].Trim() : null,
                                NEODK98_MR = (NEODK98_MRIndex != -1) ? datas[NEODK98_MRIndex].Trim() : null,
                                SFM07_DR = (SFM07_DRIndex != -1) ? datas[SFM07_DRIndex].Trim() : null,
                                SFM07_DI = (SFM07_DIIndex != -1) ? datas[SFM07_DIIndex].Trim() : null,
                                SFM07_DS = (SFM07_DSIndex != -1) ? datas[SFM07_DSIndex].Trim() : null,
                                SFM07_MS = (SFM07_MSIndex != -1) ? datas[SFM07_MSIndex].Trim() : null,
                                SFM07_MR = (SFM07_MRIndex != -1) ? datas[SFM07_MRIndex].Trim() : null,
                                DIN04_DR = (DIN04_DRIndex != -1) ? datas[DIN04_DRIndex].Trim() : null,
                                DIN04_DI = (DIN04_DIIndex != -1) ? datas[DIN04_DIIndex].Trim() : null,
                                DIN04_DS = (DIN04_DSIndex != -1) ? datas[DIN04_DSIndex].Trim() : null,
                                DIN04_MS = (DIN04_MSIndex != -1) ? datas[DIN04_MSIndex].Trim() : null,
                                DIN04_MR = (DIN04_MRIndex != -1) ? datas[DIN04_MRIndex].Trim() : null,
                                CRG96_DR = (CRG96_DRIndex != -1) ? datas[CRG96_DRIndex].Trim() : null,
                                CRG96_DI = (CRG96_DIIndex != -1) ? datas[CRG96_DIIndex].Trim() : null,
                                CRG96_DS = (CRG96_DSIndex != -1) ? datas[CRG96_DSIndex].Trim() : null,
                                CRG96_MS = (CRG96_MSIndex != -1) ? datas[CRG96_MSIndex].Trim() : null,
                                CRG96_MR = (CRG96_MRIndex != -1) ? datas[CRG96_MRIndex].Trim() : null,
                                AFA00_DR = (AFA00_DRIndex != -1) ? datas[AFA00_DRIndex].Trim() : null,
                                AFA00_DI = (AFA00_DIIndex != -1) ? datas[AFA00_DIIndex].Trim() : null,
                                AFA00_DS = (AFA00_DSIndex != -1) ? datas[AFA00_DSIndex].Trim() : null,
                                AFA00_MS = (AFA00_MSIndex != -1) ? datas[AFA00_MSIndex].Trim() : null,
                                AFA00_MR = (AFA00_MRIndex != -1) ? datas[AFA00_MRIndex].Trim() : null,
                                MENS00_DR = (MENS00_DRIndex != -1) ? datas[MENS00_DRIndex].Trim() : null,
                                MENS00_DI = (MENS00_DIIndex != -1) ? datas[MENS00_DIIndex].Trim() : null,
                                MENS00_DS = (MENS00_DSIndex != -1) ? datas[MENS00_DSIndex].Trim() : null,
                                MENS00_MS = (MENS00_MSIndex != -1) ? datas[MENS00_MSIndex].Trim() : null,
                                MENS00_MR = (MENS00_MRIndex != -1) ? datas[MENS00_MRIndex].Trim() : null,
                                SRGA98_DR = (SRGA98_DRIndex != -1) ? datas[SRGA98_DRIndex].Trim() : null,
                                SRGA98_DI = (SRGA98_DIIndex != -1) ? datas[SRGA98_DIIndex].Trim() : null,
                                SRGA98_DS = (SRGA98_DSIndex != -1) ? datas[SRGA98_DSIndex].Trim() : null,
                                SRGA98_MS = (SRGA98_MSIndex != -1) ? datas[SRGA98_MSIndex].Trim() : null,
                                SRGA98_MR = (SRGA98_MRIndex != -1) ? datas[SRGA98_MRIndex].Trim() : null,
                                BSAC00_DR = (BSAC00_DRIndex != -1) ? datas[BSAC00_DRIndex].Trim() : null,
                                BSAC00_DI = (BSAC00_DIIndex != -1) ? datas[BSAC00_DIIndex].Trim() : null,
                                BSAC00_DS = (BSAC00_DSIndex != -1) ? datas[BSAC00_DSIndex].Trim() : null,
                                BSAC00_MS = (BSAC00_MSIndex != -1) ? datas[BSAC00_MSIndex].Trim() : null,
                                BSAC00_MR = (BSAC00_MRIndex != -1) ? datas[BSAC00_MRIndex].Trim() : null,
                                OTHER_DR = (OTHER_DRIndex != -1) ? datas[OTHER_DRIndex].Trim() : null,
                                OTHER_DI = (OTHER_DIIndex != -1) ? datas[OTHER_DIIndex].Trim() : null,
                                OTHER_DS = (OTHER_DSIndex != -1) ? datas[OTHER_DSIndex].Trim() : null,
                                OTHER_MS = (OTHER_MSIndex != -1) ? datas[OTHER_MSIndex].Trim() : null,
                                OTHER_MR = (OTHER_MRIndex != -1) ? datas[OTHER_MRIndex].Trim() : null,
                                ant_status = "A",
                                ant_active = false,
                                ant_createdate = createDate,
                                ant_createuser = createUser
                            });
                        }
                    }
                }

                #endregion

                if (tCAntibiotic.Count > 0)
                {
                    #region Save Data
                    if (bReplace)
                    {
                        var delSpecimen = await DeleteAntibioticListDataAsyn(tCAntibiotic);
                    }
                    var resultSpecimen = await SaveAntibioticListDataAsync(tCAntibiotic);
                    #endregion
                }
                else
                {
                    ErrorMessage.Add(new TCProcessExcelErrorDTO
                    {
                        tcp_status = 'E',
                        tcp_Err_type = 'E',
                        tcp_Err_No = "",
                        tcp_Err_SheetName = "Total",
                        tcp_Err_Message = "Not found."
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Add(new TCProcessExcelErrorDTO
                {
                    tcp_status = 'E',
                    tcp_Err_type = 'E',
                    tcp_Err_No = "",
                    tcp_Err_SheetName = "Total",
                    tcp_Err_Message = ex.Message
                });
            }
            return ErrorMessage;
        }

        public async Task<List<TCProcessExcelErrorDTO>> ValidateAndImportSpecimenFile(string path, MasterTemplateDTO masterTemplate, string mst_code, string createUser, bool bReplace)
        {
            DateTime createDate = DateTime.Now;
            var ErrorMessage = new List<TCProcessExcelErrorDTO>();
            string tableMapping = string.Empty;

            List<TCWHONET_SpecimenDTO> tCWHONET_Specimen = new List<TCWHONET_SpecimenDTO>();

            try
            {
                //path = Path.Combine(path);
                #region Read File Data            
                var dataRows = File.ReadAllLines(path);
                int numericColumnIndex = 0, codeColumnIndex = 0, nameColumnIndex = 0;
                foreach (var (line, index) in dataRows.Select((v, i) => (v, i)))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var datas = line.Split("\t");
                        //Header
                        if (index == 0)
                        {
                            numericColumnIndex = Array.IndexOf(datas, "NUMERIC");
                            codeColumnIndex = Array.IndexOf(datas, "C_ENGLISH");
                            nameColumnIndex = Array.IndexOf(datas, "ENGLISH");
                            continue;
                        }

                        //Data
                        if (numericColumnIndex == -1 || codeColumnIndex == -1 || nameColumnIndex == -1)
                            continue;
                        if (datas.Length > 1)
                        {
                            tCWHONET_Specimen.Add(new TCWHONET_SpecimenDTO
                            {
                                who_spc_mst_code = mst_code,
                                who_spc_numeric = datas[numericColumnIndex],
                                who_spc_code = datas[codeColumnIndex],
                                who_spc_name = datas[nameColumnIndex],
                                who_spc_active = true,
                                who_spc_createdate = createDate,
                                who_spc_createuser = createUser
                            });
                        }
                    }
                }

                #endregion

                if (tCWHONET_Specimen.Count > 0)
                {
                    #region Save Data
                    if (bReplace)
                    {
                        var delSpecimen = await DeleteSpecimenListDataAsyn(tCWHONET_Specimen);
                    }
                    var resultSpecimen = await SaveSpecimenListDataAsync(tCWHONET_Specimen);
                    #endregion
                }
                else
                {
                    ErrorMessage.Add(new TCProcessExcelErrorDTO
                    {
                        tcp_status = 'E',
                        tcp_Err_type = 'E',
                        tcp_Err_No = "",
                        tcp_Err_SheetName = "Total",
                        tcp_Err_Message = "Not found."
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Add(new TCProcessExcelErrorDTO
                {
                    tcp_status = 'E',
                    tcp_Err_type = 'E',
                    tcp_Err_No = "",
                    tcp_Err_SheetName = "Total",
                    tcp_Err_Message = ex.Message
                });
            }
            return ErrorMessage;
        }

        public async Task<List<TCProcessExcelErrorDTO>> ValidateAndImportOrganismFile(string path, MasterTemplateDTO masterTemplate, string mst_code, string createUser, bool bReplace)
        {
            DateTime createDate = DateTime.Now;
            var ErrorMessage = new List<TCProcessExcelErrorDTO>();
            string tableMapping = string.Empty;

            List<TCWHONET_OrganismDTO> tCWHONET_Organism = new List<TCWHONET_OrganismDTO>();

            try
            {
                //path = Path.Combine(path);
                #region Read File Data              
                var dataRows = File.ReadAllLines(path);
                foreach (var (line, index) in dataRows.Select((v, i) => (v, i)))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var datas = line.Split("\t");
                        //Header
                        if (index == 0)
                            continue;

                        //Data
                        if (datas.Length > 1)
                        {
                            tCWHONET_Organism.Add(new TCWHONET_OrganismDTO
                            {
                                who_org_mst_code = mst_code,
                                who_org_code = datas[0],
                                who_org = datas[1],
                                who_gram = datas[2],
                                who_organism = datas[3],
                                who_org_clean = datas[4],
                                who_org_status = datas[5],
                                who_common = datas[6],
                                who_org_group = datas[7],
                                who_sub_group = datas[8],
                                who_genus = datas[9],
                                who_genus_code = datas[10],
                                who_sct_code = datas[11],
                                who_sct_text = datas[12],
                                who_org_active = true,
                                who_org_createdate = createDate,
                                who_org_createuser = createUser
                            });
                        }
                    }
                }

                #endregion
                if (tCWHONET_Organism.Count > 0)
                {
                    #region Save Data
                    if (bReplace)
                    {
                        var delOrganism = await DeleteOrganismListDataAsyn(tCWHONET_Organism);
                    }
                    var resultOrganism = await SaveOrganismListDataAsync(tCWHONET_Organism);
                    #endregion
                }
                else {
                    ErrorMessage.Add(new TCProcessExcelErrorDTO
                    {
                        tcp_status = 'E',
                        tcp_Err_type = 'E',
                        tcp_Err_No = "",
                        tcp_Err_SheetName = "Total",
                        tcp_Err_Message = "Not found."
                    });
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Add(new TCProcessExcelErrorDTO
                {
                    tcp_status = 'E',
                    tcp_Err_type = 'E',
                    tcp_Err_No = "",
                    tcp_Err_SheetName = "Total",
                    tcp_Err_Message = ex.Message
                });
            }
            return ErrorMessage;
        }

        public async Task<List<TCProcessExcelErrorDTO>> ValidateAndImportMacroFile(string path, MasterTemplateDTO masterTemplate, string mst_code, string createUser, bool bReplace)
        {
            DateTime createDate = DateTime.Now;
            var ErrorMessage = new List<TCProcessExcelErrorDTO>();
            string tableMapping = string.Empty;

            List<TCWHONET_MacroDTO> tCWHONET_Macro = new List<TCWHONET_MacroDTO>();

            try
            {
                //path = Path.Combine(path);
                #region Read File Data
               
                using (ZipArchive archive = ZipFile.OpenRead(path))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        tCWHONET_Macro.Add(new TCWHONET_MacroDTO
                        {
                            who_mac_mst_code = mst_code,
                            who_mac_name = entry.Name.Split(".")[0],
                            who_mac_active = true,
                            who_mac_createdate = createDate,
                            who_mac_createuser = createUser
                        });
                    }
                }
                #endregion

                #region Save Data
                if (bReplace)
                {
                    var delMacro = await DeleteMacroListDataAsyn(tCWHONET_Macro);
                }
                var resultMacro = await SaveMacroListDataAsync(tCWHONET_Macro);
                #endregion
            }
            catch (Exception ex)
            {
                ErrorMessage.Add(new TCProcessExcelErrorDTO
                {
                    tcp_status = 'E',
                    tcp_Err_type = 'E',
                    tcp_Err_No = "",
                    tcp_Err_SheetName = "Total",
                    tcp_Err_Message = ex.Message
                });
            }
            return ErrorMessage;
        }
    }
}
