using ALISS_AUTH.TC.UserLogin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS_AUTH.TC.UserLogin
{
    public interface ITCUserLoginPermissionDAC
    {
        void Insert(TCUserLoginPermission model);
        void Update(TCUserLoginPermission model);
        void Inactive(TCUserLoginPermission model);
        TCUserLoginPermission GetData(TCUserLoginPermission searchModel);
        List<TCUserLoginPermission> GetList(TCUserLoginPermission searchModel);
    }
}
