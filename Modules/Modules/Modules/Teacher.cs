using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace Modules
{
  public class Teacher : User
  {

    public class TableRow : DependencyObject
    {
      public bool IsSelected
      {
        get { return (bool)GetValue(IsSelectedProperty); }
        set { SetValue(IsSelectedProperty, value); }
      }
      public static readonly DependencyProperty IsSelectedProperty =
       DependencyProperty.Register("IsSelected", typeof(bool), typeof(TableRow), new UIPropertyMetadata(false));

      public int StundentId { get; set; }
      public string StudentFullName { get; set; }
      public int Age { get; set; }
      public double Average { get; set; }
      public int CountCompletedHomeWorks { get; set; }
      public int CountOverdueHomeWorks { get; set; }
      public int CountNotCompletedHomeWorks { get; set; }

      public override string ToString()
      {
        return $"{IsSelected,2} {StundentId,4} {StudentFullName,20}\t{Age,5}\t{Average,5}\t{CountCompletedHomeWorks,5}\t{CountOverdueHomeWorks,5}\t{CountNotCompletedHomeWorks,5}";
      }
    }
    public static long Add(Teacher teacher)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return (conn.Insert<Teacher>(teacher));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
        return -1;
      }
    }
    public static int Update(Teacher teacher)
    {
      string sqlRequire = @"UPDATE Teachers 
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
            firstName = teacher.FirstName,
            lastName = teacher.LastName,
            age = teacher.Age,
            birthday = teacher.Birthday,
            mail = teacher.Mail,
            passWord = teacher.PassWord,
            image = teacher.Image,
            s_id = teacher.ID,
            imageTitle = teacher.ImageTitle
          });
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          Console.WriteLine(ex.ToString());
          return 0;
        }
      }

    }

    public static Teacher GetTeacherByLogin(string login, string password)
    {
      using (var conn = new SqlConnection(Global.connectionString))
      {
        try
        {
          return conn.QueryFirstOrDefault<Teacher>("select* from Teachers WHERE Mail = @login and [PassWord] = @password", new
          {
            login = login,
            password = password,
          });
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
          return null;
        }
      }
    }


    public static void Demo()
    {
      var r = GetTableSettings(1, 1);
    }

    public static ObservableCollection<TableRow> GetTableSettings(int groupId, int courseId)
    {
      var sqlRequire = @"--запрос на имя студента, возраст, средняя оценка, кол-во выполненных дз
                        select s.ID, (s.firstName + ' ' + s.LastName) as fullName,  s.Age, AVG(phw.Grade), COUNT(phw.HomeWorkId)
                        from Students as s
                        LEFT JOIN HomeWorks as hw ON hw.CourseId = @courseId
                        LEFT JOIN ProcessedHomeWorks as phw ON phw.StudentId = s.ID and hw.ID = phw.HomeWorkId
                        WHERE s.GroupID = @groupId
                        GROUP BY s.ID, s.FirstName, s.LastName, s.Age;
                        --запрос на кол-во выполненных дз студента не в срок
                        select COUNT(phw.StudentId)
                        from Students as s
                        LEFT JOIN HomeWorks as hw ON hw.GroupId = s.GroupID and hw.CourseId = @courseId
                        LEFT JOIN ProcessedHomeWorks as phw ON phw.StudentId = s.ID and phw.HomeWorkId = s.ID and phw.DataDownoload > hw.Term and phw.DataDownoload is not null
                        WHERE s.GroupID = @groupId 
                        group by s.ID;
                        --запрос на кол-во невыполненных дз студента
                        select COUNT(h.ID)
                        from Students as s
                        LEFT JOIN HomeWorks as h ON h.GroupId = s.GroupID and 
                        h.ID NOT IN (select phw.HomeWorkId from ProcessedHomeWorks as phw  WHERE phw.DataDownoload is not null and phw.StudentId = s.ID)
                        WHERE s.GroupID = @groupId and h.CourseId = @courseId
                        GROUP BY s.ID";
      try
      {
        using (var conn = new SqlConnection(Global.connectionString))
        {
          using (var multi = conn.QueryMultiple(sqlRequire, new
          {
            groupId = groupId,
            courseId = courseId
          }))
          {
            ObservableCollection<TableRow> result = new ObservableCollection<TableRow>();
            var firstResults = multi.Read<(int id, string studentFullName, int age, double average, int countCompletedHomeWorks)>().ToList();
            foreach (var row in firstResults)
            {
              TableRow tableRow = new TableRow(); ;
              tableRow.StundentId = row.id;
              tableRow.StudentFullName = row.studentFullName;
              tableRow.Age = row.age;
              tableRow.Average = row.average;
              tableRow.CountCompletedHomeWorks = row.countCompletedHomeWorks;
              result.Add(tableRow);
            };
            var secondResults = multi.Read<int>();
            int index = 0;
            foreach (var row in secondResults)
            {
              TableRow temp = result[index];
              temp.CountOverdueHomeWorks = row;
              result[index] = temp;
              index++;
            }
            index = 0;
            var thridResult = multi.Read<int>();
            foreach (var row in thridResult)
            {
              TableRow temp = result[index];
              temp.CountNotCompletedHomeWorks = row;
              result[index] = temp;
              index++;
            }
            return result;
          };
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        return new ObservableCollection<TableRow>();
      }
    }
  }
}
