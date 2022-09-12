using AppForm;
using Modules;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RegistrationForm
{
  /// <summary>
  /// Логика взаимодействия для MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    private bool isTeacher { get; set; } = true;
    private bool isRegistration { get; set; } = false;
    public MainWindow()
    {
      InitializeComponent();
      SetPage();
    }
    
    #region DefaultEvents
    private void Drag_Move(object sender, MouseButtonEventArgs e) => this.DragMove();

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
    private void ClearFields(StackPanel stackPanel)
    {
      var mas = stackPanel.Children;
      foreach (var f in mas)
      {
        if (f is TextBox)
          ((TextBox)f).Text = "";
        else if (f is PasswordBox)
          ((PasswordBox)f).Password = "";
        else break;
      }
    }
    private void HideAll()
    {
      spRegistration.Visibility = Visibility.Collapsed;
      spEnter.Visibility = Visibility.Collapsed;
    }
    private void SetPage()
    {
      isRegistration = !isRegistration;
      HideAll();
      if (isRegistration)
      {
        spRegistration.Visibility = Visibility.Visible;
        tbHeader.Text = "Регистрация";
      }
      else
      {
        spEnter.Visibility = Visibility.Visible;
        tbHeader.Text = "Войти";
      }
    }
    #endregion

    #region HeaderEvents
    private int GetSelectIndex(object s)
    {
      var tempGrid = s as Grid;
      if (tempGrid != null)
      {
        return Grid.GetColumn(tempGrid);
      }
      var temp = s as TextBlock;
      return Grid.GetColumn(temp);
    }
    private void UserSelect_MouseLeave(object sender, MouseEventArgs e)
    {
      Grid.SetColumn(selectUser, isTeacher ? 0 : 1);
      SetBorderSelect(!isTeacher);
    }

    private void UserSelect_MouseEnter(object sender, MouseEventArgs e)
    {
      int index = GetSelectIndex(sender);
      Grid.SetColumn(selectUser, index);
      SetBorderSelect(index == 1 ? true : false);
    }

    private void UserSelect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      isTeacher = (GetSelectIndex(sender) == 0 ? true : false);
      Grid.SetColumn(selectUser, isTeacher ? 0 : 1);
    }
    private void SetBorderSelect(bool isteacher) => selectUser.CornerRadius = (isteacher ? new CornerRadius(0, 10, 10, 0) : new CornerRadius(10, 0, 0, 10));

    #endregion

    #region Registration
    private void RegClearButton_Click(object sender, RoutedEventArgs e)
    {

      if (isRegistration)
        ClearFields(spRgFields);
      else
        ClearFields(spEnterFields);
    }

    private string CheckAndGetresultRg()
    {
      var mas = spRgFields.Children;
      foreach (var f in mas)
      {
        string temp = string.Empty;
        if (f is TextBox && (temp = ((TextBox)f).Text).Trim() == "")
          return "Заполните все поля!";
        else if (f is PasswordBox && (((PasswordBox)f).Password).Trim() == "")
          return "Заполните все поля!";
      }

      if (!int.TryParse(tbRgAge.Text, out int age))
        return "Введите правильный возраст!";
      else if (age < 0 && age >= 100)
        return "Неккоректный возраст!";
      if (tbRgPassWord.Password != tbRgConfirmPassWord.Password)
        return "Пароли не совпадают!";
      else if (tbRgPassWord.Password.Length < 6)
        return "Пароль слишком короткий!";


      return "ACCEPTED";
    }
    private void RequireRegistrationButton_Click(object sender, RoutedEventArgs e)
    {
      var result = CheckAndGetresultRg();
      if (result != "ACCEPTED")
        MessageBox.Show(result);
      else
      {
        if (isTeacher)
        {
          var tempTeacher = new Teacher()
          {
            FirstName = tbRgFirstName.Text,
            LastName = tbRgLastName.Text,
            Age = int.Parse(tbRgAge.Text),
            Birthday = null,
            Mail = tbRgMail.Text,
            PassWord = tbRgPassWord.Password,
          };
          var serverAns = Teacher.Add(tempTeacher);
          if (serverAns == -1)
          {
            MessageBox.Show("ОШИБКА, ВВЕДИТЕ КОРРЕКТНЫЕ ДАННЫЕ!");
            return;
          }
          new TeacherForm(tempTeacher).Show();
          this.Close();
        }
        else
        {
          var tempStudent = new Student()
          {
            FirstName = tbRgFirstName.Text,
            LastName = tbRgLastName.Text,
            Age = int.Parse(tbRgAge.Text),
            Birthday = null,
            Mail = tbRgMail.Text,
            PassWord = tbRgPassWord.Password
          };
          var serverAns = Student.Add(tempStudent);
          if (serverAns == -1)
          {
            MessageBox.Show("ОШИБКА, ВВЕДИТЕ КОРРЕКТНЫЕ ДАННЫЕ!");
            return;
          }
          new StudentForm(tempStudent).Show();
          this.Close();
        }
      }
    }
    private void HaveAccountButton_Click(object sender, RoutedEventArgs e)
    {
      SetPage();
    }

    #endregion

    #region Enter
    private void NotHaveAccount_Click(object sender, RoutedEventArgs e)
    {
      SetPage();
    }
    private void EnterButton_Click(object sender, RoutedEventArgs e)
    {
      if (isTeacher)
      {
        var result = Teacher.GetTeacherByLogin(tbEnterMail.Text, tbEnterPassWord.Password);
        if (result == null)
        {
          MessageBox.Show("Неправильный логин или пароль!");
          return;
        }
        new TeacherForm(result).Show();
        this.Close();
      }
      else
      {
        var result = Student.GetStudentByLogin(tbEnterMail.Text, tbEnterPassWord.Password);
        if (result == null)
        {
          MessageBox.Show("Неправильный логин или пароль!");
          return;
        }
        new StudentForm(result).Show();
        this.Close();
      }
    }
    #endregion

  }
}
