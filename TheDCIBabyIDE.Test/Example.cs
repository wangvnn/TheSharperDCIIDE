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
        public int sum { get; private set; }
        public void add(int a)
        {
            sum += a;
        }
    }
    public class TestMath : GivenWhenThenFixture
    {
        public override void Specify()
        {
            
            given("a sum", () =>
            {

                var sut = new Math();

                sut.add(5);

                when("add 5", () =>
                {
                    sut.add(5);

                    then("result should be correct", () =>
                    {

                        expect(() => sut.sum == 10);
                    });

                    when("add 5", () =>
                    {
                        sut.add(5);

                        then("result should be correct", () =>
                        {

                            expect(() => sut.sum == 15);
                        });
                    });
                });

                when("add 15", () =>
                {
                    sut.add(15);

                    then("result should be correct", () =>
                    {

                        expect(() => sut.sum == 20);
                    });
                });
            });
        }
    }
}
