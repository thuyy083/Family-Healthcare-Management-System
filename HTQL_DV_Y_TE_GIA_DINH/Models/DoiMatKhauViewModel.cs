using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HTQL_DV_Y_TE_GIA_DINH.Models
{
    public class DoiMatKhauViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ.")]
        [DataType(DataType.Password)]
        public string MatKhauCu { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8}$",
    ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ số, 1 chữ cái, 1 ký tự đặc biệt và có độ dài 8 ký tự")]
        public string MatKhauMoi { get; set; }

        [DataType(DataType.Password)]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string XacNhanMatKhau { get; set; }
    }
}