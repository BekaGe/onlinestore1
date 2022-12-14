using onlineStore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;


namespace onlineStore.Controllers
{

    public class AdminController : Controller
    {
        onlinemarketingEntities1 db = new onlinemarketingEntities1();

        [HttpGet]
        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(adminn adm)
        {
            adminn ad = db.adminn.Where(x => x.ad_name == adm.ad_name && x.ad_password == adm.ad_password).SingleOrDefault();
            if (ad != null)
            {
                Session["ad_id"] = ad.ad_id.ToString();
                return RedirectToAction("Category");
            }
            else
            {
                ViewBag.error = "Invalid User Name or Password";
            }

            return View();
        }
        public ActionResult Category()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");

            }

            return View();
        }

        [HttpPost]

        public ActionResult Category(category cat, HttpPostedFileBase imgfile)
        {
            adminn ad = new adminn();
            string path = uploadimage(imgfile);

            if (path.Equals("-1"))
            {
                ViewBag.error = "image could not be uploaded";

            }
            else
            {
                category ca = new category();
                ca.cat_name = cat.cat_name;
                ca.cat_image = path;
                ca.cat_status = 1;
                ca.ad_id_fk = Convert.ToInt32(Session["ad_id"].ToString());

                db.category.Add(ca);

                db.SaveChanges();


                return RedirectToAction("ViewCategory");
            }


            return View();

        }
        public ActionResult ViewCategory(int ? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.category.OrderByDescending(x => x.cat_id).ToList();
            IPagedList<category> cate = list.ToPagedList(pageindex, pagesize);

            return View(cate);
        }

        public ActionResult CreateAdd()

        {
            return View();

        }
        //public ActionResult remove(int? id)
        //{
        //    List<category> li2 = TempData["category"] as List<category>;
        //    category c = li2.Where(x => x.cat_id == id).SingleOrDefault();
        //    li2.Remove(c);
        //    int h = 0;
           
        //    TempData["total"] = h;




        //    return RedirectToAction("category");

        //}
        public string uploadimage(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if( extension.ToLower().Equals(".jpg")|| extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/Upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName); 
                        //   ViewBag.Message = "File uploaded successfuly";

                    }
                     catch( Exception ex)
                    {
                        path = "-1";

                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");

                }
            }
                 else

                {

                    Response.Write("<script>alert('Please select a file'); </script>");

                    path = "-1";

                }
                return path;
            }
        }
}
