using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;

namespace WebWhatsappBotFree
{
    class DTTOEXCEL
    {
        static Excel.Application xlApp;
         static Excel.Workbook xlWorkBook;
         static Excel.Worksheet xlWorkSheet;
        public static void excelolustur(DataTable dt, string excelPath, string sayfadi)
        {
            
           
            try {
                object misValue = System.Reflection.Missing.Value;
                xlApp = new Excel.Application();
                xlApp.Visible = false;
                xlApp.DisplayAlerts = false;
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkBook.Sheets[3].Delete();
                xlWorkBook.Sheets[2].Delete();
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                xlWorkSheet.Name = sayfadi;
                xlWorkSheet.Columns.NumberFormat = "@";
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    for (int j = 0; j <= dt.Columns.Count - 1; j++)
                    {
                        xlWorkSheet.Cells[i + 1, j + 1] = dt.Rows[i].ItemArray[j].ToString().Trim();
                    }
                }
                xlWorkSheet.Columns.AutoFit();
                xlWorkBook.SaveAs(excelPath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(false);
                xlApp.Quit();
                exceliramdensil(xlWorkSheet);
                exceliramdensil(xlWorkBook);
                exceliramdensil(xlApp);

            }
            catch
            {
                xlWorkBook.Close(false);
                xlApp.Quit();
                exceliramdensil(xlWorkSheet);
                exceliramdensil(xlWorkBook);
                exceliramdensil(xlApp);
            }
        }

        public static void exceliramdensil(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

    }
}

