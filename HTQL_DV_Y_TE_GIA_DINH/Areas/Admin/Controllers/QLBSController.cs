using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using PagedList;


namespace HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Controllers
{
    public class QLBSController : Controller
    {
        csdl_HTQLEntities db = new csdl_HTQLEntities();


        // GET: Admin/QLBS
        [CustomAuthorize]
        public ActionResult getListBS(string searchString, int? page)
        {
            var listBS = from s in db.BAC_SI_GIA_DINH select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                listBS = listBS.Where(s => s.Ho_Ten.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;
            listBS = listBS.OrderBy(s => s.Ho_Ten);
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            return View(listBS.ToPagedList(pageNumber, pageSize));
        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult CreateBS() 
        {
            return View();
        }

        // POST: BacSi/Create
        [CustomAuthorize]
        [HttpPost, ActionName("CreateBS")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateBS(BAC_SI_GIA_DINH BS)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingTaiKhoan = db.TAI_KHOAN.FirstOrDefault(tk => tk.So_CCCD == BS.So_CCCD);
                    if (existingTaiKhoan != null)
                    {
                        ModelState.AddModelError("So_CCCD", "Số CCCD đã tồn tại.");
                        return View(BS);
                    }
                    // Tạo một tài khoản mới
                    TAI_KHOAN TK = new TAI_KHOAN
                    {
                        Ma_Quyen = 2,
                        Ho_Ten = BS.Ho_Ten,
                        Ngay_Sinh = BS.Ngay_Sinh,
                        So_CCCD = BS.So_CCCD,
                        Gioi_Tinh = BS.Gioi_Tinh,
                        Dia_Chi = BS.Dia_Chi,
                        So_Dien_Thoai = BS.So_Dien_Thoai,
                        Mat_Khau = BS.Mat_Khau
                    };
                    db.TAI_KHOAN.Add(TK);
                    db.SaveChanges();

                    int newMaTK = TK.Ma_TK;

                    // Tạo một bác sĩ mới
                    BAC_SI_GIA_DINH BacSi = new BAC_SI_GIA_DINH
                    {
                        Ma_TK = newMaTK,
                        Ma_Quyen = 2,
                        Ho_Ten = BS.Ho_Ten,
                        Ngay_Sinh = BS.Ngay_Sinh,
                        So_CCCD = BS.So_CCCD,
                        Gioi_Tinh = BS.Gioi_Tinh,
                        Dia_Chi = BS.Dia_Chi,
                        So_Dien_Thoai = BS.So_Dien_Thoai,
                        Mat_Khau = BS.Mat_Khau,
                        Chuyen_mon = BS.Chuyen_mon
                    };
                    db.BAC_SI_GIA_DINH.Add(BacSi);
                    db.SaveChanges();

                    return RedirectToAction("getListBS");
                }
            }
            catch(RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Loi luu du lieu");
            }
            return View(BS);

        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult EditBS(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BAC_SI_GIA_DINH BS = db.BAC_SI_GIA_DINH.SingleOrDefault(s => s.Ma_TK == id);
            return View(BS);
        }

        [CustomAuthorize]
        [HttpPost, ActionName("EditBS")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditBS(int id)
        {
            var BSUpdate = db.BAC_SI_GIA_DINH.Find(id);
            if (TryUpdateModel(BSUpdate, "", new string[] { "Ho_Ten", "Ngay_Sinh", "So_CCCD", "Gioi_Tinh", "Dia_Chi", "So_Dien_Thoai", "Chuyen_mon" }))
            {
                try
                {
                    // Cập nhật thông tin bác sĩ trong bảng BacSi
                    db.Entry(BSUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
                    TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(BSUpdate.Ma_TK);
                    if (taiKhoan != null)
                    {
                        // Cập nhật thông tin tài khoản
                        taiKhoan.Ho_Ten = BSUpdate.Ho_Ten;
                        taiKhoan.Ngay_Sinh = BSUpdate.Ngay_Sinh;
                        taiKhoan.So_CCCD = BSUpdate.So_CCCD;
                        taiKhoan.Gioi_Tinh = BSUpdate.Gioi_Tinh;
                        taiKhoan.Dia_Chi = BSUpdate.Dia_Chi;
                        taiKhoan.So_Dien_Thoai = BSUpdate.So_Dien_Thoai;
                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();
                    }
                    return RedirectToAction("DetailsBS", new { id = taiKhoan.Ma_TK });


                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }
                
            }
            return RedirectToAction("DetailsBS", new { id = id });
        }

        
        //Xác nhận xoá
        [CustomAuthorize]
        [HttpGet]
        public ActionResult DeleteBS(int? id)
        {
            BAC_SI_GIA_DINH BS = db.BAC_SI_GIA_DINH.SingleOrDefault(n => n.Ma_TK == id);

            if (BS == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            int phanCongCount = db.PHAN_CONG_CHAM_SOC.Count(n => n.Ma_TK == id);
            if (phanCongCount >0)
            {
                TempData["LoiKhoaNgoai"] = "Bác sĩ này đã được phân công chăm sóc. Không thể xóa.";
                return RedirectToAction("DetailsBS", new { id = id });

            }
            if (TempData["LoiKhoaNgoai"] != null)
            {
                ViewBag.ErrorMessage = TempData["LoiKhoaNgoai"].ToString();
            }
            // Xóa thông tin bác sĩ trong bảng BAC_SI_GIA_DINH
            db.BAC_SI_GIA_DINH.Remove(BS);
            // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
            TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(BS.Ma_TK);
            if (taiKhoan != null)
            {
                // Xóa thông tin tài khoản
                db.TAI_KHOAN.Remove(taiKhoan);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();
            return RedirectToAction("getListBS");
        }

        [CustomAuthorize]
        public ActionResult DetailsBS(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            //Bao gom tat ca danh sach chapter cua boo theo id chi dinh
            var viewModel = db.BAC_SI_GIA_DINH.Include(s => s.PHAN_CONG_CHAM_SOC).SingleOrDefault(s => s.Ma_TK == id);
            if (viewModel == null)
                if (viewModel == null)
                {
                    return HttpNotFound();
                }
            return View(viewModel);
        }


    }
}