using Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AppForm.ModalWindows
{
  /// <summary>
  /// Логика взаимодействия для AddCourse.xaml
  /// </summary>
  public partial class AddCourse : Window
  {
    private int _teacherId;
    public AddCourse(int teacherId)
    {
      InitializeComponent();
      _teacherId = teacherId;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
      var result = Course.GetCourseByName(tbCourse.Text);
      if (result == null || result.Id == 0)
      {
        var c = new Course() { CourseName = tbCourse.Text };
        Course.Add(c);
        TeacherCourse.Add(new TeacherCourse() { CourseId = c.Id, TeacherId = _teacherId });
      }
      else
      {
        MessageBox.Show("Такой курс уже имеется");
        TeacherCourse.Add(new TeacherCourse() { CourseId = result.Id, TeacherId = _teacherId });
      }

      Close();
    }

    private void tbCourse_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (tbCourse.Text.Trim().Length == 0) btAdd.IsEnabled = false;
      else btAdd.IsEnabled = true;
    }
  }
}
