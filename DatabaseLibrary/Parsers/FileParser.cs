using DatabaseLibrary.Model;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DatabaseLibrary.Parsers
{
    public class FileParser
    {
        private List<UserModel> _userModel;

        public FileParser()
        {
            _userModel = new List<UserModel>();
        }

        public List<UserModel> ParseXlsx(string pathFile)
        {
            using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(pathFile)))
            {
                var myWorksheet = xlPackage.Workbook.Worksheets.First(); 
                var totalRows = myWorksheet.Dimension.End.Row;
                var totalColumns = myWorksheet.Dimension.End.Column;

                var stringBuilder = new StringBuilder();
                for (int rowNum = 2; rowNum <= totalRows; rowNum++) 
                {
                   var valiesInRow = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns].Select(c => c.Value == null ? string.Empty : c.Value.ToString()).ToList();

                    _userModel.Add(ParseStringToUserModel(valiesInRow));
                }
            }

            return _userModel;
        }

        private UserModel ParseStringToUserModel(List<string> valiesInRow)
        {
            UserModel userModel = new UserModel();

            int index = 0;
            while (index < valiesInRow.Count() -1)
            {
                userModel.Name = valiesInRow[index];
                index++;
                userModel.LicensePlate = valiesInRow[index];
                index++;
                userModel.Address = valiesInRow[index];
                index++;
                userModel.Email = valiesInRow[index];
            }

            return userModel;
        }


        public void SaveXlsx(List<UserModel> userModelItems, string path)
        {
            ExcelPackage excelPackage = new ExcelPackage();
            ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

            for (int j = 1; j <= 4; j++) 
            {
                var column = excelWorksheet.Column(j);

                column.Width = 25;
                column.Style.Font.Size = 14;
            }

            var row = excelWorksheet.Row(1);

            row.Style.Font.Bold = true;
            row.Style.Font.Size = 16;

            excelWorksheet.SetValue(1, 1, "Name");
            excelWorksheet.SetValue(1, 2, "License Plate");
            excelWorksheet.SetValue(1, 3, "Address");
            excelWorksheet.SetValue(1, 4, "Email");

            int i = 0;
            int rowIndex = 2;
            while (i < userModelItems.Count)
            {
                 excelWorksheet.SetValue(rowIndex, 1, userModelItems[i].Name);
                 excelWorksheet.SetValue(rowIndex, 2, userModelItems[i].LicensePlate);
                 excelWorksheet.SetValue(rowIndex, 3, userModelItems[i].Address);
                 excelWorksheet.SetValue(rowIndex, 4, userModelItems[i].Email);
                i++;
                rowIndex++;
            }
            

            excelPackage.SaveAs(new FileInfo(path));
        }
    }
}
