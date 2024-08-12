using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HTQL_DV_Y_TE_GIA_DINH.Models
{
    public class DangNhapViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã tài khoản.")]
        public string SoCCCD { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; }
    }

}