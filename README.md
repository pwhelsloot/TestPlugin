# TestPlugin

This repository contains a template for Platform framework applications. This template uses the same framework as the AMCS Platform is using, e.g. the ELEMOS backend and Scale, and has just enough functionality to form a fully functional application.

## Running

Complete the following steps to get up and running:

* Compile the solution. The project contains NuGet references, which may require you to authenticate with the Azure Artifacts feed;
* Deploy the database. The database project contains a file named `Database.publish.xml`. Open this file and ensure you've put in a proper reference to a database server and catalog;
* Update the database connection string in the following files:
  * `AMCS.TestPlugin.Server.Web\Web.config`
  * `AMCS.TestPlugin.Tests\App.config`
  * `AMCS.TestPlugin.IntegrationTests\App.config`
* Run the `AMCS.TestPlugin.IntegrationTests.TestFixture.EnsureAdminUser()` unit test to ensure the database has a default `admin` user with a password `admin`;
* Set the `AMCS.TestPlugin.Server.Web` project as the startup project and run the application.

This should be enough to get you up and running.

The Postman collection at https://www.getpostman.com/collections/21c1d43c12949366bc4e contains a few working examples you can use. You do need to create a [Postman environment]( https://learning.getpostman.com/docs/postman/environments-and-globals/manage-environments/) with a variable named `test-plugin-url` pointing to the running application. To test out a REST API call, you first need to call the authentication endpoint before you can call any of the other API's.

## Using this template

The `CopyTemplate` folder contains an application you can use to start a new project using the template. This application is run as follows:

```bat
CopyTemplate.exe --target-path "C:\Projects\MyProject" --target-name "MyProject"
```

This creates a copy of the template at the specified path with the specified name. The application will take care of changing file names and fixing up references to have the project renamed to the name you specify.

Repeated executions of this command will result in the same result. This would allow you to later on refresh your project from the template if necessary.

## Job System

This template implements the job system. There are a number of ways to implement the job system in an application. This example uses an out of process agent, where there's a separate web app that hosts job system jobs.

The steps below describe how this job system was added.

### Database support

The database requires a few support tables for the job system, to store scheduled jobs and history. These are in the `Schema Objects/Schemas/jobs/Tables` folder.

### Agent

The agent hosts the job system jobs. There are roughly two ways to have an agent in the job system:

* You embed it into the primary web application (`AMCS.TestPlugin.Server.Web` in this case) and have that connect out to the scheduler;
* You create a separate web application that has just the agent.

This example uses both approaches, including the separate web app. The `agentConfiguration` element is in both `Web.config` files. If you just start the server, you get an embedded agent which will process jobs. If you want to use the separate web app, you have to remove the `agentConfiguration` from the sever `Web.config` and start the agent web app. See instructions below on how to do this.

The agent was setup as follows:

* Add a reference to the `AMCS.JobSystem.Agent` NuGet package;

* Copy the `Web.config` from the `AMCS.TestPlugin.Server.Web\Web.config` file and make the following changes:

  * Add the `agentConfiguration` element like the following:

  ```xml
  <agentConfiguration
    queuePrefix="test-plugin"
    instanceName="agent">
    <queues>
      <add name="priority" queueSize="2" />
      <add name="slow" queueSize="2" />
    </queues>
  </agentConfiguration>
  ```
  
  * Remove the `owin:appStartup` `appSetting`;
  
* Remove the `amcs.server\jobSystem` element;
  
* Add a `Global.asax` to setup the agent and the framework. This roughly comes down to the following:

```cs
protected void Application_Start(object sender, EventArgs e)
{
  XmlConfigurator.Configure();

  ServiceSetup.Setup();
}
```

* Add a `Default.aspx` to have something to show. The platform template example also sets the HTTP status code if the agent isn't connected. This can be used for diagnostics.

### Server

Last, we need to connect the server to the job system. This is done by adding the `amcs.server\jobSystem` element as follows:

```xml
<amcs.server>

  <jobSystem
    scheduledJobUserId="admin"
    scheduledJobQueueInterval="60"
    instanceName="server"
    queuePrefix="test-plugin" />

</amcs.server>
```

This sets the required configuration for the scheduler client. You also need a connection string to Azure Service Bus if you want to enable remote transport:

```xml
<connectionStrings>
  <add name="JobSystemServiceBusConnectionString" connectionString="" />
</connectionStrings>
```

Then, the following code can be added to `ServiceSetup.Setup()` to use this configuration:

```cs
configuration.ConfigureJobSystem(
  serverConfiguration.JobSystem,
  connectionString,
  messageQueueConnectionString,
  serverTypes);
```

This will load the job system and make a `SchedulerClient` available in `DataServices`. If you also add the same `agentConfiguration` configuration into this `Web.config`, the above call will also setup the agent for you.

### REST API's

The above setups the basis for the job system. There are however also REST API's available that can be used by the [AMCS Job System](https://setups.amcsgroup.io/) application. To support these, the following needs to be done:

* Create entities like the ones in `AMCS.TestPlugin.Entity.Api.JobSystem`;
* Create services like the ones in `AMCS.TestPlugin.Server.Api.JobSystem`;
* Add the entities to the `AMCS.TestPlugin.Entity.ApiMetadata.xml` file.

Once these changes are made, the AMCS Job System application should work. You can also setup the status monitor to get real time updates of running jobs by changing the `ServiceSetup.Setup()` job system setup to the following:

```cs
configuration.ConfigureJobSystem(
  serverConfiguration.JobSystem,
  connectionString,
  messageQueueConnectionString,
  serverTypes);

configuration.ConfigureJobSystemStatusMonitor();
```

### Jobs

To actually add jobs, a class inheriting from `JobHandler<,>` needs to be created. The platform template application has a `DemoJob` example. If you start the platform template project and connect the AMCS Job System, you'll be able to create a scheduled job for that job, and run it.

### Running with the job system

By default the agent for the job system is started within the server application. This runs the job system in local mode, which doesn't require a remote transport to communicate between the different job system components.

Complete the following steps if you want to test remote mode:

* Right click on the **Solution 'TestPlugin'** node in the solution explorer and clicking **Set StartUp Projects...**. Then, set the two `Web` projects as **Start**;
* In the server `Web.config`, remove the following XML elements:
  * `<section name="agentConfiguration" ... />`
  * `<agentConfiguration ...`
* In both the server and agent `Web.config`, fill in the `JobSystemServiceBusConnectionString` connection string with an Azure Service Bus connection string.