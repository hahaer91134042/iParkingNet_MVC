﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// DataTypeException 的摘要描述
/// </summary>
public class DataTypeException : Exception
{
    public DataTypeException() : base()
    {
    }
    public DataTypeException(string msg):base(msg)
    {
    }
}