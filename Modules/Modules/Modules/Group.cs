using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Modules
{
  [Table("Groups")]
  public class Group
  {
    [Key]
    public int Id { get; set; }
    public string GroupName { get; set; }

    public static long Add(Group group)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return (conn.Insert<Group>(group));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return 0;
        }
      }
    }


    public static Group GetGroupByName(string name)
    {
      string sqlRequire = @"select* from Groups WHERE GroupName = @name";
      using (var conn = new SqlConnection(Global.connectionString)) {
        try
        {
          return conn.QueryFirstOrDefault<Group>(sqlRequire, new
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

    public static IEnumerable<(int groupID, string teacherFirstName, string teacherLastName, string groupName, string courseName)> GetAllActiveGroups()
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        string sqlRequire = @"select g.ID, t.FirstName,t.LastName, g.GroupName, c.CourseName
                              from GroupCourses as gc
                              inner join Groups as g ON g.ID = gc.GroupID
                              inner join Courses as c ON c.ID = gc.CourseID
                              inner join TeacherCourses as tc on gc.CourseID = tc.CourseID
                              inner join Teachers as t on t.ID = tc.TeacherID";
        try
        {
          var res = conn.Query<(int groupID, string teacherFirstName, string teacherLastName,string groupName, string courseName)>(sqlRequire);
          return res;
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    public static IEnumerable<Group> GetAllGroups()
    {
      using(var conn = new SqlConnection(Global.connectionString))
      {
        var sqlRequire = "select* from Groups";
        try
        {
          return conn.Query<Group>(sqlRequire);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    public static IEnumerable<Group> GetGroupsByTeacherId(int teacherId)
    {
      string sqlRequire = @"select g.ID, g.GroupName
                            from TeacherGroups as tg
                            INNER JOIN Groups as g ON g.ID = tg.GroupID
                            WHERE tg.TeacherID = @id";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Query<Group>(sqlRequire, new
          {
            id = teacherId
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
