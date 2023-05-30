using ALISS.MasterManagement.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS.MasterManagement.Library
{
    public interface IProcessExcelService
    {
        List<TCProcessExcelColumnDTO> Get_ProcessExcelColumn(TCProcessExcelColumnDTO searchModel);
        List<TCProcessExcelRowDTO> Get_ProcessExcelRow(TCProcessExcelRowDTO searchModel);
        List<TCProcessExcelTemplateDTO> Get_ProcessExcelTemplate(TCProcessExcelTemplateDTO searchModel);
        TCProcessExcelColumnDTO SaveProcessExcelColumnData(TCProcessExcelColumnDTO model);
        TCProcessExcelRowDTO SaveProcessExcelRowData(TCProcessExcelRowDTO model);
        TCProcessExcelTemplateDTO SaveProcessExcelTemplateData(TCProcessExcelTemplateDTO model);
        TCProcessExcelColumnDTO SaveProcessExcelColumnListData(List<TCProcessExcelColumnDTO> models);
        TCProcessExcelRowDTO SaveProcessExcelRowListData(List<TCProcessExcelRowDTO> models);
        TCProcessExcelTemplateDTO SaveProcessExcelTemplateListData(List<TCProcessExcelTemplateDTO> models);
    }
}