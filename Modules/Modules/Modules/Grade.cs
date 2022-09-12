using Dapper.Contrib.Extensions;
using System;
using System.Data.SqlClient;

namespace Modules
{
  [Table("Grades")]
  class Grade
  {
    [Key]
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public int Value { get; set; }

    public static long Add(Grade grade)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return (conn.Insert<Grade>(grade));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return 0;
        }
      }
    }


  }
}
