using HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using PagedList;

namespace HTQL_DV_Y_TE_GIA_DINH.Areas.Admin.Controllers
{
    public class QLLCSController : Controller
    {
        // GET: Admin/QLLCS
        csdl_HTQLEntities db = new csdl_HTQLEntities();


        [CustomAuthorize]
        public ActionResult getListLCS(int? page)
        {
            var listLCS = from s in db.LICH_CHAM_SOC select s;
            listLCS = listLCS.OrderBy(s => s.Trang_Thai);
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            return View(listLCS.ToPagedList(pageNumber, pageSize));
        }

        [CustomAuthorize]
        public ActionResult DetailsLCS(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var viewModel = db.LICH_CHAM_SOC.Include(s => s.HOA_DON).SingleOrDefault(s => s.Ma_Lich == id);
            if (viewModel == null)
                if (viewModel == null)
                {
                    return HttpNotFound();
                }
            ViewBag.TaiKhoanList = new SelectList(db.TAI_KHOAN, "Ma_TK", "Ho_Ten");
            return View(viewModel);
        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult DuyetLCS(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tạo một hóa đơn mới
            var newHoaDon = new HOA_DON();

            // Gán Ma_Lich từ id được truyền vào
            ViewBag.Ma_Lich = id;

            int maLich = db.LICH_CHAM_SOC.Where(bn => bn.Ma_Lich == id).Select(bn => bn.Ma_Lich).FirstOrDefault();

                // Lấy danh sách thành viên trong Hộ gia đình
                //var lichChamSoc = db.LICH_CHAM_SOC.Where(bn => bn.Ma_Lich == maLich);


            // Lấy thông tin về lịch chăm sóc
            var lichChamSoc = db.LICH_CHAM_SOC.Find(maLich);

            // Lấy thông tin về dịch vụ từ lịch chăm sóc
            var maDichVu = lichChamSoc.Ma_Dich_Vu;
            var donGia = db.DON_GIA_DICH_VU
                            .Where(d => d.Ma_Dich_Vu == maDichVu)
                            .OrderByDescending(d => d.Ngay_Gio)
                            .FirstOrDefault();

            // Tính tổng tiền
            if (donGia != null)
            {
                Debug.WriteLine("Don_Gia: " + donGia.Don_Gia);
                Debug.WriteLine("So_Ngay_Thue: " + lichChamSoc.So_Ngay_Thue);
                newHoaDon.Tong_Tien = (long)(donGia.Don_Gia * lichChamSoc.So_Ngay_Thue);
            }
            Debug.WriteLine("Tong tien: " + newHoaDon.Tong_Tien);

            // Lấy danh sách tên nhân viên chăm sóc để hiển thị cho người dùng chọn
            ViewBag.Ma_TK = new SelectList(db.NHAN_VIEN_CHAM_SOC, "Ma_TK", "Ho_Ten");
            ViewBag.Tong_Tien = newHoaDon.Tong_Tien;
            return View(newHoaDon);
        }

        [CustomAuthorize]
        [HttpPost, ActionName("DuyetLCS")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult DuyetLCS(HOA_DON hoaDon)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    THOI_DIEM TD = new THOI_DIEM
                    {
                        Ngay_Gio = DateTime.Now
                    };
                    db.THOI_DIEM.Add(TD);
                    db.SaveChanges();

                    // Lấy ngày giờ hiện tại
                    hoaDon.Ngay_Gio = TD.Ngay_Gio;

                    // Thêm hóa đơn vào cơ sở dữ liệu
                    db.HOA_DON.Add(hoaDon);
                    db.SaveChanges();


                    // Cập nhật trạng thái của lịch chăm sóc tương ứng thành true
                    var lichChamSoc = db.LICH_CHAM_SOC.Find(hoaDon.Ma_Lich);
                    if (lichChamSoc != null)
                    {
                        lichChamSoc.Trang_Thai = true;
                        db.SaveChanges();
                    }

                    return RedirectToAction("getlistLCS"); // Chuyển hướng sau khi thêm thành công
                }

                // Nếu ModelState không hợp lệ, quay lại view với model và hiển thị lỗi
                return View(hoaDon);
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Loi luu du lieu");
            }
            var listLCS = from s in db.LICH_CHAM_SOC select s;
            return View("getlistLCS", listLCS);

        }

        [CustomAuthorize]
        [HttpGet]
        public ActionResult TuChoiLCS(int? id)
        {
            LICH_CHAM_SOC LCS = db.LICH_CHAM_SOC.SingleOrDefault(n => n.Ma_Lich == id);

            if (LCS == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            
            db.LICH_CHAM_SOC.Remove(LCS);

            db.SaveChanges();
            return RedirectToAction("getListLCS");
        }

    }
}