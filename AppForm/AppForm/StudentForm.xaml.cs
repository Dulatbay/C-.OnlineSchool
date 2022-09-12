using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AppForm
{
  public partial class StudentForm : Window
  {
    private Student student;
    private void INIT()
    {
      spFields.InitFields(student);
      Controler.InitImage(image, student);
      if (student.GroupId != null)
        InitComboBoxByCoursesDetailInfo(comboBoxGroups, student.GroupId);
      tbHeader.InitTbHeader(student.FirstName, student.LastName);
      InitSpComments();
      InitSpCheckedHomeWorks();
      InitSpHomeWorksHome();
      InitCurrentHomeWorks();
      InitUnderReviewHomeWorks();
      InitOverdueHomeWorks();
      InitCompletedHomeWorks();
      InitPieChart(currentTimePerid);
    }
    public StudentForm(Student student)
    {
      InitializeComponent();
      this.student = student;
      INIT();
    }
   
    #region DefaultEvents
    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (tabControl.SelectedIndex == 4)
      {
        Close();
      }
    }
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
    private void Drag_Move(object sender, MouseButtonEventArgs e)
    {
      try
      {
        DragMove();
      }
      catch (Exception ex)
      {
      }
    }

    #endregion
    #region Profile
    private void SetProfileButton_Click(object sender, RoutedEventArgs e)
    {
      student.FirstName = tbFirstName.Text;
      student.LastName = tbLastName.Text;
      bool isNum = int.TryParse(tbAge.Text, out int age);
      if (isNum)
        student.Age = age;
      else tbAge.Text = student.Age.ToString();
      if (tbDatatime.SelectedDate != null)
      {
        if (DateTime.Now.Year - ((DateTime)tbDatatime.SelectedDate).Year != age)
        {
          MessageBox.Show("Укажите правильные данные в календаре");
        }
        else student.Birthday = tbDatatime.SelectedDate;
      }
      student.Mail = tbMail.Text;
      var resultUpdate = Student.Update(student);
      if (resultUpdate == 0) MessageBox.Show("Неизвестная ошибка при изменении данных пользователя");
    }

    private void InitComboBoxAllGroups()
    {
      cbGroups.Items.Clear();
      var groups = Group.GetAllGroups();
      foreach (var group in groups)
      {
        ComboBoxItem comboBoxItem = new ComboBoxItem()
        {
          Content = new TextBlock() { Text = group.GroupName },
          Tag = group.Id,
        };
        cbGroups.Items.Add(comboBoxItem);
      }
      if (cbGroups.Items.Count == 0) btConfirm.IsEnabled = false;
    }
    private void GroupChangeButton_Click(object sender, RoutedEventArgs e)
    {
      btSetGroup.Visibility = Visibility.Hidden;
      spGroupChange.Visibility = Visibility.Visible;
      btConfirm.IsEnabled = true;
      InitComboBoxAllGroups();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      btSetGroup.Visibility = Visibility.Visible;
      spGroupChange.Visibility = Visibility.Hidden;
    }
    private void SetButton_Click(object sender, RoutedEventArgs e)
    {
      int newGroupId = (int)((ComboBoxItem)cbGroups.SelectedItem).Tag;
      Student.SetGroupStudentById(student.ID, newGroupId);
      btSetGroup.Visibility = Visibility.Visible;
      spGroupChange.Visibility = Visibility.Hidden;
      student.GroupId = newGroupId;
      InitComboBoxByCoursesDetailInfo(comboBoxGroups, student.GroupId);
      INIT();
    }

    public static void InitComboBoxByCoursesDetailInfo(ComboBox comboBox, int? groupId)
    {
      comboBox.Items.Clear();
      var courses = Course.GetCoursesByGroupId((int)groupId);

      foreach (var course in courses)
      {
        StackPanel stackPanel = new StackPanel();
        stackPanel.Children.Add(new TextBlock() { Text = course.teacherFirstName + " " + course.teacherLastName, FontSize = 10, Opacity = 0.3 });
        stackPanel.Children.Add(new TextBlock() { Text = course.courseName, FontSize = 13 });
        ComboBoxItem comboBoxItem = new ComboBoxItem()
        {
          Content = stackPanel
        };
        comboBox.Items.Add(comboBoxItem);
      }
      if (comboBox.Items.Count > 0) ((ComboBoxItem)comboBox.Items[0]).IsSelected = true;

    }

    private void UploadImageButton_Click(object sender, RoutedEventArgs e)
    {
      Controler.UploadImage(student);
      Controler.InitImage(image, student);
    }
    #endregion
    #region Home
    private void InitSpComments()
    {
      spComments.Children.Clear();
      var comments = Comment.GetCommentsByStudentId(student.ID);
      if (comments == null || comments.GetEnumerator().MoveNext() == false)
      {
        spComments.Children.Add(new TextBlock() { Text = "Пусто" });
        return;
      }
      foreach (var comment in comments)
      {

        Border border = new Border()
        {
          Padding = new Thickness(15),
          BorderBrush = new SolidColorBrush(Colors.White),
          BorderThickness = new Thickness(1),
          CornerRadius = new CornerRadius(20),
          Margin = new Thickness(0, 0, 0, 20),
        };
        StackPanel stackPanel = new StackPanel();
        stackPanel.Children.Add(new TextBlock() { Text = $"Курс: {comment.courseName}", Opacity = 0.4, FontSize = 9 });
        stackPanel.Children.Add(new TextBlock() { Text = $"{comment.teacherName}:\n{comment.comment}", FontSize = 15 });
        border.Child = stackPanel;
        spComments.Children.Add(border);
      }
    }

    private void InitSpCheckedHomeWorks()
    {
      spCheckedHomeWorks.Children.Clear();
      var homeworks = HomeWork.GetCheckedHomeWorksByStudentId(student.ID);
      if (homeworks == null || homeworks.GetEnumerator().MoveNext() == false)
      {
        spCheckedHomeWorks.Children.Add(new TextBlock() { Text = "Пусто" });
        return;
      }
      foreach (var homework in homeworks)
      {
        spCheckedHomeWorks.Children.Add(new TextBlock() { Text = $"{homework.courseName}: {homework.grade}" });
      }
    }

    private void InitSpHomeWorksHome()
    {
      spHomeWorksHome.Children.Clear();
      var homeworks = HomeWork.GetHomeWorksByStudentId(student.ID);
      if (homeworks == null || homeworks.GetEnumerator().MoveNext() == false)
      {
        spHomeWorksHome.Children.Add(new TextBlock() { Text = "Пусто" });
        return;
      }
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
          Tag = new ArrayList() { homework.file, homework.Title },
          HorizontalAlignment = HorizontalAlignment.Right,
        };
        goHomeWork.Click += DownloadHomeWork_ButtonClick;
        grid.Children.Add(goHomeWork);
        border.Child = grid;
        spHomeWorksHome.Children.Add(border);
      }
    }

    private void DownloadHomeWork_ButtonClick(object sender, RoutedEventArgs e)
    {
      ArrayList arrayList = ((Button)sender).Tag as ArrayList;
      Controler.DownoloadFile((byte[])arrayList[0], (string)arrayList[1]);
    }

    private void InitCurrentHomeWorks()
    {
      spCurrentHomeWorks.Children.Clear();
      var homeworks = HomeWork.GetHomeWorksByStudentId(student.ID);
      if (homeworks == null || homeworks.GetEnumerator().MoveNext() == false)
      {
        tbCurrent.Visibility = Visibility.Collapsed;
        return;
      }
      tbCurrent.Visibility = Visibility.Visible;
      foreach (var homework in homeworks)
      {
        spCurrentHomeWorks.Children.Add(GetDockPanelByHomeWork(homework, (new SolidColorBrush(Colors.LightGray), "")));
      }
    }
    private void InitOverdueHomeWorks()
    {
      spOverdueHomeWorks.Children.Clear();
      var homeworks = HomeWork.GetOverdueHomeWorksByStudentId(student.ID);
      if (homeworks == null || homeworks.GetEnumerator().MoveNext() == false)
      {
        tbOverdue.Visibility = Visibility.Collapsed;
        return;
      }
      tbOverdue.Visibility = Visibility.Visible;
      foreach (var homework in homeworks)
      {
        spOverdueHomeWorks.Children.Add(GetDockPanelByHomeWork(homework, (new SolidColorBrush(Colors.IndianRed), "Просрочено")));
      }
    }

    private void InitUnderReviewHomeWorks()
    {
      spUnderReviewHomeWorks.Children.Clear();
      var homeWorks = ProcessedHomeWork.GetUnderReviewHomeWorksByStundentId(student.ID);
      if (homeWorks == null || homeWorks.GetEnumerator().MoveNext() == false)
      {
        tbUnderReview.Visibility = Visibility.Collapsed;
        return;
      }
      else
      {
        tbUnderReview.Visibility = Visibility.Visible;
        foreach (var homeWork in homeWorks)
        {
          spUnderReviewHomeWorks.Children.Add(GetDockPanelByUnderReviewHomeWork(homeWork));
        }
      }
    }

    private void InitCompletedHomeWorks()
    {
      spCompletedHomeWorks.Children.Clear();
      var homeWorks = HomeWork.GetCompletedHomeWorksByStudentId(student.ID);
      if (homeWorks == null || homeWorks.GetEnumerator().MoveNext() == false)
      {
        tbСompleted.Visibility = Visibility.Collapsed;
        return;
      }
      foreach (var homeWork in homeWorks)
      {
        spCompletedHomeWorks.Children.Add(GetDockPanelByCompletedHomeWork(homeWork));
      }
    }

    private DockPanel GetDockPanelByCompletedHomeWork((string courseName, DateTime dateChecked, int grade, byte[] file, string fileTitle, DateTime dataUpload, byte[] taskFile, string taskFileTitle, string teacherFullName, DateTime term) homeWork)
    {

      DockPanel dockPanel = new DockPanel()
      {
        LastChildFill = true,
        Background = (SolidColorBrush)this.TryFindResource("SecondaryColor"),
        Margin = new Thickness(0, 20, 40, 0),
        ToolTip = $"Курс: {homeWork.courseName}\nИмя преподователя: {homeWork.teacherFullName}\nСрок: {homeWork.term.ToShortDateString()}\nДата сдачи: {homeWork.dataUpload.ToShortDateString()}\nДата проверки: {homeWork.dateChecked.ToShortDateString()}"
      };


      TextBlock textBlock = new TextBlock()
      {
        Text = homeWork.courseName,
        FontSize = 15,
        FontWeight = FontWeights.SemiBold,
        Foreground = new SolidColorBrush(Colors.Gray),
        Padding = new Thickness(5)
      };
      DockPanel.SetDock(textBlock, Dock.Top);
      dockPanel.Children.Add(textBlock);


      StackPanel stackPanel = new StackPanel()
      {
        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF21BED1"))
      };
      stackPanel.Children.Add(new TextBlock()
      {
        Text = "Дата проверки:",
        Foreground = new SolidColorBrush(Colors.Black),
        Margin = new Thickness(4, 2, 0, 0)
      });
      stackPanel.Children.Add(new TextBlock()
      {
        Text = homeWork.dateChecked.ToShortDateString(),
        Foreground = new SolidColorBrush(Colors.Black),
        Margin = new Thickness(4, 0, 0, 0)
      });
      DockPanel.SetDock(stackPanel, Dock.Bottom);
      dockPanel.Children.Add(stackPanel);


      Grid grid = new Grid()
      {
        Margin = new Thickness(0, 20, 0, 20),
      };
      Style style = (Style)Application.Current.FindResource("MaterialDesignToolForegroundButton"); ;
      StackPanel contentbt1 = new StackPanel();
      var p = new MaterialDesignThemes.Wpf.PackIcon();
      p.Kind = MaterialDesignThemes.Wpf.PackIconKind.FileDownloadOutline;
      p.HorizontalAlignment = HorizontalAlignment.Center;
      contentbt1.Children.Add(p);
      contentbt1.Children.Add(new TextBlock() { Text = "Скачать работу" });
      Button downoloadButton = new Button()
      {
        MinHeight = 40,
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = HorizontalAlignment.Center,
        Style = style,
        Content = contentbt1,
        Tag = new ArrayList() { homeWork.file, homeWork.fileTitle },
      };
      downoloadButton.Click += DownloadHomeWork_ButtonClick;


      StackPanel contentbt2 = new StackPanel();
      p = new MaterialDesignThemes.Wpf.PackIcon();
      p.HorizontalAlignment = HorizontalAlignment.Center;
      p.Kind = MaterialDesignThemes.Wpf.PackIconKind.FileUploadOutline;
      contentbt2.Children.Add(p);
      contentbt2.Children.Add(new TextBlock() { Text = "Скачать задание" });

      Button uploadButton = new Button()
      {
        MinHeight = 40,
        VerticalAlignment = VerticalAlignment.Bottom,
        HorizontalAlignment = HorizontalAlignment.Center,
        Style = style,
        Content = contentbt2,
        Tag = new ArrayList() { homeWork.taskFile, homeWork.taskFileTitle }
      };
      uploadButton.Click += DownloadHomeWork_ButtonClick;

      Border borderGrade = new Border()
      {
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Width = 60,
        Height = 60,
        CornerRadius = new CornerRadius(100),
        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF21BED1"))
      };
      TextBlock grade = new TextBlock()
      {
        Text = homeWork.grade.ToString(),
        FontSize = 30,
        VerticalAlignment = VerticalAlignment.Center,
        HorizontalAlignment = HorizontalAlignment.Center,

      };

      borderGrade.Child = grade;
      grid.Children.Add(borderGrade);
      grid.Children.Add(downoloadButton);
      grid.Children.Add(uploadButton);
      dockPanel.Children.Add(grid);
      return dockPanel;
    }

    private DockPanel GetDockPanelByUnderReviewHomeWork((int homeWorkId, string courseName, byte[] file, string fileTitle, DateTime term, DateTime dataDownoload, byte[] homeWorkFile, string homeWorkFileTitle) homeWork)
    {
      DockPanel dockPanel = new DockPanel()
      {
        LastChildFill = true,
        Background = (SolidColorBrush)this.TryFindResource("SecondaryColor"),
        Margin = new Thickness(0, 20, 40, 0)
      };


      TextBlock textBlock = new TextBlock()
      {
        Text = homeWork.courseName,
        FontSize = 15,
        FontWeight = FontWeights.SemiBold,
        Foreground = new SolidColorBrush(Colors.Gray),
        Padding = new Thickness(5)
      };
      DockPanel.SetDock(textBlock, Dock.Top);
      dockPanel.Children.Add(textBlock);


      StackPanel stackPanel = new StackPanel()
      {
        Background = new SolidColorBrush(Colors.Yellow)
      };
      stackPanel.Children.Add(new TextBlock()
      {
        Text = "Дата сдачи:",
        Foreground = new SolidColorBrush(Colors.Black),
        Margin = new Thickness(4, 2, 0, 0)
      });
      stackPanel.Children.Add(new TextBlock()
      {
        Text = homeWork.dataDownoload.ToShortDateString(),
        Foreground = new SolidColorBrush(Colors.Black),
        Margin = new Thickness(4, 0, 0, 0)
      });
      DockPanel.SetDock(stackPanel, Dock.Bottom);
      dockPanel.Children.Add(stackPanel);


      Grid grid = new Grid()
      {
        Margin = new Thickness(0, 20, 0, 20),
      };
      Style style = (Style)Application.Current.FindResource("MaterialDesignToolForegroundButton"); ;
      StackPanel contentbt1 = new StackPanel();
      var p = new MaterialDesignThemes.Wpf.PackIcon();
      p.Kind = MaterialDesignThemes.Wpf.PackIconKind.FileDownloadOutline;
      p.HorizontalAlignment = HorizontalAlignment.Center;
      contentbt1.Children.Add(p);
      contentbt1.Children.Add(new TextBlock() { Text = "Скачать работу" });
      Button downoloadButton = new Button()
      {
        MinHeight = 40,
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = HorizontalAlignment.Center,
        Style = style,
        Content = contentbt1,
        Tag = new ArrayList() { homeWork.file, homeWork.fileTitle },
      };
      downoloadButton.Click += DownloadHomeWork_ButtonClick;


      StackPanel contentbt2 = new StackPanel();
      p = new MaterialDesignThemes.Wpf.PackIcon();
      p.HorizontalAlignment = HorizontalAlignment.Center;
      p.Kind = MaterialDesignThemes.Wpf.PackIconKind.FileUploadOutline;
      contentbt2.Children.Add(p);
      contentbt2.Children.Add(new TextBlock() { Text = "Скачать задание" });

      Button uploadButton = new Button()
      {
        MinHeight = 40,
        VerticalAlignment = VerticalAlignment.Bottom,
        HorizontalAlignment = HorizontalAlignment.Center,
        Style = style,
        Content = contentbt2,
        Tag = new ArrayList() { homeWork.homeWorkFile, homeWork.homeWorkFileTitle }
      };
      uploadButton.Click += DownloadHomeWork_ButtonClick;
      grid.Children.Add(downoloadButton);
      grid.Children.Add(uploadButton);
      dockPanel.Children.Add(grid);
      return dockPanel;
    }
    private DockPanel GetDockPanelByHomeWork((int homeWorkId, string courseName, byte[] file, DateTime term, string Title) homeWork, (SolidColorBrush color, string text) homeWorkType)
    {
      DockPanel dockPanel = new DockPanel()
      {
        LastChildFill = true,
        Background = (SolidColorBrush)this.TryFindResource("SecondaryColor"),
        Margin = new Thickness(0, 20, 40, 0)
      };


      TextBlock textBlock = new TextBlock()
      {
        Text = homeWork.courseName,
        FontSize = 15,
        FontWeight = FontWeights.SemiBold,
        Foreground = new SolidColorBrush(Colors.Gray),
        Padding = new Thickness(5)
      };
      DockPanel.SetDock(textBlock, Dock.Top);
      dockPanel.Children.Add(textBlock);


      StackPanel stackPanel = new StackPanel()
      {
        Background = homeWorkType.color
      };
      stackPanel.Children.Add(new TextBlock()
      {
        Text = "Срок:    " + homeWorkType.text,
        Foreground = new SolidColorBrush(Colors.Black),
        Margin = new Thickness(4, 2, 0, 0)
      });
      stackPanel.Children.Add(new TextBlock()
      {
        Text = homeWork.term.ToShortDateString(),
        Foreground = new SolidColorBrush(Colors.Black),
        Margin = new Thickness(4, 0, 0, 0)
      });
      DockPanel.SetDock(stackPanel, Dock.Bottom);
      dockPanel.Children.Add(stackPanel);


      Grid grid = new Grid()
      {
        Margin = new Thickness(0, 20, 0, 20),
      };
      Style style = (Style)Application.Current.FindResource("MaterialDesignToolForegroundButton"); ;
      StackPanel contentbt1 = new StackPanel();
      var p = new MaterialDesignThemes.Wpf.PackIcon();
      p.Kind = MaterialDesignThemes.Wpf.PackIconKind.FileDownloadOutline;
      p.HorizontalAlignment = HorizontalAlignment.Center;
      contentbt1.Children.Add(p);
      contentbt1.Children.Add(new TextBlock() { Text = "Скачать" });
      Button downoloadButton = new Button()
      {
        MinHeight = 40,
        VerticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment = HorizontalAlignment.Center,
        Style = style,
        Content = contentbt1,
        Tag = new ArrayList() { homeWork.file, homeWork.Title },
      };
      downoloadButton.Click += DownloadHomeWork_ButtonClick;


      StackPanel contentbt2 = new StackPanel();
      p = new MaterialDesignThemes.Wpf.PackIcon();
      p.HorizontalAlignment = HorizontalAlignment.Center;
      p.Kind = MaterialDesignThemes.Wpf.PackIconKind.FileUploadOutline;
      contentbt2.Children.Add(p);
      contentbt2.Children.Add(new TextBlock() { Text = "Загрузить" });

      Button uploadButton = new Button()
      {
        MinHeight = 40,
        VerticalAlignment = VerticalAlignment.Bottom,
        HorizontalAlignment = HorizontalAlignment.Center,
        Style = style,
        Content = contentbt2,
        Tag = homeWork.homeWorkId
      };
      uploadButton.Click += UploadButton_Click;
      grid.Children.Add(downoloadButton);
      grid.Children.Add(uploadButton);

      dockPanel.Children.Add(grid);

      return dockPanel;
    }

    private void UploadButton_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      if (openFileDialog.ShowDialog() == true)
      {
        ProcessedHomeWork processedHomeWork = new ProcessedHomeWork()
        {
          DataDownoload = DateTime.Now,
          HomeWorkId = (int)((Button)sender).Tag,
          StudentId = student.ID,
          HomeWorkFile = Controler.GetBytesInFile(openFileDialog.FileName),
          HomeWorkFileTitle = openFileDialog.SafeFileName,
        };
        ProcessedHomeWork.Add(processedHomeWork);
      }
      INIT();
    }

    #endregion
    #region State

    enum TimePeriod
    {
      Day = 1,
      Week = 7,
      Month = 31,
      Year = 365
    }
    TimePeriod currentTimePerid = TimePeriod.Day;

    private void InitPieChart(TimePeriod timePeriod)
    {
      SeriesCollection series = new SeriesCollection();
      var values = HomeWork.GetHomeWorksDetailInfoByStudentId(student.ID, DateTime.Now.AddDays(-(int)timePeriod));
      if (values == null) return;
      foreach (var value in values)
      {
        series.Add(new PieSeries() { Title = value.courseName, Values = new ChartValues<ObservableValue>() { new ObservableValue(value.avg) } });
      }
      pieChart.SeriesCollection = series;
    }

    private int GetIndex(UIElement uIElement)
    {
      return Grid.GetColumn(uIElement);
    }
    private int GetIndex(TimePeriod timePeriod)
    {
      switch (timePeriod)
      {
        case TimePeriod.Day: return 0;
        case TimePeriod.Week: return 1;
        case TimePeriod.Month: return 2;
        case TimePeriod.Year: return 3;
        default: return -1;
      }
    }
    private TimePeriod GetPeriodTime(int index)
    {
      switch (index)
      {
        case 0: return TimePeriod.Day;
        case 1: return TimePeriod.Week;
        case 2: return TimePeriod.Month;
        case 3: return TimePeriod.Year;
        default: throw new Exception($"GetPeriodTime(), index - {index}");
      }
    }
    private void TimePeriod_MouseEnter(object sender, MouseEventArgs e)
    {
      int index = GetIndex((UIElement)sender);
      Grid.SetColumn(borderSelectedTime, index);
    }

    private void TimePeriod_MouseLeave(object sender, MouseEventArgs e)
    {
      Grid.SetColumn(borderSelectedTime, GetIndex(currentTimePerid));
    }

    private void TimePeriod_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      int index = GetIndex((UIElement)sender);
      Grid.SetColumn(borderSelectedTime, index);
      currentTimePerid = GetPeriodTime(index);
      InitPieChart(currentTimePerid);
    }
    #endregion

  }
}
