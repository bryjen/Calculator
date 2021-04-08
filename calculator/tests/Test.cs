using System;
using System.Collections.Generic;
using calculator.main;
using NUnit.Framework;
using static calculator.main.Expression;

namespace calculator.tests
{
    public interface Test
    {
        [Test]
        public void Run()
        {
            
        }
    }

    public class CalcTest : Test
    {
        public string Name { get; private set; }
        
        public string Expression { get; private set; }
        
        public string[] ExpressionArray { get; private set; }
        
        public bool AssertValid { get; private set; }
        
        public bool GotValid { get; private set; }
        
        public bool IsResultValid { get; private set; }
        
        public double? AssertResult { get; private set; }
        
        public double? GotResult { get; private set; }
        

        public CalcTest(string name, string expresssion, bool assertValid, double? assertResult)
        {
            Name = name;
            Expression = expresssion;
            ExpressionArray = new Expression(expresssion).ExpressionList.ToArray();
            AssertValid = assertValid;
            AssertResult = assertResult;
            GotResult = null;
        }

        [Test]
        public void Run()   //@Override
        {
            var expression = new Expression(Expression);
            try
            {
                Assert.AreEqual(AssertValid, expression.Valid);
                GotValid = true;
            }
            catch (Exception)
            {
                GotValid = false;
            }

            if (!GotValid) return;

            try
            {
                GotResult = expression.Solve();
                Assert.AreEqual(AssertResult, GotResult);
                IsResultValid = true;
            }
            catch (Exception)
            {
                IsResultValid = false;
            }
        }
    }
}