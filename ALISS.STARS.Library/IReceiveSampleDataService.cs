using System;
using System.Collections.Generic;
using ALISS.STARS.DTO;

namespace ALISS.STARS.Library
{
    public interface IReceiveSampleDataService
    {
        #region Mapping

        List<ReceiveSampleListsDTO> GetList();
        List<ReceiveSampleListsDTO> GetStarsResultList(string Param);
        List<ReceiveSampleListsDTO> SaveReceiveSampleData(List<ReceiveSampleListsDTO> models, string format);
        #endregion
    }
}
