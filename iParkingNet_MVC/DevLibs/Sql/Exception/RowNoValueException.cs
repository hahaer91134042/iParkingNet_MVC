using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class RowNoValueException : Exception
{
    public RowNoValueException(string message) : base(message)
    {
    }
}
