using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace AppForm.Charts
{
  /// <summary>
  /// Логика взаимодействия для PieChart.xaml
  /// </summary>
  public partial class PieChart : UserControl
  {
    public PieChart()
    {
      InitializeComponent();
      //SeriesCollection = new SeriesCollection
      //      {
      //          new PieSeries
      //          {
      //              Title = "Chrome",
      //              Values = new ChartValues<ObservableValue> { new ObservableValue(8) },
      //              DataLabels = true
      //          },
      //          new PieSeries
      //          {
      //              Title = "Mozilla",
      //              Values = new ChartValues<ObservableValue> { new ObservableValue(6) },
      //              DataLabels = true
      //          },
      //          new PieSeries
      //          {
      //              Title = "Opera",
      //              Values = new ChartValues<ObservableValue> { new ObservableValue(10) },
      //              DataLabels = true
      //          },
      //          new PieSeries
      //          {
      //              Title = "Explorer",
      //              Values = new ChartValues<ObservableValue> { new ObservableValue(4) },
      //              DataLabels = true
      //          }
      //      };
      DataContext = this;
    }
    public SeriesCollection SeriesCollection
    {
      get { return (SeriesCollection)GetValue(SeriesCollectionProperty); }
      set
      {
        SetValue(SeriesCollectionProperty, value);
      }
    }
    public static readonly DependencyProperty SeriesCollectionProperty =
      DependencyProperty.Register("SeriesCollection",
                                  typeof(SeriesCollection),
                                  typeof(PieChart),
                                  new PropertyMetadata(ActiveButtonColorPropertyChanged));
    private static void ActiveButtonColorPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
    {
      if (source is PieChart)
      {
        PieChart control = source as PieChart;
        control.SeriesCollection= (SeriesCollection)e.NewValue;
      }
    }
    private void PieChart_DataClick(object sender, ChartPoint chartPoint)
    {

    }
  }
}
