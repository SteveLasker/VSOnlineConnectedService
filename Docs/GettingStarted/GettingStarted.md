#Getting Started#

Now that you have the Visual Studio Online Connected Service, lets add some sample code to use it.

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
	            VSOnlineService service = new VSOnlineService();
	            string query = "Select [System.Id] From WorkItems Where[System.WorkItemType] = 'Bug' order by [System.CreatedDate] desc";
	            List<WorkItem> workItems = await service.GetWorkItems<WorkItem>(query);
	
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
 