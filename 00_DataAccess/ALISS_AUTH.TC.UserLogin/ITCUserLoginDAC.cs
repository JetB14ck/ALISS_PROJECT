using ALISS_AUTH.TC.UserLogin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALISS_AUTH.TC.UserLogin
{
    public interface ITCUserLoginDAC
    {
        void Insert(TCUserLogin model);
        void Update(TCUserLogin model);
        void Inactive(TCUserLogin model);
        TCUserLogin GetData(TCUserLogin searchModel);
        List<TCUserLogin> GetList(TCUserLogin searchModel);
    }
}
