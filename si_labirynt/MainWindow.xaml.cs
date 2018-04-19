using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Collections;
using System.Windows.Threading;

namespace si_labirynt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class Node
    {
        public int nr, x, y;
        public List<byte> walls = new List<byte>();

        public Node(int number, int px, int py)
        {
            nr = number;
            x = px;
            y = py;

            walls.Add(1);
            walls.Add(1);
            walls.Add(1);
            walls.Add(1);
        }

        public Node(Node g)
        {
            nr = g.nr;
            x = g.x;
            y = g.y;

            walls.Add(g.walls[0]);
            walls.Add(g.walls[1]);
            walls.Add(g.walls[2]);
            walls.Add(g.walls[3]);
        }

        public int isNeighbour(Node g)
        {
            if (y == g.y + 1 && x == g.x)
                return 1;
            if (x == g.x - 1 && y == g.y)
                return 2;
            if (y == g.y - 1 && x == g.x)
                return 3;
            if (x == g.x + 1 && y == g.y)
                return 4;

            return 0;
        }

        public void addNeighbours(byte up, byte right, byte bottom, byte left)
        {
            walls[0] *= up;
            walls[1] *= right;
            walls[2] *= bottom;
            walls[3] *= left;
        }
    }

    public partial class NodeGrid
    {
        public int columns, rows;
        public List<List<Node>> nodes;

        public NodeGrid(int n, int m, List<List<Node>> NodeList)
        {
            columns = n;
            rows = m;
            nodes = new List<List<Node>>(NodeList);
        }

        //get node object from number
        public Node getNode(int num)
        {
            int i, j;

            i = (num % columns) - 1;

            j = num / columns;

            if (i == -1)
                j -= 1;

            if (i == -1)
                i = columns - 1;


            if (i < columns && j < rows && i >= 0 && j >= 0)
                return nodes[i][j];

            return null;
        }

        public bool areNeighbours(int num1, int num2)
        {
            if (num1 + columns == num2)
                return true;
            if (num1 - columns == num2)
                return true;
            if (num1 + 1 == num2 && num2 % columns != 1)
                return true;
            if (num1 - 1 == num2 && num1 % columns != 1)
                return true;

            return false;
        }

        // set nodes' walls
        public void setWalls(BitArray[] matrix)
        {
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                {
                    nodes[i][j].walls = new List<byte>();
                    nodes[i][j].walls.Add(1);
                    nodes[i][j].walls.Add(1);
                    nodes[i][j].walls.Add(1);
                    nodes[i][j].walls.Add(1);
                }

            for (int i = 1; i <= columns * rows; i++)
                for (int j = 1; j <= columns * rows; j++)
                {
                    if (i == j)
                        continue;

                    if (matrix[i - 1][j - 1] == true)
                    {
                        Node a = getNode(i);
                        Node b = getNode(j);

                        if (a.isNeighbour(b) == 1)
                        {
                            a.addNeighbours(0, 1, 1, 1);
                            b.addNeighbours(1, 1, 0, 1);
                        }
                        else
                        if (a.isNeighbour(b) == 2)
                        {
                            a.addNeighbours(1, 0, 1, 1);
                            b.addNeighbours(1, 1, 1, 0);
                        }
                        else
                        if (a.isNeighbour(b) == 3)
                        {
                            a.addNeighbours(1, 1, 0, 1);
                            b.addNeighbours(0, 1, 1, 1);
                        }
                        else
                        if (a.isNeighbour(b) == 4)
                        {
                            a.addNeighbours(1, 1, 1, 0);
                            b.addNeighbours(1, 0, 1, 1);
                        }
                    }
                }
        }
    }


    public partial class MainWindow : Window
    {
        BitArray[] matrix;
        List<List<int>> adj = new List<List<int>>();
        List<List<Node>> nodes = new List<List<Node>>();
        int columns, rows, startNode, endNode, pathLength, sizeLimit;
        Node selectedNode = null;
        double drawInterval = 0.2;
        NodeGrid grid;
        Rectangle rect = null;
        bool canvasDirty = false, disableChecking = false, noData = true, editMode = false, windowLoaded = false, matrixUpToDate = false, simInProgress = false;
        string currentAlg = "DFS";
        Random r = new Random();


        public MainWindow()
        {
            InitializeComponent();
            MenuChoice.Header = "Wybrany algorytm: DFS";
            drawInterval = 0.2;
            pathLength = 0;
            columns = 0;
            rows = 0;
            sizeLimit = -1;
            windowLoaded = true;
            EditTip.Visibility = Visibility.Hidden;
            GridEdit.IsEnabled = false;
        }

        //calculations
        public void recalculateNodes(int n, int m)
        {
            nodes = new List<List<Node>>();

            for (int i = 0; i < n; i++)
            {
                List<Node> row = new List<Node>();

                for (int j = 0; j < m; j++)
                    row.Add(new Node(n * j + i + 1, i, j));

                nodes.Add(row);
            }
        }

        public void convertAdjToMatrix()
        {
            matrix = new BitArray[adj.Count];
            for (int i = 0; i < matrix.Length; i++)
                matrix[i] = new BitArray(columns * rows);

            int k = 0, l = 0;

            for (int i = 0; i < adj.Count(); i++)
            {
                for (int j = 0; j < adj.Count(); j++)
                    if (l < adj[k].Count())
                        if (i == k && j == adj[k][l] - 1)
                        {
                            matrix[i][j] = true;
                            l++;
                        }
                        else
                            matrix[i][j] = false;
                    else
                        matrix[i][j] = false;
                
                k++;
                l = 0;
            }

            recalculateNodes(columns, rows);
            grid = new NodeGrid(columns, rows, nodes);
            grid.setWalls(matrix);

            matrixUpToDate = true;
        }

        public void convertMatrixToAdj()
        {
            BarStatus.Value = 0;
            BarOperacja.Text = "Konwersja danych";
            PopupBar.IsOpen = true;

            int prog = 0;
            double progmax = columns * rows;

            adj = new List<List<int>>();

            for (int i = 0; i < progmax; i++)
            {
                List<int> row = new List<int>();

                for (int j = 0; j < progmax; j++)
                {
                    if (matrix[i][j] == true)
                        row.Add(j + 1);
                }

                adj.Add(row);
                prog = (int)(((i + 1) / progmax) * 100);
                BarStatus.Dispatcher.Invoke(() => BarStatus.Value = prog, DispatcherPriority.Background);
            }

            PopupBar.IsOpen = false;
        }

        public void makeRandomMatrix()
        {
            int rand, prog = 0;
            double progmax = columns * rows;

            for (int i = 0; i < rows * columns; i++)
            {
                for (int j = i + 1; j < rows * columns; j++)
                {
                    rand = r.Next(0, 10);
                    if (rand > 4)
                    {
                        if (grid.getNode(i + 1).isNeighbour(grid.getNode(j + 1)) > 0)
                        {
                            matrix[i][j] = true;
                            matrix[j][i] = true;
                        }
                        else
                        {
                            matrix[i][j] = false;
                            matrix[j][i] = false;
                        }

                    }
                    else
                    {
                        matrix[i][j] = false;
                        matrix[j][i] = false;
                    }
                }

                prog = (int)(((i + 1) / progmax) * 100);
                BarStatus.Dispatcher.Invoke(() => BarStatus.Value = prog, DispatcherPriority.Background);
            }
        }

        //drawing
        public void drawRect(int x, int y, int width, int height, Brush color)
        {
            rect = new Rectangle();
            rect.Width = width;
            rect.Height = height;
            rect.Stroke = color;
            rect.Fill = color;
            canvas1.Children.Add(rect);
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }

        public void drawBorder(int x, int y, Brush color, int s1, int s2, int s3, int s4)
        {
            if (s1 > 0)
                drawRect(x, y, 20, 2, color);

            if (s2 > 0)
                drawRect(x + 18, y, 2, 20, color);

            if (s3 > 0)
                drawRect(x, y + 18, 20, 2, color);

            if (s4 > 0)
                drawRect(x, y, 2, 20, color);
        }

        public void drawField(int indx, int indy, Brush BorderColor, Brush FillColor, Node g, bool full)
        {
            indx *= 20;
            indy *= 20;

            if (startNode == g.nr)
            {
                drawRect(indx, indy, 20, 20, Brushes.Yellow);
                drawBorder(indx, indy, BorderColor, g.walls[0], g.walls[1], g.walls[2], g.walls[3]);
                drawActivePath(g, null);

                return;
            }
            else if (endNode == g.nr)
            {
                drawRect(indx, indy, 20, 20, Brushes.Red);
                drawBorder(indx, indy, BorderColor, g.walls[0], g.walls[1], g.walls[2], g.walls[3]);

                return;
            }

            if (FillColor != Brushes.White || selectedNode == g || full)
                drawRect(indx, indy, 20, 20, FillColor);
            drawBorder(indx, indy, BorderColor, g.walls[0], g.walls[1], g.walls[2], g.walls[3]);
        }

        public void drawSelected()
        {
            int x = selectedNode.x;
            int y = selectedNode.y;
            drawField(x, y, Brushes.Black, Brushes.White, selectedNode, false);
            drawRect(x * 20 + 5, y * 20 + 5, 10, 2, Brushes.MediumVioletRed);
            drawRect(x * 20 + 5, y * 20 + 5, 2, 10, Brushes.MediumVioletRed);
            drawRect(x * 20 + 13, y * 20 + 5, 2, 10, Brushes.MediumVioletRed);
            drawRect(x * 20 + 5, y * 20 + 13, 10, 2, Brushes.MediumVioletRed);
        }

        public void drawPath(Node a, Node b, Brush color)
        {
            if (!matrixUpToDate)
                convertAdjToMatrix();

            if (b == null)
            {
                drawRect(a.x * 20 + 6, a.y * 20 + 6, 9, 9, color);
                return;
            }

            drawRect(b.x * 20 + 6, b.y * 20 + 6, 9, 9, color);

            if (a.isNeighbour(b) == 1)
                drawRect(a.x * 20 + 8, a.y * 20 - 6, 5, 12, color);

            if (a.isNeighbour(b) == 2)
                drawRect(a.x * 20 + 15, a.y * 20 + 8, 12, 5, color);

            if (a.isNeighbour(b) == 3)
                drawRect(a.x * 20 + 8, a.y * 20 + 15, 5, 12, color);

            if (a.isNeighbour(b) == 4)
                drawRect(a.x * 20 - 6, a.y * 20 + 8, 12, 5, color);
        }

        public void drawActivePath(Node a, Node b)
        {
            drawPath(a, b, Brushes.LimeGreen);
        }

        public void drawDroppedPath(Node a, Node b)
        {
            drawPath(a, b, Brushes.LightGray);
        }

        public void drawSeenPath(Node a, Node b)
        {
            drawPath(a, b, Brushes.PaleVioletRed);
        }

        public void drawLabyrinth()
        {
            canvas1.Children.Clear();

            if (noData)
                return;

            int prog = 0;
            double progmax = columns;
            BarStatus.Value = 0;
            BarOperacja.Text = "Rysowanie labiryntu";
            BarKroki.Text = "";
            PopupBar.IsOpen = true;

            for (int i = 0; i < grid.columns; i++)
            {
                for (int j = 0; j < grid.rows; j++)
                    drawField(i, j, Brushes.Black, Brushes.White, grid.nodes[i][j], false);
                
                prog = (int)(((i + 1) / progmax) * 100);
                BarStatus.Dispatcher.Invoke(() => BarStatus.Value = prog, DispatcherPriority.Background);
            }

            PopupBar.IsOpen = false;
            canvasDirty = false;
        }

        private void StartSimulation(object sender, RoutedEventArgs e)
        {
            if (simInProgress)
            {
                MessageBox.Show("Symulacja jest aktualnie wykonywana.", "Trwa symulacja", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (editMode)
            {
                MessageBox.Show("Nie można rozpocząć symulacji w trakcie edycji. Wpierw zakończ edycję.", "Trwa edytowanie", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (noData)
            {
                MessageBox.Show("Nie ma danych. Wczytaj labirynt z pliku lub stwórz nowy.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(canvasDirty)
                drawLabyrinth();

            if (currentAlg == "DFS")
            {
                simInProgress = true;
                canvasDirty = true;
                DFS();
            }
            else
            if (currentAlg == "BFS")
            {
                simInProgress = true;
                canvasDirty = true;
                BFS();
            }
        }


        //algorithms
        public async Task DFS()
        {
            int[] odwiedzone = new int[1 + rows * columns];
            Stack<int> stos = new Stack<int>();
            int dystans = 0;
            int[] sasiedziOdwiedzeni = new int[1 + rows * columns];
            for (int i = 0; i < rows * columns; i++)
            {
                sasiedziOdwiedzeni[i] = 0;
                odwiedzone[i] = 0;
            }
            int licznik = 0;
            stos.Push(startNode);
            odwiedzone[startNode] = 1;

            if (!(adj[startNode - 1].Count == 0))
            {
                while (stos.Peek() != endNode)
                {
                    if (!simInProgress)
                        break;

                    for (int i = 0; i < adj[stos.Peek() - 1].Count(); i++)
                    {
                        if (!simInProgress)
                            break;

                        if (odwiedzone[adj[stos.Peek() - 1][i]] != 1)
                        {
                            int g = stos.Peek();
                            sasiedziOdwiedzeni[g]++;
                            odwiedzone[g] = 1;
                            stos.Push(adj[stos.Peek() - 1][i]);
                            sasiedziOdwiedzeni[stos.Peek()]++;
                            drawSeenPath(grid.getNode(g), grid.getNode(stos.Peek()));
                            await Task.Delay((int)(drawInterval * 1000));
                            dystans++;
                            break;
                        }
                        else
                        {
                            int pp = sasiedziOdwiedzeni[stos.Peek()];

                            int ppp = adj[stos.Peek() - 1].Count();
                            if (sasiedziOdwiedzeni[stos.Peek()] == adj[stos.Peek() - 1].Count())
                            {
                                int pom = 0;
                                int g = stos.Peek();
                                int x = 0;
                                for (int j = 0; j < adj[stos.Peek() - 1].Count(); j++)
                                {

                                    if (odwiedzone[adj[stos.Peek() - 1][j]] != 0)
                                    {
                                        pom++;
                                    }
                                    else
                                        x = adj[stos.Peek() - 1][j];
                                }
                                if (pom != adj[stos.Peek() - 1].Count())
                                {
                                    sasiedziOdwiedzeni[g]++;
                                    odwiedzone[g] = 1;
                                    stos.Push(x);
                                    sasiedziOdwiedzeni[stos.Peek()]++;
                                    drawSeenPath(grid.getNode(g), grid.getNode(stos.Peek()));
                                    await Task.Delay((int)(drawInterval * 1000));
                                    dystans++;
                                    break;
                                }
                                odwiedzone[g] = 1;
                                stos.Pop();
                                drawDroppedPath(grid.getNode(stos.Peek()), grid.getNode(g));
                                await Task.Delay((int)(drawInterval * 1000));
                                dystans--;
                                break;
                            }
                            else
                            {
                                int pom = 0;
                                for (int j = 0; j < adj[stos.Peek() - 1].Count(); j++)
                                {
                                    if (odwiedzone[adj[stos.Peek() - 1][j]] != 0)
                                    {
                                        pom++;
                                    }
                                }
                                if (pom == adj[stos.Peek() - 1].Count())
                                {
                                    int g = stos.Peek();
                                    odwiedzone[g] = 1;
                                    dystans--;
                                    stos.Pop();
                                    drawDroppedPath(grid.getNode(stos.Peek()), grid.getNode(g));
                                    await Task.Delay((int)(drawInterval * 1000));
                                }
                            }
                        }
                    }

                    if (stos.Peek() == startNode)
                    {
                        bool finish = false;
                        for (int i = 0; i < adj[startNode - 1].Count; i++)
                            if (odwiedzone[adj[startNode - 1][i]] == 1)
                                finish = true;
                            else
                                finish = false;

                        if (finish)
                        {
                            licznik = 1;
                            break;
                        }
                    }
                }


                if (simInProgress)
                {
                    if (licznik == 1)
                    {
                        simInProgress = false;
                        var toolTip = new ToolTip();

                        toolTip.Content = "Nie znaleziono ścieżki do wyjścia z labiryntu !!";
                        toolTip.StaysOpen = false;
                        toolTip.IsOpen = true;
                        await Task.Delay(3000);
                        toolTip.IsOpen = false;
                    }
                    else
                    {
                        int pom = stos.Peek();
                        drawActivePath(grid.getNode(pom), grid.getNode(pom));
                        while (pom != startNode)
                        {
                            pom = stos.Peek();
                            if (pom == startNode)
                                break;
                            stos.Pop();
                            drawActivePath(grid.getNode(pom), grid.getNode(stos.Peek()));
                        }
                        await Task.Delay((int)(drawInterval * 1000));
                        pathLength = dystans;


                        simInProgress = false;
                        var toolTip = new ToolTip();

                        string a = "Znaleziono ścieżkę do wyjścia z labiryntu o długości:  ";
                        string b = pathLength.ToString();
                        toolTip.Content = a + b;
                        toolTip.StaysOpen = false;
                        toolTip.IsOpen = true;
                        await Task.Delay(3000);
                        toolTip.IsOpen = false;

                    }
                }
            }
            else
            {
                simInProgress = false;
                pathLength = -1;
                var toolTip = new ToolTip();

                toolTip.Content = "Nie znaleziono ścieżki do wyjścia z labiryntu !!";
                toolTip.StaysOpen = false;
                toolTip.IsOpen = true;
                await Task.Delay(3000);
                toolTip.IsOpen = false;

            }
        }


        public async Task BFS()
        {
            int[] poprzednik;
            bool[] odwiedzone;
            poprzednik = new int[1 + rows * columns];
            odwiedzone = new bool[1 + rows * columns];

            for (int i = 0; i < poprzednik.Count(); i++)
            {
                if (!simInProgress)
                    break;

                poprzednik[i] = -1;
                odwiedzone[i] = false;
            }

            Queue<int> kolejka = new Queue<int>();
            kolejka.Enqueue(startNode);
            int nieznaleziono = 0;

            while (kolejka.Peek() != endNode)
            {
                if (!simInProgress)
                    break;

                for (int i = 0; i < adj[kolejka.Peek() - 1].Count(); i++)
                {
                    if (!simInProgress)
                        break;

                    if (odwiedzone[adj[kolejka.Peek() - 1][i]] != true)
                    {
                        kolejka.Enqueue(adj[kolejka.Peek() - 1][i]);
                        odwiedzone[adj[kolejka.Peek() - 1][i]] = true;
                        poprzednik[adj[kolejka.Peek() - 1][i]] = kolejka.Peek();
                        drawDroppedPath(grid.getNode(poprzednik[adj[kolejka.Peek() - 1][i]]), grid.getNode(adj[kolejka.Peek() - 1][i]));
                        await Task.Delay((int)(drawInterval * 1000));
                    }
                }

                if (kolejka.Peek() != startNode)
                    drawSeenPath(grid.getNode(poprzednik[kolejka.Peek()]), grid.getNode(kolejka.Peek()));

                odwiedzone[kolejka.Peek()] = true;
                kolejka.Dequeue();

                if (kolejka.Count() == 0)
                {
                    nieznaleziono = 1;
                    break;
                }
            }

            if (simInProgress)
                if (nieznaleziono == 0)
                {
                    odwiedzone[kolejka.Peek()] = true;
                    drawDroppedPath(grid.getNode(poprzednik[kolejka.Peek()]), grid.getNode(kolejka.Peek()));
                    kolejka.Dequeue();
                    int pom = endNode;
                    int dystans = 0;
                    while (poprzednik[pom] != startNode && simInProgress)
                    {
                        drawActivePath(grid.getNode(poprzednik[pom]), grid.getNode(pom));
                        pom = poprzednik[pom];
                        dystans++;
                    }
                    drawActivePath(grid.getNode(poprzednik[pom]), grid.getNode(pom));
                    dystans++;

                    pathLength = dystans;

                    simInProgress = false;
                    var toolTip = new ToolTip();

                    string a = "Znaleziono ścieżkę do wyjścia z labiryntu o długości:  ";
                    string b = pathLength.ToString();
                    toolTip.Content = a + b;
                    toolTip.StaysOpen = false;
                    toolTip.IsOpen = true;
                    await Task.Delay(3000);
                    toolTip.IsOpen = false;
                }
                else
                {
                    simInProgress = false;
                    pathLength = -1;
                    var toolTip = new ToolTip();

                    toolTip.Content = "Nie znaleziono ścieżki do wyjścia z labiryntu !!";
                    toolTip.StaysOpen = false;
                    toolTip.IsOpen = true;
                    await Task.Delay(3000);
                    toolTip.IsOpen = false;
                }
        }


        // events
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!editMode)
                return;

            Point a = e.GetPosition(canvas1);

            int posX = (int)(a.X - (a.X % 20));
            int posY = (int)(a.Y - (a.Y % 20));
            int i = posX / 20;
            int j = posY / 20;
            Node g;

            if (i < grid.columns && j < grid.rows)
                if ((g = grid.getNode(columns * j + i + 1)) != null)
                {
                    if (selectedNode != null)
                        drawField(selectedNode.x, selectedNode.y, Brushes.Black, Brushes.White, selectedNode, false);

                    selectedNode = g;
                    drawSelected();
                    disableChecking = true;

                    lblX.Content = selectedNode.x.ToString();
                    lblY.Content = selectedNode.y.ToString();
                    if (selectedNode.walls[0] == 1)
                        cUp.IsChecked = true;
                    else
                        cUp.IsChecked = false;

                    if (selectedNode.walls[1] == 1)
                        cRight.IsChecked = true;
                    else
                        cRight.IsChecked = false;

                    if (selectedNode.walls[2] == 1)
                        cDown.IsChecked = true;
                    else
                        cDown.IsChecked = false;

                    if (selectedNode.walls[3] == 1)
                        cLeft.IsChecked = true;
                    else
                        cLeft.IsChecked = false;

                    if (selectedNode.nr == startNode)
                        Startowy.IsChecked = true;
                    else
                    if (selectedNode.nr == endNode)
                        Koncowy.IsChecked = true;
                    else
                        Zwykly.IsChecked = true;

                    disableChecking = false;
                }
        }

        private void EndEditing(object sender, RoutedEventArgs e)
        {
            if (startNode == -1 || endNode == -1)
            {
                MessageBox.Show("Wybierz węzeł startowy i końcowy.", "Niekompletne dane", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (selectedNode != null)
                drawField(selectedNode.x, selectedNode.y, Brushes.Black, Brushes.White, selectedNode, false);
            convertMatrixToAdj();
            editMode = false;
            WholeMenu.IsEnabled = true;
            EditTip.Visibility = Visibility.Hidden;
            GridEdit.IsEnabled = false;
        }

        private void Node_Checked(object sender, RoutedEventArgs e)
        {
            if (selectedNode == null || !editMode)
                return;

            Node g = selectedNode;

            switch (((RadioButton)e.Source).Name)
            {
                case "Startowy":
                    if (endNode == selectedNode.nr)
                        endNode = -1;
                    if(startNode != -1)
                        g = grid.getNode(startNode);
                    startNode = selectedNode.nr;
                    drawField(g.x, g.y, Brushes.Black, Brushes.White, g, true);
                    break;
                case "Koncowy":
                    if (startNode == selectedNode.nr)
                        startNode = -1;
                    if (endNode != -1)
                        g = grid.getNode(endNode);
                    endNode = selectedNode.nr;
                    drawField(g.x, g.y, Brushes.Black, Brushes.White, g, true);
                    break;
                case "Zwykly":
                    if (selectedNode.nr == startNode)
                        startNode = -1;
                    else
                    if (selectedNode.nr == endNode)
                        endNode = -1;
                    break;
            }

            drawSelected();
        }

        private void setWall(int side, bool isNotWall)
        {
            if (disableChecking)
                return;

            if (!editMode)
                return;

            if (selectedNode == null)
                return;

            Node g = new Node(selectedNode);

            switch (side)
            {
                case 1:
                    if ((g = grid.getNode(selectedNode.nr - columns)) != null)
                    {
                        if (selectedNode.isNeighbour(g) == side)
                        {
                            matrix[selectedNode.nr - 1][selectedNode.nr - columns - 1] = isNotWall;
                            matrix[selectedNode.nr - columns - 1][selectedNode.nr - 1] = isNotWall;

                            if (selectedNode.walls[0] == 1)
                            {
                                selectedNode.walls[0] = 0;
                                g.walls[2] = 0;
                            }
                            else
                            {
                                selectedNode.walls[0] = 1;
                                g.walls[2] = 1;
                            }
                        }
                    }
                    break;

                case 2:
                    if ((g = grid.getNode(selectedNode.nr + 1)) != null)
                    {
                        if (selectedNode.isNeighbour(g) == side)
                        {
                            matrix[selectedNode.nr - 1][selectedNode.nr] = isNotWall;
                            matrix[selectedNode.nr][selectedNode.nr - 1] = isNotWall;

                            if (selectedNode.walls[1] == 1)
                            {
                                selectedNode.walls[1] = 0;
                                g.walls[3] = 0;
                            }
                            else
                            {
                                selectedNode.walls[1] = 1;
                                g.walls[3] = 1;
                            }
                        }
                    }
                    break;

                case 3:
                    if ((g = grid.getNode(selectedNode.nr + columns)) != null)
                    {
                        if (selectedNode.isNeighbour(g) == side)
                        {
                            matrix[selectedNode.nr - 1][selectedNode.nr + columns - 1] = isNotWall;
                            matrix[selectedNode.nr + columns - 1][selectedNode.nr - 1] = isNotWall;

                            if (selectedNode.walls[2] == 1)
                            {
                                selectedNode.walls[2] = 0;
                                g.walls[0] = 0;
                            }
                            else
                            {
                                selectedNode.walls[2] = 1;
                                g.walls[0] = 1;
                            }
                        }
                    }
                    break;

                case 4:
                    if ((g = grid.getNode(selectedNode.nr - 1)) != null)
                    {
                        if (selectedNode.isNeighbour(g) == side)
                        {
                            matrix[selectedNode.nr - 1][selectedNode.nr - 2] = isNotWall;
                            matrix[selectedNode.nr - 2][selectedNode.nr - 1] = isNotWall;

                            if (selectedNode.walls[3] == 1)
                            {
                                selectedNode.walls[3] = 0;
                                g.walls[1] = 0;
                            }
                            else
                            {
                                selectedNode.walls[3] = 1;
                                g.walls[1] = 1;
                            }
                        }
                    }
                    break;
            }

            if (g != null)
            {
                drawRect(g.x * 20, g.y * 20, 20, 20, Brushes.White);
                drawField(g.x, g.y, Brushes.Black, Brushes.White, g, false);
            }
            drawSelected();
        }

        private void modifyWalls(object sender, RoutedEventArgs e)
        {
            if (selectedNode == null)
                return;

            CheckBox ck = (CheckBox)e.Source;

            switch (ck.Name)
            {
                case "cUp":
                    if (ck.IsChecked == true)
                        setWall(1, false);
                    else
                        setWall(1, true);
                    break;

                case "cRight":
                    if (ck.IsChecked == true)
                        setWall(2, false);
                    else
                        setWall(2, true);
                    break;

                case "cDown":
                    if (ck.IsChecked == true)
                        setWall(3, false);
                    else
                        setWall(3, true);
                    break;

                case "cLeft":
                    if (ck.IsChecked == true)
                        setWall(4, false);
                    else
                        setWall(4, true);
                    break;
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    if (cDown.IsChecked == true)
                        cDown.IsChecked = false;
                    else
                        cDown.IsChecked = true;
                    break;
                case Key.Up:
                    if (cUp.IsChecked == true)
                        cUp.IsChecked = false;
                    else
                        cUp.IsChecked = true;
                    break;
                case Key.Left:
                    if (cLeft.IsChecked == true)
                        cLeft.IsChecked = false;
                    else
                        cLeft.IsChecked = true;
                    break;
                case Key.Right:
                    if (cRight.IsChecked == true)
                        cRight.IsChecked = false;
                    else
                        cRight.IsChecked = true;
                    break;
            }

            e.Handled = true;
        }

        private void ShowInfo(object sender, RoutedEventArgs e)
        {
            WindowInfo wnd = new WindowInfo();
            wnd.Owner = this;
            wnd.Show();
        }


        private void ChangeAlg(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)e.Source).Name == "MenuDFS")
            {
                currentAlg = "DFS";
                MenuChoice.Header = "Wybrany algorytm: DFS";
            }
            else if (((MenuItem)e.Source).Name == "MenuBFS")
            {
                currentAlg = "BFS";
                MenuChoice.Header = "Wybrany algorytm: BFS";
            }
        }

        private void StopSimulation(object sender, RoutedEventArgs e)
        {
            if (simInProgress)
            {
                simInProgress = false;
                MessageBox.Show("Zatrzymano symulację.", "Zatrzymano symulację.", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SetDrawInterval(object sender, SelectionChangedEventArgs e)
        {
            if (windowLoaded)
            {
                int i = SpeedComboBox.SelectedIndex;
                if (i == 0)
                    drawInterval = 1;
                if (i == 1)
                    drawInterval = 0.5;
                if (i == 2)
                    drawInterval = 0.2;
                if (i == 3)
                    drawInterval = 0.05;
                if (i == 4)
                    drawInterval = 0.01;
                if (i == 5)
                    drawInterval = 0;
            }
        }

        private void NewFile(object sender, RoutedEventArgs e)
        {
            if (simInProgress)
            {
                MessageBox.Show("Nie można wykonać operacji w trakcie symulacji.", "Trwa symulacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NewLabyrinth dlg = new NewLabyrinth();
            dlg.Owner = this;

            if (dlg.ShowDialog() == true)
            {
                columns = dlg.columns;
                rows = dlg.rows;
                bool random = dlg.random;

                if ((columns <= 0 || rows <= 0) || (columns <= 1 && rows <= 1))
                {
                    MessageBox.Show("Labirynt jest zbyt mały.", "Błąd dodawania", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if ((columns * rows > sizeLimit && sizeLimit > 0) || rows > 9999 || columns > 9999)
                {
                    MessageBox.Show("Labirynt jest zbyt duży.", "Błąd dodawania", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int prog = 0;
                double progmax = columns * rows;

                startNode = 1;
                endNode = columns * rows;

                nodes = new List<List<Node>>();
                recalculateNodes(columns, rows);
                grid = new NodeGrid(columns, rows, nodes);
                matrix = new BitArray[columns * rows];

                for (int i = 0; i < matrix.Length; i++)
                    matrix[i] = new BitArray(columns * rows);

                BarStatus.Value = 0;
                if (random)
                    BarKroki.Text = "1/3";
                else
                    BarKroki.Text = "1/2";
                BarOperacja.Text = "Generowanie macierzy";
                PopupBar.IsOpen = true;

                for (int i = 0; i < rows * columns; i++)
                {
                    for (int j = 0; j < rows * columns; j++)
                    {
                        if (grid.areNeighbours(i + 1, j + 1))
                            matrix[i][j] = true;
                        else
                            matrix[i][j] = false;
                    }
                    
                    prog = (int)(((i + 1) / progmax) * 100);
                    BarStatus.Dispatcher.Invoke(() => BarStatus.Value = prog, DispatcherPriority.Background);
                }

                PopupBar.IsOpen = false;

                if (random)
                {
                    BarStatus.Value = 0;
                    BarKroki.Text = "2/3";
                    BarOperacja.Text = "Losowanie ścian";
                    PopupBar.IsOpen = true;

                    makeRandomMatrix();

                    PopupBar.IsOpen = false;
                    BarKroki.Text = "3/3";
                }
                else
                    BarKroki.Text = "2/2";

                grid.setWalls(matrix);
                convertMatrixToAdj();

                selectedNode = null;
                noData = false;
                matrixUpToDate = false;
                simInProgress = false;

                lblColumns.Content = columns.ToString();
                lblRows.Content = rows.ToString();

                canvas1.Width = columns * 20;
                canvas1.Height = rows * 20;
                drawLabyrinth();
            }

        }

        private void EditFile(object sender, RoutedEventArgs e)
        {
            if (simInProgress)
            {
                MessageBox.Show("Nie można wykonać operacji w trakcie symulacji.", "Trwa symulacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (noData)
            {
                MessageBox.Show("Nie ma danych. Wczytaj labirynt z pliku lub stwórz nowy.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if(canvasDirty)
                drawLabyrinth();
            editMode = true;
            WholeMenu.IsEnabled = false;
            EditTip.Visibility = Visibility.Visible;
            GridEdit.IsEnabled = true;
        }

        public string[] convertToAdjString()
        {
            string[] ss;
            ss = new string[1 + rows * columns];

            // fill array with strings
            ss[0] = columns.ToString() + " " + rows.ToString() + " " + startNode.ToString() + " " + endNode.ToString();

            for (int i = 1; i < rows * columns + 1; i++)
            {
                for (int j = 0; j < adj[i - 1].Count; j++)
                {
                    if (j == adj[i - 1].Count - 1)
                        ss[i] += (adj[i - 1][j].ToString());
                    else
                        ss[i] += (adj[i - 1][j].ToString() + " ");
                }
            }

            return ss;
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if (simInProgress)
            {
                MessageBox.Show("Nie można wykonać operacji w trakcie symulacji.", "Trwa symulacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (noData)
            {
                MessageBox.Show("Brak danych. Nie można zapisać.", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Labirynt"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Dokument tekstowy (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                File.WriteAllLines(filename, convertToAdjString());
            }
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            if (simInProgress)
            {
                MessageBox.Show("Nie można wykonać operacji w trakcie symulacji.", "Trwa symulacja", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Dokument tekstowy (*.txt)|*.txt";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                string[] readText = File.ReadAllLines(filename);
                adj = new List<List<int>>();

                for (int i = 0; i < readText.Count(); i++)
                {
                    if (i == 0)
                    {
                        string[] values = readText[0].Split(' ');
                        int.TryParse(values[0], out columns);
                        int.TryParse(values[1], out rows);
                        int.TryParse(values[2], out startNode);
                        int.TryParse(values[3], out endNode);
                    }
                    else
                    {
                        string[] values = readText[i].Split(' ');

                        List<int> ln = new List<int>(values.Count());

                        for (int j = 0; j < values.Count(); j++)
                        {
                            int temp;
                            int.TryParse(values[j], out temp);
                            ln.Add(temp);
                        }

                        adj.Add(ln);
                    }
                }


                nodes = new List<List<Node>>();
                selectedNode = null;
                editMode = false;
                matrixUpToDate = true;
                simInProgress = false;

                nodes = new List<List<Node>>();
                recalculateNodes(columns, rows);
                grid = new NodeGrid(columns, rows, nodes);
                matrix = new BitArray[columns * rows];

                for (int i = 0; i < matrix.Length; i++)
                    matrix[i] = new BitArray(columns * rows);

                noData = false;

                convertAdjToMatrix();
                drawLabyrinth();
            }
        }

    }
}
