using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using log4net;
using MissionskyOA.Models;
using MissionskyOA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using System.IO;
using MissionskyOA.Core.Common;
using ThoughtWorks.QRCode.Codec;

namespace MissionskyOA.Portal.Controllers
{
    [AuthorizeFilter]
    public class BookController : Controller
    {
        /// <summary>
        /// Log instance.
        /// </summary>
        protected static readonly ILog Log = LogManager.GetLogger(typeof (BookController));

        private IBookService BookService;
        private IUserService UserService;

        //
        // GET: /Area/

        public BookController(IBookService bookService, IUserService userService)
        {
            this.BookService = bookService;
            this.UserService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 异步加载数据
        /// </summary>
        /// <param name="dRequest"></param>
        /// <returns></returns>
        public ActionResult Read([DataSourceRequest] DataSourceRequest dRequest)
        {
            SortModel sort = null;
            if (dRequest.Sorts != null && dRequest.Sorts.Count > 0)
            {
                sort = dRequest.Sorts[0].ToSortModel();
            }
            FilterModel filter = null;
            if (dRequest.Filters != null && dRequest.Filters.Count > 0)
            {
                filter = ((FilterDescriptor) dRequest.Filters[0]).ToSortModel();
            }
            var areaResult = BookService.List(dRequest.Page, dRequest.PageSize, sort, filter);
            DataSourceResult result = new DataSourceResult()
            {
                Data = areaResult.Data,
                Total = areaResult.Total
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
            var userList = new List<SelectListItem>();
            var allUsers = UserService.GetAllUsers();
            userList.Add(new SelectListItem() {Value = "0", Text = "--Please select a user--"});
            foreach (UserModel p in allUsers)
            {
                userList.Add(new SelectListItem() {Value = (p.Id).ToString(), Text = p.EnglishName});
            }
            ViewData["ProjectManagerList"] = new SelectList(userList, "Value", "Text", "0");
            if (id.HasValue && id.Value > 0)
            {
                ViewBag.Title = "编辑书籍信息";
                ViewBag.isAddNew = false;
                var model = this.BookService.GetBookDetail(id.Value, 0);
                if (model.Donor != null && model.Donor == 0)
                {
                    ViewData["ProjectManagerList"] = new SelectList(userList, "Value", "Text", model.Donor.ToString());
                }
                return View(model);
            }
            else
            {
                ViewBag.Title = "添加书籍";
                ViewBag.isAddNew = true;
                BookModel model = new BookModel();
                return View(model);
            }
        }

        public ActionResult Upload(int id)
        {
            ViewBag.Title = "上传封面";
            //var model = this.BookService.GetBookDetail(id.Value);
            ViewBag.CurrentBookID = id;
            return View();
        }

        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AttachmentUpload(IEnumerable<HttpPostedFileBase> files, int bookId)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            foreach (var file in files)
            {
                Stream fileStream = file.InputStream;
                var buffer = new byte[file.ContentLength];
                var fileName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                var photoId = Guid.NewGuid().ToString("N");
                fileStream.Read(buffer, 0, file.ContentLength);

                var cover = new AttachmentModel()
                {
                    Content = buffer,
                    EntityId = bookId,
                    EntityType = Constant.ATTACHMENT_TYPE_BOOK_COVER,
                    Desc = file.FileName,
                    Name = fileName,
                    CreatedTime = DateTime.Now
                };
                if (ModelState.IsValid)
                {
                    try
                    {
                        this.BookService.UploadCover(cover);
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = ex.Message;
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// 编辑提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(BookModel model)
        {
            string action = Request["Submit"];
            if (action == "cancel")
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Id > 0)
                    {

                        this.BookService.UpdateBookInfo(model);

                    }
                    else
                    {
                        var book = new BaseBookModel()
                        {
                            Name = model.Name,
                            ISBN = model.ISBN,
                            Author = model.Author,
                            Desc = model.Desc,
                            Source = model.Source,
                            Donor = model.Donor,
                            Type = model.Type
                        };
                        this.BookService.Add(book);
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
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                this.BookService.Delete(id);
                return Json("OK");
            }
            catch (Exception ex)
            {
                return Json(new {error = ex.Message});
            }

        }

        /// <summary>
        /// 导出图书二维码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportQR()
        {
            var books = this.BookService.GetBookList(null);
            try
            {
                if (books == null)
                {
                    throw new KeyNotFoundException("未找到图书。");
                }

                if (!Directory.Exists(Server.MapPath("~/QRImages/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/QRImages/"));
                }

                foreach (var book in books)
                {
                    string imgName = Server.MapPath("~/QRImages/") + book.Name + "_" + book.ISBN + "_" + book.Id +
                                     ".jpg";
                    if (System.IO.File.Exists(imgName))
                    {
                        System.IO.File.Delete(imgName);
                    }

                    Bitmap img = MakeQRImage(book);

                    if (img != null)
                    {
                        img.Save(imgName);
                    }
                }

                string zipFile = "";
                bool bol = ZipFile(ref zipFile);
                if (bol)
                {
                    return File(zipFile, "application/x-zip-compressed",
                        "图书二维码-" + DateTime.Now.ToString("yyyy-MM-dd") + ".zip");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        /// <summary>
        /// 显示二维码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ShowQR(int id)
        {
            ViewData["BookId"] = id;
            return View();
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        private Bitmap MakeQRImage(BookModel book)
        {
            string imgName = Server.MapPath("~/QRImages/") + book.BarCode + ".jpg";
            if (System.IO.File.Exists(imgName))
            {
                System.IO.File.Delete(imgName);
            }

            string enCodeString = book.BarCode;
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            qrCodeEncoder.QRCodeScale = 3;
            Bitmap qrImg = qrCodeEncoder.Encode(enCodeString, Encoding.UTF8);

            Bitmap resultImage = new Bitmap(qrImg.Width + 20, qrImg.Height + 20);
            Graphics gResult = Graphics.FromImage(resultImage);
            gResult.Clear(System.Drawing.Color.White);
            if (System.IO.File.Exists(Server.MapPath("~/Content/images/logo.jpg"))) //如果有logo的话则添加logo
            {
                Bitmap btm = new Bitmap(Server.MapPath("~/Content/images/logo.jpg"));
                Bitmap copyImage = new Bitmap(btm, qrImg.Width/3, qrImg.Height/3);
                Graphics g = Graphics.FromImage(qrImg);
                int x = qrImg.Width/2 - copyImage.Width/2;
                int y = qrImg.Height/2 - copyImage.Height/2;
                g.DrawImage(copyImage, x, y);
            }

            gResult.DrawImage(qrImg, 10, 10);

            return resultImage;
        }

        //压缩文件
        private bool ZipFile(ref string zipFilePath)
        {
            if (!Directory.Exists(Server.MapPath("~/QRImages/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/QRImages/"));
            }

            if (!Directory.Exists(Server.MapPath("~/Temporary/")))
            {
                Directory.CreateDirectory(Server.MapPath("~/Temporary/"));
            }

            string dirPath = Server.MapPath("~/QRImages");
            zipFilePath = Server.MapPath("~/Temporary/") + "图书二维码-" + DateTime.Now.ToString("yyyy-MM-dd") + ".zip";

            if (!Directory.Exists(dirPath))
            {
                Log.Error("要压缩的文件夹不存在！");
                return false;
            }
            try
            {
                string[] filenames = Directory.GetFiles(dirPath);
                using (ZipOutputStream s = new ZipOutputStream(System.IO.File.Create(zipFilePath)))
                {
                    s.SetLevel(9);
                    byte[] buffer = new byte[1024*10];
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
                Log.Error(ex);
                return false;
            }
            return true;
        }
    }
}
