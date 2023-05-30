using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ALISS.LabFileUpload.DTO;

namespace ALISS.LabFileUpload.Library
{
    public interface ILabFileUploadService
    {
        LabFileUploadDataDTO SaveLabFileUploadData(LabFileUploadDataDTO model);

        LabFileUploadDataDTO GetLabFileUploadDataById(string lfu_id);

        List<LabFileUploadDataDTO> GetLabFileUploadListWithModel(LabFileUploadSearchDTO model);

        List<LabFileErrorHeaderListDTO> GetLabFileErrorHeaderListBylfuId(string lfu_id);
        List<LabFileErrorDetailListDTO> GetLabFileErrorDetailListBylfuId(string lfu_id);
        List<LabFileSummaryHeaderListDTO> GetLabFileSummaryHeaderBylfuId(string lfu_id);
        List<LabFileSummaryDetailListDTO> GetLabFileSummaryDetailBylfuId(string fsh_id);
        List<LabFileLabAlertSummaryListDTO> GetLabFileLabAlertSummaryBylfuId(string lfu_id);
        List<LabFileSummaryDetailListDTO> GetLabFileSummaryDetailListBylfuId(string lfu_Id);
        List<LabFileExportMappingErrorDTO> GetLabFileExportMappingError(string[] lfu_ids);
        List<LabFileUploadDataDTO> GenerateBoxNo(List<LabFileUploadDataDTO> list, string format);

    }
}
