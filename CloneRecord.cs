/// <summary>
/// Clone Records using plugins
/// </summary>

namespace CloneRecordsUsingPlugins
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public class CloneRecord : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service

            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            // Obtain the execution context from the service provider. 
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            // Obtain the organization service reference which you will need for 
            // web service calls. 
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            try
            {
                Entity getRecordDetails = (Entity)context.InputParameters["Target"];

                // Clone Exact Same Record.
                CloneExactRecord(service, getRecordDetails);

                // Clone and Modify the Original record.
                ModifyAndCloneRecord(service, getRecordDetails);
            }
            catch (InvalidPluginExecutionException ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }

        /// <summary>
        /// Modify and Clone the Record.
        /// </summary>
        /// <param name="service">Organization service.</param>
        /// <param name="getRecordDetails">Curretn Record id.</param>
        private void ModifyAndCloneRecord(IOrganizationService service, Entity getRecordDetails)
        {
            // Get the Current Record to Clone
            Entity cloneRecord = service.Retrieve("opportunity", getRecordDetails.Id, new ColumnSet(true));

            // Modify the Field and Create Current Record
            cloneRecord.Attributes["name"] = cloneRecord.Attributes["name"].ToString() + " Cloned Record";
            cloneRecord.Attributes["estimatedclosedate"] = DateTime.Now.AddYears(1);

            // Remove Unique id
            cloneRecord.Attributes.Remove("opportunityid");

            // Generate new ID
            cloneRecord.Id = Guid.NewGuid();

            // Create Record
            Guid getCreatedOpportunity = service.Create(cloneRecord);
        }

        /// <summary>
        /// Clone Exact Values from the Record.
        /// </summary>
        /// <param name="service">Organization service.</param>
        /// <param name="getRecordDetails">Current Record Details.</param>
        private void CloneExactRecord(IOrganizationService service, Entity getRecordDetails)
        {
            // get the Opportunity Record
            Entity cloneRecord = service.Retrieve("opportunity", getRecordDetails.Id, new ColumnSet(true));
            // Remove the Unique ID
            cloneRecord.Attributes.Remove("opportunityid");
            // Add new Unique ID
            cloneRecord.Id = Guid.NewGuid();
            // Create the record
            Guid getCreatedOpportunity = service.Create(cloneRecord);
        }
    }
}
