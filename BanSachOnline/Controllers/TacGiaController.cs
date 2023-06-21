using BanSachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSachOnline.Controllers
{
    [HandleError]
    public class TacGiaController : Controller
    {
        QL_BanSachDataContext db = new QL_BanSachDataContext();
        // GET: TacGia
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TacGia()
        {
            var lstTacGia = db.TACGIAs.ToList();
            return View(lstTacGia);
        }

        public ActionResult SachTheoTacGia(int MaTG)
        {
            List<SACH> listSachTheoTacGia = db.SACHes.Where(t => t.MATG == MaTG).ToList();
            if (listSachTheoTacGia.Count == 0)
            {
                ViewBag.TB = "Không có sách nào thuộc tác giả này !";
            }
            else
            {
                string ten = db.TACGIAs.Single(t => t.MATG == MaTG).TENTG;
                ViewBag.TenTacGia = ten;
            }
            return View(listSachTheoTacGia);
        }

        public ActionResult ShowTacGia()
        {
            var lstTacGia = db.TACGIAs.ToList();
            return View(lstTacGia);
        }
    }
}