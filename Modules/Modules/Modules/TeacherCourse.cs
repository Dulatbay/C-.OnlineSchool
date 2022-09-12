using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Modules
{
  [Table("TeacherCourses")]
  public class TeacherCourse
  {
    
    [ExplicitKey]
    public int TeacherId { get; set; }
    [ExplicitKey]
    public int CourseId { get; set; }

    public static long Add(TeacherCourse teacherCourse)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Insert<TeacherCourse>(teacherCourse);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          Console.WriteLine(ex.Message);
          return 0;
        }
      }
    }
    public static bool DeleteCourse(int teacherId, int courseId)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {

        try
        {
          var teacherCourse = conn.QueryFirstOrDefault<TeacherCourse>("select* from TeacherCourses WHERE TeacherID = @teacherId and CourseId = @courseId", new
          {
            courseId = courseId,
            teacherId = teacherId
          });
          return conn.Delete<TeacherCourse>(teacherCourse);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return false;
        }
      }
    }


    public static TeacherCourse GetTeacherCourse(int teacherId, int courseId)
    {
      string sqlRequire = @"select* from TeacherCourses as tc WHERE tc.TeacherId = @teacherId and tc.CourseId = @courseId";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.QueryFirstOrDefault<TeacherCourse>(sqlRequire, new
          {
            teacherId = teacherId,
            courseId = courseId,
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
