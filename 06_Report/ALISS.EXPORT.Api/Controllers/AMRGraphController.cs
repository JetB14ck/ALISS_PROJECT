//using ALISS.EXPORT.Api.DTO;
//using ALISS.EXPORT.Api.Models;
using ALISS.EXPORT.Library.Model;
using ALISS.EXPORT.Library.DTO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace ALISS.EXPORT.Api.Controllers
{
    public class AMRGraphController : ApiController
    {
        ALISSEntities db = new ALISSEntities(true);

        [HttpGet]
        [Route("api/AMRGraph/Get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("api/AMRGraph/GetDB")]
        public IEnumerable<string> GetDB()
        {
            var organismMaster = new List<sp_GET_TCOrganism_Active_Result>();
            organismMaster = db.sp_GET_TCOrganism_Active(null).ToList();
            return new string[] { "Connection DB", "OK" };
        }

        [HttpGet]
        [Route("api/AMRGraph/GetFile")]
        public IHttpActionResult GetFile()
        {
            var log_start = new LogWriter("Start AMRGraph/GetFile ...");
            var modelYearFrom = 2015; 
            var modelYearTo = 2020; 
            var modelSelectedOrg = new List<string> { "Escherichia coli" }; 
            var modelSelectedAnti = new List<string> { "GEN", "IPM" };
            var modelStrSelectedOrg = string.Join(",", modelSelectedOrg);
            var modelStrSelectedAnti = string.Join(",", modelSelectedAnti);
            var modelSelectedSIR = "S"; 
            var modelGraphFormat = 1; //  1 = Line , 2 = Bar ;
            var modelSubGraph = 1;  // 0 = none , 1 = Specimen , 2 = Ward//model.sub_graph; 
            var strReportTitle = "";
            var modelSelectedWardList = new List<string>();//model.wardlist;
            var modelSelectedSpecimenList = new List<string> { "bl","ur" };//model.specimenlist;

            // Get Specimen Master
            var SpecimenMasters = db.sp_GET_TCSpecimen_Active(null).ToList();
            var SpecimenMaster = SpecimenMasters.Select(w => w.spc_code).Distinct().ToList();
            // Get Ward Type Master
            var WardTypeMasters = db.sp_GET_TCWardType_Active(null);
            var WardTypeMaster = WardTypeMasters.Select(w => w.wrd_name).Distinct().ToList();

            if (modelSelectedSIR == "S") { strReportTitle = "Susceptibility of "; }
            else if (modelSelectedSIR == "I") { strReportTitle = "Intermediate of "; }
            else { strReportTitle = "Resistance of "; }

            //var queryRaw = new List<sp_GET_RPAntibicromialResistance_Result>();
            //var query = new List<sp_GET_RPAntibicromialResistance_Result>();
            var queryAll = new List<sp_GET_RPAntibicromialResstAll_Result>();
            var queryWard = new List<sp_GET_RPAntibicromialResstWard_Result>();
            var querySpcimen = new List<sp_GET_RPAntibicromialResstSpecimen_Result>();
            var queryRawAll = new List<sp_GET_RPAntibicromialResstAll_Result>();
            var queryRawWard = new List<sp_GET_RPAntibicromialResstWard_Result>();
            var queryRawSpcimen = new List<sp_GET_RPAntibicromialResstSpecimen_Result>();

            var organismMaster = new List<sp_GET_TCOrganism_Active_Result>();
            var antibioticMaster = new List<sp_GET_TCAntibiotic_Active_Result>();

            organismMaster = db.sp_GET_TCOrganism_Active(null).ToList();
            antibioticMaster = db.sp_GET_TCAntibiotic_Active(null).ToList();
            var AntiLabelName = db.sp_GET_RPAntibioticName(null).ToList();

            ReportDocument rpt = new ReportDocument();
            var strReportName = "";
            object dsReport = new object();

            //query = db.sp_GET_RPAntibicromialResistance(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
            //queryRaw = query.ToList();
            if (modelSubGraph == 0)
            {
                queryAll = db.sp_GET_RPAntibicromialResstAll(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                queryRawAll = queryAll.ToList();
                dsReport = queryAll;
            }
            else if (modelSubGraph == 1)
            {
                querySpcimen = db.sp_GET_RPAntibicromialResstSpecimen(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                queryRawSpcimen = querySpcimen.ToList();
                dsReport = querySpcimen;
            }
            else if (modelSubGraph == 2)
            {
                queryWard = db.sp_GET_RPAntibicromialResstWard(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                queryRawWard = queryWard.ToList();
                dsReport = queryWard;
            }

                 
            // add empty 
            for (var iYear = modelYearFrom; iYear <= modelYearTo; iYear++)
            {
                foreach (var orgn in modelSelectedOrg)
                {
                    foreach (var drug in modelSelectedAnti)
                    {
                        var drugname = AntiLabelName.Where(s => s.ant_code == drug).FirstOrDefault().ant_name;
                        if (modelSubGraph == 0)
                        {
                            var check = queryRawAll.Where(w => w.year == iYear && w.anti_code == drug && w.org_code == orgn).ToList();
                            if (check.Count() == 0)
                            {
                                var emptData = new sp_GET_RPAntibicromialResstAll_Result();
                                emptData.year = iYear;
                                emptData.anti_code = drug;
                                emptData.anti_name = drugname;//(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                emptData.org_code = orgn;
                                //ToDo : Find Org Name From Org Code
                                //emptData.org_name = ((orgn == "alc" )? "Alcaligenes sp." : "Bacteroides denticola"); 
                                emptData.org_name = orgn; //(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                emptData.spc_code = "";
                                emptData.ward_type = "";
                                emptData.percent_s = 0;
                                emptData.percent_i = 0;
                                emptData.percent_r = 0;

                                queryAll.Add(emptData);
                            }
                        }

                        else if (modelSubGraph == 1)
                        {
                            foreach (var spc in SpecimenMaster)
                            {
                                var checkSpec = queryRawSpcimen.Where(w => w.year == iYear && w.anti_code == drug
                                                         && w.org_code == orgn && w.spc_code == spc).ToList();

                                var spcName = SpecimenMasters.Where(s => s.spc_code == spc).FirstOrDefault();

                                if (checkSpec.Count() == 0)
                                {
                                    var emptData = new sp_GET_RPAntibicromialResstSpecimen_Result();
                                    emptData.year = iYear;
                                    emptData.anti_code = drug;
                                    //ToDo : Find Anti Name From Anti Code
                                    emptData.anti_name = (antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                    emptData.org_code = orgn;
                                    //ToDo : Find Org Name From Org Code
                                    emptData.org_name = orgn; //(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                    emptData.spc_code = spc;
                                    emptData.spc_name = (!string.IsNullOrEmpty(spcName.spc_name)) ? spcName.spc_name : "";
                                    emptData.ward_type = "";
                                    emptData.percent_s = 0;
                                    emptData.percent_i = 0;
                                    emptData.percent_r = 0;

                                    querySpcimen.Add(emptData);
                                }
                            }
                        }

                        else if (modelSubGraph == 2)
                        {
                            foreach (var ward in WardTypeMaster)
                            {
                                var checkWard = queryRawWard.Where(w => w.year == iYear && w.anti_code == drug
                                                         && w.org_code == orgn && w.ward_type == ward).ToList();
                                if (checkWard.Count() == 0)
                                {
                                    var emptData = new sp_GET_RPAntibicromialResstWard_Result();
                                    emptData.year = iYear;
                                    emptData.anti_code = drug;
                                    emptData.anti_name = (antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                    emptData.org_code = orgn;
                                    emptData.org_name = orgn;//(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                    emptData.spc_code = "";
                                    emptData.ward_type = ward;
                                    emptData.percent_s = 0;
                                    emptData.percent_i = 0;
                                    emptData.percent_r = 0;

                                    queryWard.Add(emptData);
                                }
                            }
                        }
                    }
                }
            }

            //case test : หลายเชื้อ 1 ยา หรือ 1:1 (no-sub)
            //Note : where ยา (Data ทั้งหมดต้องเป็นยา 1 ตัวเท่านั้น)

            if (modelSelectedOrg.Count() > 0 && modelSelectedAnti.Count() == 1)
            {
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAntiOrg.rpt";
                    strReportTitle += modelStrSelectedAnti;
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMRAntiSubSpc.rpt";
                    strReportTitle += string.Format("{0} by {1}", modelStrSelectedAnti, "Specimen");
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMRAntiSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}", modelStrSelectedAnti, "Ward Type");
                }
            }


            //case test : 1 เชื้อ หลายยา 
            //Note : where เชื้อ (Data ทั้งหมดต้องเป็นเชื้อ 1 ตัวเท่านั้น)
            if (modelSelectedOrg.Count() == 1 && modelSelectedAnti.Count() > 1)
            {
                //query = db.sp_GET_RPAntibicromialResistance(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAnti.rpt";
                    strReportTitle += modelStrSelectedOrg;
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMROrgSubSpec.rpt";
                    strReportTitle += string.Format("{0} by {1}", modelStrSelectedOrg, "Specimen");
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMROrgSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}", modelStrSelectedOrg, "Ward Type");
                }

            }

            //case test : หลายเชื้อ หลายยา (no-sub)
            //Note จำนวนกราฟ = จำนวนยา , จำนวน series = จำนวนเชื้อ
            if (modelSelectedOrg.Count() > 1 && modelSelectedAnti.Count() > 1)
            {
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAntiOrg.rpt";
                    strReportTitle += modelStrSelectedOrg;
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMRAntiSubSpc.rpt";
                    strReportTitle += string.Format("{0} by {1}", modelStrSelectedAnti, "Specimen");
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMRAntiSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}", modelStrSelectedAnti, "Ward Type");
                }
            }

            
            var Targetpath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Report"), strReportName);
            rpt.Load(Targetpath);
            rpt.SetDataSource(dsReport);
            rpt.SetParameterValue("paramStrTitle", strReportTitle);
            rpt.SetParameterValue("paramStrSelectedSIR", modelSelectedSIR);
            rpt.SetParameterValue("paramIntChartType", modelGraphFormat);

            // --------------- Export to PDF -------------------------------

            var stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            // processing the stream.
            var sm = ReadFully(stream);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(sm.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    //FileName = "AMR.xls"
                    FileName = "AMR.pdf"
                };
            result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/pdf");

            rpt.Dispose();

            var response = ResponseMessage(result);
            var log_end = new LogWriter("End AMRGraph/GetFile ...");
            return response;
        }

        [HttpPost]
        [Route("api/AMRGraph/ExportGraph")]
        public IHttpActionResult ExportGraph(AMRGraphSearchDTO model)
        //[HttpGet]
        //[Route("api/AMRGraph/ExportGraph")]
        //public IHttpActionResult ExportGraph()
        {
            var log_start = new LogWriter("Start ExportGraph ...");

            var modelYearFrom = model.start_year; //2015; 
            var modelYearTo =   model.end_year; //2020; 
            var modelSelectedOrg =  model.organism; //new List<string> { "alc", "bde" }; 
            var modelSelectedAnti =  model.antibiotic; // new List<string> { "AMX", "CEC" };
            var modelStrSelectedOrg = string.Join(",", modelSelectedOrg);
            var modelStrSelectedAnti = string.Join(",", modelSelectedAnti);
            var modelSelectedSIR =  model.sir; //"S"; 
            var modelGraphFormat = model.graph_format ; //  1 = Line , 2 = Bar ;
            var modelSubGraph = model.sub_graph;  // 0 = none , 1 = Specimen , 2 = Ward//model.sub_graph; 
            var modelSelectedWardList = model.wardlist;
            var modelSelectedSpecimenList = model.specimenlist;
            var strReportTitle = "";

            // Get Specimen Master
            var SpecimenMasters = db.sp_GET_TCSpecimen_Active(null).ToList();
            //var SpecimenMaster = SpecimenMasters.Select(w => w.spc_code).Distinct().ToList();
            var WardTypeMasters = db.sp_GET_TCWardType_Active(null);
            var WardTypeMaster = WardTypeMasters.Select(w => w.wrd_name).Distinct().ToList();

            if (modelSelectedSIR == "S") { strReportTitle =  "Susceptibility rates of ";}
            else if (modelSelectedSIR == "I") { strReportTitle = "Intermediate rates of "; }
            else { strReportTitle = "Resistance rates of "; }

            var queryAll = new List<sp_GET_RPAntibicromialResstAll_Result>();
            var queryWard = new List<sp_GET_RPAntibicromialResstWard_Result>();
            var querySpcimen = new List<sp_GET_RPAntibicromialResstSpecimen_Result>();
            var queryRawAll = new List<sp_GET_RPAntibicromialResstAll_Result>();
            var queryRawWard = new List<sp_GET_RPAntibicromialResstWard_Result>();
            var queryRawSpcimen = new List<sp_GET_RPAntibicromialResstSpecimen_Result>();

            var organismMaster = new List<sp_GET_TCOrganism_Active_Result>();
            var antibioticMaster = new List<sp_GET_TCAntibiotic_Active_Result>();

            organismMaster = db.sp_GET_TCOrganism_Active(null).ToList();
            antibioticMaster = db.sp_GET_TCAntibiotic_Active(null).ToList();

            var AntiLabelName = db.sp_GET_RPAntibioticName(null).ToList();
            //var AntiName = AntiLabelName.Select(s => s.ant_name).Distinct().ToList();

            ReportDocument rpt = new ReportDocument();
            var strReportName = "";
            object dsReport = new object();

            //query = db.sp_GET_RPAntibicromialResistance(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
            //queryRaw = query.ToList();

            if (modelSubGraph == 0)
            {
                queryAll = db.sp_GET_RPAntibicromialResstAll(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                queryRawAll = queryAll.ToList();
                dsReport = queryAll;
            }
            else if (modelSubGraph == 1)
            {
                querySpcimen = db.sp_GET_RPAntibicromialResstSpecimen(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                querySpcimen = querySpcimen.Where(w => modelSelectedSpecimenList.Contains(w.spc_code)).ToList();

                queryRawSpcimen = querySpcimen.ToList();
                dsReport = querySpcimen;
            }
            else if (modelSubGraph == 2)
            {
                queryWard = db.sp_GET_RPAntibicromialResstWard(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                queryWard = queryWard.Where(w => modelSelectedWardList.Contains(w.ward_type)).ToList();

                queryRawWard = queryWard.ToList();
                dsReport = queryWard;
            }

            // add empty 
            for (var iYear = modelYearFrom; iYear <= modelYearTo; iYear++)
            {   
                foreach(var orgn in modelSelectedOrg)
                {               
                    foreach (var drug in modelSelectedAnti)
                    {
                        var drugname = AntiLabelName.Where(s => s.ant_code == drug).FirstOrDefault().ant_name;
                        if (modelSubGraph == 0)
                        {
                            var check = queryRawAll.Where(w => w.year == iYear && w.anti_code == drug && w.org_code == orgn).ToList();
                            if (check.Count() == 0)
                            {
                                var emptData = new sp_GET_RPAntibicromialResstAll_Result();
                                emptData.year = iYear;
                                emptData.anti_code = drug;
                                emptData.anti_name = drugname;//(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null)? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                emptData.org_code = orgn;
                                emptData.org_name = orgn;//(organismMaster.Find(t => t.org_mst_ORG == orgn) != null)? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                emptData.spc_code = "";
                                emptData.spc_name = "";
                                emptData.ward_type = "";
                                emptData.percent_s = 0;
                                emptData.percent_i = 0;
                                emptData.percent_r = 0;

                                queryAll.Add(emptData);
                            }
                        }                      
                        
                        else if(modelSubGraph == 1)
                        {
                            foreach (var spc in modelSelectedSpecimenList)
                            {
                                var checkSpec = queryRawSpcimen.Where(w => w.year == iYear && w.anti_code == drug
                                                         && w.org_code == orgn && w.spc_code == spc).ToList();

                                var spcName = SpecimenMasters.Where(s => s.spc_code == spc).FirstOrDefault();
                                if (checkSpec.Count() == 0)
                                {
                                    var emptData = new sp_GET_RPAntibicromialResstSpecimen_Result();
                                    emptData.year = iYear;
                                    emptData.anti_code = drug;
                                    emptData.anti_name = drugname;//(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                    emptData.org_code = orgn;
                                    emptData.org_name = orgn; //(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                    emptData.spc_code = spc;
                                    emptData.spc_name = (!string.IsNullOrEmpty(spcName.spc_name))? spcName.spc_name :  "";
                                    emptData.ward_type = "";
                                    emptData.percent_s = 0;
                                    emptData.percent_i = 0;
                                    emptData.percent_r = 0;

                                    querySpcimen.Add(emptData);
                                }
                            }
                        }
                        
                        else if(modelSubGraph == 2)
                        {
                            foreach (var ward in modelSelectedWardList)
                            {
                                var checkWard = queryRawWard.Where(w => w.year == iYear && w.anti_code == drug
                                                         && w.org_code == orgn && w.ward_type == ward).ToList();
                                if (checkWard.Count() == 0)
                                {
                                    var emptData = new sp_GET_RPAntibicromialResstWard_Result();
                                    emptData.year = iYear;
                                    emptData.anti_code = drug;
                                    emptData.anti_name = drugname;//(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                    emptData.org_code = orgn;
                                    emptData.org_name = orgn; //(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                    emptData.spc_code = "";
                                    emptData.spc_name = "";
                                    emptData.ward_type = ward;
                                    emptData.percent_s = 0;
                                    emptData.percent_i = 0;
                                    emptData.percent_r = 0;

                                    queryWard.Add(emptData);
                                }
                            }
                        }           
                    }          
                }
            }

            //case test : หลายเชื้อ 1 ยา หรือ 1:1 (no-sub)
            //Note : where ยา (Data ทั้งหมดต้องเป็นยา 1 ตัวเท่านั้น)

            //var modelStrSelectedOrg = string.Join(",", modelSelectedOrg);
            //var modelStrSelectedAnti = string.Join(",", modelSelectedAnti);
            string strMultiOrgName = "";
            foreach (var orgn in modelSelectedOrg)
            {
                strMultiOrgName += (organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                strMultiOrgName += " and ";
            }
            strMultiOrgName = strMultiOrgName.Substring(0, strMultiOrgName.Length - 4);

            string strMultiAntiName = "";
            foreach (var drug in modelSelectedAnti)
            {
                strMultiAntiName += (antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                strMultiAntiName += " and ";
            }
            strMultiAntiName = strMultiAntiName.Substring(0, strMultiAntiName.Length - 4);

            if (modelSelectedOrg.Count() > 0 && modelSelectedAnti.Count() == 1)
            {
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAntiOrg.rpt";
                    strReportTitle += strMultiAntiName;
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMRAntiSubSpc.rpt";
                    strReportTitle += string.Format("{0} by {1}", strMultiAntiName, "Specimen");
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMRAntiSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}", strMultiAntiName, "Ward Type");
                }
            }
            

            //case test : 1 เชื้อ หลายยา 
            //Note : where เชื้อ (Data ทั้งหมดต้องเป็นเชื้อ 1 ตัวเท่านั้น)
            if (modelSelectedOrg.Count() == 1 && modelSelectedAnti.Count() > 1)
            {
                //query = db.sp_GET_RPAntibicromialResistance(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAnti.rpt";
                    strReportTitle += strMultiOrgName;
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMROrgSubSpec.rpt";
                    strReportTitle += string.Format("{0} by {1}", strMultiOrgName, "Specimen");
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMROrgSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}", strMultiOrgName, "Ward Type");
                }
               
            }

            //case test : หลายเชื้อ หลายยา (no-sub)
            //Note จำนวนกราฟ = จำนวนยา , จำนวน series = จำนวนเชื้อ
            if (modelSelectedOrg.Count() > 1 && modelSelectedAnti.Count() > 1)
            {
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAntiOrg.rpt";
                    strReportTitle += strMultiOrgName;
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMRAntiSubSpc.rpt";
                    strReportTitle += string.Format("{0} by {1}", strMultiAntiName, "Specimen");
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMRAntiSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}", strMultiAntiName, "Ward Type");
                }
            }


            var Targetpath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Report"), strReportName);
            rpt.Load(Targetpath);
            rpt.SetDataSource(dsReport);
            rpt.SetParameterValue("paramStrTitle", strReportTitle);
            rpt.SetParameterValue("paramStrSelectedSIR", modelSelectedSIR);
            rpt.SetParameterValue("paramIntChartType", modelGraphFormat);

            // --------------- Export to PDF -------------------------------

            var stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            // processing the stream.
            var sm = ReadFully(stream);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(sm.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    //FileName = "AMR.xls"
                    FileName = "AMR.pdf"
                };
            result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/pdf");

            rpt.Dispose();

            var response = ResponseMessage(result);
            var log_end = new LogWriter("End ExportGraph ...");
            return response;
        }


        //[HttpGet]
        //[Route("api/AMRGraph/ExportGraphByHosp")]
        //public IHttpActionResult ExportGraphByHosp()
        [HttpPost]
        [Route("api/AMRGraph/ExportGraphByHosp")]
        public IHttpActionResult ExportGraphByHosp(AMRGraphSearchDTO model)
        {
            var log_start = new LogWriter("Start ExportGraphByHosp ...");

            var modelYearFrom = model.start_year; //2015; 
            var modelYearTo = model.end_year; //2020; 
            var modelSelectedOrg = model.organism; //new List<string> { "Enterococcus faecalis" };  
            var modelSelectedAnti = model.antibiotic; //new List<string> { "PEN" ,"AMK"};  
            var modelStrSelectedOrg = string.Join(",", modelSelectedOrg);
            var modelStrSelectedAnti = string.Join(",", modelSelectedAnti);
            var modelSelectedSIR = model.sir; // "R"; 
            var modelGraphFormat = model.graph_format; //  1 = Line , 2 = Bar ;
            var modelSubGraph = model.sub_graph; // 0 = none , 1 = Specimen , 2 = Ward//model.sub_graph; 
            var strReportTitle = "";
            var modelHosCode =  model.hos_code;  //"001073400";
            var modelHosName =  model.hos_name; //"โรงพยาบาลสมุทรสาคร";
            var modelSelectedWardList = model.wardlist; //new List<string>() { "ipd","opd" };
            var modelSelectedSpecimenList = model.specimenlist; //new List<string>() { "bl","ur"};

            // Get Specimen Master
            var SpecimenMasters = db.sp_GET_TCSpecimen_Active(null).ToList();
            //var SpecimenMaster = SpecimenMasters.Select(w => w.spc_code).Distinct().ToList();
            var WardTypeMasters = db.sp_GET_TCWardType_Active(null);
            var WardTypeMaster = WardTypeMasters.Select(w => w.wrd_name).Distinct().ToList();

            if (modelSelectedSIR == "S") { strReportTitle = "Susceptibility rates of "; }
            else if (modelSelectedSIR == "I") { strReportTitle = "Intermediate rates of "; }
            else { strReportTitle = "Resistance rates of "; }

            var queryAll = new List<sp_GET_RPAntibicromialResstAll_byHosp_Result>();
            var queryWard = new List<sp_GET_RPAntibicromialResstWard_byHosp_Result>();
            var querySpcimen = new List<sp_GET_RPAntibicromialResstSpecimen_byHosp_Result>();
            var queryRawAll = new List<sp_GET_RPAntibicromialResstAll_byHosp_Result>();
            var queryRawWard = new List<sp_GET_RPAntibicromialResstWard_byHosp_Result>();
            var queryRawSpcimen = new List<sp_GET_RPAntibicromialResstSpecimen_byHosp_Result>();

            var organismMaster = new List<sp_GET_TCOrganism_Active_Result>();
            var antibioticMaster = new List<sp_GET_TCAntibiotic_Active_Result>();

            organismMaster = db.sp_GET_TCOrganism_Active(null).ToList();
            antibioticMaster = db.sp_GET_TCAntibiotic_Active(null).ToList();
            var AntiLabelName = db.sp_GET_RPAntibioticName(null).ToList();

            ReportDocument rpt = new ReportDocument();
            var strReportName = "";
            object dsReport = new object();

            //query = db.sp_GET_RPAntibicromialResistance(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
            //queryRaw = query.ToList();

            if (modelSubGraph == 0)
            {
                queryAll = db.sp_GET_RPAntibicromialResstAll_byHosp(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo, modelHosCode).ToList();
                queryRawAll = queryAll.ToList();
                dsReport = queryAll;
            }
            else if (modelSubGraph == 1)
            {
                querySpcimen = db.sp_GET_RPAntibicromialResstSpecimen_byHosp(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo, modelHosCode).ToList();
                querySpcimen = querySpcimen.Where(w => modelSelectedSpecimenList.Contains(w.spc_code)).ToList();

                queryRawSpcimen = querySpcimen.ToList();
                dsReport = querySpcimen;
            }
            else if (modelSubGraph == 2)
            {
                queryWard = db.sp_GET_RPAntibicromialResstWard_byHosp(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo, modelHosCode).ToList();
                queryWard = queryWard.Where(w => modelSelectedWardList.Contains(w.ward_type)).ToList();

                queryRawWard = queryWard.ToList();
                dsReport = queryWard;
            }

            // add empty 
            for (var iYear = modelYearFrom; iYear <= modelYearTo; iYear++)
            {
                foreach (var orgn in modelSelectedOrg)
                {
                    foreach (var drug in modelSelectedAnti)
                    {
                        var drugname = AntiLabelName.Where(s => s.ant_code == drug).FirstOrDefault().ant_name;
                        if (modelSubGraph == 0)
                        {
                            var check = queryRawAll.Where(w => w.year == iYear && w.anti_code == drug && w.org_code == orgn).ToList();
                            if (check.Count() == 0)
                            {
                                var emptData = new sp_GET_RPAntibicromialResstAll_byHosp_Result();
                                emptData.year = iYear;
                                emptData.anti_code = drug;
                                emptData.anti_name = drugname;//(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                emptData.org_code = orgn;
                                emptData.org_name = orgn; // (organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                emptData.spc_code = "";
                                emptData.spc_name = "";
                                emptData.ward_type = "";
                                emptData.percent_s = 0;
                                emptData.percent_i = 0;
                                emptData.percent_r = 0;
                                
                                queryAll.Add(emptData);
                            }
                        }

                        else if (modelSubGraph == 1)
                        {
                            foreach (var spc in modelSelectedSpecimenList)
                            {
                                var checkSpec = queryRawSpcimen.Where(w => w.year == iYear && w.anti_code == drug
                                                         && w.org_code == orgn && w.spc_code == spc).ToList();

                                var spcName = SpecimenMasters.Where(s => s.spc_code == spc).FirstOrDefault();

                                if (checkSpec.Count() == 0)
                                {
                                    var emptData = new sp_GET_RPAntibicromialResstSpecimen_byHosp_Result();
                                    emptData.year = iYear;
                                    emptData.anti_code = drug;
                                    emptData.anti_name = drugname;// (antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                    emptData.org_code = orgn;
                                    emptData.org_name = orgn; //(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                    emptData.spc_code = spc;
                                    emptData.spc_name = (!string.IsNullOrEmpty(spcName.spc_name)) ? spcName.spc_name : "";
                                    emptData.ward_type = "";
                                    emptData.percent_s = 0;
                                    emptData.percent_i = 0;
                                    emptData.percent_r = 0;

                                    querySpcimen.Add(emptData);
                                }
                            }
                        }

                        else if (modelSubGraph == 2)
                        {
                            foreach (var ward in modelSelectedWardList)
                            {
                                var checkWard = queryRawWard.Where(w => w.year == iYear && w.anti_code == drug
                                                         && w.org_code == orgn && w.ward_type == ward).ToList();
                                if (checkWard.Count() == 0)
                                {
                                    var emptData = new sp_GET_RPAntibicromialResstWard_byHosp_Result();
                                    emptData.year = iYear;
                                    emptData.anti_code = drug;
                                    emptData.anti_name = drugname;//(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                    emptData.org_code = orgn;
                                    emptData.org_name = orgn;//(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                    emptData.spc_code = "";
                                    emptData.spc_name = "";
                                    emptData.ward_type = ward;
                                    emptData.percent_s = 0;
                                    emptData.percent_i = 0;
                                    emptData.percent_r = 0;

                                    queryWard.Add(emptData);
                                }
                            }
                        }
                    }
                }
            }

            //case test : หลายเชื้อ 1 ยา หรือ 1:1 (no-sub)
            //Note : where ยา (Data ทั้งหมดต้องเป็นยา 1 ตัวเท่านั้น)

            //var modelStrSelectedOrg = string.Join(",", modelSelectedOrg);
            //var modelStrSelectedAnti = string.Join(",", modelSelectedAnti);
            string strMultiOrgName = "";
            foreach (var orgn in modelSelectedOrg)
            {
                strMultiOrgName += (organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                strMultiOrgName += " and ";
            }
            strMultiOrgName = strMultiOrgName.Substring(0, strMultiOrgName.Length - 4);

            string strMultiAntiName = "";
            foreach (var drug in modelSelectedAnti)
            {
                strMultiAntiName += (antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                strMultiAntiName += " and ";
            }
            strMultiAntiName = strMultiAntiName.Substring(0, strMultiAntiName.Length - 4);

            if (modelSelectedOrg.Count() > 0 && modelSelectedAnti.Count() == 1)
            {
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAntiOrg.rpt";
                    strReportTitle += string.Format("{0}" + Environment.NewLine + "({1})"
                                                    , strMultiAntiName  , modelHosName);
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMRAntiSubSpc.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Specimen", modelHosName);
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMRAntiSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Ward Type", modelHosName);
                }
            }


            //case test : 1 เชื้อ หลายยา 
            //Note : where เชื้อ (Data ทั้งหมดต้องเป็นเชื้อ 1 ตัวเท่านั้น)
            if (modelSelectedOrg.Count() == 1 && modelSelectedAnti.Count() > 1)
            {
                //query = db.sp_GET_RPAntibicromialResistance(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAnti.rpt";
                    strReportTitle += string.Format("{0}" + Environment.NewLine + "({1})"
                                                    , strMultiOrgName, modelHosName);
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMROrgSubSpec.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiOrgName, "Specimen", modelHosName);
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMROrgSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiOrgName, "Ward Type", modelHosName);
                }

            }

            //case test : หลายเชื้อ หลายยา (no-sub)
            //Note จำนวนกราฟ = จำนวนยา , จำนวน series = จำนวนเชื้อ
            if (modelSelectedOrg.Count() > 1 && modelSelectedAnti.Count() > 1)
            {
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAntiOrg.rpt";
                    strReportTitle += string.Format("{0}" + Environment.NewLine + "({1})"
                                                    , strMultiOrgName, modelHosName);
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMRAntiSubSpc.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Specimen", modelHosName);
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMRAntiSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Ward Type", modelHosName);
                }
            }


            var Targetpath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Report"), strReportName);
            rpt.Load(Targetpath);
            rpt.SetDataSource(dsReport);
            rpt.SetParameterValue("paramStrTitle", strReportTitle);
            rpt.SetParameterValue("paramStrSelectedSIR", modelSelectedSIR);
            rpt.SetParameterValue("paramIntChartType", modelGraphFormat);

            // --------------- Export to PDF -------------------------------

            var stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            // processing the stream.
            var sm = ReadFully(stream);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(sm.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    //FileName = "AMR.xls"
                    FileName = "AMR.pdf"
                };
            result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/pdf");

            rpt.Dispose();

            var response = ResponseMessage(result);
            var log_end = new LogWriter("End ExportGraphByHosp ...");
            return response;
        }


        //[HttpGet]
        //[Route("api/AMRGraph/ExportGraphByAreaH")]
        //public IHttpActionResult ExportGraphByAreaH()
        [HttpPost]
        [Route("api/AMRGraph/ExportGraphByAreaH")]
        public IHttpActionResult ExportGraphByAreaH(AMRGraphSearchDTO model)
        {
            var log_start = new LogWriter("Start ExportGraphByAreH ...");

            var modelYearFrom =  model.start_year; //2015;
            var modelYearTo = model.end_year; //2020; 
            var modelSelectedOrg =  model.organism; //new List<string> { "Enterococcus faecalis", "Enterococcus faecium" };
            var modelSelectedAnti = model.antibiotic; // new List<string> { "PEN" }; 
            var modelStrSelectedOrg = string.Join(",", modelSelectedOrg);
            var modelStrSelectedAnti = string.Join(",", modelSelectedAnti);
            var modelSelectedSIR = model.sir;  //"R";
            var modelGraphFormat = model.graph_format; //  1 = Line , 2 = Bar ;
            var modelSubGraph = model.sub_graph;  // 0 = none , 1 = Specimen , 2 = Ward
            var strReportTitle = "";
            var modelArhCode =  model.arh_code; // "05"; 
            var modelArhName = model.arh_name; // "เขตที่5"; 
            var modelSelectedWardList = model.wardlist;
            var modelSelectedSpecimenList = model.specimenlist;

            // Get Specimen Master
            var SpecimenMasters = db.sp_GET_TCSpecimen_Active(null).ToList();
            //var SpecimenMaster = SpecimenMasters.Select(w => w.spc_code).Distinct().ToList();
            var WardTypeMasters = db.sp_GET_TCWardType_Active(null);
            var WardTypeMaster = WardTypeMasters.Select(w => w.wrd_name).Distinct().ToList();

            if (modelSelectedSIR == "S") { strReportTitle = "Susceptibility rates of "; }
            else if (modelSelectedSIR == "I") { strReportTitle = "Intermediate rates of "; }
            else { strReportTitle = "Resistance rates of "; }

            var queryAll = new List<sp_GET_RPAntibicromialResstAll_byAreaH_Result>();
            var queryWard = new List<sp_GET_RPAntibicromialResstWard_byAreaH_Result>();
            var querySpcimen = new List<sp_GET_RPAntibicromialResstSpecimen_byAreaH_Result>();
            var queryRawAll = new List<sp_GET_RPAntibicromialResstAll_byAreaH_Result>();
            var queryRawWard = new List<sp_GET_RPAntibicromialResstWard_byAreaH_Result>();
            var queryRawSpcimen = new List<sp_GET_RPAntibicromialResstSpecimen_byAreaH_Result>();

            var organismMaster = new List<sp_GET_TCOrganism_Active_Result>();
            var antibioticMaster = new List<sp_GET_TCAntibiotic_Active_Result>();

            organismMaster = db.sp_GET_TCOrganism_Active(null).ToList();
            antibioticMaster = db.sp_GET_TCAntibiotic_Active(null).ToList();
            var AntiLabelName = db.sp_GET_RPAntibioticName(null).ToList();

            ReportDocument rpt = new ReportDocument();
            var strReportName = "";
            object dsReport = new object();

            //query = db.sp_GET_RPAntibicromialResistance(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
            //queryRaw = query.ToList();

            if (modelSubGraph == 0)
            {
                queryAll = db.sp_GET_RPAntibicromialResstAll_byAreaH(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo, modelArhCode).ToList();
                queryRawAll = queryAll.ToList();
                dsReport = queryAll;
            }
            else if (modelSubGraph == 1)
            {
                querySpcimen = db.sp_GET_RPAntibicromialResstSpecimen_byAreaH(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo, modelArhCode).ToList();
                querySpcimen = querySpcimen.Where(w => modelSelectedSpecimenList.Contains(w.spc_code)).ToList();

                queryRawSpcimen = querySpcimen.ToList();
                dsReport = querySpcimen;
            }
            else if (modelSubGraph == 2)
            {
                queryWard = db.sp_GET_RPAntibicromialResstWard_byAreaH(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo, modelArhCode).ToList();
                queryWard = queryWard.Where(w => modelSelectedWardList.Contains(w.ward_type)).ToList();

                queryRawWard = queryWard.ToList();
                dsReport = queryWard;
            }

            // add empty 
            for (var iYear = modelYearFrom; iYear <= modelYearTo; iYear++)
            {
                foreach (var orgn in modelSelectedOrg)
                {
                    foreach (var drug in modelSelectedAnti)
                    {
                        var drugname = AntiLabelName.Where(s => s.ant_code == drug).FirstOrDefault().ant_name;
                        if (modelSubGraph == 0)
                        {
                            var check = queryRawAll.Where(w => w.year == iYear && w.anti_code == drug && w.org_code == orgn).ToList();
                            if (check.Count() == 0)
                            {
                                var emptData = new sp_GET_RPAntibicromialResstAll_byAreaH_Result();
                                emptData.year = iYear;
                                emptData.anti_code = drug;
                                emptData.anti_name = drugname; //(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                emptData.org_code = orgn;
                                emptData.org_name = (organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                emptData.spc_code = "";
                                emptData.ward_type = "";
                                emptData.percent_s = 0;
                                emptData.percent_i = 0;
                                emptData.percent_r = 0;

                                queryAll.Add(emptData);
                            }
                        }

                        else if (modelSubGraph == 1)
                        {
                            foreach (var spc in modelSelectedSpecimenList)
                            {
                                var checkSpec = queryRawSpcimen.Where(w => w.year == iYear && w.anti_code == drug
                                                         && w.org_code == orgn && w.spc_code == spc).ToList();

                                var spcName = SpecimenMasters.Where(s => s.spc_code == spc).FirstOrDefault();

                                if (checkSpec.Count() == 0)
                                {
                                    var emptData = new sp_GET_RPAntibicromialResstSpecimen_byAreaH_Result();
                                    emptData.year = iYear;
                                    emptData.anti_code = drug;
                                    emptData.anti_name = drugname;//(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                    emptData.org_code = orgn;
                                    emptData.org_name = orgn;//(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                    emptData.spc_code = spc;
                                    emptData.spc_name = (!string.IsNullOrEmpty(spcName.spc_name)) ? spcName.spc_name : "";
                                    emptData.ward_type = "";
                                    emptData.percent_s = 0;
                                    emptData.percent_i = 0;
                                    emptData.percent_r = 0;

                                    querySpcimen.Add(emptData);
                                }
                            }
                        }

                        else if (modelSubGraph == 2)
                        {
                            foreach (var ward in modelSelectedWardList)
                            {
                                var checkWard = queryRawWard.Where(w => w.year == iYear && w.anti_code == drug
                                                         && w.org_code == orgn && w.ward_type == ward).ToList();
                                if (checkWard.Count() == 0)
                                {
                                    var emptData = new sp_GET_RPAntibicromialResstWard_byAreaH_Result();
                                    emptData.year = iYear;
                                    emptData.anti_code = drug;
                                    emptData.anti_name = drugname;//(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                    emptData.org_code = orgn;
                                    emptData.org_name = orgn;//(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                    emptData.spc_code = "";
                                    emptData.spc_name = "";
                                    emptData.ward_type = ward;
                                    emptData.percent_s = 0;
                                    emptData.percent_i = 0;
                                    emptData.percent_r = 0;

                                    queryWard.Add(emptData);
                                }
                            }
                        }
                    }
                }
            }

            //case test : หลายเชื้อ 1 ยา หรือ 1:1 (no-sub)
            //Note : where ยา (Data ทั้งหมดต้องเป็นยา 1 ตัวเท่านั้น)

            //var modelStrSelectedOrg = string.Join(",", modelSelectedOrg);
            //var modelStrSelectedAnti = string.Join(",", modelSelectedAnti);
            string strMultiOrgName = "";
            foreach (var orgn in modelSelectedOrg)
            {
                strMultiOrgName += (organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                strMultiOrgName += " and ";
            }
            strMultiOrgName = strMultiOrgName.Substring(0, strMultiOrgName.Length - 4);

            string strMultiAntiName = "";
            foreach (var drug in modelSelectedAnti)
            {
                strMultiAntiName += (antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                strMultiAntiName += " and ";
            }
            strMultiAntiName = strMultiAntiName.Substring(0, strMultiAntiName.Length - 4);

            if (modelSelectedOrg.Count() > 0 && modelSelectedAnti.Count() == 1)
            {
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAntiOrg.rpt";
                    strReportTitle += string.Format("{0}" + Environment.NewLine + "({1})"
                                                    , strMultiAntiName, modelArhName);
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMRAntiSubSpc.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Specimen", modelArhName);
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMRAntiSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Ward Type", modelArhName);
                }
            }


            //case test : 1 เชื้อ หลายยา 
            //Note : where เชื้อ (Data ทั้งหมดต้องเป็นเชื้อ 1 ตัวเท่านั้น)
            if (modelSelectedOrg.Count() == 1 && modelSelectedAnti.Count() > 1)
            {
                //query = db.sp_GET_RPAntibicromialResistance(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAnti.rpt";
                    strReportTitle += string.Format("{0}" + Environment.NewLine + "({1})"
                                                    , strMultiOrgName, modelArhName);
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMROrgSubSpec.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiOrgName, "Specimen", modelArhName);
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMROrgSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiOrgName, "Ward Type", modelArhName);
                }

            }

            //case test : หลายเชื้อ หลายยา (no-sub)
            //Note จำนวนกราฟ = จำนวนยา , จำนวน series = จำนวนเชื้อ
            if (modelSelectedOrg.Count() > 1 && modelSelectedAnti.Count() > 1)
            {
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAntiOrg.rpt";
                    strReportTitle += string.Format("{0}" + Environment.NewLine + "({1})"
                                                    , strMultiOrgName, modelArhName);
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMRAntiSubSpc.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Specimen", modelArhName);
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMRAntiSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Ward Type", modelArhName);
                }
            }


            var Targetpath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Report"), strReportName);
            rpt.Load(Targetpath);
            rpt.SetDataSource(dsReport);
            rpt.SetParameterValue("paramStrTitle", strReportTitle);
            rpt.SetParameterValue("paramStrSelectedSIR", modelSelectedSIR);
            rpt.SetParameterValue("paramIntChartType", modelGraphFormat);

            // --------------- Export to PDF -------------------------------

            var stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            // processing the stream.
            var sm = ReadFully(stream);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(sm.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    //FileName = "AMR.xls"
                    FileName = "AMR.pdf"
                };
            result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/pdf");

            rpt.Dispose();

            var response = ResponseMessage(result);
            var log_end = new LogWriter("End ExportGraphByAreaH ...");
            return response;
        }

        //[HttpGet]
        //[Route("api/AMRGraph/ExportGraphByProv")]
        //public IHttpActionResult ExportGraphByProv()
        [HttpPost]
        [Route("api/AMRGraph/ExportGraphByProv")]
        public IHttpActionResult ExportGraphByProv(AMRGraphSearchDTO model)
        {
            var log_start = new LogWriter("Start ExportGraphByProv ...");

            var modelYearFrom =   model.start_year; //2015;
            var modelYearTo =   model.end_year; //2020;
            var modelSelectedOrg = model.organism; //new List<string> { "Acinetobacter calcoaceticus-baumannii complex" };  
            var modelSelectedAnti =  model.antibiotic; //new List<string> { "AMK" ,"CIP"};  
            var modelStrSelectedOrg = string.Join(",", modelSelectedOrg);
            var modelStrSelectedAnti = string.Join(",", modelSelectedAnti);
            var modelSelectedSIR = model.sir; //"S"; 
            var modelGraphFormat = model.graph_format; //1 = Line , 2 = Bar ;//model.graph_format; 
            var modelSubGraph =  model.sub_graph;  // 0 = none , 1 = Specimen , 2 = Ward//model.sub_graph; 
            var strReportTitle = "";
            var modelPrvCode = model.prv_code; //"48"; 
            var modelPrvName = model.prv_name; //"นครพนม";
            var modelSelectedWardList = model.wardlist; //new List<string>() { "ipd","opd" };
            var modelSelectedSpecimenList = model.specimenlist; // new List<string>() { "bl","ur"};

            // Get Specimen Master
            var SpecimenMasters = db.sp_GET_TCSpecimen_Active(null).ToList();
            //var SpecimenMaster = SpecimenMasters.Select(w => w.spc_code).Distinct().ToList();
            var WardTypeMasters = db.sp_GET_TCWardType_Active(null);
            var WardTypeMaster = WardTypeMasters.Select(w => w.wrd_name).Distinct().ToList();

            if (modelSelectedSIR == "S") { strReportTitle = "Susceptibility rates of "; }
            else if (modelSelectedSIR == "I") { strReportTitle = "Intermediate rates of "; }
            else { strReportTitle = "Resistance rates of "; }

            var queryAll = new List<sp_GET_RPAntibicromialResstAll_byProv_Result>();
            var queryWard = new List<sp_GET_RPAntibicromialResstWard_byProv_Result>();
            var querySpcimen = new List<sp_GET_RPAntibicromialResstSpecimen_byProv_Result>();
            var queryRawAll = new List<sp_GET_RPAntibicromialResstAll_byProv_Result>();
            var queryRawWard = new List<sp_GET_RPAntibicromialResstWard_byProv_Result>();
            var queryRawSpcimen = new List<sp_GET_RPAntibicromialResstSpecimen_byProv_Result>();

            var organismMaster = new List<sp_GET_TCOrganism_Active_Result>();
            var antibioticMaster = new List<sp_GET_TCAntibiotic_Active_Result>();

            organismMaster = db.sp_GET_TCOrganism_Active(null).ToList();
            antibioticMaster = db.sp_GET_TCAntibiotic_Active(null).ToList();
            var AntiLabelName = db.sp_GET_RPAntibioticName(null).ToList();

            ReportDocument rpt = new ReportDocument();
            var strReportName = "";
            object dsReport = new object();

            //query = db.sp_GET_RPAntibicromialResistance(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
            //queryRaw = query.ToList();

            if (modelSubGraph == 0)
            {
                queryAll = db.sp_GET_RPAntibicromialResstAll_byProv(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo, modelPrvCode).ToList();
                queryRawAll = queryAll.ToList();
                dsReport = queryAll;
            }
            else if (modelSubGraph == 1)
            {
                querySpcimen = db.sp_GET_RPAntibicromialResstSpecimen_byProv(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo, modelPrvCode).ToList();
                querySpcimen = querySpcimen.Where(w => modelSelectedSpecimenList.Contains(w.spc_code)).ToList();

                queryRawSpcimen = querySpcimen.ToList();
                dsReport = querySpcimen;
            }
            else if (modelSubGraph == 2)
            {
                queryWard = db.sp_GET_RPAntibicromialResstWard_byProv(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo, modelPrvCode).ToList();
                queryWard = queryWard.Where(w => modelSelectedWardList.Contains(w.ward_type)).ToList();

                queryRawWard = queryWard.ToList();
                dsReport = queryWard;
            }

            // add empty 
            for (var iYear = modelYearFrom; iYear <= modelYearTo; iYear++)
            {
                foreach (var orgn in modelSelectedOrg)
                {
                    foreach (var drug in modelSelectedAnti)
                    {
                        var drugname = AntiLabelName.Where(s => s.ant_code == drug).FirstOrDefault().ant_name;
                        if (modelSubGraph == 0)
                        {
                            var check = queryRawAll.Where(w => w.year == iYear && w.anti_code == drug && w.org_code == orgn).ToList();
                            if (check.Count() == 0)
                            {
                                var emptData = new sp_GET_RPAntibicromialResstAll_byProv_Result();
                                emptData.year = iYear;
                                emptData.anti_code = drug;
                                emptData.anti_name = drugname;//(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                emptData.org_code = orgn;
                                emptData.org_name = orgn; // (organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                emptData.spc_code = "";
                                emptData.spc_name = "";
                                emptData.ward_type = "";
                                emptData.percent_s = 0;
                                emptData.percent_i = 0;
                                emptData.percent_r = 0;

                                queryAll.Add(emptData);
                            }
                        }

                        else if (modelSubGraph == 1)
                        {
                            foreach (var spc in modelSelectedSpecimenList)
                            {
                                var checkSpec = queryRawSpcimen.Where(w => w.year == iYear && w.anti_code == drug
                                                         && w.org_code == orgn && w.spc_code == spc).ToList();

                                var spcName = SpecimenMasters.Where(s => s.spc_code == spc).FirstOrDefault();

                                if (checkSpec.Count() == 0)
                                {
                                    var emptData = new sp_GET_RPAntibicromialResstSpecimen_byProv_Result();
                                    emptData.year = iYear;
                                    emptData.anti_code = drug;
                                    emptData.anti_name = drugname;// (antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                    emptData.org_code = orgn;
                                    emptData.org_name = orgn; //(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                    emptData.spc_code = spc;
                                    emptData.spc_name = (!string.IsNullOrEmpty(spcName.spc_name)) ? spcName.spc_name : "";
                                    emptData.ward_type = "";
                                    emptData.percent_s = 0;
                                    emptData.percent_i = 0;
                                    emptData.percent_r = 0;

                                    querySpcimen.Add(emptData);
                                }
                            }
                        }

                        else if (modelSubGraph == 2)
                        {
                            foreach (var ward in modelSelectedWardList)
                            {
                                var checkWard = queryRawWard.Where(w => w.year == iYear && w.anti_code == drug
                                                         && w.org_code == orgn && w.ward_type == ward).ToList();
                                if (checkWard.Count() == 0)
                                {
                                    var emptData = new sp_GET_RPAntibicromialResstWard_byProv_Result();
                                    emptData.year = iYear;
                                    emptData.anti_code = drug;
                                    emptData.anti_name = drugname;//(antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                                    emptData.org_code = orgn;
                                    emptData.org_name = orgn;//(organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                                    emptData.spc_code = "";
                                    emptData.spc_name = "";
                                    emptData.ward_type = ward;
                                    emptData.percent_s = 0;
                                    emptData.percent_i = 0;
                                    emptData.percent_r = 0;

                                    queryWard.Add(emptData);
                                }
                            }
                        }
                    }
                }
            }

            //case test : หลายเชื้อ 1 ยา หรือ 1:1 (no-sub)
            //Note : where ยา (Data ทั้งหมดต้องเป็นยา 1 ตัวเท่านั้น)

            //var modelStrSelectedOrg = string.Join(",", modelSelectedOrg);
            //var modelStrSelectedAnti = string.Join(",", modelSelectedAnti);
            string strMultiOrgName = "";
            foreach (var orgn in modelSelectedOrg)
            {
                strMultiOrgName += (organismMaster.Find(t => t.org_mst_ORG == orgn) != null) ? organismMaster.Find(t => t.org_mst_ORG == orgn).org_mst_ORGANISM : orgn;
                strMultiOrgName += " and ";
            }
            strMultiOrgName = strMultiOrgName.Substring(0, strMultiOrgName.Length - 4);

            string strMultiAntiName = "";
            foreach (var drug in modelSelectedAnti)
            {
                strMultiAntiName += (antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug) != null) ? antibioticMaster.Find(t => t.ant_mst_WHON5_CODE == drug).ant_name : drug;
                strMultiAntiName += " and ";
            }
            strMultiAntiName = strMultiAntiName.Substring(0, strMultiAntiName.Length - 4);

            if (modelSelectedOrg.Count() > 0 && modelSelectedAnti.Count() == 1)
            {
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAntiOrg.rpt";
                    strReportTitle += string.Format("{0}" + Environment.NewLine + "({1})"
                                                    , strMultiAntiName, modelPrvName);
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMRAntiSubSpc.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Specimen", modelPrvName);
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMRAntiSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Ward Type", modelPrvName);
                }
            }


            //case test : 1 เชื้อ หลายยา 
            //Note : where เชื้อ (Data ทั้งหมดต้องเป็นเชื้อ 1 ตัวเท่านั้น)
            if (modelSelectedOrg.Count() == 1 && modelSelectedAnti.Count() > 1)
            {
                //query = db.sp_GET_RPAntibicromialResistance(modelStrSelectedOrg, modelStrSelectedAnti, modelYearFrom, modelYearTo).ToList();
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAnti.rpt";
                    strReportTitle += string.Format("{0}" + Environment.NewLine + "({1})"
                                                    , strMultiOrgName, modelPrvName);
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMROrgSubSpec.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiOrgName, "Specimen", modelPrvName);
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMROrgSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiOrgName, "Ward Type", modelPrvName);
                }

            }

            //case test : หลายเชื้อ หลายยา (no-sub)
            //Note จำนวนกราฟ = จำนวนยา , จำนวน series = จำนวนเชื้อ
            if (modelSelectedOrg.Count() > 1 && modelSelectedAnti.Count() > 1)
            {
                if (modelSubGraph == 0) // No SubGraph
                {
                    strReportName = "rptAMRMultiAntiOrg.rpt";
                    strReportTitle += string.Format("{0}" + Environment.NewLine + "({1})"
                                                    , strMultiOrgName, modelPrvName);
                }
                else if (modelSubGraph == 1) // Sub Specimen
                {
                    strReportName = "rptAMRAntiSubSpc.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Specimen", modelPrvName);
                }
                else // Sub Ward Type
                {
                    strReportName = "rptAMRAntiSubWard.rpt";
                    strReportTitle += string.Format("{0} by {1}" + Environment.NewLine + "({2})"
                                                    , strMultiAntiName, "Ward Type", modelPrvName);
                }
            }


            var Targetpath = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/Report"), strReportName);
            rpt.Load(Targetpath);
            rpt.SetDataSource(dsReport);
            rpt.SetParameterValue("paramStrTitle", strReportTitle);
            rpt.SetParameterValue("paramStrSelectedSIR", modelSelectedSIR);
            rpt.SetParameterValue("paramIntChartType", modelGraphFormat);

            // --------------- Export to PDF -------------------------------

            var stream = rpt.ExportToStream(ExportFormatType.PortableDocFormat);
            // processing the stream.
            var sm = ReadFully(stream);
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(sm.ToArray())
            };
            result.Content.Headers.ContentDisposition =
                new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    //FileName = "AMR.xls"
                    FileName = "AMR.pdf"
                };
            result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("application/pdf");

            rpt.Dispose();

            var response = ResponseMessage(result);
            var log_end = new LogWriter("End ExportGraphByProv ...");
            return response;
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    } // end ApiController
}
