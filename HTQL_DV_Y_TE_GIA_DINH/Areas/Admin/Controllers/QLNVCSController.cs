using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using System.Web.UI.WebControls;
using PagedList;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Controllers
{
    public class QLNVCSController : Controller
    {
        // GET: Admin/QLNVCS
        csdl_HTQLEntities db = new csdl_HTQLEntities();


        // GET: Admin/QLNVCS
        [CustomAuthorize]
        public ActionResult getListNVCS(string searchString, int? page)
        {
            var listNVCS = from s in db.NHAN_VIEN_CHAM_SOC select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                listNVCS = listNVCS.Where(s => s.Ho_Ten.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;
            listNVCS = listNVCS.OrderBy(s => s.Ho_Ten);
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            return View(listNVCS.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        [CustomAuthorize]
        public ActionResult CreateNVCS()
        {
            return View();
        }

        // POST: NhanVienCS/Create
        [CustomAuthorize]
        [HttpPost, ActionName("CreateNVCS")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateNVCS(NHAN_VIEN_CHAM_SOC NVCS)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingTaiKhoan = db.TAI_KHOAN.FirstOrDefault(tk => tk.So_CCCD == NVCS.So_CCCD);
                    if (existingTaiKhoan != null)
                    {
                        ModelState.AddModelError("So_CCCD", "Số CCCD đã tồn tại.");
                        return View(NVCS);
                    }
                    // Tạo một tài khoản mới
                    TAI_KHOAN TK = new TAI_KHOAN
                    {
                        Ma_Quyen = 3,
                        Ho_Ten = NVCS.Ho_Ten,
                        Ngay_Sinh = NVCS.Ngay_Sinh,
                        So_CCCD = NVCS.So_CCCD,
                        Gioi_Tinh = NVCS.Gioi_Tinh,
                        Dia_Chi = NVCS.Dia_Chi,
                        So_Dien_Thoai = NVCS.So_Dien_Thoai,
                        Mat_Khau = NVCS.Mat_Khau
                    };
                    db.TAI_KHOAN.Add(TK);
                    db.SaveChanges();
                    int newMaTK = TK.Ma_TK;
                    // Tạo một bác sĩ mới
                    NHAN_VIEN_CHAM_SOC NhanVienCS = new NHAN_VIEN_CHAM_SOC
                    {
                        Ma_TK = newMaTK,
                        Ma_Quyen = 3,
                        Ho_Ten = NVCS.Ho_Ten,
                        Ngay_Sinh = NVCS.Ngay_Sinh,
                        So_CCCD = NVCS.So_CCCD,
                        Gioi_Tinh = NVCS.Gioi_Tinh,
                        Dia_Chi = NVCS.Dia_Chi,
                        So_Dien_Thoai = NVCS.So_Dien_Thoai,
                        Mat_Khau = NVCS.Mat_Khau,
                        Kinh_nghiem = NVCS.Kinh_nghiem
                    };
                    db.NHAN_VIEN_CHAM_SOC.Add(NhanVienCS);
                    db.SaveChanges();

                    return RedirectToAction("getListNVCS");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Loi luu du lieu");
            }
            return View(NVCS);

        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult EditNVCS(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            NHAN_VIEN_CHAM_SOC NVCS = db.NHAN_VIEN_CHAM_SOC.SingleOrDefault(s => s.Ma_TK == id);
            return View(NVCS);
        }

        [CustomAuthorize]
        [HttpPost, ActionName("EditNVCS")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditNVCS(int id)
        {
            var NVCSUpdate = db.NHAN_VIEN_CHAM_SOC.Find(id);
            if (TryUpdateModel(NVCSUpdate, "", new string[] { "Ma_Quyen", "Ho_Ten", "Ngay_Sinh", "So_CCCD", "Gioi_Tinh", "Dia_Chi", "So_Dien_Thoai" , "Trang_Thai_Lam_Viec", "Kinh_nghiem" }))
            {
                try
                {
                    // Cập nhật thông tin bác sĩ trong bảng NhanVienCS
                    db.Entry(NVCSUpdate).State = EntityState.Modified;

                    // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
                    TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(NVCSUpdate.Ma_TK);
                    if (taiKhoan != null)
                    {
                        // Cập nhật thông tin tài khoản
                        taiKhoan.Ma_Quyen = NVCSUpdate.Ma_Quyen;
                        taiKhoan.Ho_Ten = NVCSUpdate.Ho_Ten;
                        taiKhoan.Ngay_Sinh = NVCSUpdate.Ngay_Sinh;
                        taiKhoan.So_CCCD = NVCSUpdate.So_CCCD;
                        taiKhoan.Gioi_Tinh = NVCSUpdate.Gioi_Tinh;
                        taiKhoan.Dia_Chi = NVCSUpdate.Dia_Chi;
                        taiKhoan.So_Dien_Thoai = NVCSUpdate.So_Dien_Thoai;
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }


                return RedirectToAction("getListNVCS");
            }
            return View(NVCSUpdate);
        }


        //Xác nhận xoá
        [CustomAuthorize]
        [HttpGet]
        public ActionResult DeleteNVCS(int? id)
        {
            NHAN_VIEN_CHAM_SOC NVCS = db.NHAN_VIEN_CHAM_SOC.SingleOrDefault(n => n.Ma_TK == id);
            if (NVCS == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            
            if (NVCS == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            int Count = db.HOA_DON.Count(n => n.Ma_TK == id);
            if (Count > 0)
            {
                TempData["LoiKhoaNgoai"] = "Nhân viên này đã có lịch chăm sóc. Không thể xóa.";
                return RedirectToAction("DetailsNVCS", new { id = id });

            }
            if (TempData["LoiKhoaNgoai"] != null)
            {
                ViewBag.ErrorMessage = TempData["LoiKhoaNgoai"].ToString();
            }

            // Xóa thông tin nhân viên chăm sóc trong bảng NHAN_VIEN_CHAM_SOC
            db.NHAN_VIEN_CHAM_SOC.Remove(NVCS);
            // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
            TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(NVCS.Ma_TK);
            if (taiKhoan != null)
            {
                // Xóa thông tin tài khoản
                db.TAI_KHOAN.Remove(taiKhoan);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();
            return RedirectToAction("getListNVCS");
        }

        [CustomAuthorize]
        public ActionResult DetailsNVCS(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            //Bao gom tat ca danh sach chapter cua boo theo id chi dinh
            var viewModel = db.NHAN_VIEN_CHAM_SOC.Include(s => s.HOA_DON).SingleOrDefault(s => s.Ma_TK == id);
            if (viewModel == null)
                if (viewModel == null)
                {
                    return HttpNotFound();
                }
            return View(viewModel);
        }

    }
}