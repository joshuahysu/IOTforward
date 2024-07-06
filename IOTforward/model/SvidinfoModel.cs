using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTforward.model
{
    // 定义模型类，用于映射查询结果
    public class SvidinfoModel
    {

        public int Id { get; set; }
        public int uid { get; set; }
        public string Class { get; set; }
        public string parameterid { get; set; }
        public string remark { get; set; }
        public string category { get; set; }
        public string tagname { get; set; }
        public string serveraddress { get; set; }
        public string serverfunctioncode { get; set; }
        public string address { get; set; }

        public string num { get; set; }
        public string functioncode { get; set; }
        public string readfreq { get; set; }
        public string scaletype { get; set; }
        public string scalemultiple { get; set; }
        public string scaleoffset { get; set; }
        public string unit { get; set; }
        public string max { get; set; }
        public string min { get; set; }
        public string signed { get; set; }
        public string Fixed { get; set; }
        public int mergemode { get; set; }
        public int Private { get; set; }
        public string startbit { get; set; }
        public string endbit { get; set; }
        public string endian { get; set; }
        public string valuetype { get; set; }
        public string intnum { get; set; }

    }
}
