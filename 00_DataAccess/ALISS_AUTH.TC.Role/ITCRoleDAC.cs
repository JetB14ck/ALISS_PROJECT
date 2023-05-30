using ALISS_AUTH.TC.Role.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS_AUTH.TC.Role
{
    public interface ITCRoleDAC
    {
        void Insert(TCRole model);
        void Update(TCRole model);
        void Inactive(TCRole model);
        TCRole GetData(TCRole searchModel);
        List<TCRole> GetList(TCRole searchModel);
    }
}
