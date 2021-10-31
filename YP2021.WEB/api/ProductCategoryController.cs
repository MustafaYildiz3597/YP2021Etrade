using Nero.BLL.Repository;
using Nero.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Nero.Web.api
{
    public class ProductCategoryController : ApiController
    {
        private BPClubDBEntities db = new BPClubDBEntities();

        //[HttpPost]
        //public IHttpActionResult SectionList()
        //{
        //    try
        //    {
        //        var list = new ProductCategoryRepo().CategoryListByParentID(0);

        //        return Ok(list);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("İşlem hatası." + ex.Message);
        //    }
        //}

        [HttpPost]
        public IHttpActionResult ListByParent(CategoryListBySectionModel model)
        {
            try
            {
                var list = new ProductCategoryRepo().CategoryListByParentID(model.SectionID);

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        public class CategoryListBySectionModel
        {
            public int SectionID { get; set; }
        }

    }
}
