using Microsoft.Win32;
using Modules;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AppForm
{
  static class Controler
  {
    public static void InitFields(this StackPanel stackPanel, User user)
    {
      foreach (var field in stackPanel.Children)
      {
        if (field is TextBox)
        {
          var textBox = (TextBox)field;
          if (textBox.Name == "tbFirstName") textBox.Text = user.FirstName;
          else if (textBox.Name == "tbLastName") textBox.Text = user.LastName;
          else if (textBox.Name == "tbAge") textBox.Text = user.Age.ToString();
          else if (textBox.Name == "tbMail") textBox.Text = user.Mail;
        }
        else if (field is PasswordBox)
        {
          ((PasswordBox)field).Password = user.PassWord;
        }
        else if (field is DatePicker)
        {
          if (user.Birthday != null)
            ((DatePicker)field).SelectedDate = DateTime.Parse(((DateTime)user.Birthday).ToShortDateString());
        }
      }
    }



    public static void InitTbHeader(this TextBlock textBlock, string firstName, string lastName)
    {
      textBlock.Text = textBlock.Text.Replace("@user", firstName + " " + lastName);
    }

    public static byte[] GetBytesInFile(string name)
    {
      FileInfo fileInfo = new FileInfo(name);
      using (FileStream fs = fileInfo.OpenRead())
      {
        byte[] bytes = new byte[fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        return bytes;
      }
    }

    public static void DownoloadFile(byte[] data, string title)
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog()
      {
        Title = "Скачать домашнее задание",
        Filter = $"{Path.GetExtension(title)} | *{Path.GetExtension(title)}",
        AddExtension = false,
      };
      if (true == saveFileDialog.ShowDialog())
      {
        using (var fs = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
        {
          fs.Write(data, 0, data.Length);
        }
      }
    }

    public static void UploadImage(Teacher user)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = "Выберите изображение";
      openFileDialog.Filter = "Picture files | *png; *jpg; *jpeg;";
      if (openFileDialog.ShowDialog() == true)
      {
        byte[] values = GetBytesInFile(openFileDialog.FileName);
        user.Image = values;
        user.ImageTitle = openFileDialog.SafeFileName;
        Teacher.Update((Teacher)user);
      }
    }


    public static void UploadImage(Student user)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      openFileDialog.Title = "Выберите изображение";
      openFileDialog.Filter = "Picture files | *png; *jpg; *jpeg;";
      if (openFileDialog.ShowDialog() == true)
      {
        byte[] values = GetBytesInFile(openFileDialog.FileName);
        user.Image = values;
        user.ImageTitle = openFileDialog.SafeFileName;
        Student.Update((Student)user);
      }
    }


    public static void InitImage(this System.Windows.Shapes.Ellipse imageBrush, User user)
    {
      if (user.ImageTitle == null) return;
      try
      {
        using (var fs = new FileStream(user.ImageTitle, FileMode.OpenOrCreate))
        {
          fs.Write(user.Image, 0, user.Image.Length);
          fs.Close();
          BitmapImage bitmapImage = new BitmapImage();
          bitmapImage.BeginInit();
          bitmapImage.UriSource = new Uri(fs.Name);
          bitmapImage.EndInit();
          imageBrush.Fill = new ImageBrush() { ImageSource = bitmapImage, Stretch = Stretch.UniformToFill };
        }
      }
      catch (Exception ex)
      {

      }
    }



  }
}
