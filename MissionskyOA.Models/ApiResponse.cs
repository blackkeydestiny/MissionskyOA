using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionskyOA.Models
{
    public class ApiResponse
    {
        private int _statusCode;
        public int StatusCode {
            get
            {
                if (_statusCode == 0)
                {
                    _statusCode = 200;
                }

                return _statusCode;
            }

            set
            {
                _statusCode = value;
            }

        }

        public string Message { get; set; }

        public string MessageDetail { get; set; }
    }


    public class ApiResponse<T>:ApiResponse
    {
        public virtual T Result { get; set; }
    }
}
