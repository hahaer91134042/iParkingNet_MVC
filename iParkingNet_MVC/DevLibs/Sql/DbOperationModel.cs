using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevLibs
{
    public abstract class DbOperationModel
    {
        //protected int id = 0;
        //因為要能夠把ID的DbRowKey分開設定所以子類別get set 要加上base.Id來設定Id
        //[DbRowKey("Id", DbAction.NOINSERT, false)]
        public int Id { get; set; } = -1;

        protected E convertIntToEnum<E>(int input) 
        {
            return (E)Enum.ToObject(typeof(E), input);
        }
        protected int convertEnumToInt<E>(E input)
        {
            if (Enum.IsDefined(typeof(E), input))
            {
                return Convert.ToInt32(input);
            }
            return 0;
        }
        protected E convertStringToEnum<E>(string input)
        {
            return (E)Enum.Parse(typeof(E), input, false);
        }
        protected string convertEnumToString<E>(E input)
        {
            if (Enum.IsDefined(typeof(E), input))
            {
                return input.ToString();
            }
            return "";
        }
    }
}