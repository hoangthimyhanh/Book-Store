using BanSachOnline.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSachOnline.Controllers
{
    [HandleError]
    public class SachController : Controller
    {
        // GET: Sach
        QL_BanSachDataContext db = new QL_BanSachDataContext();
        public ActionResult ShowAll(int? page)
        {
            if (page == null) page = 1;
            var lstSach = db.SACHes.ToList();
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(lstSach.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ChiTietSach(int ma)
        {
            SACH s = db.SACHes.SingleOrDefault(t => t.MASACH == ma);
            if (s == null)
            {
                return HttpNotFound();
            }
            return PartialView(s);
        }
        public ActionResult LoaiSach()
        {
            var lst = db.THELOAIs.OrderBy(t => t.TENTL);
            return PartialView(lst);

        }
        public ActionResult SachTheoCD(int MaLoai)
        {
            var listSach = db.SACHes.Where(s => s.MATL == MaLoai).ToList();

            if (listSach.Count == 0)
            {
                ViewBag.TB = "Không có sách thuộc chủ  đề này!";
            }
            else
            {
                string ten = db.THELOAIs.Single(t => t.MATL == MaLoai).TENTL;
                ViewBag.TenTheLoai = ten;
            }
            return PartialView(listSach);
        }

        public ActionResult TimKiemSach(string txt_search)
        {
            var lisTimKiem = (from tk in db.SACHes where tk.TENSACH.Contains(txt_search) select tk).ToList();
            if (lisTimKiem.Count == 0)
            {
                ViewBag.kqTK = string.Format("Không Tìm Thấy {0} !!!", txt_search);
            }
            ViewBag.TuKHoa = txt_search;
            return PartialView(lisTimKiem);
        }
        public ActionResult SortSach_Gia()
        {
            var lisTimKiem = db.SACHes.OrderBy(s => s.DONGIABAN).ToList();
            return PartialView(lisTimKiem);
        }
        ///// -------------------------------------------------------------------------------------------------

    }
}