using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Data.SqlClient;

namespace Modules
{
  [Table("TeacherGroups")]
  public class TeacherGroups
  {
    [ExplicitKey]
    public int TeacherId { get; set; }
    [ExplicitKey]
    public int GroupId { get; set; }

    public static long Add(TeacherGroups teacherGroups)
    {
      using(var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Insert(teacherGroups);
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
          return 0;
        }
      }
    }

    public static bool Delete(int teacherId, int groupId)
    {
      try
      {
        using(var conn = new SqlConnection(Global.connectionString))
        {
          var teacherGroup = conn.QueryFirstOrDefault<TeacherGroups>("select* from TeacherGroups WHERE @teacherId = TeacherID and @groupId = GroupID",
            new
            {
              groupId = groupId,
              teacherId = teacherId
            });
          return conn.Delete<TeacherGroups>(teacherGroup);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return false;
      }
    }

  }
}
