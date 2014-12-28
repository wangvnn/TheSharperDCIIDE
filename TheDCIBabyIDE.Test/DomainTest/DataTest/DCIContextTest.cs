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


                        expect(() => dciContext.Name.EndsWith("FrontLoadContext"));
                        expect(() => dciContext.Usecase.Length > 0);

                        expect(() => dciContext.Roles.Count == 4); 

                        expect(() => dciContext.Roles[0].Name.EndsWith("FrontLoader"));
                        expect(() => dciContext.Roles[1].Name.EndsWith("UnPlannedActivity"));
                        expect(() => dciContext.Roles[2].Name.EndsWith("AllActivities"));
                        expect(() => dciContext.Roles[3].Name.EndsWith("ProjectStart"));

                        expect(() => dciContext.Roles[1].Interface.Name.Equals("UnPlannedActivityRole"));
                        expect(() => dciContext.Roles[1].Interface.Signatures.Count == 2);
                    }); 
                });
            });
        }
    }
}
