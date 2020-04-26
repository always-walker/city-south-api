using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitySouth.Data
{
    public class HouseBatch
    {
        //[Required(ErrorMessage = "请选择小区")]
        [Range(1, 10000000, ErrorMessage = "请选择小区")]
        public int EstateId { get; set; }
        [Required(ErrorMessage = "请输入房屋类型")]
        public string HouseType { get; set; }
        [Required(ErrorMessage = "请输入联系电话")]
        public string ContactTel { get; set; }
        [Required(ErrorMessage = "请输入房屋结构")]
        public string Structure { get; set; }
        public bool IsPlace { get; set; }
        //[Required(ErrorMessage = "请输入起始楼栋数")]
        [Range(1, 100, ErrorMessage = "请输入起始楼栋数")]
        public int StartBuilding { get; set; }
        //[Required(ErrorMessage = "请输入结束楼栋数")]
        [Range(1, 100, ErrorMessage = "请输入结束楼栋数")]
        public int EndBuilding { get; set; }
        //[Required(ErrorMessage = "请输入结束单元数")]
        [Range(1, 100, ErrorMessage = "请输入结束单元数")]
        public int StartUnit { get; set; }
        //[Required(ErrorMessage = "请输入结束单元数")]
        [Range(1, 100, ErrorMessage = "请输入结束单元数")]
        public int EndUnit { get; set; }
        //[Required(ErrorMessage = "请输入总层高")]
        [Range(1, 100, ErrorMessage = "请输入起始楼层")]
        public int StartFloor { get; set; }
        [Range(1, 100, ErrorMessage = "请输入结束楼层")]
        public int EndFloor { get; set; }
        public DateTime? HandDate { get; set; }
        public string News { get; set; }
        public bool EmptyState { get; set; }
        [Required(ErrorMessage = "请输入每层楼所有房号")]
        public List<HouseModel> ModelList { get; set; }
    }
    public class HouseModel 
    {
        [Range(1, 100, ErrorMessage = "请输入房号")]
        public int HouseNo { get; set; }
        [Required(ErrorMessage = "请输入户型")]
        public string Model { get; set; }
        [Range(1, 10000, ErrorMessage = "请输入房屋建筑面积")]
        public double Floorage { get; set; }
    }
}
