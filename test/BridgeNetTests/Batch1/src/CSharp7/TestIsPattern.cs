using Bridge.Test.NUnit;

namespace Bridge.ClientTest.CSharp7
{
    /// <summary>
    /// The test here consists in checking C# 7's 'is' syntax support.
    /// </summary>
    [Category(Constants.MODULE_BASIC_CSHARP)]
    [TestFixture(TestNameFormat = "Is pattern - {0}")]
    public class TestIsPattern
    {
        /// <summary>
        /// Explores several 'is' pattern syntax variations, and fail if they
        /// do not behave the expected way.
        /// </summary>
        [Test]
        public static void IsPatternTests()
        {
            object o = "16";
            if (o is int i || (o is string s && int.TryParse(s, out i)))
            {
                object o1 = "17";
                if (o1 is int i1 || (o1 is string s1 && int.TryParse(s1, out i1)))
                {
                    Assert.AreEqual(17, i1, "'is' expression may be bound to nested conditionals.");
                }
                else
                {
                    Assert.Fail("'is' expression can't reliably nest conditional expressions.");
                }

                Assert.AreEqual(16, i, "String-filled object can be inferred from composite 'is' conditional expression.");
            }
            else
            {
                Assert.Fail("String-filled object cannot be inferred from composite 'is' conditional expression.");
            }

            // Added after initial set of tests, checks against 'null' value matching.
            o = null;
            if (o is null)
            {
                Assert.Null(o, "Null can be used within 'is' expression.");
            }
            else
            {
                Assert.Fail("Null-bound variable evaluates to false when checked against 'is null'.");
            }

            if (!(o is int ii))
            {
                Assert.Null(o, "Null can be compared with non-nullable type within 'is' expression.");
            }
            else
            {
                Assert.Fail("Null-bound variable matched a non-nullable type within 'is' expression.");
            }

            o = double.NaN;
            const double value = double.NaN;
            if (o is value)
            {
                Assert.True(o.Equals(double.NaN), "'is' argument may be a variable.");
            }
            else
            {
                Assert.Fail("'is' argument as a variable does not resolve to the variable's type.");
            }
        }

        private class Round
        {
            public int R = 2;
        }

        private static string NotIs(object gear, object dragged)
        {
            if (gear == null || !(dragged is Round round))
            {
                return "out";
            }
            return "round " + round.R;
        }

        private static string IsNot(object gear, object dragged)
        {
            if (gear == null || dragged is not Round round)
            {
                return "out";
            }
            return "round " + round.R;
        }

        private static string ThreeWay(object gear, object other, object dragged)
        {
            if (gear == null || other == null || !(dragged is Round round))
            {
                return "out";
            }
            return "round " + round.R;
        }

        private static string UsedInTheOr(object gear, object dragged)
        {
            if (gear == null || !(dragged is Round round) || round.R > 5)
            {
                return "out";
            }
            return "round " + round.R;
        }

        private static string NotOfAnAnd(object gear, object dragged)
        {
            if (!(gear != null && dragged is Round round))
            {
                return "out";
            }
            return "round " + round.R;
        }

        private static string AndInBrackets(object gear, object dragged)
        {
            if (gear != null && (dragged is Round round))
            {
                return "round " + round.R;
            }
            return "out";
        }

        private static string Property(object gear, object dragged)
        {
            if (gear == null || !(dragged is Round { R: 2 } round))
            {
                return "out";
            }
            return "round " + round.R;
        }

        private static string Var(object gear, object dragged)
        {
            if (gear == null || !(dragged is var seen && seen != null))
            {
                return "out";
            }
            return seen is Round ? "seen round" : "seen other";
        }

        /// <summary>
        /// A variable an 'is' pattern declares in the right side of '||' or '&&' is still in scope after the condition.
        /// </summary>
        [Test]
        public static void PatternVariableAfterLogicalOperatorTests()
        {
            var round = new Round();
            var wide = new Round { R = 9 };

            Assert.AreEqual("out", NotIs(null, round));
            Assert.AreEqual("out", NotIs("gear", "text"));
            Assert.AreEqual("round 2", NotIs("gear", round));

            Assert.AreEqual("out", IsNot(null, round));
            Assert.AreEqual("out", IsNot("gear", null));
            Assert.AreEqual("round 2", IsNot("gear", round));

            Assert.AreEqual("out", ThreeWay("gear", null, round));
            Assert.AreEqual("round 2", ThreeWay("gear", "other", round));

            Assert.AreEqual("out", UsedInTheOr("gear", wide));
            Assert.AreEqual("round 2", UsedInTheOr("gear", round));

            Assert.AreEqual("out", NotOfAnAnd(null, round));
            Assert.AreEqual("round 2", NotOfAnAnd("gear", round));

            Assert.AreEqual("out", AndInBrackets("gear", "text"));
            Assert.AreEqual("round 2", AndInBrackets("gear", round));

            Assert.AreEqual("out", Property("gear", wide));
            Assert.AreEqual("round 2", Property("gear", round));

            Assert.AreEqual("out", Var("gear", null));
            Assert.AreEqual("seen round", Var("gear", round));
            Assert.AreEqual("seen other", Var("gear", "text"));

            var items = new object[] { new Round { R = 1 }, new Round { R = 3 }, "text", new Round { R = 5 } };
            int sum = 0;
            for (int j = 0; !(j >= items.Length || !(items[j] is Round each)); j++)
            {
                sum += each.R;
            }
            Assert.AreEqual(4, sum);
        }
    }
}