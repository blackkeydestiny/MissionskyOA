using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Aspose.Cells;
using MissionskyOA.Models;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace MissionskyOA.Portal.Extionsions
{
    public class ExportExcel<T>
    {
        /// <summary>
        /// 生成Excel文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Workbook GetExcel(HttpContextBase context, IEnumerable<T> source)
        {
            var wb = new Workbook();
            var ws = wb.Worksheets[0];

            return wb;
        }

        public void FillAssetData(List<AssetModel> source, Workbook wb, int barcodeId)
        {
            Style style1 = wb.Styles[wb.Styles.Add()];//新增样式
            style1.IsTextWrapped = false;//单元格内容自动换行
            style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //应用边界线 左边界线
            style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //应用边界线 右边界线
            style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin; //应用边界线 上边界线
            style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //应用边界线 下边界线
            wb.Worksheets.RemoveAt(0);//移除第一个空sheet
            var groupTypes = source.Select(it => new { Id = it.TypeId, Name = it.TypeName }).Distinct();
            
            foreach (var type in groupTypes)
            {
                int row = 0;
                wb.Worksheets.Add(type.Name);
                var ws = wb.Worksheets[type.Name];
                var itemsByType = source.Where(it => it.TypeId == type.Id).ToList();

                //添加表头
                var first = itemsByType.First();
                ws.Cells[row, 0].Value = "类别";
                ws.Cells[row, 1].Value = "状态";
                ws.Cells[row, 2].Value = "用户";
                int headerCols = 3;
                foreach (var attr in first.AssetInfoes)
                {
                    ws.Cells[0, headerCols].Value = attr.AttributeName;
                    headerCols += 1;
                }
                row += 1;

                //填充数据
                foreach (var item in itemsByType)
                {
                    ws.Cells[row, 0].Value = item.TypeName;
                    ws.Cells[row, 1].Value = item.StatusName;
                    ws.Cells[row, 2].Value = item.UserName;
                    int col = 3;
                    foreach (var assetInfo in item.AssetInfoes)
                    {
                        ws.Cells[row, col].Value = assetInfo.AttributeValue;
                        col += 1;
                    }
                    row += 1;
                }
                Aspose.Cells.Range r = ws.Cells.CreateRange(0, 0, row, headerCols);
                r.SetStyle(style1);
            }

        }
    }
}