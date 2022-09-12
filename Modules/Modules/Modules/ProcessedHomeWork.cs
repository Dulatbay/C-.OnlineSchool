using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Modules
{
  [Table("ProcessedHomeWorks")]
  public class ProcessedHomeWork
  {
    public int StudentId { get; set; }
    public int HomeWorkId { get; set; }

    public DateTime? DataDownoload { get; set; }

    public int? Grade { get; set; }

    public byte[] HomeWorkFile { get; set; }
    public DateTime? DataChecked { get; set; }
    public string HomeWorkFileTitle { get; set; }
    public static long Add(ProcessedHomeWork processedHomeWork)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return (conn.Insert<ProcessedHomeWork>(processedHomeWork));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return 0;

        }
      }
    }
    public static bool CheckHomeWork(int studentId, int homeWorkId, int grade)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          var result = conn.QueryFirstOrDefault<ProcessedHomeWork>("UPDATE ProcessedHomeWorks SET DataChecked = @datatime, Grade = @grade WHERE StudentId = @studentId and HomeWorkId = @homeworkid", new
          {
            datatime = DateTime.Now,
            grade = grade,
            studentId = studentId,
            homeworkid = homeWorkId,
          });
          return conn.Update(result);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return false;
        }
      }
    }

    public static IEnumerable<(int homeWorkId, string courseName, byte[] file,  string fileTitle, DateTime term, DateTime dataDownoload, byte[] homeWorkFile, string homeWorkFileTitle)> GetUnderReviewHomeWorksByStundentId(int studentId)
    {
      string sqlRequire = @"select h.ID, c.CourseName, phw.HomeWorkFile, phw.HomeWorkFileTitle, h.Term, phw.DataDownoload, h.HomeWorkFile, h.HomeWorkFileTitle  
                            from ProcessedHomeWorks as phw
                            INNER JOIN Students as s ON s.ID = phw.StudentId
                            INNER JOIN HomeWorks as h ON h.ID = phw.HomeWorkId
                            INNER JOIN Courses as c ON c.ID = h.CourseId
                            WHERE s.ID = @studentId and phw.DataChecked is null and phw.DataDownoload is not null";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<(int, string, byte[], string, DateTime, DateTime, byte[], string)>(sqlRequire, new
          {
            studentId = studentId
          });
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          MessageBox.Show(ex.Message);
          return null;
        }
      }
    }
  }
}
