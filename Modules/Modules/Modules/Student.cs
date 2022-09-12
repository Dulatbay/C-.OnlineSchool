using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace Modules
{
  [Table("Students")]
  public class Student : User
  {
    public int? GroupId { get; set; }

    public static long Add(Student user)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return (conn.Insert<Student>(user));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return 0;
        }
      }
    }

    public static List<Student> GetAllStudents()
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return (conn.Query<Student>("select* from Students;").ToList());
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
        return new List<Student>();
      }
    }
    public static Student GetStudentById(int id)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return (conn.QueryFirstOrDefault<Student>("select* from Students WHERE ID = @ID", new { ID = id }));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    public static Student GetStudentByLogin(string login, string password)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.QueryFirstOrDefault<Student>("select* from Students WHERE Mail = @login and PassWord = @password", new
          {
            login = login,
            password = password
          });
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }

    public static int SetGroupStudentById(int id, int? groupId)
    {
      string sqlRequire = @"UPDATE Students SET GroupID = @g_id WHERE ID = @s_id";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          var res = conn.Execute(sqlRequire, new { g_id = groupId, s_id = id, });
          return res;
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return 0;
        }
      }
    }

    public static int Update(Student student)
    {
      string sqlRequire = @"UPDATE Students 
                            SET FirstName = @firstName,
                            LastName = @lastName, 
                            Age = @age,
                            Birthday = @birthday,
                            Mail = @mail,
                            PassWord = @passWord,
                            Image = @image,
                            ImageTitle = @imageTitle
                            WHERE ID = @s_id";
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.Execute(sqlRequire, new
          {
            firstName = student.FirstName,
            lastName = student.LastName,
            age = student.Age,
            birthday = student.Birthday,
            mail = student.Mail,
            passWord = student.PassWord,
            image = student.Image,
            s_id = student.ID,
            imageTitle = student.ImageTitle
          });
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
          Console.WriteLine(ex.Message);
          return 0;
        }
      }

    }

  }
}
