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
    public class QLHGDController : Controller
    {
        // GET: Admin/QLHGD
        csdl_HTQLEntities db = new csdl_HTQLEntities();


        // GET: Admin/QLHGD
        [CustomAuthorize]
        public ActionResult getListHGD(string searchString, int? page)
        {
            var listHGD = from s in db.HO_GIA_DINH select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                listHGD = listHGD.Where(s => s.Chu_Ho.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;
            listHGD = listHGD.OrderBy(s => s.Chu_Ho);
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            return View(listHGD.ToPagedList(pageNumber, pageSize));
        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult CreateHGD()
        {
            return View();
        }

        // POST: HoGD/Create
        [CustomAuthorize]
        [HttpPost, ActionName("CreateHGD")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateHGD(HO_GIA_DINH HGD)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tạo một HGD mới
                    db.HO_GIA_DINH.Add(HGD);
                    db.SaveChanges();
                    return RedirectToAction("getListHGD");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Loi luu du lieu");
            }
            var listHGD = from s in db.HO_GIA_DINH select s;
            return View(HGD);

        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult EditHGD(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HO_GIA_DINH HGD = db.HO_GIA_DINH.SingleOrDefault(s => s.Ma_HGD == id);
            return View(HGD);
        }

        [CustomAuthorize]
        [HttpPost, ActionName("EditHGD")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditHGD(int id)
        {
            var HGDUpdate = db.HO_GIA_DINH.Find(id);
            if (TryUpdateModel(HGDUpdate, "", new string[] { "SoDienThoai_HGD", "Dia_Chi_HGD", "Chu_Ho" }))
            {
                try
                {
                    // Cập nhật thông tin bác sĩ trong bảng HoGD
                    db.Entry(HGDUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DetailsHGD", new { id = id });
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }


                return RedirectToAction("getListHGD");
            }
            return RedirectToAction("DetailsHGD", new { id = id });
        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult DeleteHGD(int? id)
        {
            HO_GIA_DINH HGD = db.HO_GIA_DINH.SingleOrDefault(n => n.Ma_HGD == id);

            if (HGD == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            int Count = db.BENH_NHAN.Count(n => n.Ma_HGD == id);
            if (Count > 0)
            {
                TempData["LoiKhoaNgoai"] = "Hộ gia đình này đã có thành viên. Vui lòng xóa thành viên trong gia đình trước";
                return RedirectToAction("DetailsHGD", new { id = id });

            }
            if (TempData["LoiKhoaNgoai"] != null)
            {
                ViewBag.ErrorMessage = TempData["LoiKhoaNgoai"].ToString();
            }
            // Xóa thông tin bác sĩ trong bảng BAC_SI_GIA_DINH
            db.HO_GIA_DINH.Remove(HGD);
            // Lấy thông tin tài khoản tương ứng từ bảng TaiKhoan

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();
            return RedirectToAction("getListHGD");
        }


        /*

        //Hiển thị thông tin cần xoá
        [HttpGet]
        public ActionResult DeleteHGD(int id)
        {
            HO_GIA_DINH HGD = db.HO_GIA_DINH.SingleOrDefault(n => n.Ma_HGD == id);
            if (HGD == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(HGD);
        }
        //Xác nhận xoá
        [HttpPost, ActionName("DeleteHGD")]
        public ActionResult DeleteHGDConfirm(int id)
        {
            HO_GIA_DINH HGD = db.HO_GIA_DINH.SingleOrDefault(n => n.Ma_HGD == id);

            if (HGD == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            // Xóa thông tin bác sĩ trong bảng HO_GIA_DINH
            db.HO_GIA_DINH.Remove(HGD);

            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();
            return RedirectToAction("getListHGD");
        }
        */

        [CustomAuthorize]
        public ActionResult DetailsHGD(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            //Bao gom tat ca danh sach chapter cua boo theo id chi dinh
            var viewModel = db.HO_GIA_DINH.Include(s => s.BENH_NHAN).SingleOrDefault(s => s.Ma_HGD == id);
            if (viewModel == null)
                if (viewModel == null)
                {
                    return HttpNotFound();
                }
            ViewBag.Ma_HGD = id;
            return View(viewModel);
        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult ThemPhanCong()
        {
            // Lấy danh sách Hộ gia đình
            var listHGD = db.HO_GIA_DINH.ToList().Select(h => new SelectListItem
            {
                Value = h.Ma_HGD.ToString(), 
                Text = h.Chu_Ho 
            }).ToList();

            // Lấy danh sách bác sĩ
            var listBacSi = db.BAC_SI_GIA_DINH.ToList().Select(b => new SelectListItem
            {
                Value = b.Ma_TK.ToString(), 
                Text = b.Ho_Ten 
            }).ToList();

            ViewBag.Ma_HGD = listHGD;
            ViewBag.Ma_TK = listBacSi;

            return View();
        }

        [CustomAuthorize]
        [HttpPost, ActionName("ThemPhanCong")]
        [ValidateAntiForgeryToken]
        public ActionResult ThemPhanCong(PHAN_CONG_CHAM_SOC phanCong)
        {
            if (ModelState.IsValid)
            {
                db.PHAN_CONG_CHAM_SOC.Add(phanCong);
                db.SaveChanges();
                return RedirectToAction("getListHGD"); // Chuyển hướng đến trang chính sau khi thêm thành công
            }

            return View(phanCong);
        }


    }
}