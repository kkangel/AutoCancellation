using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCancellation
{
    class DatePass
    {
        public string  DateStart { get; set; }
        public string DateEnd { get; set; }

        public DatePass(string dateStart, string dateEnd)
        {
            this.DateStart = dateStart;
            this.DateEnd = dateEnd;
        }
    }
}
