using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Web.UI.WebControls;
using System.Diagnostics;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.BacSi.Controllers
{
    public class QLBNController : Controller
    {
        // GET: BacSi/QLBN
        csdl_HTQLEntities db = new csdl_HTQLEntities();
        [CustomAuthorize]
        public ActionResult ds_benhnhan(string searchString, int? page)
        {
            var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];
            int maTK_BacSi = taiKhoan.Ma_TK;

            // Lấy danh sách Hộ gia đình mà bác sĩ được phân công chăm sóc
            var listHGD = db.PHAN_CONG_CHAM_SOC
                            .Where(p => p.Ma_TK == maTK_BacSi)
                            .Select(p => p.Ma_HGD)
                            .ToList();

            // Lấy danh sách bệnh nhân thuộc các Hộ gia đình đã lấy được ở trên
            var listBenhNhan = db.BENH_NHAN
                                .Where(bn => listHGD.Contains(bn.Ma_HGD));

            if (!string.IsNullOrEmpty(searchString))
            {
                listBenhNhan = listBenhNhan.Where(bn => bn.Ho_Ten.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;
            listBenhNhan = listBenhNhan.OrderBy(s => s.Ho_Ten);
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(listBenhNhan.ToPagedList(pageNumber, pageSize));
        }
        [CustomAuthorize]
        public ActionResult chitietBN(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var viewModel = db.BENH_NHAN.Include(s => s.SO_THEO_DOI_SUC_KHOE).SingleOrDefault(s => s.Ma_TK == id);
            if (viewModel == null)
                if (viewModel == null)
                {
                    return HttpNotFound();
                }
            return View(viewModel);
        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult Them_So_Theo_Doi_SK(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BENH_NHAN benhNhan = db.BENH_NHAN.Find(id);
            if (benhNhan == null)
            {
                return HttpNotFound();
            }

            ViewBag.Ma_TK = id;

            var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];
            if (taiKhoan != null)
            {
                ViewBag.BAC_Ma_TK = taiKhoan.Ma_TK;
            }

            return View();
        }

        // POST: BenhNhan/Create
        [CustomAuthorize]
        [HttpPost, ActionName("Them_So_Theo_Doi_SK")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Them_So_Theo_Doi_SK(SO_THEO_DOI_SUC_KHOE soTheoDoi)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    // Lấy thông tin tài khoản được đăng nhập
                    var taiKhoan = (TAI_KHOAN)Session["TaiKhoanDangNhap"];

                    // Gán giá trị cho trường BAC_Ma_TK từ session
                    soTheoDoi.BAC_Ma_TK = taiKhoan.Ma_TK;

                    // Tạo một thời điểm mới với ngày giờ hiện tại
                    THOI_DIEM thoiDiem = new THOI_DIEM
                    {
                        Ngay_Gio = DateTime.Today
                    };

                    // Thêm thời điểm mới vào cơ sở dữ liệu
                    db.THOI_DIEM.Add(thoiDiem);
                    

                    // Gán ngày giờ mới cho sổ theo dõi sức khỏe
                    soTheoDoi.Ngay_Gio = thoiDiem.Ngay_Gio;
                    //Debug.WriteLine("Mã BN: " + soTheoDoi.Ma_TK);
                    //Debug.WriteLine("Mã BS: " + soTheoDoi.BAC_Ma_TK);
                    //Debug.WriteLine("Ngay gio std: " + soTheoDoi.Ngay_Gio);
                    //Debug.WriteLine("Ngay gio td: " + thoiDiem.Ngay_Gio);
                    //Debug.WriteLine("các tssk: " + soTheoDoi.Cac_Thong_So_Suc_Khoe);
                    //Debug.WriteLine("chỉ định dd: " + soTheoDoi.Chi_Dinh_Dinh_Duong);
                    //Debug.WriteLine("ghi chu: " + soTheoDoi.Ghi_Chu);
                    //Debug.WriteLine("don thuoc: " + soTheoDoi.Don_Thuoc);
                    //Debug.WriteLine("STT: " + soTheoDoi.So_Thu_Tu);


                    // Thêm sổ theo dõi sức khỏe mới vào cơ sở dữ liệu
                    db.SO_THEO_DOI_SUC_KHOE.Add(soTheoDoi);
                    db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

                    return RedirectToAction("chitietBN", "QLBN", new {id=soTheoDoi.Ma_TK}); // Chuyển hướng về danh sách bệnh nhân sau khi thêm thành công
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Loi luu du lieu");
            }
            return View(soTheoDoi);
        }
    }
}