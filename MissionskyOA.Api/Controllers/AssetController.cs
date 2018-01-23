using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using MissionskyOA.Api.ApiException;
using MissionskyOA.Api.Filter;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Services;
using System.Configuration;

namespace MissionskyOA.Api.Controllers
{
    /// <summary>
    /// 资产管理
    /// </summary>
    [RoutePrefix("api/assets")]
    public class AssetController : BaseController
    {
        private IAssetTypeService AssetTypeService { get; set; }
        private IAssetService AssetService { get; set; }
        private IAssetTransactionService AssetTransactionService { get; set; }
        private IAssetInventoryService AssetInventoryService { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public AssetController(IAssetTypeService assetTypeService,
            IAssetService assetService,
            IAssetTransactionService assetTransactionService,
            IAssetInventoryService assetInventoryService)
        {
            this.AssetTypeService = assetTypeService;
            this.AssetService = assetService;
            this.AssetTransactionService = assetTransactionService;
            this.AssetInventoryService = assetInventoryService;
        }


        /// <summary>
        /// 获取资产分类
        /// </summary>
        /// <returns></returns>
        [Route("types")]
        [HttpGet]
        public ApiListResponse<AssetTypeModel> GetAssetTypes()
        {
            var types = this.AssetTypeService.GetAll();
            var response = new ApiListResponse<AssetTypeModel>()
            {
                Result = types
            };

            return response;
        }

        /// <summary>
        /// 获取我的资产列表
        /// </summary>
        /// <returns></returns>
        [Route("my")]
        [HttpGet]
        public ApiListResponse<AssetModel> GetMyAssets()
        {
            if (this.Member == null)
            {
                throw new Exception("当前没有授权的用户");
            }
            var assets = this.AssetService.MyList(this.Member.Id);
            HiddenAmount(assets);
            var response = new ApiListResponse<AssetModel>()
            {
                Result = assets
            };

            return response;
        }

        /// <summary>
        /// 查找资产
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [Route("search")]
        [HttpPost]
        public ApiPagingListResponse<AssetModel> SearchAssets(AssetSearchModel search, int pageIndex = 0, int pageSize = 25)
        {
            var query = this.AssetService.SearchAssets(search, pageIndex, pageSize, this.Member.Id);
            HiddenAmount(query.Result);
            var result = new PaginationModel<AssetModel>();
            //分页
            result.Page = new Page()
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = query.TotalPages,
                TotalCount = query.TotalCount
            };

            return new ApiPagingListResponse<AssetModel>
            {
                Result = query.Result,
                Page = result.Page
            };
        }

        /// <summary>
        /// 根据编码查询资产信息
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [Route("getAssetByBarcode")]
        [HttpGet]
        public ApiResponse<AssetModel> GetAssetBarcode(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                throw new Exception("参数不能为空.");
            }

            var asset = this.AssetService.GetAssetByBarcode(barcode);
            HiddenAmount(asset);
            var response = new ApiResponse<AssetModel>()
            {
                Result = asset
            };

            return response;
        }

        /// <summary>
        /// 资产转移
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("transfer")]
        [HttpPost]
        public ApiResponse<bool> TransferAsset(AssetTransactionModel model)
        {
            if (this.Member.Id != model.InUserId)
            {
                throw new Exception("当前用户必须为接收者");
            }
            if (model.InUserId == model.OutUserId)
            {
                throw new Exception("转出者和接收者不能为相同用户");
            }
            model.BusinessType = BusinessType.AssetTransfer;
            bool isProduction = false;
            if (ConfigurationManager.AppSettings["IsProduction"] != null && ConfigurationManager.AppSettings["IsProduction"].ToLower() == "true")
            {
                isProduction = true;
            }
            var result = this.AssetTransactionService.Add(model, isProduction);
            var response = new ApiResponse<bool>()
            {
                Result = result
            };

            return response;
        }

        /// <summary>
        /// 获取单个资产转移记录详情
        /// </summary>
        /// <param name="transactionId">转移记录Id</param>
        /// <returns></returns>
        [Route("transfer/{transactionId:int}")]
        [HttpGet]
        public ApiResponse<AssetTransactionModel> TransferAsset(int transactionId)
        {
            var result = this.AssetTransactionService.GetAssetTransactionById(transactionId);
            var response = new ApiResponse<AssetTransactionModel>()
            {
                Result = result
            };

            return response;
        }

        /// <summary>
        /// 取消资产转移
        /// </summary>
        /// <param name="transactionId">转移记录Id</param>
        /// <returns></returns>
        [Route("transferCancel/{transactionId:int}")]
        [HttpGet]
        public ApiResponse<bool> TransferCancel(int transactionId)
        {
            if (transactionId <= 0)
            {
                throw new Exception("参数transactionId无效.");
            }
            var transaction = this.AssetTransactionService.GetAssetTransactionById(transactionId);
            if (transaction == null)
            {
                throw new Exception("未找到资产转移记录");
            }
            if (transaction.Asset.Status == (AssetStatus.WaitOut))
            {
                if (this.Member.Id != transaction.OutUserId)
                {
                    throw new Exception("取消操作必须为资产转出者.");
                }
            }
            if (transaction.Asset.Status == (AssetStatus.WaitIn))
            {
                if (this.Member.Id != transaction.InUserId)
                {
                    throw new Exception("取消操作必须为资产接收者.");
                }
            }

            bool isProduction = false;
            if (ConfigurationManager.AppSettings["IsProduction"] != null && ConfigurationManager.AppSettings["IsProduction"].ToLower() == "true")
            {
                isProduction = true;
            }
            var result = this.AssetTransactionService.Cancel(transactionId, isProduction);
            var response = new ApiResponse<bool>()
            {
                Result = result
            };

            return response;
        }

        /// <summary>
        /// 确认转出资产
        /// </summary>
        /// <param name="transactionId">转移记录Id</param>
        /// <returns></returns>
        [Route("confirmOut/{transactionId:int}")]
        [HttpGet]
        public ApiResponse<bool> TransferConfirmOut(int transactionId)
        {
            if (transactionId <= 0)
            {
                throw new Exception("参数transactionId无效.");
            }
            var transaction = this.AssetTransactionService.GetAssetTransactionById(transactionId);
            if (transaction == null)
            {
                throw new Exception("未找到资产转移记录");
            }
            if (this.Member.Id != transaction.OutUserId)
            {
                throw new Exception("当前用户必须为资产转出者.");
            }
            bool isProduction = false;
            if (ConfigurationManager.AppSettings["IsProduction"] != null && ConfigurationManager.AppSettings["IsProduction"].ToLower() == "true")
            {
                isProduction = true;
            }
            var result = this.AssetTransactionService.Confirm(transactionId, 0, isProduction);
            var response = new ApiResponse<bool>()
            {
                Result = result
            };

            return response;
        }

        /// <summary>
        /// 确认转入资产
        /// </summary>
        /// <param name="transactionId">转移记录Id</param>
        /// <returns></returns>
        [Route("confirmIn/{transactionId:int}")]
        [HttpGet]
        public ApiResponse<bool> TransferConfirmIn(int transactionId)
        {
            if (transactionId <= 0)
            {
                throw new Exception("参数transactionId无效.");
            }
            var transaction = this.AssetTransactionService.GetAssetTransactionById(transactionId);
            if (transaction == null)
            {
                throw new Exception("未找到资产转移记录");
            }
            if (this.Member.Id != transaction.InUserId)
            {
                throw new Exception("当前用户必须为资产接收者.");
            }
            bool isProduction = false;
            if (ConfigurationManager.AppSettings["IsProduction"] != null && ConfigurationManager.AppSettings["IsProduction"].ToLower() == "true")
            {
                isProduction = true;
            }
            var result = this.AssetTransactionService.Confirm(transactionId, 1, isProduction);
            var response = new ApiResponse<bool>()
            {
                Result = result
            };

            return response;
        }

        /// <summary>
        /// 提交盘点记录
        /// </summary>
        /// <param name="inventoryId">盘点任务的Id</param>
        /// <param name="model">Body数据</param>
        /// <returns></returns>
        [Route("submitInventory/{inventoryId:int}")]
        [HttpPost]
        public ApiResponse<bool> TransferAsset(int inventoryId, List<AssetInventoryRecordModel> model)
        {
            if (inventoryId <= 0)
            {
                throw new Exception("无效的inventoryId.");
            }
            var response = new ApiResponse<bool>()
            {
                Result = this.AssetInventoryService.SubmitInventoryInfo(model, inventoryId, this.Member.Id)
            };

            return response;
        }

        /// <summary>
        /// 获取我的盘点记录
        /// </summary>
        /// <param name="inventoryId">盘点任务的Id</param>
        /// <returns></returns>
        [Route("myInventory/{inventoryId:int}")]
        [HttpGet]
        public ApiResponse<AssetInventoryModel> GetMyInventory(int inventoryId)
        {
            if (this.Member == null)
            {
                throw new Exception("当前没有授权的用户");
            }
            var response = new ApiResponse<AssetInventoryModel>()
            {
                Result = this.AssetInventoryService.GetAssetInventory(inventoryId, this.Member.Id)
            };

            return response;
        }

        /// <summary>
        /// 隐藏金额
        /// </summary>
        /// <param name="list"></param>
        private void HiddenAmount(List<AssetModel> list)
        {
            foreach (var item in list)
            {
                if (item.AssetInfoes != null && item.AssetInfoes.Count > 0)
                {
                    foreach (var info in item.AssetInfoes)
                    {
                        if (info.AttributeName != null && info.AttributeName.Contains("金额"))
                        {
                            info.AttributeValue = "****";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 隐藏金额
        /// </summary>
        /// <param name="asset"></param>
        private void HiddenAmount(AssetModel asset)
        {
            if (asset.AssetInfoes != null && asset.AssetInfoes.Count > 0)
            {
                foreach (var info in asset.AssetInfoes)
                {
                    if (info.AttributeName != null && info.AttributeName.Contains("金额"))
                    {
                        info.AttributeValue = "****";
                    }
                }
            }
        }

    }
}
