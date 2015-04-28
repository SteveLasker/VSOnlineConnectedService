#What Happened#
The following changes were made to your project

- A NuGet reference to `NewtonSoft.Json` was added
- `app/web.config` values were added
- A `VSOnlineService.cs` file was added, providing REST wrapper calls
- A Model file representing a WorkItem was added to `\VSOnline\WorkItem.cs`  

#Consuming Visual Studio Online in an ASP.NET MVC Application#
To consume Visual Studio Online in an ASP.NET MVC Application we'll


1. Add an MVC Controller 
2. Add a Razor View
2.   Add a Menu Item to the default ASP.NET MVC app navigation .cshtml files
##Adding a WorkItem Controller##

- Right click on the Controllers folder and choose Add Controller
- Name the controller **WorkItem**Controller

Add the following code, noting the change to your projects namespace: 

	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Web.Mvc;
	#error Update the namespace to match your project
	using WebApplication1.VSOnline;
	using WebApplication1.Models.VSOnline;
	namespace WebApplication1.Controllers
	{
	    public class WorkItemController : Controller
	    {
	        public async Task<ActionResult> Index()
	        {
	            string query = "Select [System.Id] From WorkItems Where[System.WorkItemType] = 'Bug' order by [System.CreatedDate] desc";
	            List<WorkItem> workItems = await new VSOnlineService().GetWorkItems<WorkItem>(query);
	
	            return View(workItems);
	        }
	    }
	}

##Add a View##
To view the results of our Controller query, add a view

- Right click inside the Index() method and chose **Add View**
- In the Add View scaffolding dialog, chose:
- Tempalte: **List**
- Model class: **WorkItem** (YourNamespace.Models.VSOnline)

##Add a Menu to view the WorkItems##

- Open the **Views\Shared\_Layout.cshtml** file

Add the following code to the menu 

	<li>@Html.ActionLink("VSOnline WorkItems", "Index", "WorkItem")</li>
##Run the app##
Hit F5, and try it out

