using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    /// <summary>
    /// 服务父类型
    /// </summary>
    public class ServiceBase
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof(ServiceBase));

        /// <summary>
        /// 记录DbEntityValidationException异常到日志
        /// </summary>
        /// <param name="dbEx"></param>
        public virtual void LogDatabaseError(DbEntityValidationException dbEx)
        {
            if (dbEx != null)
            {
                var entityValidationError = dbEx.EntityValidationErrors.FirstOrDefault();

                if (entityValidationError != null)
                {
                    var validationError = entityValidationError.ValidationErrors.FirstOrDefault();

                    if (validationError != null)
                    {
                        Log.Error(string.Format("保存字段{0}出错：{1}",validationError.PropertyName, validationError.ErrorMessage));
                    }
                }
            }
        }
    }
}
