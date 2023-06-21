using BanSachOnline.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace BanSachOnline.Controllers
{
    [HandleError]
    public class AdminController : Controller
    {
        // GET: Admin
        QL_BanSachDataContext db = new QL_BanSachDataContext();
        public ActionResult DangNhap()
        {
            return View();
        }
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("DangNhap");
        }
        public ActionResult IndexAD()
        {
            return View();
        }
        public ActionResult TimKiemSachAD(string txt_search)
        {
            if (txt_search != null)
            {
                var lisTimKiem = (from tk in db.SACHes where tk.TENSACH.Contains(txt_search) select tk).ToList();
                if (lisTimKiem.Count == 0)
                {
                    ViewBag.kqTK = string.Format("Không Tìm Thấy {0} !!!", txt_search);
                }
                ViewBag.TuKHoa = txt_search;
                return PartialView(lisTimKiem);
            }

            return PartialView();
        }
        public ActionResult ShowSach(int? page)
        {
            if (page == null) page = 1;
            var lstSach = db.SACHes.ToList();
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return PartialView(lstSach.ToPagedList(pageNumber, pageSize));

        }

        // Chi Tiết chihr sửa thêm xóa Sản phẩm
        public ActionResult DeleteBook(int ma)
        {
            SACH Book = db.SACHes.Single(s => s.MASACH == ma);
            var CTHD = db.CHITIETDHs.Where(hanghoa => hanghoa.MASACH == ma).Count();
            var GH = db.GIOHANGs.Where(hanghoa => hanghoa.MASACH == ma).Count();

            if (Book == null)
            {
                return HttpNotFound();
            }
            ViewBag.kq = "";
            if (CTHD != 0 || GH != 0)
            {
                ViewBag.TB = "SÁCH CÓ DỮ LIỆU BÁN HÀNG, KHÔNG THỂ XÓA !!!";
                ViewBag.kq = "1";
                //return HttpNotFound();
            }
            return PartialView(Book);
        }
        [HttpPost, ActionName("DeleteBook")]
        public ActionResult DeleteBookTT(int ma)
        {
            SACH Book = db.SACHes.SingleOrDefault(s => s.MASACH == ma);
            if (Book == null)
            {
                return HttpNotFound();
            }
            db.SACHes.DeleteOnSubmit(Book);
            db.SubmitChanges();
            return RedirectToAction("ShowSach");
        }

        public ActionResult CreateSach()
        {
            ViewBag.MALOAI = new SelectList(db.THELOAIs, "MATL", "TENTL");
            ViewBag.MANXB = new SelectList(db.NHAXUATBANs, "MANXB", "TENNXB");
            ViewBag.MATG = new SelectList(db.TACGIAs, "MATG", "TENTG");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateSach(SACH sach, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid)
            {

                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    var _FileName = Path.GetFileName(uploadFile.FileName);
                    var _path = Path.Combine(Server.MapPath("~/DULIEU/Images/"), _FileName);

                    uploadFile.SaveAs(_path);
                    sach.HINHANH = _FileName;

                }
                db.SACHes.InsertOnSubmit(sach);
                db.SubmitChanges();
                return RedirectToAction("ShowSach");

            }
            ViewBag.Message = "không thành công!!";
            return View(sach);
        }

        public ActionResult EditSach(int? ma)
        {

            var Sach = db.SACHes.FirstOrDefault(s => s.MASACH == ma);
            if (ma == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Sach == null)
            {
                ViewBag.SEdit = "Không có Sách!";
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.MALOAI = new SelectList(db.THELOAIs, "MATL", "TENLOAI", Sach.MATL);
            ViewBag.MANXB = new SelectList(db.NHAXUATBANs, "MANXB", "TENNXB", Sach.MANXB);
            ViewBag.MATG = new SelectList(db.TACGIAs, "MATG", "TENTG", Sach.MATG);
            return View(Sach);
        }
        [HttpPost, ActionName("EditSach")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditSach(SACH sach, HttpPostedFileBase uploadFile, FormCollection form, int? ma)
        {
            var S = db.SACHes.FirstOrDefault(x => x.MASACH == sach.MASACH);
            ViewBag.MALOAI = new SelectList(db.THELOAIs, "MATL", "TENLOAI", sach.MATL);
            ViewBag.MANXB = new SelectList(db.NHAXUATBANs, "MANXB", "TENNXB", sach.MANXB);
            ViewBag.MATG = new SelectList(db.TACGIAs, "MATG", "TENTG", sach.MATG);
            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadFile != null && uploadFile.ContentLength > 0)
                    {
                        var _FileName = Path.GetFileName(uploadFile.FileName);
                        var _path = Path.Combine(Server.MapPath("~/DULIEU/Images"), _FileName);
                        uploadFile.SaveAs(_path);
                        sach.HINHANH = _FileName;
                        S.HINHANH = sach.HINHANH;
                    }
                    S.TENSACH = sach.TENSACH;
                    S.MANXB = sach.MANXB;
                    S.MOTA = sach.MOTA;
                    S.DONGIABAN = sach.DONGIABAN;
                    S.MATG = sach.MATG;
                    S.MATL = sach.MATL;
                    S.SLTON = sach.SLTON;
                    S.HINHANH = S.HINHANH;
                    db.SubmitChanges();
                    return RedirectToAction("ShowSach");
                }
                catch
                {
                    ViewBag.Message = "Không thành công!!";
                }
            }
            return View(sach);
        }
        public ActionResult DetailsSach(int? ma)
        {

            var Sach = db.SACHes.FirstOrDefault(s => s.MASACH == ma);
            if (ma == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (Sach == null)
            {
                ViewBag.SEdit = "Không có Sách!";
            }
            ViewBag.MALOAI = new SelectList(db.THELOAIs, "MATL", "TENLOAI", Sach.MATL);
            ViewBag.MANXB = new SelectList(db.NHAXUATBANs, "MANXB", "TENNXB", Sach.MANXB);
            ViewBag.MATG = new SelectList(db.TACGIAs, "MATG", "TENTG", Sach.MATG);
            return View(Sach);
        }

        //-------------------------------------------
        public ActionResult ShowTG()
        {
            var tg = db.TACGIAs.ToList();
            return View(tg);
        }
        public ActionResult CreateTG()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateTG(TACGIA TG)
        {
            if (ModelState.IsValid)
            {


                db.TACGIAs.InsertOnSubmit(TG);
                db.SubmitChanges();
                return RedirectToAction("ShowSach");

            }
            ViewBag.Message = "không thành công!!";
            return View(TG);
        }
        //public ActionResult DeleteTG(int id)
        //{
        //    TACGIA Book = db.TACGIAs.Single(s => s.MATG == id);
        //    var CTHD = db.SACHes.SingleOrDefault(tac => tac.MATG == id);

        //    if (Book == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    if( CTHD != null )
        //    {
        //        ViewBag.TB = "TÁC GIẢ CÓ DỮ LIỆU SÁCH, KHÔNG THỂ XÓA !!!";
        //        ViewBag.kq = "1";
        //    }    
        //    return PartialView(Book);
        //}
        //[HttpPost, ActionName("DeleteTG")]
        //public ActionResult DeleteTGTT(int ma)
        //{
        //    TACGIA Book = db.TACGIAs.SingleOrDefault(s => s.MATG == ma);
        //    if (Book == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    db.TACGIAs.DeleteOnSubmit(Book);
        //    db.SubmitChanges();
        //    return RedirectToAction("ShowTG");
        //}
        //////-----------------------------

        // Xác nhận đơn hàng
        public ActionResult ShowListDonHang()
        {
            List<DONHANG> donh = db.DONHANGs.ToList();

            if (donh.Count > 0)
            {
                return View(donh);
            }
            ViewBag.ThonBao = "Không có đơn hàng nào!";
            return View();
        }

        public ActionResult ShowListDonHangChuaDuyet()
        {
            ViewBag.ThonBao= null;
            List<GIAOHANG> giaohang = db.GIAOHANGs.Where(x => x.XACNHANDH== "Chưa Xác Nhận").ToList();
    

            if (giaohang.Count > 0)
            {
                ViewBag.DHang = giaohang;
                return View(giaohang);
            }
            ViewBag.ThonBao = "Không có đơn hàng nào chờ duyệt!";
            return View();
        }
        public ActionResult ShowChiTietDonHang(int idHoaDon)
        {
            DONHANG donhang = db.DONHANGs.FirstOrDefault(x => x.MADH == idHoaDon);

            List<CHITIETDH> ctdh = db.CHITIETDHs.Where(x => x.MADH == idHoaDon).ToList();
            GIAOHANG giaohangs = db.GIAOHANGs.FirstOrDefault(x => x.MADH == idHoaDon);

            ViewBag.CTDH = ctdh;
            ViewBag.GHang = giaohangs;
            ViewBag.DonHang = donhang;
            return PartialView();
        }
        public ActionResult UpdateDonhangOK(int idHoaDon)
        {
            DONHANG donhang = db.DONHANGs.FirstOrDefault(x => x.MADH == idHoaDon);

          
            GIAOHANG giaohangs = db.GIAOHANGs.FirstOrDefault(x => x.MADH == idHoaDon);
            giaohangs.TTGIAOHANG = "Đã Được Xác Nhận";
            giaohangs.XACNHANDH = "Đã Xác Nhận";
            db.SubmitChanges();

            return RedirectToAction("ShowListDonHangChuaDuyet");
        }
        public ActionResult ShowListDonHangDaDuyet()
        {
            ViewBag.ThonBao = null;
            List<GIAOHANG> giaohang = db.GIAOHANGs.Where(x => x.XACNHANDH == "Đã Xác Nhận").ToList();


            if (giaohang.Count > 0)
            {
                ViewBag.DHang = giaohang;
                return View(giaohang);
            }
            ViewBag.ThonBao = "Không có đơn hàng nào đã duyệt!";
            return View();
        }
        public ActionResult ShowListDonHangDaHuy()
        {
            ViewBag.ThonBao = null;
            List<GIAOHANG> giaohang = db.GIAOHANGs.Where(x => x.XACNHANDH == "Huỷ Đơn Hàng").ToList();


            if (giaohang.Count > 0)
            {
                ViewBag.DHang = giaohang;
                return View(giaohang);
            }
            ViewBag.ThonBao = "Không có đơn hàng nào huỷ!";
            return View();
        }
        public ActionResult ShowListDonHangOnline()
        {
            ViewBag.ThonBao = null;
            List<GIAOHANG> giaohang = db.GIAOHANGs.ToList();


            if (giaohang.Count > 0)
            {
                ViewBag.DHang = giaohang;
                return View(giaohang);
            }
            ViewBag.ThonBao = "Không có đơn hàng nào !";
            return View();
        }
        public ActionResult UpdateDonhangHuy(int idHoaDon)
        {
            DONHANG donhang = db.DONHANGs.FirstOrDefault(x => x.MADH == idHoaDon);


            GIAOHANG giaohangs = db.GIAOHANGs.FirstOrDefault(x => x.MADH == idHoaDon);
            giaohangs.TTGIAOHANG = "Đã Bị Huỷ Đơn Hàng";
            giaohangs.XACNHANDH = "Huỷ Đơn Hàng";
            db.SubmitChanges();

            return RedirectToAction("ShowListDonHangChuaDuyet");
        }
    }

}