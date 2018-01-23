using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionskyOA.Core.Enum;
using MissionskyOA.Core.Pager;
using MissionskyOA.Data;
using MissionskyOA.Models;

namespace MissionskyOA.Services
{
    public partial interface IDepartmentService
    {
        /// <summary>
        /// 显示所有部门信息
        /// </summary>
        /// <returns>部门信息</returns>
        ListResult<DepartmentModel> List(int pageNo, int pageSize, SortModel sort, FilterModel filter);

        /// <summary>
        /// 添加部门信息
        /// </summary>
        /// <param name="model">部门信息</param>
        /// <returns></returns>
        /// <remarks>是否更新成功</remarks>
        bool AddDepartment(DepartmentModel model);

        /// <summary>
        /// 根据ID查部门信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// <remarks>部门信息</remarks>
        DepartmentModel GetDepartmentByID(int id);

         /// <summary>
        /// 根据ID更新部门信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// <remarks>是否更新成功</remarks>
        bool UpdateDepartment(int id, DepartmentModel model);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        /// <remarks>是否删除成功</remarks>
        bool deleteDepartment(int id);

    }
}
