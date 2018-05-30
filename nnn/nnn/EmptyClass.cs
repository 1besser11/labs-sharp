using System;
namespace nnn
{
    class HelloWorld
    {
        static bool calcAngle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            double angle = Math.Atan2(y2 - y1, x2 - x1);
            double degrees = angle * (180 / Math.PI);
            double angle1 = Math.Atan2(y3 - y2, x3 - x2);
            double degrees1 = angle1 * (180 / Math.PI);
            Console.WriteLine(degrees - degrees1);
            int sub = Convert.ToInt32(degrees - degrees1);
            if (sub == 0 || sub == 180 || sub == -180)
            {
                return false;
            }
            else if (sub % 90 == 0)
                return true;
            else
                return false;
        }



    
        static void Main(string[] args)
        {
            int[] c;   // номер хода, на котором посещается вершина
            int[] path; // номера посещаемых вершин

            int v0 = 0;    // начальная вершина
            int[] y, x;
            int[,] matrix;
            int size;
            int chosen_path;
            String mode = Console.ReadLine();
            switch (mode)
            {
                case "rand":
                    size = GetSize();
                    if (size == -1)
                        return;
                    c = new int[size];
                    path = new int[size];
                    x = GenerateRandomCoordinates(size, 10);
                    y = GenerateRandomCoordinates(size, 10);
                    chosen_path = GenerateRandomPath(size);
                    if (chosen_path == -1)
                        return;
                    matrix = GenerateRandom(size);

                    break;


                case "input":
                    x = readCoordinates("x");
                    y = readCoordinates("y");
                    if (x.GetLength(0)==y.GetLength(0))
                    {
                        matrix = readMatrix(x.GetLength(0));
                        c = new int[x.GetLength(0)];
                        path = new int[x.GetLength(0)];
                        chosen_path = GetChosenPath();
                    }
                    else{
                        return;
                    }
                    break;

                case "default":
                    x = new int[] { 0, 0, 1, 1, 2 };
                    y = new int[] { 0, 1, 1, 0, 0 };
                    c = new int[5];
                    path = new int[5];
                    chosen_path = 1;
                    matrix = new int[5, 5]
                            {
                                    {0, 1, 1, 1, 1},
                                    {1, 0, 1, 1, 1},
                                    {1, 1, 0, 1, 1},
                                    {1, 1, 1, 0, 1},
                                    {1, 1, 1, 1, 0}
                            };
                    break;

                default:
                    return;

            }
            for (int i = 0; i < path.GetLength(0); i ++)
            {
                path[i] = -1;
            }


            int[,] z = GenerateRandom(5);//readMatrix();
            PrintMatrix(matrix);
            if(!checkIfMatrixCorrect(z))
                return;
            int n = 5;

            for (int j = 0; j < n; j++) c[j] = -1;
            path[0] = v0;
            c[v0] = 0;
            if (gamilton(1, n, path, matrix, c, v0, x, y, chosen_path) == 1||path[path.GetLength(0)-1]!=-1) { Console.WriteLine("oKAY"); }
            else { Console.WriteLine("Нет решений\n"); }
            /* My first program in C# */

            for (int i = 0; i < path.GetLength(0); i++)
                Console.Write(path[i] + "->");
            Console.WriteLine();
            for (int i = 0; i < path.GetLength(0); i++)
                Console.Write(c[i] + "->");

        }


        static int[] getNumOfWays(int[,] a)
        {
            int[] res = new int[a.GetLength(0)];
            for (int i = 0; i < a.GetLength(0); i++)
            {
                res[i] = 0;
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (i != j && a[i, j] == 1)
                    {
                        res[i] += 1;
                    }
                }
            }
            return res;
        }

        static int gamilton(int k, int n, int[] path, int[,] a, int[] c, int v0,int[] x, int[] y, int chosen_path = 0)
        {

            int v, q1 = 0;
            if (chosen_path == 0)
            {
                for (v = 0; v < n && q1 != 1; v++)
                {
                    if (k < 2)
                    {
                        if (a[v, path[k - 1]] == 1 || a[path[k - 1], v] == 1)
                        {
                            if (k == n ) q1 = 1;
                           
                            else if (c[v] == -1)
                            {
                                c[v] = k; path[k] = v;
                                q1 = gamilton(k + 1, n, path, a, c, v0, x, y);
                                if (q1 != 1) c[v] = -1;
                            }
                            else continue;
                        }
                    }
                    else
                    {
                        if ((a[v, path[k - 1]] == 1 || a[path[k - 1], v] == 1) && calcAngle(x[path[k - 2]], y[path[k - 2]], x[path[k - 1]], y[path[k - 1]], x[v], y[v]) )
                        {
                            if (k == n) q1 = 1;
                            else if (c[v] == -1)
                            {
                                c[v] = k; path[k] = v;
                                q1 = gamilton(k + 1, n, path, a, c, v0, x, y);
                                if (q1 != 1) c[v] = -1;
                            }
                            else continue;
                        }
                    }
                }
                return q1;
            }
           else{
                int z = 0;
                for (v = 0; v < n && q1 != 1; v++)
                {
                    if (a[v, path[k - 1]] == 1 || a[path[k - 1], v] == 1)
                    {
                        z++;
                        if (z == chosen_path)
                        {
                            if (k == n) q1 = 1;
                            else if (c[v] == -1)
                            {
                                c[v] = k; path[k] = v;
                                q1 = gamilton(k + 1, n, path, a, c, v0, x, y);
                                if (q1 != 1) c[v] = -1;
                            }
                            else continue;
                        }
                    }
                }
                return q1;
            }
           
        }
 

        static bool checkIfMatrixCorrect(int[,] a) {
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (a[i, j] != a[j, i]){
                        return false;
                    }


                }
            }
            return true;
        }



        static int[] readCoordinates(string x)
        {
            
            Console.Write(String.Format("Enter the {0}: ", x));
            String foo = Console.ReadLine();
            string[] tokens = foo.Split(',');
            int size = tokens.GetLength(0);
            Console.Write(size);
            int[] numsX = new int[size];
            int i = 0;
            foreach (string s in tokens)
            {
                numsX[i] = Convert.ToInt32(s);
                i++;
            }
            foreach (int z in numsX)
                Console.WriteLine(z);
            return numsX;
        }


        static int[,] readMatrix(int size)
        {
            

            if (size < 0)
                return null;
            int[,] nums = new int[size, size];
            for (int j = 0; j < size; j++)
            {
                string foo = Console.ReadLine();
                string[] tokens = foo.Split(',');
                if (tokens.GetLength(0) > size)
                    return null;

               

                int i = 0;
                foreach (string s in tokens)
                {

                    nums[j, i] = Convert.ToInt32(s);
                    i++;
                }

                for (; i < size; i++)
                {
                    nums[j, i] = 0;
                }
            }
            return nums;
        }
        static void PrintMatrix(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    Console.Write(matrix[i, j]);
                    Console.Write(" ");
                }
                Console.Write("\n"); 
            }

        }

        static int[,] GenerateRandom(int size)
        {
            int[,] matrics = new int[size, size];
            Random rnd = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = i; j < size; j++)
                {
                    if (i == j)
                    {
                        matrics[i, j] = 0;
                    }
                    else
                    {
                        if (rnd.Next(-3, 10)>0)
                            matrics[i, j] = 1;
                        else
                            matrics[i, j] = 0;
                        matrics[j, i] = matrics[i, j];
                    }

                }
            }
            return matrics;

        }

        static int GenerateRandomPath(int size)
        {
            if (size < 0)
                return -1;
            Random rnd = new Random();
            return rnd.Next(0, size);


        }

        static int[] GenerateRandomCoordinates(int size, int range)
        {
            int[] nums = new int[size];
            Random rnd = new Random();
            for (int i = 0; i < size; i++)
            {
                nums[i] = rnd.Next(-range, range);
            }
            return nums;

        }
        static int GetSize()
        {

            try
            {
                return Convert.ToInt32(Console.ReadLine());
            }
            catch(Exception)
            {
                Console.WriteLine("String is not in correct format");
                return -1;
            }

        }
        static int GetChosenPath()
        {

            try
            {
                return Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine("String is not in correct format");
                return -1;
            }

        }



    }
}
