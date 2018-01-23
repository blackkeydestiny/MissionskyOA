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
    /// <summary>
    /// 资产转移接口
    /// </summary>
    public interface IAssetTransactionService
    {
        /// <summary>
        /// 添加转移记录
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isProduction"></param>
        /// <returns></returns>
        bool Add(AssetTransactionModel model, bool isProduction = false);

        /// <summary>
        ///  确认转出或转入
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="outOrIn">0为转出,1为转入</param>
        /// <param name="isProduction"></param>
        /// <returns></returns>
        bool Confirm(int transactionId, int outOrIn, bool isProduction = false);

        /// <summary>
        /// 取消资产转移
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="isProduction"></param>
        /// <returns></returns>
        bool Cancel(int transactionId, bool isProduction = false);

        /// <summary>
        /// 获取单个转移记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        AssetTransactionModel GetAssetTransactionById(int id);
    }
}
