using KimHaiQuang.TheDCIBabyIDE.Domain.Data;
using NJasmine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDCIBabyIDE.Test.DomainTest.DataTest
{
    public class DCIContextTest : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("A DCI Context", () => {

                var dciContext = new DCIContext();

                when("load dci context file", () => {

                    dciContext.LoadFromFile(TestInfo.FrontLoadOperation);

                    then("it should have some roles", () => {

                        expect(() => dciContext.Roles.Count > 0);

                    });
                });
            });
        }
    }
}
