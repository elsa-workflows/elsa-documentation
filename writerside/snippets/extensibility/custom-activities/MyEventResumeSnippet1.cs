var bookmarkPayload = "MyEvent";
var activityTypeName = ActivityTypeNameHelper.GenerateTypeName<MyEvent>();
await _workflowRuntime.TriggerWorkflowsAsync(activityTypeName, bookmarkPayload);