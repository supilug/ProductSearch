using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProductSearch
{
    class Program
    {
        static List<int[]> OutputData = new List<int[]>();
        static Bm boyer = new Bm();
        static string path = @"product.txt";
        static string[] lines = null;
        static List<int> ShowDistinctData = new List<int>();

        static void Main(string[] args)
        {
            readData();
            if (lines != null)
            {

                Console.Write("Product Search - Input your Keyword (s): ");
                string x = Console.ReadLine();
                //input keyword
                char[] s = { ' ' };
                string[] pat = x.Split(s, StringSplitOptions.RemoveEmptyEntries);

                //search by boyer moore
                searchData(pat);

                //show result
                ShowResult();

                Console.Read();
            }

        }

        static void readData()
        {
            if (File.Exists(path))
            {
                lines = System.IO.File.ReadAllLines(path);
            }
            else
            {
                Console.Write("Error: No data to open");
                Console.Read();
            }
        }

        static void searchData(string[] pattern)
        {
            if (lines != null)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    for (int j = 0; j < pattern.Length; j++)
                    {
                        int[] FindBoyer = boyer.BM_Matcher(lines[i].ToLower(),pattern[j].ToLower());
                        if(FindBoyer.Length > 0)
                            OutputData.Add(new int[] {i,j, FindBoyer[0]});
                    }
                }

                ShowDistinctData = OutputData.Select(i => i[0]).Distinct().ToList();
            }

        }

        static void ShowResult()
        {
            Console.WriteLine("Search Result is:");
            foreach (int item in ShowDistinctData)
            {
                Console.WriteLine(lines[item]);
            }
            Console.WriteLine("- "+ShowDistinctData.Count + " product(s) matched");
        }
    }
    // ส่วนของ boyer mooore Algorithm
    public class Bm
    {
        public Kmp _kmp = new Kmp();
        public int[] Compute_Last_Occurence_Function(string pattern)
        {
            const int sigma_size = 128;
            int m = pattern.Length;
            int[] lambda = new int[sigma_size];
            for (int i = 0; i < lambda.Length; i++)
            {
                lambda[i] = -1;
            }
            for (int j = 0; j < m; j++)
            {
                lambda[(int)pattern[j]] = j;
            }
            return lambda;
        }
        public int[] Compute_Good_Suffix_Function(string pattern)
        {
            int m = pattern.Length;
            var pi = _kmp.ComputePrefixFunction(pattern);
            var P_0 = Reverse(pattern);
            var pi_0 = _kmp.ComputePrefixFunction(P_0);
            int[] gamma = new int[m + 1];
            int j;
            for (j = 0; j <= m; j++)
            {
                gamma[j] = (m - 1) - pi[m - 1];
            }
            for (int i = 0; i < m; i++)
            {
                j = (m - 1) - pi_0[i];
                if (gamma[j] > i - pi_0[i])
                {
                    gamma[j] = i - pi_0[i];
                }
            }
            return gamma;
        }

        private string Reverse(string text)
        {
            if (text == null)
                return null;
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new String(array);
        }

        public int[] BM_Matcher(string Text, string Pattern)
        {
            int n = Text.Length;
            int m = Pattern.Length;
            int[] lambda = Compute_Last_Occurence_Function(Pattern);
            int[] gamma = Compute_Good_Suffix_Function(Pattern);
            int s = 0;
            List<int> validshifts = new List<int>();

            while (s <= n - m)
            {
                int j = m - 1;
                while (j >= 0 && Pattern[j] == Text[s + j])
                {
                    j--;
                }
                if (j < 0)
                {
                    validshifts.Add(s);
                    s = s + gamma[0];
                }
                else
                {
                    s = s + Math.Max(gamma[j], j - lambda[Text[s + j]]);
                }
            }
            return validshifts.ToArray();
        }
    }
    // ส่วนของ knuth-Morris-Pratt Algorithm
    public class Kmp
    {
        public void kmp_search(string P, string T)
        {
            int n = T.Length;
            int m = P.Length;
            int[] pi = ComputePrefixFunction(P);
            int q = 0;

            for (int i = 1; i <= n; i++)
            {
                while (q > 0 && P[q] != T[i - 1])
                {
                    q = pi[q - 1];
                }
                if (P[q] == T[i - 1]) { q++; }
                if (q == m)
                {
                    //Record a match was found here  
                    q = pi[q - 1];
                }
            }
        }

        public int[] ComputePrefixFunction(string P)
        {
            int m = P.Length;
            int[] pi = new int[m];
            int k = 0;
            pi[0] = 0;

            for (int q = 1; q < m; q++)
            {
                while (k > 0 && P[k] != P[q]) { k = pi[k]; }

                if (P[k] == P[q]) { k++; }
                pi[q] = k;
            }
            return pi;
        }
    }

}