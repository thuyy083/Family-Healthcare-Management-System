using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using PagedList;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Controllers
{
    public class QLBNController : Controller
    {
        // GET: Admin/QLBN
        csdl_HTQLEntities db = new csdl_HTQLEntities();


        [CustomAuthorize]
        public ActionResult getListBN(string searchString, int? page)
        {
            var benhNhan = from s in db.BENH_NHAN select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                benhNhan = benhNhan.Where(s => s.Ho_Ten.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;
            benhNhan = benhNhan.OrderBy(s => s.Ho_Ten);
            int pageSize = 4; 
            int pageNumber = (page ?? 1);

            return View(benhNhan.ToPagedList(pageNumber, pageSize));
        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult CreateBN(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Ma_HGD = id;
            return View();
        }

        // POST: BenhNhan/Create
        [CustomAuthorize]
        [HttpPost, ActionName("CreateBN")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateBN(BENH_NHAN BN)
        {
           
            try
            {
                if (ModelState.IsValid)
                {
                    var existingTaiKhoan = db.TAI_KHOAN.FirstOrDefault(tk => tk.So_CCCD == BN.So_CCCD);
                    if (existingTaiKhoan != null)
                    {
                        ModelState.AddModelError("So_CCCD", "Số CCCD đã tồn tại.");
                        return View(BN);
                    }
                    // Tạo một tài khoản mới
                    TAI_KHOAN TK = new TAI_KHOAN
                    {
                        Ma_Quyen = 4,
                        Ho_Ten = BN.Ho_Ten,
                        Ngay_Sinh = BN.Ngay_Sinh,
                        So_CCCD = BN.So_CCCD,
                        Gioi_Tinh = BN.Gioi_Tinh,
                        Dia_Chi = BN.Dia_Chi,
                        So_Dien_Thoai = BN.So_Dien_Thoai,
                        Mat_Khau = BN.Mat_Khau
                    };
                    db.TAI_KHOAN.Add(TK);
                    db.SaveChanges();

                    int newMaTK = TK.Ma_TK;

                    // Tạo một bệnh nhân mới
                    BENH_NHAN BenhNhan = new BENH_NHAN
                    {
                        Ma_TK = newMaTK,
                        Ma_HGD = BN.Ma_HGD,
                        Ma_Quyen = 4,
                        Ho_Ten = BN.Ho_Ten,
                        Ngay_Sinh = BN.Ngay_Sinh,
                        So_CCCD = BN.So_CCCD,
                        Gioi_Tinh = BN.Gioi_Tinh,
                        Dia_Chi = BN.Dia_Chi,
                        So_Dien_Thoai = BN.So_Dien_Thoai,
                        Mat_Khau = BN.Mat_Khau,
                        So_BHYT = BN.So_BHYT
                    };
                    db.BENH_NHAN.Add(BenhNhan);
                    db.SaveChanges();

                    return RedirectToAction("getListBN");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Loi luu du lieu");
            }
            return View(BN);

        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult EditBN(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            BENH_NHAN BN = db.BENH_NHAN.SingleOrDefault(s => s.Ma_TK == id);
            return View(BN);
        }

        
        [HttpPost, ActionName("EditBN")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [CustomAuthorize]
        public ActionResult EditBN(int id)
        {
            var BNUpdate = db.BENH_NHAN.Find(id);
            if (TryUpdateModel(BNUpdate, "", new string[] {"Ma_HGD", "Ho_Ten", "Ngay_Sinh", "So_CCCD", "Gioi_Tinh", "Dia_Chi", "So_Dien_Thoai", "So_BHYT" }))
            {
                try
                {
                    // Cập nhật thông tin bệnh nhân trong bảng BenhNhan
                    db.Entry(BNUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
                    TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(BNUpdate.Ma_TK);
                    if (taiKhoan != null)
                    {
                        // Cập nhật thông tin tài khoản
                        taiKhoan.Ho_Ten = BNUpdate.Ho_Ten;
                        taiKhoan.Ngay_Sinh = BNUpdate.Ngay_Sinh;
                        taiKhoan.So_CCCD = BNUpdate.So_CCCD;
                        taiKhoan.Gioi_Tinh = BNUpdate.Gioi_Tinh;
                        taiKhoan.Dia_Chi = BNUpdate.Dia_Chi;
                        taiKhoan.So_Dien_Thoai = BNUpdate.So_Dien_Thoai;
                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();
                    }
                    return RedirectToAction("DetailsBN", new { id = taiKhoan.Ma_TK });


                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }


                return RedirectToAction("DetailsBN", new { id = id });
            }
            return View(BNUpdate);
        }


        [CustomAuthorize]
        [HttpGet]
        public ActionResult DeleteBN(int? id)
        {
            BENH_NHAN BN = db.BENH_NHAN.SingleOrDefault(n => n.Ma_TK == id);

            if (BN == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Lấy danh sách sổ theo dõi sức khỏe của bệnh nhân
            var so = db.SO_THEO_DOI_SUC_KHOE.Where(s => s.Ma_TK == id).ToList();

            // Xóa từng sổ theo dõi sức khỏe
            foreach (var sow in so)
            {
                db.SO_THEO_DOI_SUC_KHOE.Remove(sow);
            }

            // Xóa thông tin bệnh nhân trong bảng BENH_NHAN
            db.BENH_NHAN.Remove(BN);

            // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan
            TAI_KHOAN taiKhoan = db.TAI_KHOAN.Find(BN.Ma_TK);
            if (taiKhoan != null)
            {
                // Xóa thông tin tài khoản
                db.TAI_KHOAN.Remove(taiKhoan);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();
            return RedirectToAction("getListBN");
        }


        [CustomAuthorize]
        public ActionResult DetailsBN(int? id)
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

    }
}