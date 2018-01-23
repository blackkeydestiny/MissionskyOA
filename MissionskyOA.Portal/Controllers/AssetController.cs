using Kendo.Mvc;
using Kendo.Mvc.UI;
using MissionskyOA.Core.Enum;
using MissionskyOA.Models;
using MissionskyOA.Portal.Common;
using MissionskyOA.Portal.Extionsions;
using MissionskyOA.Portal.Models;
using MissionskyOA.Services;
using PagedList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;

namespace MissionskyOA.Portal.Controllers
{
    [AuthorizeFilter]
    public class AssetController : Controller
    {
        IUserService UserService;
        IAssetAttributeService AssetAttributeService;
        IAssetTypeService AssetTypeService;
        IAssetService AssetService;
        //
        // GET: /Area/

        public AssetController(IUserService userService,
            IAssetTypeService assetTypeService,
            IAssetService assetService,
            IAssetAttributeService assetAttributeService)
        {
            this.UserService = userService;
            this.AssetTypeService = assetTypeService;
            this.AssetService = assetService;
            this.AssetAttributeService = assetAttributeService;

            var types = new List<SelectListItem>();
            types.Add(new SelectListItem() { Text = "--选择分类--", Value = null });
            this.AssetTypeService.GetAll().ForEach(it =>
            {
                types.Add(new SelectListItem() { Text = it.Name, Value = it.Id.ToString() });
            });
            ViewBag.Types = types;
        }

        public ActionResult Index(string barCode, string userName, int? typeId, DateTime? buyDate, int page = 1)
        {
            AssetSearchModel search = new AssetSearchModel()
            {
                BarCode = barCode,
                TypeId = typeId,
                BuyDate = buyDate,
                UserName = userName
            };
            var assets = this.AssetService.List(search);
            var attrs = this.AssetAttributeService.GetAll();
            var barcodeAttr = attrs.Where(it => it.Name == "编号").FirstOrDefault();
            if (Request["Submit"] != null)
            {
                string action = Request["Submit"];
                //导出二维码
                if (action == "exportQR")
                {
                    try
                    {
                        if (barcodeAttr == null)
                        {
                            throw new Exception("资产属性没有配置编号");
                        }
                        if (!Directory.Exists(Server.MapPath("~/QRImages/")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/QRImages/"));
                        }
                        foreach (var item in assets)
                        {
                            var barcode = item.AssetInfoes.Where(it => it.AttributeId == barcodeAttr.Id).FirstOrDefault();
                            if (barcode != null)
                            {
                                string enCodeString = barcode.AttributeValue;
                                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                                qrCodeEncoder.QRCodeScale = 3;
                                Bitmap qrImg = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);

                                Bitmap resultImage = new Bitmap(qrImg.Width + 20, qrImg.Height + 20);
                                Graphics gResult = Graphics.FromImage(resultImage);
                                gResult.Clear(System.Drawing.Color.White);
                                if (System.IO.File.Exists(Server.MapPath("~/Content/images/logo.jpg")))//如果有logo的话则添加logo
                                {
                                    Bitmap btm = new Bitmap(Server.MapPath("~/Content/images/logo.jpg"));
                                    Bitmap copyImage = new Bitmap(btm, qrImg.Width / 3, qrImg.Height / 3);
                                    Graphics g = Graphics.FromImage(qrImg);
                                    int x = qrImg.Width / 2 - copyImage.Width / 2;
                                    int y = qrImg.Height / 2 - copyImage.Height / 2;
                                    g.DrawImage(copyImage, x, y);
                                }
                                gResult.DrawImage(qrImg, 10, 10);
                                resultImage.Save(Server.MapPath("~/QRImages/") + barcode.AttributeValue + ".jpg");
                            }
                        }
                        string zipFile = "";
                        bool bol = ZipFile(ref zipFile);
                        if (bol)
                        {
                            return File(zipFile, "application/x-zip-compressed", "资产二维码-" + DateTime.Now.ToString("yyyy-MM-dd") + ".zip");
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = ex.Message;
                    }
                }
                //导出数据
                else if (action == "export")
                {
                    try
                    {
                        if (barcodeAttr == null)
                        {
                            throw new Exception("资产属性没有配置编号");
                        }
                        var workbook = new Aspose.Cells.Workbook();
                        var descriptor = new ExportExcel<AssetModel>();
                        descriptor.FillAssetData(assets, workbook, barcodeAttr.Id);

                        var ms = new MemoryStream();
                        string fileName = "资产信息-" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";
                        workbook.Save(ms, Aspose.Cells.SaveFormat.Xlsx);
                        return File(ms.ToArray(), "application/vnd.ms-excel", fileName);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = ex.Message;
                    }
                }
            }

            AssetList assetList = new AssetList()
            {
                BarCode = barCode,
                TypeId = typeId,
                BuyDate = buyDate,
                UserName = userName
            };

            if (barcodeAttr != null)
            {
                assetList.BarCodeAttrId = barcodeAttr.Id;
            }
            else
            {
                assetList.BarCodeAttrId = 0;
            }

            int pageSize = 15;
            assetList.List = assets.AsEnumerable().ToPagedList(page, pageSize);
            return View(assetList);
        }

        /// <summary>
        /// 根据TypeId获取属性
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public JsonResult GetAssetAttributesByTypeId(int typeId)
        {
            var types = this.AssetTypeService.GetAll();
            var type = types.Where(it => it.Id == typeId).FirstOrDefault();
            if (type != null)
            {
                return Json(type.Attributes, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUsers([DataSourceRequest]DataSourceRequest dRequest)
        {
            var members = this.UserService.GetAllUsers();
            return Json(members, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadTypes([DataSourceRequest]DataSourceRequest dRequest)
        {
            var types = AssetTypeService.GetAll();
            DataSourceResult result = new DataSourceResult()
            {
                Data = types,
                Total = types.Count
            };

            return Json(result);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                var model = this.AssetService.GetAssetById(id.Value);
                return View(model);
            }
            else
            {
                AssetModel model = new AssetModel();
                return View(model);
            }
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(AssetModel model, FormCollection form)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            if (model.TypeId == 0)
            {
                ModelState.AddModelError("TypeId", "请选择分类");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (form != null && form.AllKeys != null && form.AllKeys.Length > 0)
                    {
                        if (model.AssetInfoes == null)
                        {
                            model.AssetInfoes = new List<AssetInfoModel>();
                        }
                        foreach (var item in form.AllKeys.Where(it => it.StartsWith("###")))
                        {
                            var attrModel = new AssetInfoModel();
                            string[] attrIds = item.Split('$');
                            attrModel.AttributeId = int.Parse(attrIds[1]);
                            attrModel.AttributeValue = form[item];
                            model.AssetInfoes.Add(attrModel);
                        }
                    }
                    if (model.Id > 0)
                    {
                        this.AssetService.Update(model);
                    }
                    else
                    {
                        this.AssetService.Add(model);
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }

            return View(model);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BatchDelete(int[] ids)
        {
            try
            {
                this.AssetService.BatchRemove(ids);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                this.AssetService.Remove(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        public ActionResult Import()
        {
            AssetImport model = new AssetImport();
            return View(model);
        }

        /// <summary>
        /// 提交导入数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import(AssetImport model, HttpPostedFileBase file)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            if (!model.TypeId.HasValue || model.TypeId.Value <= 0)
            {
                ModelState.AddModelError("TypeId", "请选择分类.");
            }
            if (file == null)
            {
                ModelState.AddModelError("DataFile", "文件不能为空.");
            }

            if (file != null)
            {
                if (Path.GetExtension(file.FileName).ToLower() != ".xlsx")
                {
                    ModelState.AddModelError("DataFile", "文件格式必须是.xlsx");
                    return View(model);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var typeAttributes = this.AssetAttributeService.GetAll();

                    var barcodeAttr = typeAttributes.Where(it => it.Name == "编号").FirstOrDefault();
                    if (barcodeAttr == null)
                    {
                        ModelState.AddModelError("", "[编号]是资产必须的,请先在系统添加[编号]属性.");
                        return View(model);
                    }

                    var buyDateAttr = typeAttributes.Where(it => it.Name == "采购日期").FirstOrDefault();
                    if (buyDateAttr == null)
                    {
                        ModelState.AddModelError("", "[采购日期]是资产必须的,请先在系统添加[采购日期]属性.");
                        return View(model);
                    }

                    var orderAreaAttr = typeAttributes.Where(it => it.Name == "入账区域").FirstOrDefault();
                    if (orderAreaAttr == null)
                    {
                        ModelState.AddModelError("", "[入账区域]是资产必须的,请先在系统添加[入账区域]属性.");
                        return View(model);
                    }


                    var typeAttribute = this.AssetTypeService.GetTypeById(model.TypeId.Value);
                    var barcodeRelation = typeAttribute.Attributes.Where(it => it.Id == barcodeAttr.Id).FirstOrDefault();
                    if (null == barcodeRelation)
                    {
                        ModelState.AddModelError("", "[编号]是资产必须的,请先在系统将[编号]和导入类型进行关联设置.");
                        return View(model);
                    }

                    var buyDateRelation = typeAttribute.Attributes.Where(it => it.Id == buyDateAttr.Id).FirstOrDefault();
                    if (null == buyDateRelation)
                    {
                        ModelState.AddModelError("", "[采购日期]是资产必须的,请先在系统将[采购日期]和导入类型进行关联设置.");
                        return View(model);
                    }

                    var orderAreaRelation = typeAttribute.Attributes.Where(it => it.Id == orderAreaAttr.Id).FirstOrDefault();
                    if (null == orderAreaRelation)
                    {
                        ModelState.AddModelError("", "[入账区域]是资产必须的,请先在系统将[入账区域]和导入类型进行关联设置.");
                        return View(model);
                    }

                    var workbook = new Aspose.Cells.Workbook(file.InputStream);
                    var ws = workbook.Worksheets[0];
                    var rows = ws.Cells.MaxDataRow;
                    var cols = ws.Cells.MaxDataColumn;

                    //检查必填字段[采购日期]
                    bool hasBuyDate = false;
                    int colBuyDateIndex = -1;
                    for (int i = 0; i <= cols; i++)
                    {
                        if (null != ws.Cells[0, i].Value && ws.Cells[0, i].Value.ToString() == "采购日期")
                        {
                            hasBuyDate = true;
                            colBuyDateIndex = i;
                            break;
                        }
                    }
                    if (!hasBuyDate)
                    {
                        ModelState.AddModelError("", "[采购日期]是资产必须的,请修改表格后再导入.");
                        return View(model);
                    }

                    //Aspose.Cells.CellArea firstRow=new Aspose.Cells.CellArea()
                    //{
                    // StartRow=0,
                    // EndRow=0,
                    // StartColumn=0,
                    // EndColumn=cols
                    //};
                    //ws.Cells.FindString("入账区域",ws.Cells[0,0],firstRow);
                    //Cell Cells.FindString(inputString,previousCell,area);
                    bool hasOrderArea = false;
                    int colOrderAreaIndex = -1;
                    for (int i = 0; i <= cols; i++)
                    {
                        if (null != ws.Cells[0, i].Value && ws.Cells[0, i].Value.ToString() == "入账区域")
                        {
                            hasOrderArea = true;
                            colOrderAreaIndex = i;
                            break;
                        }
                    }
                    if (!hasOrderArea)
                    {
                        ModelState.AddModelError("", "[入账区域]是资产必须的,请修改表格后再导入.");
                        return View(model);
                    }

                    bool hasUseUser = false;
                    int colsUseUserIndex = -1;
                    for (int i = 0; i <= cols; i++)
                    {
                        if (null != ws.Cells[0, i].Value && ws.Cells[0, i].Value.ToString() == "使用人")
                        {
                            hasUseUser = true;
                            colsUseUserIndex = i;
                            break;
                        }
                    }

                    var allUsers = this.UserService.GetAllUsers();
                    var defaultUser = this.UserService.GetAllUsers().Where(it => it.Email.ToLower() == "assets.manager@missionsky.com").First();
                    if (defaultUser == null)
                    {
                        ModelState.AddModelError("", "未在系统找到默认使用人:assets.manager@missionsky.com");
                        return View(model);
                    }

                    string codePrefix = "";
                    switch (typeAttribute.Name)
                    {
                        case "主机":
                            codePrefix = "HC";
                            break;
                        case "显示器":
                            codePrefix = "LD";
                            break;
                        case "MACMINI电脑":
                            codePrefix = "MC";
                            break;
                        case "笔记本电脑":
                            codePrefix = "NC";
                            break;
                        case "平板电脑":
                            codePrefix = "TC";
                            break;
                        case "手机":
                            codePrefix = "MP";
                            break;
                        case "午休床":
                            codePrefix = "TY";
                            break;
                        case "其他固定资产":
                            codePrefix = "OA";
                            break;
                        case "其他低值易耗品":
                            codePrefix = "OG";
                            break;
                        default:
                            codePrefix = "OA";
                            break;
                    }
                    List<AssetModel> importList = new List<AssetModel>();
                    for (int row = 1; row <= rows; row++)
                    {
                        AssetModel newModel = new AssetModel()
                        {
                            TypeId = model.TypeId.Value,
                            Status = AssetStatus.Normal,
                            UserId = defaultUser.Id,
                            AssetInfoes = new List<AssetInfoModel>()
                        };
                        #region 属性
                        for (int column = 0; column <= cols; column++)
                        {
                            //资产使用人设置
                            if (hasUseUser && colsUseUserIndex == column)
                            {
                                if (ws.Cells[row, column].Value != null && ws.Cells[row, column].Value.ToString().Trim() != "")
                                {
                                    var assetUser = allUsers.Where(it => it.EnglishName.Trim().ToLower() == ws.Cells[row, column].Value.ToString().Trim().ToLower()).FirstOrDefault();
                                    if (assetUser == null)
                                    {
                                        ModelState.AddModelError("", "未在系统找到使用人:" + ws.Cells[row, column].Value.ToString());
                                        return View(model);
                                    }
                                    newModel.UserId = assetUser.Id;
                                }
                                continue;
                            }
                            //采购日期
                            if (colBuyDateIndex == column)
                            {
                                if (ws.Cells[row, column].Value != null && ws.Cells[row, column].Value.ToString().Trim() != "")
                                {
                                    DateTime tempDate = DateTime.Now;
                                    if (!DateTime.TryParse(ws.Cells[row, column].Value.ToString(), out tempDate))
                                    {
                                        ModelState.AddModelError("", "第" + (row + 1) + "行[采购日期]日期格式不正确.");
                                        return View(model);
                                    }
                                    var attrModel = new AssetInfoModel();
                                    attrModel.AttributeId = buyDateAttr.Id;
                                    attrModel.AttributeValue = tempDate.ToString("MM/dd/yyyy");
                                    newModel.AssetInfoes.Add(attrModel);
                                }
                                else
                                {
                                    var attrModel = new AssetInfoModel();
                                    attrModel.AttributeId = buyDateAttr.Id;
                                    attrModel.AttributeValue = DateTime.Now.ToString("MM/dd/yyyy");
                                    newModel.AssetInfoes.Add(attrModel);
                                }
                                continue;
                            }
                            //入账区域
                            if (colOrderAreaIndex == column)
                            {
                                if (ws.Cells[row, column].Value != null && ws.Cells[row, column].Value.ToString().Trim() != "")
                                {
                                    var text = ws.Cells[row, column].Value.ToString().Trim();
                                    if (text.ToLower() != "sz" && text.ToLower() != "hk")
                                    {
                                        text = "SZ";
                                    }
                                    var attrModel = new AssetInfoModel();
                                    attrModel.AttributeId = orderAreaAttr.Id;
                                    attrModel.AttributeValue = text.ToUpper();
                                    newModel.AssetInfoes.Add(attrModel);
                                }
                                else
                                {
                                    var attrModel = new AssetInfoModel();
                                    attrModel.AttributeId = orderAreaAttr.Id;
                                    attrModel.AttributeValue = "SZ";
                                    newModel.AssetInfoes.Add(attrModel);
                                }
                                continue;
                            }
                            if (ws.Cells[row, column].Value != null && ws.Cells[row, column].Value.ToString().Trim() != "")
                            {
                                var attrCol = typeAttributes.Where(it => it.Name == ws.Cells[0, column].Value.ToString()).FirstOrDefault();
                                if (attrCol != null)
                                {
                                    var attrColRelation = typeAttribute.Attributes.Where(it => it.Id == attrCol.Id).FirstOrDefault();
                                    if (attrColRelation != null)
                                    {
                                        //添加其他字段不能并保证不重复写入
                                        var addedAttrIds = newModel.AssetInfoes.Select(it => it.AttributeId).ToList();
                                        if (!addedAttrIds.Contains(attrCol.Id))
                                        {
                                            var attrModel = new AssetInfoModel();
                                            attrModel.AttributeId = attrCol.Id;
                                            attrModel.AttributeValue = ws.Cells[row, column].Value.ToString().Trim();
                                            newModel.AssetInfoes.Add(attrModel);
                                        }
                                    }
                                }
                            }
                        }
                        //资产类型已经关联的字段而Excel里面没有的都添加为空(确保字段数量一致),除去条码字段,条码属于自动生成
                        var allAddedAttrIds = newModel.AssetInfoes.Select(it => it.AttributeId).ToList();
                        foreach (var relatedAttr in typeAttribute.Attributes.Where(it => it.Id != barcodeAttr.Id))
                        {
                            if (!allAddedAttrIds.Contains(relatedAttr.Id))
                            {
                                newModel.AssetInfoes.Add(new AssetInfoModel()
                                {
                                    AttributeId = relatedAttr.Id,
                                    AttributeValue = null
                                });
                            }
                        }
                        #endregion
                        importList.Add(newModel);
                    }

                    ViewBag.Message = this.AssetService.Import(importList, codePrefix);
                    return View(model);
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                }
            }

            return View(model);
        }


        //压缩文件
        public bool ZipFile(ref string zipFilePath)
        {
            string err = "";
            if (!Directory.Exists(Server.MapPath("~/QRImages/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/QRImages/"));
            }
            if (!Directory.Exists(Server.MapPath("~/Temporary/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Temporary/"));
            }
            string dirPath = Server.MapPath("~/QRImages");
            zipFilePath = Server.MapPath("~/Temporary/") + "资产二维码-" + DateTime.Now.ToString("yyyy-MM-dd") + ".zip";
            if (!Directory.Exists(dirPath))
            {
                err = "要压缩的文件夹不存在！";
                return false;
            }
            try
            {
                string[] filenames = Directory.GetFiles(dirPath);
                using (ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(zipFilePath)))
                {
                    s.SetLevel(9);
                    byte[] buffer = new byte[1024 * 10];
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = System.IO.File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                    //删除图片
                    foreach (string file in filenames)
                    {
                        System.IO.File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            return true;
        }


    }
}