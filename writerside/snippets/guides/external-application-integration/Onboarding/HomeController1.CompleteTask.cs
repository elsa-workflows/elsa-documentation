public async Task<IActionResult> CompleteTask(int taskId, CancellationToken cancellationToken)
{
    var task = dbContext.Tasks.FirstOrDefault(x => x.Id == taskId);

    if (task == null)
        return NotFound();

    await elsaClient.ReportTaskCompletedAsync(task.ExternalId, cancellationToken: cancellationToken);

    task.IsCompleted = true;
    task.CompletedAt = DateTimeOffset.Now;

    dbContext.Tasks.Update(task);
    await dbContext.SaveChangesAsync(cancellationToken);

    return RedirectToAction("Index");
}