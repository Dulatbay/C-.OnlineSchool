using Modules;
using System.Windows;
using System.Windows.Controls;

namespace AppForm.ModalWindows
{
  /// <summary>
  /// Логика взаимодействия для AddGroup.xaml
  /// </summary>
  public partial class AddGroup : Window
  {
    private int _teacherId;

    public AddGroup(int teacherId)
    {
      InitializeComponent();
      _teacherId = teacherId;
      INIT();
    }

    private void INIT()
    {
      InitComboBoxGroups();
      InitComboBoxCourses();
    }
    private void InitComboBoxGroups()
    {
      var groups = Group.GetAllGroups();
      if (groups != null)
      {
        foreach (var group in groups)
        {
          ComboBoxItem comboBoxItem = new ComboBoxItem();
          comboBoxItem.IsSelected = true;
          comboBoxItem.Content = new TextBlock() { Text = group.GroupName };
          comboBoxGroups.Items.Add(comboBoxItem);
        }
      }
    }
    private void InitComboBoxCourses()
    {
      cbCourses.Items.Clear();
      var courses = Course.GetCoursesByTeacherId(_teacherId);
      if (courses != null)
      {
        foreach (var course in courses)
        {
          ComboBoxItem comboBoxItem = new ComboBoxItem();
          comboBoxItem.IsSelected = true;
          comboBoxItem.Content = new TextBlock() { Text = course.CourseName };
          cbCourses.Items.Add(comboBoxItem);
        }
      }
    }

    #region DefaulEvents
    private void cbGroup_Checked(object sender, RoutedEventArgs e)
    {
      if (comboBoxGroups != null && tbNameNewGroup != null)
      {
        comboBoxGroups.IsEnabled = true;
        tbNameNewGroup.IsEnabled = false;
      }
    }

    private void cbGroup_Unchecked(object sender, RoutedEventArgs e)
    {
      comboBoxGroups.IsEnabled = false;
      tbNameNewGroup.IsEnabled = true;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void cbCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (cbCourses != null && cbCourses.SelectedItem != null)
      {
        btAdd.IsEnabled = true;
      }
      else
      {
        btAdd.IsEnabled = false;
      }
    }

    private void btAdd_Click(object sender, RoutedEventArgs e)
    {
      if (tbNameNewGroup.Text.Trim().Length < 2 && !(bool)iscbGroup.IsChecked)
      {
        MaterialDesignThemes.Wpf.HintAssist.SetHelperText(tbNameNewGroup, "Введите корректное название группы!");
      }
      else
      {
        Group group;
        if (!(bool)iscbGroup.IsChecked)
        {
          group = new Group() { GroupName = tbNameNewGroup.Text };
          Group.Add(group);
        }
        else
        {
          group = Group.GetGroupByName(((TextBlock)(((ComboBoxItem)(comboBoxGroups.SelectedItem)).Content)).Text);
        }
        if(cbCourses.SelectedItem is null)
        {
          MessageBox.Show("Нет предмета!");
          return;
        }
        Course course = Course.GetCourseByName(((TextBlock)(((ComboBoxItem)(cbCourses.SelectedItem)).Content)).Text);
        var result = GroupCourses.GetGroupCourses(group.Id, course.Id);
        if (result != null && result.CourseId == course.Id)
        {
          MessageBox.Show("У такой группы уже есть данный курс");
          return;
        }


        TeacherGroups.Add(new TeacherGroups() { GroupId = group.Id, TeacherId = _teacherId });
        GroupCourses.Add(new GroupCourses() { CourseId = course.Id, GroupId = group.Id });
        this.Close();
      }
    }
    #endregion

    private void comboBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (comboBoxGroups != null && comboBoxGroups.SelectedItem != null)
      {
        btAdd.IsEnabled = true;
      }
      else
      {
        btAdd.IsEnabled = false;
      }
    }

    private void NotHaveCourseButton_Click(object sender, RoutedEventArgs e)
    {
      new AddCourse(_teacherId).ShowDialog();

      InitComboBoxCourses();
    }
  }
}
