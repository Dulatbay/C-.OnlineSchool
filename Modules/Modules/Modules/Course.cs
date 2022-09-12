using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Modules
{
  [Table("Courses")]
  public class Course
  {
    [Key]
    public int Id { get; set; }
    public string CourseName { get; set; }

    public static long Add(Course course)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {

        try
        {
          return (conn.Insert<Course>(course));
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          Console.WriteLine(ex.Message);
          return 0;
        }
      }
    }
    public static Course GetCourseByName(string name)
    {
      string sqlRequire = "select* from Courses WHERE @name = CourseName";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.QueryFirstOrDefault<Course>(sqlRequire, new
          {
            name = name
          });
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    public static IEnumerable<Course> GetAllCourses()
    {
      string sqlRequire = "select* from Courses";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<Course>(sqlRequire);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    public static IEnumerable<Course> GetCoursesByTeacherId(int teacherId)
    {
      string sqlRequire = @"select c.ID, c.CourseName
                            from TeacherCourses as tc
                            INNER JOIN Courses as c ON c.ID = tc.CourseID
                            WHERE tc.TeacherID = @id";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<Course>(sqlRequire, new
          {
            id = teacherId
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

    public static IEnumerable<Course> GetCoursesByTeacherIdWithGroupId(int teacherId, int groupId)
    {
      string sqlRequire = @"select c.ID, c.CourseName
                            from TeacherCourses as tg
                            INNER JOIN Teachers as t ON t.ID = tg.TeacherID
                            INNER JOIN Courses as c ON c.ID = tg.CourseID
                            INNER JOIN TeacherGroups as tc ON tc.TeacherID = t.ID
                            INNER JOIN Groups as g ON g.ID = tc.GroupID
                            INNER JOIN GroupCourses as gc ON gc.CourseID = tg.CourseID and gc.GroupID = g.ID
                            WHERE tg.TeacherID = @teacherId and tc.GroupID = @groupId";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<Course>(sqlRequire, new
          {
            groupId = groupId,
            teacherId = teacherId
          });
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    public static IEnumerable<(string courseName, string teacherFirstName, string teacherLastName)> GetCoursesByGroupId(int groupId)
    {
      string sqlRequire = @"select c.CourseName,t.FirstName, t.LastName
                            from GroupCourses as gc
                            INNER JOIN Courses as c ON c.ID = gc.CourseID
                            INNER JOIN TeacherCourses as tc ON tc.CourseID = c.ID
                            INNER JOIN Teachers as t ON t.ID = tc.TeacherID
                            WHERE gc.GroupID = @groupId";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<(string, string, string)>(sqlRequire, new { groupId = groupId });
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          MessageBox.Show(ex.Message);
          MessageBox.Show(ex.ToString());
          return null;
        }
      }
    }

    
  }
}
