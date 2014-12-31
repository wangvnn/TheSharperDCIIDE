using ProkonDCI.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProkonDCI.Domain.Operation
{
    public sealed class FrontLoadContext 
    {
        #region Usecase

        // Use case: Front Load Operation to calculate the early start time of all activities
        // Steps: User vs System
        // 1. User wants to see the early start of all Activities (trigger)
        //    2. System/FrontLoader plans front load
        //    3. FrontLoader sets all Activities' early start to unplanned
        //    4. FrontLoader finds the Activity that is unplanned 
        //       but all of its predecessors has beed planned 
        //    5. FrontLoader asks the above Activity to frontload itself
        //    6. The Activity then finds the 'max early finish' from its predecessors
        //      6.a If it cannot find the 'max early finish' then its early start = ProjectStart
        //      6.b If it found the 'max early finish' then its early start = max early finish+1
        //    7. Repeat Step 4 until FrontLoader cannot find any unplanned Activity

        #endregion

        #region Roles

        private FrontLoadContext FrontLoader { get; private set; }

        private UnPlannedActivityRole UnPlannedActivity { get; private set; }
        public interface UnPlannedActivityRole
        {
            int EarlyStart { get; set; }
            void TestMethod();
        }

        private List<Activity> AllActivities { get; private set; }

        private int ProjectStart { get; private set; }

        #endregion

        #region Context

        private ActivityDependencyGraph Model { get; set; }

        private List<Activity> Predecessors
        {
            get
            {
                return Model.PredecessorsOf((Activity)UnPlannedActivity);
            }
        }

        public FrontLoadContext(ActivityDependencyGraph model)
        {
            Model = model;
            AllActivities = Model.AllActivities;
            ProjectStart = Model.ProjectStart; ;
            FrontLoader = this;
        }

        public void FrontLoad()
        {
            FrontLoaderRole_Plans();
        }

        #endregion

        #region FrontLoader_Methods

        public void FrontLoader_Plans()
        {
            AllActivities.ForEach(a => a.EarlyStart = 0);

            while (FrontLoader_FindUnPlannedActivity())
            {
                UnPlannedActivity_FrontLoad();
            }
        }

        private bool FrontLoader_FindUnPlannedActivity()
        {
            UnPlannedActivity = (UnPlannedActivityRole) AllActivities.FirstOrDefault(a => a.EarlyStart == 0 &&
                !Model.PredecessorsOf(a).Any(p => p.EarlyFinish == 0));

            return UnPlannedActivity != null;
        }

        #endregion

        #region UnPlannedActivity_Methods

        public void UnPlannedActivity_FrontLoad()
        {
            Activity maxPred = Predecessors.FirstOrDefault(p => p.EarlyFinish == Predecessors.Max(m => m.EarlyFinish));
            if (maxPred != null)
            {
                UnPlannedActivity.EarlyStart = maxPred.EarlyFinish + 1;
            }
            else
            {
                UnPlannedActivity.EarlyStart = ProjectStart;
            }
        }

        #endregion    
    }
}