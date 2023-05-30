using ALISS.ANTIBIOTREND.DTO;
using ALISS.ANTIBIOTREND.Library.DataAccess;
using AutoMapper;
using Log4NetLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALISS.ANTIBIOTREND.Library
{
    public class AntibiotrendService : IAntibiotrendService
    {
        private static readonly ILogService log = new LogService(typeof(AntibiotrendService));

        private readonly AntibiotrendContext _db;
        private readonly IMapper _mapper;

        public AntibiotrendService(AntibiotrendContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public List<SP_AntimicrobialResistanceDTO> GetAMRWithModel(SP_AntimicrobialResistanceSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            //var searchModel = JsonSerializer.Deserialize<MenuSearchDTO>(param);

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResistance {0},{1},{2},{3}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }

        public List<NationHealthStrategyDTO> GetAMRNationStrategyWithModel(AMRStrategySearchDTO searchModel)
        {
            log.MethodStart();

            List<NationHealthStrategyDTO> objList = new List<NationHealthStrategyDTO>();

       
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.AMRNationHealthStrategyListDTOs.FromSqlRaw<NationHealthStrategyDTO>("sp_GET_RPNationHealthStrategy {0},{1}"
                                                                                                    , searchModel.month_start_str
                                                                                                    , searchModel.month_end_str
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<NationHealthStrategyDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }

        public List<AntibiotrendAMRStrategyDTO> GetAntibiotrendAMRStrategyWithModel(AMRStrategySearchDTO searchModel)
        {
            log.MethodStart();

            List<AntibiotrendAMRStrategyDTO> objList = new List<AntibiotrendAMRStrategyDTO>();


            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.AntibiotrendAMRStrategyListDTOs.FromSqlRaw<AntibiotrendAMRStrategyDTO>("sp_GET_RPAntibiotrendAMRStrategy_1 {0},{1}"
                                                                                                    , searchModel.month_start_str
                                                                                                    , searchModel.month_end_str
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<AntibiotrendAMRStrategyDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }

        public List<SP_AntimicrobialResistanceDTO> GetAMRByOverallWithModel(SP_AntimicrobialResistanceSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            //var searchModel = JsonSerializer.Deserialize<MenuSearchDTO>(param);

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstAll {0},{1},{2},{3}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }

        public List<SP_AntimicrobialResistanceDTO> GetAMRByOverallByAreaHWithModel(SP_AntimicrobialResistanceAreaHSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstAll_byAreaH {0},{1},{2},{3},{4}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    , searchModel.arh_code
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<SP_AntimicrobialResistanceDTO> GetAMRByOverallByHospWithModel(SP_AntimicrobialResistanceHospSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstAll_byHosp {0},{1},{2},{3},{4}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    , searchModel.hos_code
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<SP_AntimicrobialResistanceDTO> GetAMRByOverallByProvWithModel(SP_AntimicrobialResistanceProvinceSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstAll_byProv {0},{1},{2},{3},{4}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    , searchModel.prv_code
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<SP_AntimicrobialResistanceDTO> GetAMRBySpecimenWithModel(SP_AntimicrobialResistanceSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            //var searchModel = JsonSerializer.Deserialize<MenuSearchDTO>(param);

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstSpecimen {0},{1},{2},{3}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<SP_AntimicrobialResistanceDTO> GetAMRBySpecimenByHospWithModel(SP_AntimicrobialResistanceHospSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            //var searchModel = JsonSerializer.Deserialize<MenuSearchDTO>(param);

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstSpecimen_byHosp {0},{1},{2},{3},{4}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    , searchModel.hos_code
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<SP_AntimicrobialResistanceDTO> GetAMRBySpecimenByProvWithModel(SP_AntimicrobialResistanceProvinceSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstSpecimen_byProv {0},{1},{2},{3},{4}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    , searchModel.prv_code
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<SP_AntimicrobialResistanceDTO> GetAMRBySpecimenByAreaHWithModel(SP_AntimicrobialResistanceAreaHSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstSpecimen_byAreaH {0},{1},{2},{3},{4}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    , searchModel.arh_code
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<SP_AntimicrobialResistanceDTO> GetAMRByWardWithModel(SP_AntimicrobialResistanceSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            //var searchModel = JsonSerializer.Deserialize<MenuSearchDTO>(param);

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstWard {0},{1},{2},{3}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<SP_AntimicrobialResistanceDTO> GetAMRByWardByHospWithModel(SP_AntimicrobialResistanceHospSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstWard_byHosp {0},{1},{2},{3},{4}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    , searchModel.hos_code
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<SP_AntimicrobialResistanceDTO> GetAMRByWardByAreaHWithModel(SP_AntimicrobialResistanceAreaHSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstWard_byAreaH {0},{1},{2},{3},{4}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    , searchModel.arh_code
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<SP_AntimicrobialResistanceDTO> GetAMRByWardByProvWithModel(SP_AntimicrobialResistanceProvinceSearchDTO searchModel)
        {
            log.MethodStart();

            List<SP_AntimicrobialResistanceDTO> objList = new List<SP_AntimicrobialResistanceDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.DropdownAMRListDTOs.FromSqlRaw<SP_AntimicrobialResistanceDTO>("sp_GET_RPAntibicromialResstWard_byProv {0},{1},{2},{3},{4}"
                                                                                                    , searchModel.org_codes
                                                                                                    , searchModel.anti_codes
                                                                                                    , searchModel.start_year
                                                                                                    , searchModel.end_year
                                                                                                    , searchModel.prv_code
                                                                                                    ).ToList();

                    objList = _mapper.Map<List<SP_AntimicrobialResistanceDTO>>(objDataList);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    // TODO: Handle failure
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();

            return objList;
        }
        public List<AntibioticNameDTO> GetAntibioticNames()
        {
            log.MethodStart();

            List<AntibioticNameDTO> objList = new List<AntibioticNameDTO>();

            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                    var objDataList = _db.AntibioticListDTOs.FromSqlRaw<AntibioticNameDTO>("sp_GET_RPAntibioticName"
                                                                                           ).ToList();

                    objList = _mapper.Map<List<AntibioticNameDTO>>(objDataList);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
                finally
                {
                    trans.Dispose();
                }
            }

            log.MethodFinish();
            return objList;
        }
    }
}
