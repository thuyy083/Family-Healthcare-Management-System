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
    public class QLDVController : Controller
    {
        // GET: Admin/QLDV
        csdl_HTQLEntities db = new csdl_HTQLEntities();


        // GET: Admin/QLDV
        [CustomAuthorize]
        public ActionResult getListDV(string searchString, int? page)
        {
            var listDV = from s in db.DICH_VU_CHAM_SOC_NGUOI_BENH select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                listDV = listDV.Where(s => s.Ten_Dich_Vu.Contains(searchString));
            }
            ViewBag.CurrentFilter = searchString;
            listDV = listDV.OrderBy(s => s.Ten_Dich_Vu);
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            return View(listDV.ToPagedList(pageNumber, pageSize));
        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult CreateDV()
        {
            return View();
        }

        // POST: DichVu/Create
        [CustomAuthorize]
        [HttpPost, ActionName("CreateDV")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateDV(DICH_VU_CHAM_SOC_NGUOI_BENH DV)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tạo một dịch vụ mới
                    DICH_VU_CHAM_SOC_NGUOI_BENH DichVu = new DICH_VU_CHAM_SOC_NGUOI_BENH
                    {
                        Ten_Dich_Vu = DV.Ten_Dich_Vu,
                        Mo_Ta = DV.Mo_Ta,
                        Noi_Cham_Soc = DV.Noi_Cham_Soc,
                        Gioi_Tinh_Cua_Nhan_Vien_Cham_Soc = DV.Gioi_Tinh_Cua_Nhan_Vien_Cham_Soc
                    };
                    db.DICH_VU_CHAM_SOC_NGUOI_BENH.Add(DichVu);
                    db.SaveChanges();

                    return RedirectToAction("getListDV");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Loi luu du lieu");
            }
            var listDV = from s in db.DICH_VU_CHAM_SOC_NGUOI_BENH select s;
            return View("getlistDV", listDV);

        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult EditDV(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DICH_VU_CHAM_SOC_NGUOI_BENH DV = db.DICH_VU_CHAM_SOC_NGUOI_BENH.SingleOrDefault(s => s.Ma_Dich_Vu == id);
            return View(DV);
        }

        [CustomAuthorize]
        [HttpPost, ActionName("EditDV")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditDV(int id)
        {
            var DVUpdate = db.DICH_VU_CHAM_SOC_NGUOI_BENH.Find(id);
            if (TryUpdateModel(DVUpdate, "", new string[] { "Ten_Dich_Vu", "Mo_Ta", "Noi_Cham_Soc", "Gioi_Tinh_Cua_Nhan_Vien_Cham_Soc"}))
            {
                try
                {
                    // Cập nhật thông tin dịch vụ trong bảng DichVu
                    db.Entry(DVUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Error Save Data");
                }


                return RedirectToAction("getListDV");
            }
            return View(DVUpdate);
        }

        //Hiển thị thông tin cần xoá
        [HttpGet]
        [CustomAuthorize]
        public ActionResult DeleteDV(int? id)
        {
            DICH_VU_CHAM_SOC_NGUOI_BENH DV = db.DICH_VU_CHAM_SOC_NGUOI_BENH.SingleOrDefault(n => n.Ma_Dich_Vu == id);

            if (DV == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Kiểm tra xem có bản ghi nào trong bảng DON_GIA_DICH_VU sử dụng khóa ngoại của bản ghi DICH_VU_CHAM_SOC_NGUOI_BENH không
            int Count = db.LICH_CHAM_SOC.Count(n => n.Ma_Dich_Vu == id);
            if (Count > 0)
            {
                TempData["LoiKhoaNgoai"] = "Dịch vụ này đã có người đặt lịch. Không thể xóa.";
                return RedirectToAction("DetailsDV", new { id = id });

            }
            if (TempData["LoiKhoaNgoai"] != null)
            {
                ViewBag.ErrorMessage = TempData["LoiKhoaNgoai"].ToString();
            }

            var dg = db.DON_GIA_DICH_VU.Where(s => s.Ma_Dich_Vu == id).ToList();

            foreach (var dongia in dg)
            {
                db.DON_GIA_DICH_VU.Remove(dongia);
            }
            // Xóa thông tin dịch vụ trong bảng DICH_VU_CHAM_SOC_NGUOI_BENH
            db.DICH_VU_CHAM_SOC_NGUOI_BENH.Remove(DV);


            // Lưu thay đổi vào cơ sở dữ liệu
            db.SaveChanges();
            return RedirectToAction("getListDV");
        }

        [CustomAuthorize]
        public ActionResult DetailsDV(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            //Bao gom tat ca danh sach chapter cua boo theo id chi dinh
            var viewModel = db.DICH_VU_CHAM_SOC_NGUOI_BENH.Include(s => s.DON_GIA_DICH_VU).SingleOrDefault(s => s.Ma_Dich_Vu == id);
            if (viewModel == null)
                if (viewModel == null)
                {
                    return HttpNotFound();
                }
            return View(viewModel);
        }

        [CustomAuthorize]
        public ActionResult CreateDonGia(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Ma_Dich_Vu = id;
            return View();
        }
        [CustomAuthorize]
        [HttpPost, ActionName("CreateDonGia")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateDonGia(DON_GIA_DICH_VU DonGia)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    THOI_DIEM TD = new THOI_DIEM
                    {
                        Ngay_Gio= DateTime.Now
                    };
                    db.THOI_DIEM.Add(TD);
                    db.SaveChanges();

                    DON_GIA_DICH_VU DG = new DON_GIA_DICH_VU
                    {
                        Ma_Dich_Vu=DonGia.Ma_Dich_Vu,
                        Don_Gia = DonGia.Don_Gia,
                        Ngay_Gio = TD.Ngay_Gio
                    };
                    db.DON_GIA_DICH_VU.Add(DG);
                    db.SaveChanges();
                    return RedirectToAction("DetailsDV", "QLDV", new { id = DonGia.Ma_Dich_Vu });
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Loi luu du lieu");
            }
            var listDG = from s in db.DON_GIA_DICH_VU select s;
            return View("getListDV", listDG);
        }
    }
}