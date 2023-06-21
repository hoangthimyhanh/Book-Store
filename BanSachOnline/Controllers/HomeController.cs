using BanSachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSachOnline.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        QL_BanSachDataContext db = new QL_BanSachDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Baner()
        {
            return View();
        }
        public ActionResult NhaSach_Footer()
        {
            return View();
        }
        public ActionResult Show4()
        {
            var lstSach = db.SACHes.OrderBy(t => t.TENSACH).Take(6).ToList();


            return View(lstSach);
        }
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
        public ActionResult API_WEB()
        {
            return View();
        }
    }
}