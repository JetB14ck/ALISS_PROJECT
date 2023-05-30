using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ALISS.Master.DTO;
using ALISS.Master.Library;
using ALISS.Master.Library.DataAccess;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ALISS.Master.Api.Controllers
{
    //[Route("api/[controller]")]
    public class TRNoticeMessageController : Controller
    {
        private readonly ITRNoticeMessageService _service;

        public TRNoticeMessageController(MasterContext db, IMapper mapper)
        {
            _service = new TRNoticeMessageService(db, mapper);
        }

        [HttpGet]
        [Route("api/TRNoticeMessage/GetList/{username}")]
        public IEnumerable<TRNoticeMessageDTO> Get_List(string username)
        {
            var objReturn = _service.GetList(username);

            return objReturn;
        }

        [HttpPost]
        [Route("api/TRNoticeMessage/Get_ListByModel/")]
        public IEnumerable<TRNoticeMessageDTO> Get_ListByModel([FromBody]TRNoticeMessageDTO searchModel)
        {
            var objReturn = _service.GetListWithModel(searchModel);

            return objReturn;
        }

        [HttpPost]
        [Route("api/TRNoticeMessage/Get_ListByModelByRole/")]
        public IEnumerable<TRNoticeMessageDTO> Get_ListByModelByRole([FromBody]TRNoticeMessageDTO searchModel)
        {
            var objReturn = _service.GetListWithModelByRole(searchModel);

            return objReturn;
        }
    }
}
