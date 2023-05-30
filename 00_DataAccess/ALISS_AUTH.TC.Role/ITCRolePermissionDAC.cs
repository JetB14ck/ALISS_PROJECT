using ALISS_AUTH.TC.Role.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS_AUTH.TC.Role
{
    public interface ITCRolePermissionDAC
    {
        void Insert(TCRolePermission model);
        void Update(TCRolePermission model);
        void Inactive(TCRolePermission model);
        TCRolePermission GetData(TCRolePermission searchModel);
        List<TCRolePermission> GetList(TCRolePermission searchModel);
    }
}
