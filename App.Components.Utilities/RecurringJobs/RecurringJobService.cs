using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace App.Components.Utilities.RecurringJobs
{
    public class RecurringJobService : IRecurringJobService
    {
        private readonly IRecurringJobManager _recurringJobManager;        
        private readonly ILogger<RecurringJobService> _logger;
        public List<string> JobsList { private set; get; }
        
        public RecurringJobService(IRecurringJobManager recurringJobManager, ILogger<RecurringJobService> logger)
        {
            _recurringJobManager = recurringJobManager;
            _logger = logger;
            JobsList = new List<string>();
        }
        public void AddOrUpdate(string JobUniqueName, Expression<Action> Job, string cronExpression)
        {
            try
            {
                _recurringJobManager.AddOrUpdate(JobUniqueName, Job, cronExpression);
                if(JobsList.Contains(JobUniqueName))                                    
                    _logger.LogInformation("RecurringJobService => Job:{0} has been updated in EC2 {2} with this CronExpression({1}) ", JobUniqueName, cronExpression);                
                else
                {
                    JobsList.Add(JobUniqueName);
                    _logger.LogInformation("RecurringJobService => Job:{0} has been created in EC2 {2} with this CronExpression({1})", JobUniqueName, cronExpression);
                }
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "RecurringJobService => Failed to create Job:{0} in EC2 {2} with this CronExpression({1})", JobUniqueName, cronExpression);
            }
            
        }

        public void RemoveIfExists(string JobUniqueName)
        {
            try
            {
                _recurringJobManager.RemoveIfExists(JobUniqueName);
                if (JobsList.Contains(JobUniqueName))
                {
                    JobsList.Remove(JobUniqueName);
                    _logger.LogInformation("RecurringJobService => Job:{0} has been Removed from RecurringJobService", JobUniqueName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RecurringJobService => Failed to Remove Job:{0} from RecurringJobService", JobUniqueName);
            }                    
            
        }

       
    }
}
