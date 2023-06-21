using BanSachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanSachOnline.Controllers
{
    [HandleError]
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        QL_BanSachDataContext db = new QL_BanSachDataContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult DangKy()
        {

            return View();
        }
        [HttpPost]
        public ActionResult DangKy(KHACHHANG kh, ACCOUNT ac, FormCollection f)
        {
            //gan cac gia tri nguoi dung tu form f cho cac bien
            var hoten = f["HotenKH"];
            var tendn = f["TenDN"];
            var matkhau = f["MatKhau"];
            var dienthoai = f["Dienthoai"];
            var rematkhau = f["ReMatkhau"];
            var email = f["Email"];
            var soCMND = f["CMND"];
            if (string.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên không được bỏ trống!";
            }
            if (string.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Tên đăng nhập không được bỏ trống!";
            }
            if (string.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Vui lòng nhập lại mật khẩu!";
            }
            if (string.IsNullOrEmpty(rematkhau))
            {
                ViewData["Loi4"] = "Vui lòng nhập lại mật khẩu!";
            }
            if (string.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi5"] = "Vui lòng nhập số điện thoại(Yêu cầu gồm 10 số)!";
            }
            if (string.IsNullOrEmpty(soCMND))
            {
                ViewData["Loi6"] = "Vui lòng nhập số CMND/CCCD(Yêu cầu gồm 12 số)!";
            }
            if (string.IsNullOrEmpty(email))
            {
                ViewData["Loi8"] = "Vui lòng nhập email!";
            }
            if (!string.IsNullOrEmpty(hoten) && !string.IsNullOrEmpty(tendn) && !string.IsNullOrEmpty(matkhau) && !string.IsNullOrEmpty(rematkhau) && !string.IsNullOrEmpty(dienthoai))
            {
                //gan gia tri cho doi tuong kh
                kh.TENKH = hoten;
                kh.USERNAME = tendn;
                ac.USERNAME = tendn;
                ac.PASSWORD = matkhau;
                kh.SODIENTHOAI = dienthoai;
                kh.SOCCCD = soCMND;
                kh.EMAIL = email;
                //ghi du lieu xuong csdl
                var KQ = db.ACCOUNTs.SingleOrDefault(t => t.USERNAME == kh.USERNAME);
                var KQ1 = db.KHACHHANGs.SingleOrDefault(t => t.EMAIL == kh.EMAIL);
                var KQ2 = db.KHACHHANGs.SingleOrDefault(t => t.SODIENTHOAI == kh.SODIENTHOAI);
                var KQ3 = db.KHACHHANGs.SingleOrDefault(t => t.SOCCCD == kh.SOCCCD);
                if (KQ != null || KQ1 != null || KQ2 != null || KQ3 != null)
                {

                    if (KQ != null) { ViewData["Loi2"] = "Tên đăng nhập đã tồn tại vui lòng nhập tên khác!"; }
                    if (KQ1 != null) { ViewData["Loi8"] = "Mail đã có!"; }
                    if (KQ2 != null) { ViewData["Loi5"] = "Số điện thoại đã có!"; }
                    if (KQ3 != null) { ViewData["Loi6"] = "CMND/CCCD  đã có!"; }
                    return View();

                }
                else
                {
                    if (dienthoai.Length == 10 && soCMND.Length == 12 && soCMND != null && dienthoai != null && tendn != null && matkhau != null && rematkhau != null && email != null)
                    {
                        db.ACCOUNTs.InsertOnSubmit(ac);
                        db.KHACHHANGs.InsertOnSubmit(kh);

                        db.SubmitChanges();
                        //ViewBag.TB = "Đăng ký thành công!";
                        return RedirectToAction("DangNhap", "NguoiDung");
                    }
                    else
                    {
                        ViewData["Loi2"] = "Tên đăng nhập đã tồn tại vui lòng nhập tên khác!";
                        ViewData["Loi5"] = "Yêu cầu nhập lại số điện thoại gồm 10 số!";
                        ViewData["Loi6"] = "Yêu cầu nhập lại số CMND/CCCD vào gồm 12 số!";
                        ViewData["Loi5"] = "Yêu cầu nhập lại email!";
                        return View();
                    }
                }
            }
            return View();
        }

        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            //khai bao cac bien nhan gia tri tu form f
            var tendn = f["TenDN"];
            var matkhau = f["MatKhau"];

            if (string.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Tên đăng nhập không được bỏ trống!";
            }
            if (string.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Mật khẩu không được bỏ trống!";
            }

            if (!string.IsNullOrEmpty(tendn) && !string.IsNullOrEmpty(matkhau))
            {
                ACCOUNT ac = db.ACCOUNTs.SingleOrDefault(t => t.USERNAME == tendn && t.PASSWORD == matkhau);
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(t => t.USERNAME == tendn);
                NHANVIEN nv = db.NHANVIENs.SingleOrDefault(t => t.USERNAME == tendn);
                if (ac != null)
                {
                    if (nv != null && ac.USERNAME == nv.USERNAME)
                    {
                        ViewBag.TB = "Đăng nhập thành công!";
                        Session["NV"] = nv;
                        return RedirectToAction("DangNhap", "Admin");
                    }

                    if (kh != null && ac.USERNAME == kh.USERNAME)
                    {
                        ViewBag.TB = "Đăng nhập thành công!";
                        Session["KH"] = kh;
                        return RedirectToAction("Index", "Home");
                    }
                    //return RedirectToAction("GioHang", "GioHang");
                }
                else
                {
                    ViewData["Loi1"] = "Tên đăng nhập hoặc mật khẩu sai vui lòng nhập lại!";
                }
            }
            return View();
        }
    }
}