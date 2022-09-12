using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Data.SqlClient;

namespace Modules
{
  [Table("GroupCourses")]
  public class GroupCourses
  {
    public int GroupId { get; set; }
    public int CourseId { get; set; }


    public static long Add(GroupCourses groupCourses)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Insert<GroupCourses>(groupCourses);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return 0;
        }
      }
    }
    public static GroupCourses GetGroupCourses(int groupId, int courseId)
    {
      string sqlRequire = @"select* from GroupCourses WHERE GroupId = @groupId and CourseId = @courseId";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.QueryFirstOrDefault<GroupCourses>(sqlRequire, new
          {
            courseId = courseId,
            groupId = groupId,
          });
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    public static bool DeleteGroupCourse(int groupId, int courseId)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {

        try
        {
          var groupCourses = conn.QueryFirstOrDefault<GroupCourses>("select* from GroupCourses WHERE GroupId = @groupId and CourseId = @courseId", new
          {
            courseId = courseId,
            groupId = groupId
          });
          return conn.Delete<GroupCourses>(groupCourses);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return false;
        }
      }
    }

  }
}
