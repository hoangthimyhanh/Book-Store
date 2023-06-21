using BanSachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BanSachOnline.Controllers
{
    [HandleError]
    public class GioHangController : Controller
    {
        // GET: GioHang
        QL_BanSachDataContext db = new QL_BanSachDataContext();
        //lay gio hang
        public List<GIOHANG> LayGioHang()
        {
            List<GIOHANG> lstGioHang = Session["GioHang"] as List<GIOHANG>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GIOHANG>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        //them vao gio hang
        public ActionResult ThemGioHang(int ma, int sl, string strURL)
        {
            KHACHHANG kh = Session["KH"] as KHACHHANG;
            if (kh == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung", new { strURL = strURL });
            }
            //lay ra gio hang
            var lstGioHang = db.GIOHANGs.Where(x => x.MAKH == kh.MAKH).ToList();
            //kiem tra sach nay da co trong session "Gio Hang" hay chua ?
            GIOHANG Sach = lstGioHang.SingleOrDefault(t => t.MASACH == ma);
            if (Sach == null)// chua co trong gio hang
            {
                Sach = new GIOHANG();
                Sach.MAKH = kh.MAKH;
                Sach.MASACH = ma;
                Sach.SOLUONG = sl;
                db.GIOHANGs.InsertOnSubmit(Sach);
                db.SubmitChanges();
                return Redirect(strURL);
            }
            else
            {
                Sach.SOLUONG++;
                db.SubmitChanges();
                return Redirect(strURL);
            }
        }

        private int TongSoLuong()
        {
            KHACHHANG kh = Session["KH"] as KHACHHANG;
            if (kh == null)
            {
                return 0;
            }
            var lstGioHang = db.GIOHANGs.Where(x => x.MAKH == kh.MAKH).ToList();
            if (lstGioHang != null)
            {
                return (int)lstGioHang.Sum(x => x.SOLUONG);
            }

            return 0;
        }

        private double TongThanhTien()
        {
            double ttt = 0;
            List<GIOHANG> lstGioHang = Session["GioHang"] as List<GIOHANG>;
            if (lstGioHang != null)
            {
                ttt += lstGioHang.Sum(t => (double)t.TONGTHANHTIEN);
            }
            return ttt;
        }

        public ActionResult GioHang(string strURL)
        {
            KHACHHANG kh = Session["KH"] as KHACHHANG;
            if (kh == null)
            {
                return RedirectToAction("DangNhap", "NguoiDung", new { strURL = strURL });
            }
            var lstGioHang = db.GIOHANGs.Where(x => x.MAKH == kh.MAKH).ToList();
            var lstSach = db.SACHes.ToList();
            ViewBag.lstGioHang = lstGioHang;
            ViewBag.lstSach = lstSach;
            return View();
        }

        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            return PartialView();
        }

        public ActionResult XoaGioHang(int MaSP, int idkh)
        {

            GIOHANG s = db.GIOHANGs.SingleOrDefault(t => t.MASACH == MaSP && t.MAKH == idkh);
            if (s != null)
            {
                db.GIOHANGs.DeleteOnSubmit(s);
                db.SubmitChanges();
                Session["DeleteCart"] = "Xóa thành công.";
                Session["ResultDelete"] = "t";
            }
            else
            {
                Session["DeleteCart"] = "Không tìm thấy!";
                Session["ResultDelete"] = "f";
            }
            return RedirectToAction("GioHang", "GioHang");
        }

        public ActionResult XoaAll(int idkh)
        {
            db.GIOHANGs.DeleteAllOnSubmit(db.GIOHANGs.Where(x => x.MAKH == idkh));
            db.SubmitChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CapNhatGioHang(int MaSP, int idkh, FormCollection f)
        {
            int amount;
            bool kt = int.TryParse(f["amount"], out amount);
            if (kt)
            {
                GIOHANG s = db.GIOHANGs.SingleOrDefault(x => x.MASACH == MaSP && x.MAKH == idkh);
                if (s != null)
                {
                    s.SOLUONG = amount;
                    db.SubmitChanges();
                    Session["UpdateCart"] = "Cập nhật thành công.";
                    Session["ResultUpdate"] = "t";
                }
                else
                {
                    Session["UpdateCart"] = "Không tìm thấy!";
                    Session["ResultUpdate"] = "f";
                }
            }
            else
            {
                Session["UpdateCart"] = "Cập nhật thất bại!";
                Session["ResultUpdate"] = "f";
            }
            return RedirectToAction("GioHang", "GioHang");
        }

        public ActionResult Payment(int idKH)
        {
            List<GIOHANG> carts = Session["ToPay"] as List<GIOHANG>;
            Session["PayList"] = carts;
            Session["Sachs"] = db.SACHes.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Payment(FormCollection fc)
        {
            string addressDetails = fc["address"];
            string name, phone;
            name = fc["name"];
            phone = fc["phone"];
            if (string.IsNullOrEmpty(addressDetails) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || !(new Regex(@"^([0-9]{10,11})$").IsMatch(phone)))
            {
                ViewData["Error"] = "Lỗi";
                return View();
            }
            List<GIOHANG> GHs = Session["PayList"] as List<GIOHANG>;
            int idBill = 0;
            int idKH = GHs.FirstOrDefault().MAKH;
            foreach (var item in GHs)
            {
                if (idBill == 0)
                {
                    try
                    {

                        DateTime date = DateTime.Now;
                        DONHANG Donhang = new DONHANG();
                        Donhang.NGAYMUA = date;
                        Donhang.THANHTIEN = 0;
                        Donhang.MAKH = idKH;
                        Donhang.MALDH = 1;
                        Donhang.TRANGTHAI = "Chưa Thanh Toán";

                        db.DONHANGs.InsertOnSubmit(Donhang);
                        db.SubmitChanges();
                        idBill = db.DONHANGs.Max(x => x.MADH);

                        GIAOHANG giaohang = new GIAOHANG();
                        giaohang.MADH = idBill;
                        giaohang.NGUOINHAN = name;
                        giaohang.SDTNGUOINHAN = phone;
                        giaohang.DIACHIGIAOHANG = addressDetails;
                        giaohang.XACNHANDH = "Chưa Xác Nhận";
                        giaohang.TTGIAOHANG = "Chờ Xác Nhận";
                        DateTime datetime = DateTime.Now;
                        giaohang.THOIGIANDATHANG = datetime;
                        db.GIAOHANGs.InsertOnSubmit(giaohang);
                        db.SubmitChanges();

                    }
                    catch
                    {
                        idBill = 0;
                    }
                }
                if (idBill > 0)
                {
                    try
                    {
                        CHITIETDH ct = new CHITIETDH();
                        ct.MADH = idBill;
                        ct.MASACH = item.MASACH;
                        ct.SOLUONG = item.SOLUONG;
                        db.CHITIETDHs.InsertOnSubmit(ct);
                        db.SubmitChanges();
                        db.GIOHANGs.DeleteOnSubmit(db.GIOHANGs.SingleOrDefault(x => x.MASACH == item.MASACH && item.MAKH == idKH));
                        db.SubmitChanges();
                    }
                    catch
                    {
                        idBill = 0;
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }
        public ActionResult PaymentList(int idKH)
        {
            List<GIOHANG> carts = db.GIOHANGs.Where(x => x.MAKH == idKH).ToList();
            int countRemove = carts.RemoveAll(x => x.SACH.SLTON < x.SOLUONG);
            if (carts.Count <= 0)
                return RedirectToAction("Index", "Home");
            Session["ToPay"] = carts;
            if (countRemove > 0)
                Session["Remove"] = countRemove;
            return RedirectToAction("Payment", "GioHang", new { idKH = idKH });
        }

        public ActionResult DanhSachDonHang(int idKH)
        {

            ViewBag.TB = null;
            List<DONHANG> donhangs = db.DONHANGs.Where(x => x.MAKH == idKH).ToList();
            if (donhangs.Count <= 0)
            {
                ViewBag.TB = "Bạn chưa mua đơn nào !";
                return PartialView();
            }
            return PartialView(donhangs);
        }
        public ActionResult ChiTietDonHangDonHang(int idHoaDon, int idKH)
        {
            DONHANG donhang = db.DONHANGs.FirstOrDefault(x => x.MADH == idHoaDon);
            if (idKH == donhang.MAKH)
            {
                List<CHITIETDH> ctdh = db.CHITIETDHs.Where(x => x.MADH == idHoaDon).ToList();
                GIAOHANG giaohangs = db.GIAOHANGs.FirstOrDefault(x => x.MADH == idHoaDon);

                ViewBag.CTDH = ctdh;
                ViewBag.GHang = giaohangs;
                ViewBag.DonHang = donhang;
                return PartialView();
            }
            return RedirectToAction("PageNotFound", "Error");
        }
    }
}