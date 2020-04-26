using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace CitySouth.Test
{
    public class Excel
    {
        private DataTable dataTable;
        private XSSFWorkbook xssfworkbook;
        public Excel(DataTable dataTable)
        {
            this.dataTable = dataTable;
            InitializeWorkbook();
            FillExcel();
        }
        public static DataTable ExcelToDataTable(string fileName)
        {
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                IWorkbook workbook = null;
                if (fileName.EndsWith(".xlsx"))
                    workbook = new XSSFWorkbook(fs);
                else
                    workbook = new HSSFWorkbook(fs);
                sheet = workbook.GetSheetAt(0);
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        ICell cell = firstRow.GetCell(i);
                        if (cell != null)
                        {
                            string cellValue = cell.StringCellValue;
                            if (cellValue != null)
                            {
                                while (data.Columns.Contains(cellValue))
                                    cellValue = cellValue + "1";
                                DataColumn column = new DataColumn(cellValue);
                                data.Columns.Add(column);
                            }
                        }
                    }
                    startRow = sheet.FirstRowNum + 1;

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　

                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            ICell cell = row.GetCell(j);
                            if (cell != null) //同理，没有数据的单元格都默认是null
                                dataRow[j] = cell.ToString();
                        }
                        data.Rows.Add(dataRow);
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
        }
        public void Save(string filename)
        {
            FileStream file = new FileStream(filename, FileMode.Create);
            xssfworkbook.Write(file);
            file.Close();
            xssfworkbook.Close();
        }
        public MemoryStream ToStream()
        {
            MemoryStream ms = new MemoryStream();
            xssfworkbook.Write(ms);
            xssfworkbook.Close();
            return ms;
        }
        private void FillExcel() {
            ICell cell;
            IRow row;
            ISheet sheet = xssfworkbook.CreateSheet("Sheet1");
            int colIndex = 0;
            row = sheet.CreateRow(0);
            foreach (DataColumn col in dataTable.Columns)
            {
                cell = row.CreateCell(colIndex);
                cell.SetCellValue(col.ColumnName);
                colIndex++;
            }
            int rowIndex = 1;
            foreach (DataRow dr in dataTable.Rows)
            {
                row = sheet.CreateRow(rowIndex);
                for (colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++)
                {
                    cell = row.CreateCell(colIndex);
                    cell.SetCellValue(dr[colIndex].ToString());
                }
                rowIndex++;
            }
        }
        private void InitializeWorkbook()
        {
            xssfworkbook = new XSSFWorkbook();
            //DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            //dsi.Company = "贵州财经大学图书馆";
            //xssfworkbook.set = dsi;
            //SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            //si.Subject = "科研支持中心制造";
            //xssfworkbook.SummaryInformation = si;
        }
    }
}
