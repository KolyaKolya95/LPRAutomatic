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
    }
}
