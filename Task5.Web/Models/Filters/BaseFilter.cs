using System;
using System.Linq.Expressions;

namespace Task5.Web.Models.Filters
{
    public abstract class BaseFilter
    {
        public TextFieldCriteria NameCriteria { get; set; }
        public virtual Expression<Func<string, string, bool>> ToExpression()
        {
            Expression<Func<string, string, bool>> expression = null;
            switch (NameCriteria)
            {
                case TextFieldCriteria.Contains:
                    expression = (x, y) => x.Contains(y);
                    break;
                case TextFieldCriteria.Ends:
                    expression = (x, y) => x.EndsWith(y);
                    break;
                case TextFieldCriteria.Starts:
                    expression = (x, y) => x.StartsWith(y);
                    break;
            }
            return expression;
        }
    }
}