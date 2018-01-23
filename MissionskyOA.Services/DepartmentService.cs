using System;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Email;
using MissionskyOA.Core.Security;
using MissionskyOA.Models.Account;
using MissionskyOA.Services;
using MissionskyOA.Data;
using MissionskyOA.Models;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using System.Threading;
using MissionskyOA.Resources;

namespace MissionskyOA.Services
{
    public class DepartmentService:IDepartmentService
    {
        /// <summary>
        /// 显示所有部门信息
        /// </summary>
        /// <returns>部门信息</returns>
        public ListResult<DepartmentModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var list = dbContext.Departments.AsEnumerable();

                if (sort != null)
                {
                    switch (sort.Member)
                    {
                        case "CreatedDate":
                            if (sort.Direction == SortDirection.Ascending)
                            {
                                list = list.OrderBy(item => item.CreatedDate);
                            }
                            else
                            {
                                list = list.OrderByDescending(item => item.CreatedDate);
                            }
                            break;
                        default:
                            break;
                    }
                }

                var count = list.Count();

                list = list.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

                ListResult<DepartmentModel> result = new ListResult<DepartmentModel>();
                result.Data = new List<DepartmentModel>();
                list.ToList().ForEach(item =>
                {
                    result.Data.Add(item.ToModel());
                });

                result.Total = count;
                return result;
            }
        }

        /// <summary>
        /// 添加部门信息
        /// </summary>
        /// <param name="model">部门信息</param>
        /// <returns></returns>
        /// <remarks>是否更新成功</remarks>
        public bool AddDepartment(DepartmentModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var departmentEntity = model.ToEntity();
                departmentEntity.CreatedDate = DateTime.Now;
                departmentEntity.No="MSSKY";
                var entity = dbContext.Departments.Add(departmentEntity);
                dbContext.SaveChanges();
                return true;
            }

        }

        /// <summary>
        /// 根据ID查部门信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// <remarks>部门信息</remarks>
        public DepartmentModel GetDepartmentByID(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Departments.Where(it => it.Id == id).FirstOrDefault(); 
                return entity.ToModel();
            }
        }

        public bool UpdateDepartment(int id, DepartmentModel model)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Departments.Where(it => it.Id == id).FirstOrDefault();
                if(model.No!=null&&! model.No.Equals(entity.No))
                {
                    entity.No = model.No;
                }
                if (model.Name != null && !model.Name.Equals(entity.Name))
                {
                    entity.Name = model.Name;
                }
                entity.DepartmentHead = model.DepartmentHead;

                dbContext.SaveChanges();
                return true;
            }
        }

        public bool deleteDepartment(int id)
        {
            using (var dbContext = new MissionskyOAEntities())
            {
                var entity = dbContext.Departments.Where(it => it.Id == id).FirstOrDefault();
                if(entity.Status==0)
                {
                    entity.Status = 1;
                }
                else if(entity.Status==1)
                {
                    entity.Status = 0;
                }
                
                dbContext.SaveChanges();
                return true;
            }
        }
    }
}
