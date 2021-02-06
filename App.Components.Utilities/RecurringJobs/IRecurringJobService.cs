using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace App.Components.Utilities.RecurringJobs
{
    public interface IRecurringJobService
    {
        void AddOrUpdate(string JobUniqueName, Expression<Action> Job, string cronExpression);       
        void RemoveIfExists(string JobUniqueName);
    }
}
