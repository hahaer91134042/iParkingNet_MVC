using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// NewebPayInfoBuilder 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public class NewebPayInfoParser : BaseParser
    {
        public static NewebPayInfoParser Parse(object model)
        {
            return new NewebPayInfoParser(model);
        }
        private object Model;
        NewebPayInfoParser(object model)
        {
            Model = model;
        }

        public string GetInfo()
        {
            var builder = new StringBuilder();
            foreach (var property in Model.GetType().GetProperties())
            {
                if (!property.isDefinedAttr<NewebPaySet>()) continue;

                var set = property.getAttribute<NewebPaySet>();
                var value = property.GetValue(Model, null);
                if (set.IsNeed)
                {
                    //if (value.isNullOrEmpty())
                    //    throw new ArgumentNullException($"Property->[{property.Name}] Not Be Null obj!!");
                    builder.Append($"{set.Key}={value}&");
                }
                else
                {
                    if (!value.isNullOrEmpty())
                        builder.Append($"{set.Key}={value}&");
                }
            }

            return builder.removeLastChar().ToString();
        }
    }
}
