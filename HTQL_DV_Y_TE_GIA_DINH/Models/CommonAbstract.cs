using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTQL_DV_Y_TE_GIA_DINH.Models
{
    public abstract class CommonAbstract
    {
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set;}
        public string ModifiedDate { get; set;}
        public string ModifiedBy { get; set;}
    }
}