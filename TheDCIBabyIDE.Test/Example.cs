using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NJasmine;

namespace TheDCIBabyIDE.Test
{
    class Math
    {
        public int add(int a, int b)
        {
            return a + b;
        }
    }
    public class TestMath : GivenWhenThenFixture
    {
        public override void Specify()
        {
            
            given("two numbers", () =>
            {

                var a = 10;
                var b = 5;

                when("add 2 numbers", () =>
                {

                    var sut = new Math();

                    var c = sut.add(a, b);

                    then("result should be correct", () =>
                    {

                        expect(() => c == 15);
                    });
                });
            });
        }
    }
}
