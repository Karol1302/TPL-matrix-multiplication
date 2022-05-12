using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

class Program
{
    static void fillmatrix(int [,] matrix)
    {
        int z = 10;
        Random rdm = new Random();
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(0); j++)
            {
                matrix[i, j] = rdm.Next(-z, z);
            }
        }
    }

    static int [,] Sekwencyjnie(int [,] matrixA, int[,] matrixB)
    {

        int[,] result = new int[matrixA.GetLength(0), matrixB.GetLength(0)];

        for (int i = 0; i < matrixA.GetLength(0); i++)
            for (int j = 0; j < matrixB.GetLength(1); j++)
                for(int k = 0; k < matrixA.GetLength(1); k++)
                    result [i, j] += matrixA [i, k] * matrixB[k, j]; 

        return result;
    }

    static int[,] Rownolegle1(int[,] matrixA, int[,] matrixB)
    {
        int[,] result = new int[matrixA.GetLength(0), matrixB.GetLength(1)];
        int p = 2;

        for (int l = 0; l < p; l++)                
            Parallel.For(0, matrixA.GetLength(0)/p, i =>
            {
                for( int j = 0; j < matrixB.GetLength(0);j++)
                {
                    for( int k = 0; k < matrixA.GetLength(1);k++)
                    {
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            });
     
        return result;
    }

    private async static Task<int[,]> Rownolegle2(int[,] matrixA, int[,] matrixB)
    {
        int[,] result = await Task.Factory.StartNew(() =>
       {
           int[,] result2 = new int[matrixA.GetLength(0), matrixB.GetLength(0)];

           for (int i = 0; i < matrixA.GetLength(0); i++)
               for(int j = 0;j < matrixB.GetLength(0); j++)
                    result2[i, j] = Task.Factory.StartNew(() =>
                    {
                       int iloczyn = 0;
                       for( int k = 0; k < matrixA.GetLength(0); k++)
                           iloczyn += matrixA[i, k] * matrixB[k, j];

                       return iloczyn;
                    }).Result;

           return result2;
       });

        return result;

    }


    static void Main (string [] args)
    {
        int N = 500;
        Stopwatch sw = new Stopwatch();
        int[,] array1 = new int[N, N];
        int[,] array2 = new int[N, N];
        fillmatrix(array1);
        fillmatrix(array2);

        sw.Start();
        Sekwencyjnie(array1, array2);
        sw.Stop();
        Console.WriteLine($"Czas sekwencyjnie: {sw.ElapsedMilliseconds}ms");
        sw.Reset();

        sw.Start();
        Rownolegle1(array1, array2);
        sw.Stop();
        Console.WriteLine($"Czas PrallelFor: {sw.ElapsedMilliseconds}ms");
        sw.Reset();

        sw.Start();
        var r2T = Rownolegle2(array1, array2);
        sw.Stop();
        Console.WriteLine($"Czas Task: {sw.ElapsedMilliseconds}ms");
    }
}