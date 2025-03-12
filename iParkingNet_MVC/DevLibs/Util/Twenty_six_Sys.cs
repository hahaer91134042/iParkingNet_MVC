using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Twenty_six_Sys 的摘要描述
/// </summary>
public class Twenty_six_Sys
{
    private static string[] Level = {"A", "B", "C", "D", "E", "F", "G",
 "H", "I", "G", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
 "U", "V", "W", "X", "Y", "Z" };
    /// <summary>
            /// 数字26进制，转换成字母，用递归算法
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>

    public static string Num_to_letter(Int64 value)
    {
        //此处判断输入的是否是正确的数字，略（正在表达式判断）
        long remainder = value % 26;
        //remainder = (remainder == 0) ? 26 : remainder;
        long front = (value - remainder) / 26;
        if (front < 26)
        {
            return Level[front - 1] + Level[remainder];
        }
        else
        {
            return Num_to_letter(front) + Level[remainder];
        }
        //return "";
    }
    /// <summary>
            /// 26进制字母转换成数字
            /// </summary>
            /// <param name="letter"></param>
            /// <returns></returns>
    public static int Letter_to_num(string str)
    {
        //此处判断是否是由A-Z字母组成的字符串，略（正在表达式片段）
        char[] letter = str.ToCharArray(); //拆分字符串
        int reNum = 0;
        int power = 1; //用于次方算值
        int times = 1;  //最高位需要加1
        int num = letter.Length;//得到字符串个数
                                //得到最后一个字母的尾数值
        reNum += Char_num(letter[num - 1]);
        //得到除最后一个字母的所以值,多于两位才执行这个函数
        if (num >= 2)
        {
            for (int i = num - 1; i > 0; i--)
            {
                power = 1;//致1，用于下一次循环使用次方计算
                for (int j = 0; j < i; j++)           //幂，j次方，应该有函数
                {
                    power *= 26;
                }
                reNum += (power * (Char_num(letter[num - i - 1]) + times));  //最高位需要加1，中间位数不需要加一
                times = 0;
            }
        }
        //Console.WriteLine(letter.Length);
        return reNum;
    }
    /// <summary>
            /// 输入字符得到相应的数字，这是最笨的方法，还可用ASIICK编码；
            /// </summary>
            /// <param name="ch"></param>
            /// <returns></returns>
    private static int Char_num(char ch)
    {
        switch (ch)
        {
            case 'A':
                return 0;
            case 'B':
                return 1;
            case 'C':
                return 2;
            case 'D':
                return 3;
            case 'E':
                return 4;
            case 'F':
                return 5;
            case 'G':
                return 6;
            case 'H':
                return 7;
            case 'I':
                return 8;
            case 'J':
                return 9;
            case 'K':
                return 10;
            case 'L':
                return 11;
            case 'M':
                return 12;
            case 'N':
                return 13;
            case 'O':
                return 14;
            case 'P':
                return 15;
            case 'Q':
                return 16;
            case 'R':
                return 17;
            case 'S':
                return 18;
            case 'T':
                return 19;
            case 'U':
                return 20;
            case 'V':
                return 21;
            case 'W':
                return 22;
            case 'X':
                return 23;
            case 'Y':
                return 24;
            case 'Z':
                return 25;

        }
        return -1;
    }
}
