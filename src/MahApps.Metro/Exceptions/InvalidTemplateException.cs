using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahApps.Metro
{
    /// <summary>
    /// 模版无效
    /// </summary>
    public class InvalidTemplateException : Exception
    {
        public InvalidTemplateException(string message)
       : base(message)
        {
        } 
    }
}
