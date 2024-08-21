# Scheduler:
## Introduction:

The Scheduler is a Minimal API that enables the scheduling and execution of various background tasks using [Hangfire](https://www.hangfire.io/). 
This scalable solution allows you to create custom schedulers and tasks, with the "ReminderScheduler" serving as an example to guide you.
I also utilize the [PredicateBuilder](http://www.albahari.com/nutshell/predicatebuilder.aspx)  from  [LINQKit](http://www.albahari.com/nutshell/linqkit.aspx). 
This tool is particularly useful for translating complex C# code into SQL, ensuring your code remains both readable and optimized for performance.
Additionally, you will find middleware for exception handling, which enhances your project's robustness.

## Endpoints:
- `GET /scheduler/remainder` -> Retrieve all reminder schedules.
- `GET /scheduler/remainder/undeleted` -> Get reminder schedules by status.
- `GET /scheduler/remainder/id`-> Retrieve a reminder schedule by ID.
- `DELETE /scheduler/remainder`-> Soft delete a reminder schedule by ID.
- `PATCH /scheduler/remainder/un-delete`-> Restore a soft-deleted reminder schedule by ID.
- `POST /scheduler/remainder`-> Create a new reminder schedule.
- `PUT /scheduler/remainder`-> Update an existing reminder schedule.
