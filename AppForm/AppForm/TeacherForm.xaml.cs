using AppForm.ModalWindows;
using Microsoft.Win32;
using Modules;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Modules.Teacher;

namespace AppForm
{
  public partial class TeacherForm : Window
  {
    private Teacher teacher;
    private ObservableCollection<Teacher.TableRow> tabledatas = new ObservableCollection<Teacher.TableRow>();
    private void INIT()
    {
      spFields.InitFields(teacher);
      Controler.InitImage(image, teacher);
      tbHeader.InitTbHeader(teacher.FirstName, teacher.LastName);
      InitComboBoxProfileGroups();
      InitComboBoxProfileCourses();
      InitHomeComboBoxGroups();
      InitUnChekedHomeWorks();
      InitTableSettings();
      InitSpHomeWorks();
      InitSettingsComboBoxGroups();
    }

    public TeacherForm(Teacher teacher)
    {
      InitializeComponent();
      this.teacher = teacher;
      INIT();
    }
  

    #region DefaultEvents
    private void HideButton_Click(object sender, RoutedEventArgs e)
    {
      if (WindowState == WindowState.Normal || WindowState == WindowState.Minimized)
        WindowState = WindowState.Minimized;
      else
        WindowState = WindowState.Normal;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }
    private void Drag_Move(object sender, MouseButtonEventArgs e) => DragMove();

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (tabContol.SelectedIndex == 3)
      {
        Close();
      }
    }

    #endregion
    #region Profile

    
    private void UploadImageButton_Click(object sender, RoutedEventArgs e)
    {
      Controler.UploadImage(teacher);
      Controler.InitImage(image, teacher);
    }

    private void SetProfileButton_Click(object sender, RoutedEventArgs e)
    {
      teacher.FirstName = tbFirstName.Text;
      teacher.LastName = tbLastName.Text;
      bool isNum = int.TryParse(tbAge.Text, out int age);
      if (isNum)
        teacher.Age = age;
      else tbAge.Text = teacher.Age.ToString();
      if (tbDatatime.SelectedDate != null)
      {
        if (DateTime.Now.Year - ((DateTime)tbDatatime.SelectedDate).Year != age)
        {
          MessageBox.Show("Укажите правильные данные в календаре");
        }
        else teacher.Birthday = tbDatatime.SelectedDate;
      }
      teacher.Mail = tbMail.Text;
      var resultUpdate = Teacher.Update(teacher);
      if (resultUpdate == 0) MessageBox.Show("Неизвестная ошибка при изменении данных пользователя");
    }
    private void AddCourseButton_Click(object sender, RoutedEventArgs e)
    {
      new AddCourse(teacher.ID).ShowDialog();
      InitComboBoxProfileCourses();
    }

    private void AddGroupButton_Click(object sender, RoutedEventArgs e)
    {
      new AddGroup(teacher.ID).ShowDialog();
      InitComboBoxProfileCourses();
      InitComboBoxProfileGroups();
      InitHomeComboBoxGroups();
      InitComboBoxHomeCoursesByGroupName();
      InitSettingsComboBoxGroups();
      InitTableSettings();
    }

    private void InitComboBoxProfileGroups()
    {
      try
      {
        comboBoxGroups.Items.Clear();
        var groups = Group.GetGroupsByTeacherId(teacher.ID);
        foreach (var group in groups)
        {
          ComboBoxItem comboBoxItem = new ComboBoxItem();
          comboBoxItem.IsSelected = true;
          comboBoxItem.Content = new TextBlock() { Text = group.GroupName };
          comboBoxGroups.Items.Add(comboBoxItem);
        }
      }
      catch (Exception ex)
      {
        //
      }
    }

    private void InitComboBoxProfileCourses()
    {
      try
      {
        comboBoxCourses.Items.Clear();
        var courses = Course.GetCoursesByTeacherId(teacher.ID);
        foreach (var course in courses)
        {
          ComboBoxItem comboBoxItem = new ComboBoxItem()
          {
            Content = new TextBlock() { Text = course.CourseName }
          };
          comboBoxItem.IsSelected = true;
          comboBoxCourses.Items.Add(comboBoxItem);
        }
      }
      catch (Exception ex)
      {
        //
      }

    }
    #endregion
    #region Home
    private void InitHomeComboBoxGroups()
    {
      try
      {
        cbHomeGroups.Items.Clear();
        cbSettingsGroups.Items.Clear();
        var groups = Group.GetGroupsByTeacherId(teacher.ID);
        if (groups != null)
        {
          foreach (var group in groups)
          {
            ComboBoxItem comboBoxItem = new ComboBoxItem()
            {
              Content = new TextBlock() { Text = group.GroupName },
              Tag = group.Id
            };
            cbHomeGroups.Items.Add(comboBoxItem);
            comboBoxItem = new ComboBoxItem()
            {
              Content = new TextBlock() { Text = group.GroupName },
              Tag = group.Id
            };
            cbSettingsGroups.Items.Add(comboBoxItem);
          }
          if (cbHomeGroups.Items.Count > 0)
          {
            ((ComboBoxItem)cbHomeGroups.Items[0]).IsSelected = true;
            ((ComboBoxItem)cbSettingsGroups.Items[0]).IsSelected = true;
          }
        }
      }
      catch (Exception ex)
      {
        //
      }
    }
   
    private void InitComboBoxHomeCoursesByGroupName()
    {
      cbHomeGroupCourses.Items.Clear();
      if (cbHomeGroups == null || cbHomeGroups.SelectedItem == null)
        return;
      int groupId = (int)((ComboBoxItem)cbHomeGroups.SelectedItem).Tag;

      var courses = Course.GetCoursesByTeacherIdWithGroupId(teacher.ID, groupId);
      if (courses != null)
      {
        foreach (var course in courses)
        {
          ComboBoxItem comboBoxItem = new ComboBoxItem()
          {
            Content = new TextBlock() { Text = course.CourseName },
            Tag = course.Id
          };
          cbHomeGroupCourses.Items.Add(comboBoxItem);
        }
        if (cbHomeGroupCourses.Items.Count > 0)
          ((ComboBoxItem)cbHomeGroupCourses.Items[0]).IsSelected = true;
      }
    }

    private void cbHomeGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      InitComboBoxHomeCoursesByGroupName();
    }

    private void cbHomeGroupCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      InitUnChekedHomeWorks();
      InitSpHomeWorks();
    }
    private void InitUnChekedHomeWorks()
    {
      try
      {
        spUncheckedHomeWorks.Children.Clear();
        int courseId, groupId;
        if (cbHomeGroupCourses.SelectedItem is ComboBoxItem)
          courseId = (int)(((ComboBoxItem)cbHomeGroupCourses.SelectedItem).Tag);
        else return;
        if (comboBoxGroups.SelectedItem is ComboBoxItem)
          groupId = (int)(((ComboBoxItem)cbHomeGroups.SelectedItem).Tag);
        else return;
        var homeWorks = HomeWork.GetHomeWorksByGroupIdWithCourseId(groupId, courseId, teacher.ID);
        if (homeWorks.GetEnumerator().MoveNext() == false)
        {
          spUncheckedHomeWorks.Children.Add(new TextBlock() { Text = "Пусто" });
        }
        else
        {
          foreach (var homeWork in homeWorks)
          {
            spUncheckedHomeWorks.Children.Add(GetStackPanelForUnchekedHomeWork(homeWork));
          }
        }
      }
      catch (Exception ex)
      {

      }

    }

    private StackPanel GetStackPanelForUnchekedHomeWork((int homeWorkId, int studentId, int courseId, int groupId, string firstName, string lastName, string groupName, string courseName, DateTime dataLoad, byte[] file, string fileTitle) homeworkDetailInfo)
    {
      try
      {
        StackPanel stackPanel = new StackPanel();
        Border border = new Border()
        {
          BorderBrush = new SolidColorBrush(Colors.AliceBlue),
          Padding = new Thickness(10),
          BorderThickness = new Thickness(1),
          CornerRadius = new CornerRadius(10)
        };
        Grid grid = new Grid();
        StackPanel fields = new StackPanel()
        {
          HorizontalAlignment = HorizontalAlignment.Left,
        };
        fields.Children.Add(new TextBlock() { Text = $"Дата: {homeworkDetailInfo.dataLoad.ToShortDateString()}", FontSize = 10, Opacity = 0.8 });
        fields.Children.Add(new TextBlock() { Text = $"Имя: {homeworkDetailInfo.firstName} {homeworkDetailInfo.lastName}" });
        fields.Children.Add(new TextBlock() { Text = $"Курс: {homeworkDetailInfo.courseName}", FontSize = 9, Opacity = 0.5 });
        fields.Children.Add(new TextBlock() { Text = $"Группа: {homeworkDetailInfo.groupName}" });
        StackPanel tools = new StackPanel()
        {
          HorizontalAlignment = HorizontalAlignment.Right,
          Orientation = Orientation.Horizontal
        };
        Style styleGrade = (Style)Application.Current.Resources["MaterialDesignOutlinedTextBox"];
        TextBox grade = new TextBox()
        {
          MaxLength = 2,
          VerticalAlignment = VerticalAlignment.Center,
          Margin = new Thickness(0, 0, 20, 0),
          Padding = new Thickness(8),
          Style = styleGrade,
        };
        MaterialDesignThemes.Wpf.TextFieldAssist.SetSuffixText(grade, "/10");
        MaterialDesignThemes.Wpf.TextFieldAssist.SetCharacterCounterVisibility(grade, Visibility.Hidden);
        Style stylebutton = (Style)Application.Current.Resources["MaterialDesignToolForegroundButton"];
        Button btDownoloadFile = new Button()
        {
          Content = "Скачать работу",
          Style = stylebutton,
          Margin = new Thickness(0, 0, 20, 0),
          Tag = new ArrayList() { homeworkDetailInfo.file, homeworkDetailInfo.fileTitle },
        };
        btDownoloadFile.Click += BtDownoloadFile_Click;
        Button btChecked = new Button()
        {
          Content = "Проверено",
          Tag = new ArrayList() { homeworkDetailInfo.studentId, homeworkDetailInfo.homeWorkId, grade }
        };
        btChecked.Click += BtChecked_Click;
        tools.Children.Add(grade);
        tools.Children.Add(btDownoloadFile);
        tools.Children.Add(btChecked);
        grid.Children.Add(tools);
        grid.Children.Add(fields);
        border.Child = grid;
        stackPanel.Children.Add(border);
        StackPanel bottomTools = new StackPanel()
        {
          HorizontalAlignment = HorizontalAlignment.Right,
          Orientation = Orientation.Horizontal,
        };
        TextBox textbox = new TextBox()
        {
          MinWidth = 150
        };
        MaterialDesignThemes.Wpf.HintAssist.SetHint(textbox, "Напишите комментарии");
        Style style = (Style)Application.Current.Resources["MaterialDesignFlatButton"];
        Button leaveComments = new Button()
        {
          Style = style,
          Content = "Оставить",
          Tag = new ArrayList() { homeworkDetailInfo.studentId, homeworkDetailInfo.courseId, textbox, bottomTools }
        };

        leaveComments.Click += LeaveComments_Click;
        bottomTools.Children.Add(textbox);
        bottomTools.Children.Add(leaveComments);
        stackPanel.Children.Add(bottomTools);
        return stackPanel;
      }
      catch (Exception ex)
      {
        return new StackPanel() { Children = { new TextBlock() { Text = $"Ошибка {ex.Message}" } } };
      }

    }

    private void LeaveComments_Click(object sender, RoutedEventArgs e)
    {
      var dates = ((Button)sender).Tag as ArrayList;
      if (dates == null || dates.Count == 0 || (dates[2] as TextBox).Text.Trim().Length == 0)
      {
        MessageBox.Show("Укажите корректные данные");
        return;
      }
      Comment comment = new Comment()
      {
        CourseId = (int)dates[1],
        Value = (dates[2] as TextBox).Text,
        DateComment = DateTime.Now,
        StudentId = (int)dates[0],
        TeacherId = teacher.ID
      };
      Comment.Add(comment);
      (dates[3] as StackPanel).Visibility = Visibility.Collapsed;
    }

    private void BtChecked_Click(object sender, RoutedEventArgs e)
    {
      var values = (sender as Button).Tag as ArrayList;
      int studentId = (int)values[0];
      int homeWorkId = (int)values[1];
      string grade = ((TextBox)values[2]).Text;
      if (int.TryParse(grade, out int number))
      {
        if (number > 10 || number < 0)
        {
          MessageBox.Show("Укажите корректную оценку!");
          return;
        }
        ProcessedHomeWork.CheckHomeWork(studentId, homeWorkId, number);
        InitUnChekedHomeWorks();
      }
      else
      {
        MessageBox.Show("Укажите корректную оценку!");
      }
    }

    private void BtDownoloadFile_Click(object sender, RoutedEventArgs e)
    {
      ArrayList arrayList = ((Button)sender).Tag as ArrayList;
      Controler.DownoloadFile((byte[])arrayList[0], (string)arrayList[1]);
    }

    private void AddHomeWorkProfileButton_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      if (openFileDialog.ShowDialog() == true)
      {
        int courseId, groupId;
        if (cbHomeGroupCourses.SelectedItem is ComboBoxItem)
          courseId = (int)(((ComboBoxItem)cbHomeGroupCourses.SelectedItem).Tag);
        else return;
        if (comboBoxGroups.SelectedItem is ComboBoxItem)
          groupId = (int)(((ComboBoxItem)cbHomeGroups.SelectedItem).Tag);
        else return;

        HomeWork homeWork = new HomeWork()
        {
          CourseId = courseId,
          GroupId = groupId,
          DataLoad = DateTime.Now,
          HomeWorkFile = Controler.GetBytesInFile(openFileDialog.FileName),
          TeacherId = teacher.ID,
          Term = (dtTerm.SelectedDate == null || ((DateTime)dtTerm.SelectedDate).Subtract(DateTime.Now).Days < 0 ? DateTime.Now.AddDays(7) : dtTerm.SelectedDate),
          HomeWorkFileTitle = openFileDialog.SafeFileName,
        };
        var res = HomeWork.Add(homeWork);
        if (res == 0) MessageBox.Show("Неизвестная ошибка при добавления дз");
        InitSpHomeWorks();
      }
    }

    private void InitSpHomeWorks()
    {
      spHomeWorks.Children.Clear();
      int courseId, groupId;
      if (cbHomeGroupCourses.SelectedItem is ComboBoxItem)
        courseId = (int)(((ComboBoxItem)cbHomeGroupCourses.SelectedItem).Tag);
      else return;
      if (comboBoxGroups.SelectedItem is ComboBoxItem)
        groupId = (int)(((ComboBoxItem)cbHomeGroups.SelectedItem).Tag);
      else return;

      var homeworks = HomeWork.GetHomeWorksByTeacherId(teacher.ID, courseId, groupId);
      foreach (var homework in homeworks)
      {

        Border border = new Border()
        {
          Padding = new Thickness(15),
          CornerRadius = new CornerRadius(20),
          BorderBrush = new SolidColorBrush(Colors.White),
          BorderThickness = new Thickness(1),
          Margin = new Thickness(0, 20, 0, 0)
        };
        var grid = new Grid()
        {
          MinWidth = 200,
        };
        StackPanel stackPanel = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Left };
        stackPanel.Children.Add(new TextBlock() { Text = homework.courseName });
        stackPanel.Children.Add(new TextBlock() { Text = homework.term.ToShortDateString(), FontSize = 10, Opacity = 0.4 });
        grid.Children.Add(stackPanel);
        Button goHomeWork = new Button()
        {
          Content = "Скачать",
          Tag = new ArrayList() { homework.file, homework.title, },
          HorizontalAlignment = HorizontalAlignment.Right,
        };
        goHomeWork.Click += BtDownoloadFile_Click;
        grid.Children.Add(goHomeWork);
        border.Child = grid;
        spHomeWorks.Children.Add(border);
      }
    }
    #endregion
    #region Settings
    private void InitComboBoxSettingsCoursesByGroupName()
    {
      cbSettingsGroupCourses.Items.Clear();
      if (cbSettingsGroups == null || cbSettingsGroups.SelectedItem == null)
        return;
      int groupId = (int)((ComboBoxItem)cbSettingsGroups.SelectedItem).Tag;

      var courses = Course.GetCoursesByTeacherIdWithGroupId(teacher.ID, groupId);
      if (courses != null)
      {
        foreach (var course in courses)
        {
          ComboBoxItem comboBoxItem = new ComboBoxItem()
          {
            Content = new TextBlock() { Text = course.CourseName },
            Tag = course.Id
          };
          cbSettingsGroupCourses.Items.Add(comboBoxItem);
        }
        if (cbSettingsGroupCourses.Items.Count > 0) ((ComboBoxItem)cbSettingsGroupCourses.Items[0]).IsSelected = true;
      }
    }
    private void cbSettingsGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      InitComboBoxSettingsCoursesByGroupName();
    }

    private void cbSettingsGroupCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      InitTableSettings();
    }
    private void InitTableSettings()
    {
      int courseId, groupId;
      if (cbSettingsGroupCourses.SelectedItem is ComboBoxItem)
        courseId = (int)(((ComboBoxItem)cbSettingsGroupCourses.SelectedItem).Tag);
      else return;
      if (cbSettingsGroups.SelectedItem is ComboBoxItem)
        groupId = (int)(((ComboBoxItem)cbSettingsGroups.SelectedItem).Tag);
      else return;
      tabledatas = Teacher.GetTableSettings(groupId, courseId);
      dgStudents.ItemsSource = tabledatas;
    }
    private void InitSettingsComboBoxGroups()
    {
      try
      {
        cbSettingsGroups.Items.Clear();
        var groups = Group.GetGroupsByTeacherId(teacher.ID);
        if (groups != null)
        {
          foreach (var group in groups)
          {
            ComboBoxItem comboBoxItem = new ComboBoxItem()
            {
              Content = new TextBlock() { Text = group.GroupName },
              Tag = group.Id
            };
            cbSettingsGroups.Items.Add(comboBoxItem);
          }
          if (cbSettingsGroups.Items.Count > 0)
            ((ComboBoxItem)cbSettingsGroups.Items[0]).IsSelected = true;
        }
      }
      catch (Exception ex)
      {
        //
      }

    }

    private void DeleteSelectedStudentButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        foreach (var line in dgStudents.Items)
        {
          var student = line as TableRow;
          if (student != null)
          {
            if (student.IsSelected)
            {
              Student.SetGroupStudentById(student.StundentId, null);
            }
          }
        }
        int courseId, groupId;
        if (cbHomeGroupCourses.SelectedItem is ComboBoxItem)
          courseId = (int)(((ComboBoxItem)cbHomeGroupCourses.SelectedItem).Tag);
        else return;
        if (comboBoxGroups.SelectedItem is ComboBoxItem)
          groupId = (int)(((ComboBoxItem)cbHomeGroups.SelectedItem).Tag);
        else return;

        tabledatas = Teacher.GetTableSettings(groupId, courseId);
        dgStudents.ItemsSource = tabledatas;
      }
      catch (Exception ex)
      {

      }
    }

    private void DeleteCourseButton_Click(object sender, RoutedEventArgs e)
    {
      int courseId;
      if (cbHomeGroupCourses.SelectedItem is ComboBoxItem)
        courseId = (int)(((ComboBoxItem)cbHomeGroupCourses.SelectedItem).Tag);
      else return;
      bool res = TeacherCourse.DeleteCourse(teacher.ID, courseId);
      if (res) MessageBox.Show("Успешно удалено");
      else MessageBox.Show("Неизвестная ошибка");
      INIT();
    }

    private void DeleteGroupButton_Click(object sender, RoutedEventArgs e)
    {
      int groupId;
      if (comboBoxGroups.SelectedItem is ComboBoxItem)
        groupId = (int)(((ComboBoxItem)cbHomeGroups.SelectedItem).Tag);
      else return;
      int courseId;
      if (cbHomeGroupCourses.SelectedItem is ComboBoxItem)
        courseId = (int)(((ComboBoxItem)cbHomeGroupCourses.SelectedItem).Tag);
      else return;
      bool res = TeacherGroups.Delete(teacher.ID, groupId);
      bool res2 = GroupCourses.DeleteGroupCourse(groupId, courseId);
      if (res && res2) MessageBox.Show("Успешно удалено");
      else MessageBox.Show("Неизвестная ошибка");
      INIT();
    }

    #endregion

  }
}
