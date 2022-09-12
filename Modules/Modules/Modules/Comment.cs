using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Modules
{
  [Table("Comments")]
  public class Comment
  {
    public int StudentId { get; set; }
    public int TeacherId { get; set; }
    public int CourseId { get; set; }
    public string Value { get; set; }
    public DateTime DateComment { get; set; }

    public static long Add(Comment comment)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return (conn.Insert<Comment>(comment));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return 0;
        }

      }
    }

    public static IEnumerable<(string comment, string courseName, string teacherName)> GetCommentsByStudentId(int studentId)
    {
      string sqlRequire = @"select c.Value, cs.CourseName, (t.FirstName + ' ' + t.LastName)
                            from Comments as c
                            LEFT JOIN Teachers as t ON t.ID = c.TeacherId
                            LEFT JOIN Courses as cs ON c.CourseID = cs.ID
                            WHERE c.StudentId = @id and c.DateComment > @date";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<(string, string, string)>(sqlRequire, new
          {
            id = studentId,
            date = DateTime.Now.AddDays(-3)
          });
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          Console.WriteLine(ex.Message);
          return null;
        }
      }

    }
  }
}
