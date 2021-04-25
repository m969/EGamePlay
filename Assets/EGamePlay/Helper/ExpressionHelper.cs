using UnityEngine;
using B83.ExpressionParser;

namespace EGamePlay
{
    public static class ExpressionHelper
    {
        public static ExpressionParser ExpressionParser { get; set; } = new ExpressionParser();


        public static Expression TryEvaluate(string expressionStr)
        {
            Expression expression = null;
            try
            {
                expression = ExpressionParser.EvaluateExpression(expressionStr);
            }
            catch (System.Exception e)
            {
                Log.Error(expressionStr);
                Log.Error(e);
            }
            return expression;
        }
    }
}