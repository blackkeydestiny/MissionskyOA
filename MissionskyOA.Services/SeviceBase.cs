using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Data;
using System.Data.Entity;

namespace MissionskyOA.Services
{
    public class SeviceBase
    {
        private MissionskyOAEntities _DbContext;
        protected MissionskyOAEntities DbContext
        {
            get
            {
                if (_DbContext == null)
                {
                    _DbContext = new MissionskyOAEntities();
                }

                return _DbContext;
            }
        }
    }
}
