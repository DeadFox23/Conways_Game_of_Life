using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using Microsoft.Win32;

namespace Conways_Game_of_Life
{
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        Rectangle[,] field; //= new Rectangle[12, 24];
        Boolean[,] nextGen; // = new Boolean[12, 24];
        int tickSpeed = 0;
        int sizeY = 12;
        int sizeX = 24;
        int gencount = 0;
        public MainWindow()
        {
            InitializeComponent();
            field = new Rectangle[sizeY, sizeX];
            nextGen = new Boolean[sizeY,sizeX];
            genField();
            timer.Interval = TimeSpan.FromMilliseconds(tickSpeed);
            timer.Tick += gameTick;
            timer.Stop();
        }

        private void gameTick(object? sender, EventArgs e)
        {
            gameGen();
        }



        private void genField()
        {
            field = new Rectangle[sizeY, sizeX];
            nextGen = new Boolean[sizeY, sizeX];
            int sq = 25;
            for (int i = 0; i < sizeY; i++)
            {
                for (int j = 0; j < sizeX; j++)
                {
                    
                    field[i, j] = new Rectangle();
                    field[i, j].Width = sq;
                    field[i, j].Height = sq;
                    field[i, j].Fill = Brushes.Red;
                    field[i, j].Stroke = Brushes.Black;
                    field[i, j].MouseDown += rec_MouseDown;
                    nextGen[i, j] = false;
                    Canvas_Canvas.Children.Add(field[i, j]);
                    Canvas.SetTop(field[i, j], i * sq + 25);
                    Canvas.SetLeft(field[i, j], j * sq + 150);

                }
            }
            
            Application.Current.MainWindow.Height = sizeY*sq+110;
            Application.Current.MainWindow.Width = sizeX*sq+200;
        }

        private void gameGen()
        {
            checkNeigh();
            applyNextGen();
            gencount++;
            Gen_Value.Content =gencount.ToString();
        }
        private void checkNeigh()
        {
            for (int row = 0; row < field.GetLength(0); row++)
            {
                for (int col = 0; col < field.GetLength(1); col++)
                {
                    //int minRow = int.Max(row - 1, 0);
                    //int maxRow = int.Min(row + 1, field.GetLength(0) - 1);
                    //int minCol = int.Max(col - 1, 0);
                    //int maxCol = int.Min(col + 1, field.GetLength(1) - 1);

                    int minRow = row-1 <0 ? 0 : row-1;
                    int maxRow = row + 1 > field.GetLength(0)-1 ? field.GetLength(0)-1 : row + 1;
                    int minCol = col-1 <0 ? 0 : col-1;
                    int maxCol = col +1 > field.GetLength(1)-1? field.GetLength(1)-1 : col + 1;
                    int neightbors = 0;

                    for (int i = minRow; i <= maxRow; i++)
                    {
                        for (int j = minCol; j <= maxCol; j++)
                        {
                            if (row == i && col == j) neightbors--;
                            if (field[i, j].Fill == Brushes.Green)
                            {
                                neightbors++;
                                if (neightbors > 4) break;
                            }
                            if (neightbors > 4) break;
                        }

                    }
                    {
                        if (field[row, col].Fill == Brushes.Green)
                        {
                            switch (neightbors)
                            {
                                case 0:
                                case 1:
                                    nextGen[row, col] = false; break;
                                case 2 or 3:
                                    nextGen[row, col] = true; break;
                                default:
                                    nextGen[row, col] = false; break;

                            }
                        }
                        else if (field[row,col].Fill == Brushes.Red && neightbors==2)
                        {
                            nextGen[row, col] = true;
                        }
                    }
                }
            }
        }
        public void applyNextGen()
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (nextGen[i, j])
                    {
                        field[i, j].Fill = Brushes.Green;
                    }
                    if (!nextGen[i, j])
                    {
                        field[i, j].Fill = Brushes.Red;
                    }
                }
            }
        } 
        private void UpdateField()
        {
            //idfk need fix some other time lol
            
            
            genField();
        }
        private void RemoveChild()
        {
            for (int i = 0; i < sizeY; i++)
            {
                for (int j = 0; j < sizeX; j++)
                {
                    Canvas_Canvas.Children.Remove(field[i, j]);
                }
            }
        }
        //Save the current game state
        private void SaveData_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(sizeY+","+sizeX+";");
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j].Fill == Brushes.Green)
                    {
                        sb.Append(i+","+j+";");
                    }
                }
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "nope";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, sb.ToString());
                
            }
        }
        //Load game save
        private void LoadData_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog loadFile = new OpenFileDialog();
            loadFile.Filter = "Nope file (*.nope)|*.nope";
            if (loadFile.ShowDialog() == true)
            {
                RemoveChild();
                String selectedFile = File.ReadAllText(loadFile.FileName);
                int subIndex = selectedFile.IndexOf(';');
                string dimensionsString = selectedFile.Substring(0, subIndex);
                string dataString = selectedFile.Substring(subIndex + 1);

                string[] dimensions = dimensionsString.Split(",");
                sizeY = int.Parse(dimensions[0]);
                sizeX = int.Parse(dimensions[1]);
                genField();
                string[] lines = dataString.Split(';');

                foreach (String line in lines)
                {
                    string[] split = line.Split(",");

                    if (split.Length == 2)
                    {
                        int i = int.Parse(split[0]);
                        int j = int.Parse(split[1]);

                        field[i,j].Fill = Brushes.Green;
                    }
                }
            }
            
        }
       

        //Start/Stop Button
        private void Start_Stop_Click(object sender, RoutedEventArgs e)
        {

            if (timer.IsEnabled)
            {
                timer.Stop();
            }
            else
            {
                timer.Start();
            }

        }
        //Next Button
        private void Next_Gen_Click(object sender, RoutedEventArgs e)
        {
            gameGen();
        }
        //mouse interaction
        private void rec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rec = (Rectangle)sender;
            if (rec.Fill == Brushes.Red) rec.Fill = Brushes.Green;
            else rec.Fill = Brushes.Red;
        }
        //Reset Field
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            RemoveChild();
            genField();
            timer.Stop();
            gencount = 0;
            Gen_Value.Content = gencount.ToString();
        }
        //Speed Slider 
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tickSpeed = (int)SpeedSlider.Value;
            timer.Stop();
            timer.Interval = TimeSpan.FromMilliseconds(tickSpeed);
            timer.Start();
        }
        //Field Width Slider
        private void FieldWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sizeX = (int)FieldWidth.Value;
            
            UpdateField();
        }
        //Field Height Slider
        private void FieldHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sizeY= (int)FieldHeight.Value;

            UpdateField();
        }
    }
}