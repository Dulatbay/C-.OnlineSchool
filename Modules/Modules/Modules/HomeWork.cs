using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Modules
{
  [Table("HomeWorks")]
  public class HomeWork
  {
    [Key]
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int TeacherId { get; set; }
    public int CourseId { get; set; }
    public DateTime? Term { get; set; }
    public byte[] HomeWorkFile { get; set; }
    public DateTime? DataLoad { get; set; }
    public string HomeWorkFileTitle { get; set; }
    public static long Add(HomeWork homeWork)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return (conn.Insert<HomeWork>(homeWork));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return 0;

        }
      }
    }

    public static IEnumerable<HomeWork> GetHomeWorksByGroupId(int groupId)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return (conn.Query<HomeWork>("select* from HomeWorks WHERE GroupId == @groupId", new { groupId = groupId }));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return new List<HomeWork>();
        }
      }
    }

    public static IEnumerable<(int homeWorkId, int studentId, int courseId, int groupId, string firstName, string lastName, string groupName, string courseName, DateTime dataLoad, byte[] file, string fileTitle)> GetHomeWorksByGroupIdWithCourseId(int groupId, int courseId, int teacherId)
    {

      string sqlRequire = @"select h.ID, s.ID, c.ID, g.ID, s.FirstName, s.LastName, g.GroupName, c.CourseName, phw.DataDownoload, phw.HomeWorkFile, phw.HomeWorkFileTitle
                            from ProcessedHomeWorks as phw
                            INNER JOIN Students as s ON s.ID = phw.StudentId
                            INNER JOIN HomeWorks as h ON h.ID = phw.HomeWorkId
                            INNER JOIN Groups as g ON g.ID = h.GroupId
                            INNER JOIN Courses as c ON c.ID = h.CourseId
                            WHERE h.TeacherId = @teacherID and g.ID = @groupID and c.ID = @courseID and phw.Grade is null";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<(int homeWorkId, int studentId, int courseId, int groupId, string firstName, string lastName, string groupName, string courseName, DateTime dataLoad, byte[] file, string fileTitle)>(sqlRequire, new
          {
            teacherID = teacherId,
            groupID = groupId,
            courseID = courseId,
          });
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.ToString());
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    public static IEnumerable<(string courseName, int grade)> GetCheckedHomeWorksByStudentId(int studentId)
    {
      var sqlRequire = @"select c.CourseName, phw.Grade
                         from ProcessedHomeWorks as phw
                         INNER JOIN HomeWorks as hw ON hw.ID = phw.HomeWorkId
                         INNER JOIN Courses as c ON c.ID = hw.CourseId
                         WHERE phw.DataChecked > @data and phw.StudentId = @studentId";
      string data = DateTime.Now.AddDays(-2).ToShortDateString();
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<(string, int)>(sqlRequire, new
          {
            studentId = studentId,
            data = data,
          });
        }
        catch (Exception ex)
        {
          Console.Write(ex.Message);
          return null;
        }
      }
    }

    // Вернуть невыполненные и непросроченные дз
    public static IEnumerable<(int homeWorkId, string courseName, byte[] file, DateTime term, string Title)> GetHomeWorksByStudentId(int studentId)
    {
      string sqlRequire = @"select h.ID, c.CourseName, h.HomeWorkFile, h.Term, h.HomeWorkFileTitle
                            from Students as s
                            INNER JOIN  HomeWorks as h ON h.GroupId = s.GroupID
                            INNER JOIN Courses as c ON c.ID = h.CourseId
                            WHERE h.ID NOT IN (select phw.HomeWorkId from ProcessedHomeWorks as phw WHERE HomeWorkId is not NULL and phw.StudentId = @id) and s.ID = @id and h.Term >= @date";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<(int, string, byte[], DateTime, string)>(sqlRequire, new
          {
            id = studentId,
            date = DateTime.Now
          });
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          MessageBox.Show(ex.ToString());
          MessageBox.Show(ex.StackTrace);
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    // Вернуть невыполненные и просроченные дз
    public static IEnumerable<(int homeWorkId, string courseName, byte[] file, DateTime term, string Title)> GetOverdueHomeWorksByStudentId(int studentId)
    {
      var sqlRequire = @"select h.ID, c.CourseName, h.HomeWorkFile, h.Term, h.HomeWorkFileTitle
                         from Students as s
                         INNER JOIN  HomeWorks as h ON h.GroupId = s.GroupID
                         INNER JOIN Courses as c ON c.ID = h.CourseId
                         WHERE h.ID NOT IN (select phw.HomeWorkId from ProcessedHomeWorks as phw WHERE HomeWorkId is not NULL and phw.StudentId = @id and phw.Grade is null) and s.ID = @id and h.Term < @data";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<(int homeWorkId, string courseName, byte[] file, DateTime term, string Title)>(sqlRequire,
            new
            {
              id = studentId,
              data = DateTime.Now
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


    public static IEnumerable<(string courseName, DateTime dateChecked, int grade, byte[] file, string fileTitle, DateTime dataUpload, byte[] taskFile, string taskFileTitle, string teacherFullName, DateTime term)> GetCompletedHomeWorksByStudentId(int studentId)
    {
      string sqlRequire = @"select c.CourseName, phw.DataChecked, phw.Grade, phw.HomeWorkFile, phw.HomeWorkFileTitle, phw.DataDownoload, h.HomeWorkFile, h.HomeWorkFileTitle, (t.FirstName + ' ' + t.LastName), h.Term
                            from ProcessedHomeWorks as phw
                            INNER JOIN HomeWorks as h ON h.ID = phw.HomeWorkId
                            INNER JOIN Courses as c ON c.ID = h.CourseId 
                            INNER JOIN Teachers as t ON t.ID = h.TeacherId
                            WHERE phw.Grade is not null and phw.StudentId = @id";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<(string, DateTime, int, byte[], string, DateTime, byte[], string, string, DateTime)>(sqlRequire, new
          {
            id = studentId
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

    public static IEnumerable<(int homeWorkId, string courseName, byte[] file, DateTime term, string title)> GetHomeWorksByTeacherId(int teacherId, int courseId, int groupId)
    {
      string sqlRequire = @"select h.ID, c.CourseName, h.HomeWorkFile, h.Term, h.HomeWorkFileTitle
                            from HomeWorks as h
                            inner join Courses as c ON c.ID = h.CourseId
                            where @teacherId = h.TeacherId and h.GroupId = @groupId and h.CourseId = @courseId";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<(int, string, byte[], DateTime, string)>(sqlRequire, new
          {
            teacherId = teacherId,
            courseId = courseId,
            groupId = groupId
          });
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    public static IEnumerable<(int studentId, string courseName, double avg, int count)> GetHomeWorksDetailInfoByStudentId(int studentId, DateTime dateTime)
    {
      string sqlRequire = @"select s.ID, c.CourseName, AVG(phw.Grade), COUNT(phw.HomeWorkId)
                            from Students as s
                            LEFT JOIN HomeWorks as hw ON hw.GroupId = s.GroupID
                            LEFT JOIN Courses as c ON c.ID = hw.CourseId
                            LEFT JOIN ProcessedHomeWorks as phw ON phw.StudentId = s.ID and hw.ID = phw.HomeWorkId and hw.CourseId = hw.CourseId
                            WHERE s.ID = @studentId and phw.DataChecked > @date
                            GROUP BY s.ID, c.CourseName";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<(int, string, double, int)>(sqlRequire, new
          {
            studentId = studentId,
            date = dateTime
          });
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

  }
}
