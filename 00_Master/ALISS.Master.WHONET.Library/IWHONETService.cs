using ALISS.Master.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.Master.NoticeMessage.Library
{
    public interface IWHONETService
    {
        List<TCWHONET_AntibioticsDTO> Get_TCWHONET_Antibiotics_Acitve_List();
        List<TCWHONET_AntibioticsDTO> Get_TCWHONET_Antibiotic_List(TCWHONET_AntibioticsDTO searchModel);
        List<TCWHONET_AntibioticSpeciesDTO> Get_TCWHONET_AntibioticSpecies_List(TCWHONET_AntibioticSpeciesDTO searchModel);
        List<TCWHONET_DepartmentsDTO> Get_TCWHONET_Departments_List(TCWHONET_DepartmentsDTO searchModel);
        List<TCWHONET_SpecimenDTO> Get_TCWHONET_Specimen_List(TCWHONET_SpecimenDTO searchModel);
        List<TCWHONETColumnDTO> Get_TCWHONET_Column_List(TCWHONETColumnDTO searchModel);
        List<TCWHONET_OrganismDTO> Get_TCWHONET_Organism_List(TCWHONET_OrganismDTO searchModel);
        List<TCWHONET_MacroDTO> Get_TCWHONET_Macro_List(TCWHONET_MacroDTO searchModel);
        TCWHONET_AntibioticsDTO SaveWHONET_AntibioticsListData(List<TCWHONET_AntibioticsDTO> models);
        TCWHONET_AntibioticSpeciesDTO SaveWHONET_AntibioticSpeciesListData(List<TCWHONET_AntibioticSpeciesDTO> models);
        TCWHONET_DepartmentsDTO SaveWHONET_DepartmentsListData(List<TCWHONET_DepartmentsDTO> models);
        TCAntibioticDTO SaveAntibioticListData(List<TCAntibioticDTO> models);
        TCWHONET_SpecimenDTO SaveWHONET_SpecimenListData(List<TCWHONET_SpecimenDTO> models);
        TCWHONETColumnDTO SaveWHONET_ColumnListData(List<TCWHONETColumnDTO> models);
        TCWHONET_OrganismDTO SaveWHONET_OrganismListData(List<TCWHONET_OrganismDTO> models);
        TCWHONET_MacroDTO SaveWHONET_MacroListData(List<TCWHONET_MacroDTO> models);
        TCWHONET_MacroDTO DeleteWHONET_MacroListData(List<TCWHONET_MacroDTO> objModels);
        TCWHONET_OrganismDTO DeleteWHONET_OrganismListData(List<TCWHONET_OrganismDTO> objModels);
        TCWHONETColumnDTO DeleteWHONET_ColumnListData(List<TCWHONETColumnDTO> objModels);
        TCAntibioticDTO DeleteAntibioticListData(List<TCAntibioticDTO> objModels);
        TCWHONET_SpecimenDTO DeleteWHONET_SpecimenListData(List<TCWHONET_SpecimenDTO> objModels);
        TCWHONET_DepartmentsDTO DeleteWHONET_DepartmentsListData(List<TCWHONET_DepartmentsDTO> objModels);
        TCWHONET_AntibioticSpeciesDTO DeleteWHONET_AntibioticSpeciesListData(List<TCWHONET_AntibioticSpeciesDTO> objModels);
        TCWHONET_AntibioticsDTO DeleteWHONET_AntibioticsListData(List<TCWHONET_AntibioticsDTO> objModels);
    }
}
